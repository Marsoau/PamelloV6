using AngleSharp.Dom;
using PamelloV6.API.Downloads;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using System.Web;

namespace PamelloV6.API.Model
{
	public class PamelloSong
	{
		private readonly DatabaseContext _database;
		private readonly SongDownloader _downloader;
		private Task<DownloadResult>? _downloadTask;

		public readonly SongEntity Entity;
		public bool IsDownloaded {
			get => Entity.IsDownloaded && File.Exists(@$"C:\.PamelloV6Data\Music\{Entity.Id}.mp4") && !IsDownloading;
		}
		public bool IsDownloading {
			get => _downloadTask is not null;
		}

		public PamelloSong(SongEntity songEntity,
			DatabaseContext database
		) {
			_database = database;

			Entity = songEntity;

			_downloader = new SongDownloader(this);
			_downloader.OnStart += Downloader_OnStart;
			_downloader.OnEnd += Downloader_OnEnd;
		}

		private void Downloader_OnStart() {
			Entity.IsDownloaded = false;
			Save();

			Console.WriteLine($"Starting download of song {this}");
        }

		private void Downloader_OnEnd(DownloadResult downloadResult) {
			Entity.IsDownloaded = downloadResult == DownloadResult.Success;
			_downloadTask = null;
			Save();

			Console.WriteLine($"Download of song {this} ended with {downloadResult}");
        }

		public void Save() => _database.SaveChanges();

		public Task<DownloadResult> StartDownload() {
			if (_downloadTask is not null) {
				return _downloadTask;
			}

			var souceUri = new Uri(Entity.SourceUrl);
			if (!(souceUri.Host is "www.youtube.com" or "youtu.be")) {
				throw new Exception("Only youtube source are suported for auto downloading");
			}
			
			var youtubeId = HttpUtility.ParseQueryString(souceUri.Query)["v"]
				?? throw new Exception("Cant find youtube id in url");

			_downloadTask = Task.Run(() => _downloader.DownloadFromYoutubeAsync(youtubeId));
			return _downloadTask;
		}

		public override string ToString() {
			return $"{Entity.Title} ({Entity.Id})";
		}
	}
}
