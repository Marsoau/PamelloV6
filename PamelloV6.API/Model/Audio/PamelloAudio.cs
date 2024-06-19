using Discord.Audio;
using PamelloV6.API.Downloads;
using System.Diagnostics;

namespace PamelloV6.API.Model.Audio
{
    public class PamelloAudio
    {
        public readonly PamelloSong Song;

		public MemoryStream? _audioStream;

		public AudioTime Position { get; private set; }
        public AudioTime Duration { get; private set; }
		public PamelloEpisode? CurrentEpisode { get; private set; }
		public PamelloEpisode? NextEpisode { get; private set; }

		public bool IsInitialized {
			get => _audioStream is not null;
		}

		public PamelloAudio(PamelloSong song) {
            Song = song;

			Position = new AudioTime(0);
			Duration = new AudioTime(0);
		}

        public async Task<bool> TryInitialize() {
            if (!Song.IsDownloaded) {
                await Song.StartDownload();
            }

			_audioStream = await CreateAudioStream();

			if (_audioStream is null) {
				Position.TimeValue = 0;
				Duration.TimeValue = 0;
				return false;
			}

			Position.TimeValue = _audioStream.Position;
			Duration.TimeValue = _audioStream.Length;

			var firstEpisode = Song.Episodes.FirstOrDefault();
			if (firstEpisode is null) {
				CurrentEpisode = null;
				NextEpisode = null;
			}
			else if (firstEpisode.Start.TotalSeconds == 0) {
				CurrentEpisode = firstEpisode;
				NextEpisode = GetNextEpisode();
			}
			else {
				CurrentEpisode = null;
				NextEpisode = firstEpisode;
			}

			Console.WriteLine($"Starting from {CurrentEpisode?.ToString() ?? "Start"} and {NextEpisode?.ToString() ?? "End"}");

			Console.WriteLine(Duration);

            return true;
        }

		public void Clean() {
			_audioStream?.Dispose();
			_audioStream = null;

			CurrentEpisode = null;
			NextEpisode = null;

			Position.TimeValue = 0;
			Duration.TimeValue = 0;
		}

		public PamelloEpisode? GetNextEpisode() {
			if (CurrentEpisode is null) return null;

			PamelloEpisode? closestEpisode = null;
			foreach (var episode in Song.Episodes) {
				if ((episode.Start.TotalSeconds > CurrentEpisode.Start.TotalSeconds)
					&& (
						(closestEpisode is null)
						||
						(episode.Start.TotalSeconds < closestEpisode.Start.TotalSeconds)
					)
				) {
					closestEpisode = episode;
				}
			}

			return closestEpisode;
		}

		public byte[]? NextBytes() {
			if (_audioStream is null) return null;

			while (
				(Position.TotalSeconds >= NextEpisode?.Start.TotalSeconds)
				||
				(CurrentEpisode?.Skip ?? false)
			) {
                Console.WriteLine($"Switching from {CurrentEpisode?.ToString() ?? "Start"} to {NextEpisode?.ToString() ?? "End"}");
                SwitchToNextEpisode();
			}

			int[] ints = [_audioStream.ReadByte(), _audioStream.ReadByte()];

			if (ints[0] != -1 && ints[1] != -1) {
				Position.TimeValue += 2;
				return [(byte)ints[0], (byte)ints[1]];
			}

			return null;
		}
		public void SwitchToNextEpisode() {
			if (_audioStream is null) return;

			CurrentEpisode = NextEpisode;
			NextEpisode = GetNextEpisode();

			if (CurrentEpisode is not null) _audioStream.Position = CurrentEpisode.Start.TimeValue;
			else _audioStream.Position = _audioStream.Length - 1;

			Position.TimeValue = _audioStream.Position;
		}

        public async Task<MemoryStream?> CreateAudioStream() {
            if (!Song.IsDownloaded) {
                return null;
			}

			Stream? ffmpegStream = Process.Start(new ProcessStartInfo {
				FileName = "ffmpeg",
				Arguments = $@"-hide_banner -loglevel panic -i ""C:\.PamelloV6Data\Music\{Song.Id}.mp4"" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			})?.StandardOutput.BaseStream;

			if (ffmpegStream is null) {
				return null;
			}

			var memoryStream = new MemoryStream();

			ffmpegStream.CopyTo(memoryStream);
			memoryStream.Position = 0;

			return memoryStream;
		}
	}
}
