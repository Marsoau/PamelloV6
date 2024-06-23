using AngleSharp.Dom.Events;
using AngleSharp.Text;
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
                    "updatedPlayerQueuePosition",
                    Position
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
                    "updatedPlayerQueueIsRandom",
                    IsRandom
                );
            }
        }
		public bool IsReversed {
			get => _isReversed;
			set {
				_isReversed = value;

                _events.SendToAllWithSelectedPlayer(
                    _parentPlayer.Id,
                    "updatedPlayerQueueIsReversed",
                    IsReversed
                );
            }
		}
		public bool IsNoLeftovers {
			get => _isNoLeftovers;
			set {
				_isNoLeftovers = value;

                _events.SendToAllWithSelectedPlayer(
                    _parentPlayer.Id,
                    "updatedPlayerQueueIsNoLeftovers",
                    IsNoLeftovers
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
                    "updatedPlayerNextPositionRequest",
                    NextPositionRequest
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
                }
                _current?.Clean();

				_current = value;
				if (_current is not null) {
					_current.Position.OnSecondTick += OnCurrentPositionSecondTick;
                    _current.Duration.OnSecondTick += OnCurrentDurationSecondTick;
                }
            }
		}

        public void OnCurrentDurationSecondTick() {
            _events.SendToAllWithSelectedPlayer(
                _parentPlayer.Id,
                "updatedPlayerCurrentSongTimeTotal",
                _current.Duration.TotalSeconds
            );
        }
        public void OnCurrentPositionSecondTick() {
            _events.SendToAllWithSelectedPlayer(
                _parentPlayer.Id,
                "updatedPlayerCurrentSongTimePassed",
                _current.Position.TotalSeconds
            );
        }

        public PamelloQueue(PamelloPlayer parentPlayer, IServiceProvider services) {
            _parentPlayer = parentPlayer;

			_events = services.GetRequiredService<PamelloEventsService>();
            _songs = services.GetRequiredService<PamelloSongRepository>();

			SongAudios = new List<PamelloAudio>();
		}

		public PamelloSong? AddSong(int id) {
            return InsertSong(SongAudios.Count, id);
		}

		public PamelloSong? InsertSong(int position, int id) {
			var song = _songs.Get(id);
			if (song is null) return null;

			var songAudio = new PamelloAudio(song);
			SongAudios.Insert(position, songAudio);

			if (SongAudios.Count == 1) {
				Current = SongAudios.FirstOrDefault();
			}
			else if (position <= Position) Position++;

			SendQueueUpdatedEvent();

			return song;
		}

        private void SendQueueUpdatedEvent() {
			_events.SendToAllWithSelectedPlayer(
				_parentPlayer.Id,
                "updatedPlayerQueueSongIds",
                SongAudios.Select(audio => audio.Song.Id)
			);
        }

        public PamelloSong? RemoveSong(int songPosition) {
			if (SongAudios.Count == 0) return null;

			PamelloSong? song;
			if (SongAudios.Count == 1) {
				song = SongAudios.FirstOrDefault()?.Song;
				Clear();
				return song;
			}

			songPosition = NormalizeQueuePosition(songPosition);

			song = SongAudios[songPosition].Song;
			
			SongAudios.RemoveAt(songPosition);
			if (Position == songPosition) GoToNextSong();
			else if (songPosition < Position) Position--;

            SendQueueUpdatedEvent();

            return song;
		}
		public bool MoveSong(int fromPosition, int toPosition) {
			if (SongAudios.Count <= 1) return false;

			fromPosition = NormalizeQueuePosition(fromPosition);
			toPosition = NormalizeQueuePosition(toPosition);

			if (fromPosition == toPosition) return false;

			var buffer = SongAudios[fromPosition];
			SongAudios.RemoveAt(fromPosition);

			//if (fromPosition < toPosition) toPosition--;
			SongAudios.Insert(toPosition, buffer);
			
			if (fromPosition == Position) Position = toPosition;
			else if (toPosition == Position) Position++;

            SendQueueUpdatedEvent();

            return true;
		}
		public bool SwapSongs(int fromPosition, int withPosition) {
			if (SongAudios.Count <= 1) return false;

			fromPosition = NormalizeQueuePosition(fromPosition);
			withPosition = NormalizeQueuePosition(withPosition);

			if (fromPosition == withPosition) return false;

			var buffer = SongAudios[fromPosition];
			SongAudios[fromPosition] = SongAudios[withPosition];
			SongAudios[withPosition] = buffer;

            SendQueueUpdatedEvent();

			if (fromPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = fromPosition;

            return true;
		}
		public PamelloSong? GoToSong(int songPosition, bool returnBack = false) {
			if (SongAudios.Count == 0) return null;

			var nextPosition = NormalizeQueuePosition(songPosition);

			if (Position == nextPosition) return Current?.Song;

			if (returnBack) NextPositionRequest = Position;

            Position = nextPosition;
			Current = SongAudios[Position];

			return Current.Song;
		}
		public PamelloSong? GoToNextSong() {
			if (SongAudios.Count == 0) return null;

            if (IsNoLeftovers) {
                SongAudios.RemoveAt(Position);
                SendQueueUpdatedEvent();
            }

            int nextPosition;

			if (NextPositionRequest is not null) {
				nextPosition = NextPositionRequest.Value;
				NextPositionRequest = null;
			}
			else if (IsRandom) nextPosition = Random.Shared.Next(0, SongAudios.Count - 1);
			else if (IsReversed) nextPosition = Position - 1;
			else nextPosition = Position + 1;

            nextPosition = NormalizeQueuePosition(nextPosition);

            if (IsNoLeftovers && nextPosition > Position) nextPosition--;

			if (SongAudios.Count == 0) {
				Current = null;
				return null;
			}

			Position = nextPosition;
			Current = SongAudios[Position];

			return Current.Song;
		}

		private int NormalizeQueuePosition(int position) {
			position %= SongAudios.Count;
			if (position < 0) position += SongAudios.Count;

			return position;
		}

		public void Clear() {
			SongAudios.Clear();
			Current = null;
            Position = 0;
		}
		public void Shuffle() => throw new NotImplementedException();
	}
}
