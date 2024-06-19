using AngleSharp.Text;
using PamelloV6.API.Repositories;

namespace PamelloV6.API.Model.Audio
{
    public class PamelloQueue
    {
        public readonly PamelloSongRepository _songs;

        public readonly List<PamelloAudio> SongAudios;

        private int _position;
        public int Position {
            get => _position;
            set {
				GoToSong(value);
            }
        }

		public bool IsRandom;
		public bool IsReversed;
		public bool IsNoLeftovers;

		public int? NextPositionRequest;

		private PamelloAudio? _current;
		public PamelloAudio? Current {
			get => _current;
			set {
				_current?.Clean();
				_current = value;
			}
		}

        public PamelloQueue(IServiceProvider services) {
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

            if (position <= _position) _position++;

			return song;
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
			if (_position == songPosition) GoToNextSong();

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
			
			if (fromPosition == _position) _position = toPosition;
			else if (toPosition == _position) _position++;

			return true;
		}
		public bool SwapSong(int fromPosition, int withPosition) {
			if (SongAudios.Count <= 1) return false;

			fromPosition = NormalizeQueuePosition(fromPosition);
			withPosition = NormalizeQueuePosition(withPosition);

			if (fromPosition == withPosition) return false;

			var buffer = SongAudios[fromPosition];
			SongAudios[fromPosition] = SongAudios[withPosition];
			SongAudios[withPosition] = buffer;

			return true;
		}
		public PamelloSong? GoToSong(int songPosition, bool returnBack = false) {
			if (SongAudios.Count == 0) return null;

			var nextPosition = NormalizeQueuePosition(songPosition);

			if (_position == nextPosition) return Current?.Song;

			if (returnBack) NextPositionRequest = _position;

			_position = nextPosition;
			Current = SongAudios[_position];

			return Current.Song;
		}
		public PamelloSong? GoToNextSong() {
			if (SongAudios.Count == 0) return null;

			int nextPosition;

			if (NextPositionRequest is not null) {
				nextPosition = NextPositionRequest.Value;
				NextPositionRequest = null;
			}
			else if (IsRandom) nextPosition = Random.Shared.Next(0, SongAudios.Count - 1);
			else if (IsReversed) nextPosition = _position - 1;
			else nextPosition = _position + 1;

			nextPosition = NormalizeQueuePosition(nextPosition);

			if (IsNoLeftovers) {
				SongAudios.RemoveAt(_position);
				if (nextPosition > _position) nextPosition--;
			}

			if (SongAudios.Count == 0) {
				Current = null;
				return null;
			}

			_position = nextPosition;
			Current = SongAudios[_position];

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
			_position = 0;
		}
		public void Shuffle() => throw new NotImplementedException();
	}
}
