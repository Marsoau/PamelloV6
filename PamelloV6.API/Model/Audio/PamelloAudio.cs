﻿using Discord.Audio;
using PamelloV6.API.Downloads;
using PamelloV6.API.Exceptions;
using System.Diagnostics;

namespace PamelloV6.API.Model.Audio
{
	public delegate void AudioInitializationProgressDelegate(int progress);

    public class PamelloAudio
    {
        public readonly PamelloSong Song;

		public MemoryStream? _audioStream;

		public AudioTime Position { get; private set; }
        public AudioTime Duration { get; private set; }

        public int? nextBreakPoint { get; private set; }
        public int? nextJumpPoint { get; private set; }

		public event AudioInitializationProgressDelegate? OnInitializationProgress;

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
                if (await Song.StartDownload() != DownloadResult.Success) {
                    Position.TimeValue = 0;
                    Duration.TimeValue = 0;
                    return false;
                }
            }

			_audioStream = await CreateAudioStream();

			if (_audioStream is null) {
				Position.TimeValue = 0;
				Duration.TimeValue = 0;
				return false;
			}

			Position.TimeValue = _audioStream.Position;
			Duration.TimeValue = _audioStream.Length;
			Song.PlayCount++;

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

			var ffmpegProcess = Process.Start(new ProcessStartInfo {
				FileName = "ffmpeg",
				Arguments = $@"-hide_banner -loglevel panic -i ""C:\.PamelloV6Data\Music\{Song.Id}.mp4"" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});

			if (ffmpegProcess is null) throw new PamelloException("Couldnt start a ffmpeg process");

            var ffmpegStream = ffmpegProcess.StandardOutput.BaseStream;

			if (ffmpegStream is null) {
				return null;
			}

			var memoryStream = new MemoryStream();

            var lengthLimit = AudioTime.FrequencyMultiplier * (3600) * 3;
            var min8 = AudioTime.FrequencyMultiplier * 60 * 8;
			var last10minCount = 0;

            int nextByte;
			while ((nextByte = ffmpegStream.ReadByte()) != -1 && memoryStream.Length < lengthLimit) {
				memoryStream.WriteByte((byte)nextByte);

				if (memoryStream.Length / min8 > last10minCount) {
					OnInitializationProgress?.Invoke(++last10minCount);
                }
            }

            memoryStream.Position = 0;

            if (last10minCount > 0) {
                OnInitializationProgress?.Invoke(0);
            }

            ffmpegStream.Close();
            ffmpegProcess.Close();

			return memoryStream;
		}
	}
}
