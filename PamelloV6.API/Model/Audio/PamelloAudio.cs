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

        public int? nextBreakPoint { get; private set; }
        public int? nextJumpPoint { get; private set; }

        public bool IsInitialized {
			get => Song.IsDownloaded && _audioStream is not null;
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

            UpdatePlaybackPoints();

			Console.WriteLine($"Duration: {Duration}");

            return true;
        }

		public void Clean() {
			_audioStream?.Dispose();
			_audioStream = null;

            nextBreakPoint = null;
            nextJumpPoint = null;

            Position.TimeValue = 0;
			Duration.TimeValue = 0;
		}

        public void UpdatePlaybackPoints(bool excludeCurrent = false) {
            int? closestBreakPoint = null;
            int? closestJumpPoint = null;

            foreach (var episode in Song.Episodes) {
				if (
					(excludeCurrent) ?
					(episode.Start.TotalSeconds > Position.TotalSeconds) :
					(episode.Start.TotalSeconds >= Position.TotalSeconds)
				) {
					if (episode.Skip) {
                        if (closestBreakPoint is null || episode.Start.TotalSeconds < closestBreakPoint) {
                            closestBreakPoint = episode.Start.TotalSeconds;
                        }
                    }
					else if (episode.Start.TotalSeconds > closestBreakPoint) {
                        if (closestJumpPoint is null || episode.Start.TotalSeconds < closestJumpPoint) {
                            closestJumpPoint = episode.Start.TotalSeconds;
                        }
                    }
				}
			}

            nextBreakPoint = closestBreakPoint;
            nextJumpPoint = closestJumpPoint;

            Console.WriteLine($"Updated playback points: [{nextBreakPoint}] | [{nextJumpPoint}]");
        }

		public byte[]? NextBytes() {
			if (_audioStream is null) return null;

			if (Position.TotalSeconds == nextBreakPoint) {
                if (nextJumpPoint is null) {
                    _audioStream.Position = _audioStream.Length - 1;
                    Position.TimeValue = _audioStream.Position;

                    return null;
                }

				RewindTo(new AudioTime(nextJumpPoint.Value));
            }

			int[] ints = [_audioStream.ReadByte(), _audioStream.ReadByte()];

			if (ints[0] != -1 && ints[1] != -1) {
				Position.TimeValue += 2;
				return [(byte)ints[0], (byte)ints[1]];
			}

			return null;
		}
		public void RewindTo(AudioTime time) {
			if (_audioStream is null || time.TimeValue > _audioStream.Length - 1) return;

            _audioStream.Position = time.TimeValue;
            Position.TimeValue = time.TimeValue;

			UpdatePlaybackPoints(true);
        }
        public void RewindToEpisode(int episodePosition) {
			var episode = Song.Episodes[episodePosition];
			if (episode is null) return;

			RewindTo(episode.Start);
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

			await ffmpegStream.CopyToAsync(memoryStream);
			memoryStream.Position = 0;

			return memoryStream;
		}
	}
}
