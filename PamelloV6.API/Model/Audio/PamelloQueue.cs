using AngleSharp.Dom.Events;
using AngleSharp.Text;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;

namespace PamelloV6.API.Model.Audio
{
    public class PamelloQueue
    {
		private readonly PamelloPlayer _parentPlayer;

        public readonly PamelloEventsService _events;
        public readonly PamelloSongRepository _songs;

        public readonly List<PamelloAudio> SongAudios;

        private int _position;
        public int Position {
            get => _position;
            set {
                _position = value;

                _events.SendToAllWithSelectedPlayer(
					_parentPlayer.Id,
					PamelloEvent.PlayerQueuePositionUpdated(Position)
                );
            }
        }

		private bool _isRandom;
		private bool _isReversed;
		private bool _isNoLeftovers;
		
		public bool IsRandom {
			get => _isRandom;
			set {
				_isRandom = value;

                _events.SendToAllWithSelectedPlayer(
                    _parentPlayer.Id,
					PamelloEvent.PlayerQueueIsRandomUpdated(IsRandom)
                );
            }
        }
		public bool IsReversed {
			get => _isReversed;
			set {
				_isReversed = value;

                _events.SendToAllWithSelectedPlayer(
                    _parentPlayer.Id,
                    PamelloEvent.PlayerQueueIsReversedUpdated(IsReversed)
                );
            }
		}
		public bool IsNoLeftovers {
			get => _isNoLeftovers;
			set {
				_isNoLeftovers = value;

                _events.SendToAllWithSelectedPlayer(
                    _parentPlayer.Id,
                    PamelloEvent.PlayerQueueIsNoLeftoversUpdated(IsNoLeftovers)
                );
            }
		}

        private int? _nextPositionRequest;
        public int? NextPositionRequest {
			get => _nextPositionRequest;
			set {
				_nextPositionRequest = value;

                _events.SendToAllWithSelectedPlayer(
                    _parentPlayer.Id,
                    PamelloEvent.PlayerQueueNextPositionUpdated(NextPositionRequest)
                );
            }
		}

        private PamelloAudio? _current;
		public PamelloAudio? Current {
			get => _current;
			set {
                if (_current is not null) {
                    _current.Position.OnSecondTick -= OnCurrentPositionSecondTick;
                    _current.Duration.OnSecondTick -= OnCurrentDurationSecondTick;
                    _current.OnInitializationProgress -= OnCurrentInitializationProgress;
                }
                _current?.Clean();
				_current = value;

				_events.SendToAllWithSelectedPlayer(
                    _parentPlayer.Id,
                    PamelloEvent.PlayerQueueSongUpdated(Current?.Song.Id)
                );

				if (_current is not null) {
					_current.Position.OnSecondTick += OnCurrentPositionSecondTick;
                    _current.Duration.OnSecondTick += OnCurrentDurationSecondTick;
                    _current.OnInitializationProgress += OnCurrentInitializationProgress;
                }

                OnCurrentDurationSecondTick();
                OnCurrentPositionSecondTick();
            }
		}

        private void OnCurrentInitializationProgress(int progress) {
            Console.WriteLine($"Initialized {progress} * 10 minutes");

            _events.SendToAllWithSelectedPlayer(
                _parentPlayer.Id,
                PamelloEvent.PlayerInitializationProgress(progress)
            );
        }

        public void OnCurrentDurationSecondTick() {
            _events.SendToAllWithSelectedPlayer(
                _parentPlayer.Id,
				PamelloEvent.PlayerTotalTimeUpdated(Current?.Duration.TotalSeconds ?? 0)
            );
        }
        public void OnCurrentPositionSecondTick() {
            _events.SendToAllWithSelectedPlayer(
                _parentPlayer.Id,
                PamelloEvent.PlayerCurrentTimeUpdated(Current?.Position.TotalSeconds ?? 0)
            );
        }

        public PamelloQueue(PamelloPlayer parentPlayer, IServiceProvider services) {
            _parentPlayer = parentPlayer;

			_events = services.GetRequiredService<PamelloEventsService>();
            _songs = services.GetRequiredService<PamelloSongRepository>();

            _isRandom = false;
            _isReversed = false;
            _isNoLeftovers = true;

            SongAudios = new List<PamelloAudio>();
		}

        public PamelloSong? AddSong(PamelloSong song) {
            return InsertSong(SongAudios.Count, song);
        }
        public void AddPlaylist(PamelloPlaylist playlist) {
            InsertPlaylist(SongAudios.Count, playlist);
        }

