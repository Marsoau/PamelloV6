using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using PamelloV6.API.Model;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace PamelloV6.API.Downloads
{
	public enum DownloadResult {
		Success,
		CantStart,
		NoYoutubeIdArg,
		NoDestinationArg,
		AgeRestriction,
	}

	public delegate void OnProgressDelegate(double progress);
	public delegate void OnStartDelegate();
	public delegate void OnEndDelegate(DownloadResult downloadResult);

	public class SongDownloader
	{
		public readonly PamelloSong Song;

		public event OnProgressDelegate? OnProgress;
		public event OnStartDelegate? OnStart;
		public event OnEndDelegate? OnEnd;

		public bool IsDownloading { get; private set; }

		public SongDownloader(PamelloSong song) {
			Song = song;
		}

		public async Task<DownloadResult> DownloadFromYoutubeAsync(string youtubeVideoId) {
			IsDownloading = true;
			OnStart?.Invoke();

			Process process = new Process();
			process.StartInfo = new ProcessStartInfo() {
				FileName = "python",
				Arguments = $"Scripts/downloader.py {youtubeVideoId} \"C:\\.PamelloV6Data\\Music\\{Song.Entity.Id}.mp4\"",
				UseShellExecute = false,
				RedirectStandardOutput = true
			};

            Console.WriteLine($"Started fownload of {youtubeVideoId}");
            if (!process.Start()) {
				return DownloadResult.CantStart;
			}

			await process.WaitForExitAsync();
            Console.WriteLine($"Download end of {youtubeVideoId}");

            IsDownloading = false;

			var finalResult = (DownloadResult)process.ExitCode;
			OnEnd?.Invoke(finalResult);

			return finalResult;
		}
		public void CatchUpload() => throw new NotImplementedException();
	}
}