        public void InsertPlaylist(int position, PamelloPlaylist playlist) {
			var insertPosition = NormalizeQueuePosition(position, true);

            var queueWasEmpty = SongAudios.Count == 0;
            var positionMustBeMoved = insertPosition <= Position;

            var playlistSongs = playlist.Songs;
            foreach (var song in playlistSongs) {
				SongAudios.Insert(insertPosition++, new PamelloAudio(song));
            }

			if (queueWasEmpty) {
				Current = SongAudios.FirstOrDefault();
            }
			else if (positionMustBeMoved) {
				Position += playlistSongs.Count;
            }

            SendQueueUpdatedEvent();
        }

        public PamelloSong? InsertSong(int position, PamelloSong song) {
			var songAudio = new PamelloAudio(song);

            var insertPosition = NormalizeQueuePosition(position, true);
            SongAudios.Insert(insertPosition, songAudio);

			if (SongAudios.Count == 1) {
				Current = SongAudios.FirstOrDefault();
			}
			else if (insertPosition <= Position) Position++;

			SendQueueUpdatedEvent();

			return song;
		}

        private void SendQueueUpdatedEvent() {
			_events.SendToAllWithSelectedPlayer(
				_parentPlayer.Id,
				PamelloEvent.PlayerQueueListUpdated(
					SongAudios.Select(audio => audio.Song.Id)
				)
			);
        }

        public PamelloSong RemoveSong(int songPosition) {
            if (SongAudios.Count == 0) throw new PamelloException("Queue is empty");

            PamelloSong? song;
			if (SongAudios.Count == 1) {
				song = SongAudios.First().Song;
				Clear();
                return song;
			}

			songPosition = NormalizeQueuePosition(songPosition);

			song = SongAudios[songPosition].Song;
			
			if (Position == songPosition) GoToNextSong(true);
			else {
                SongAudios.RemoveAt(songPosition);
                if (songPosition < Position) Position--;
                SendQueueUpdatedEvent();
            }

            return song;
		}
		public bool MoveSong(int fromPosition, int toPosition) {
			if (SongAudios.Count < 2) return false;

			fromPosition = NormalizeQueuePosition(fromPosition);
			toPosition = NormalizeQueuePosition(toPosition, true);

			if (fromPosition == toPosition) return true;

            var buffer = SongAudios[fromPosition];
            SongAudios.RemoveAt(fromPosition);
			if (fromPosition < toPosition) toPosition--;
            SongAudios.Insert(toPosition, buffer);

            if (fromPosition == Position) Position = toPosition;
            else if (fromPosition < Position && Position <= toPosition) {
				Position--;
			}
			else if (fromPosition > Position && Position >= toPosition) {
                Position++;
            }

            SendQueueUpdatedEvent();

            return true;
		}
		public bool SwapSongs(int inPosition, int withPosition) {
			if (SongAudios.Count <= 1) return false;

			inPosition = NormalizeQueuePosition(inPosition);
			withPosition = NormalizeQueuePosition(withPosition);

			if (inPosition == withPosition) return false;

			var buffer = SongAudios[inPosition];
			SongAudios[inPosition] = SongAudios[withPosition];
			SongAudios[withPosition] = buffer;

            SendQueueUpdatedEvent();

			if (inPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = inPosition;

            return true;
		}
		public PamelloSong GoToSong(int songPosition, bool returnBack = false) {
			if (SongAudios.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = NormalizeQueuePosition(songPosition);
			if (returnBack && Position != nextPosition) NextPositionRequest = Position;

            Position = nextPosition;
			Current = SongAudios[Position];

			return Current.Song;
		}
		public PamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false) {
			if (SongAudios.Count == 0) return null;

            int nextPosition;

			if (NextPositionRequest is not null) {
				nextPosition = NextPositionRequest.Value;
				NextPositionRequest = null;
			}
			else if (IsRandom && SongAudios.Count > 1) do {
                nextPosition = Random.Shared.Next(0, SongAudios.Count);
            } while (Position == nextPosition);
			else if (IsReversed) nextPosition = Position - 1;
			else nextPosition = Position + 1;

			if (forceRemoveCurrentSong || IsNoLeftovers && Current is not null) {
                SongAudios.RemoveAt(Position);
				if (nextPosition > Position) nextPosition--;

				SendQueueUpdatedEvent();
            }

            nextPosition = NormalizeQueuePosition(nextPosition);

			if (SongAudios.Count == 0) {
				Current = null;
				return null;
			}

			Position = nextPosition;
			Current = SongAudios[Position];

			return Current.Song;
		}

		private int NormalizeQueuePosition(int position, bool includeLastEmpty = false) {
			if (SongAudios.Count == 0) return 0;

			position %= SongAudios.Count + (includeLastEmpty ? 1 : 0);
			if (position < 0) position += SongAudios.Count;

			return position;
		}

		public void Clear() {
			SongAudios.Clear();

			Current = null;
            Position = 0;

            SendQueueUpdatedEvent();
        }
		public void Shuffle() => throw new NotImplementedException();
	}
}
