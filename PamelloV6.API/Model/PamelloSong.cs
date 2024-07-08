﻿using AngleSharp.Dom;
using PamelloV6.API.Downloads;
using PamelloV6.API.Model.Events;
using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using System.Web;

namespace PamelloV6.API.Model
{
	public class PamelloSong : PamelloEntity
	{
		private readonly SongDownloader _downloader;
		private Task<DownloadResult>? _downloadTask;

		internal readonly SongEntity Entity;

		public override int Id {
			get => Entity.Id;
		}
		public override string Name {
			get => Entity.Title;
			set {
				Entity.Title = value;
                Save();

				_events.SendToAll(
					PamelloEvent.SongNameUpdated(Id, Name)
				);
            }
		}
		public string Author {
			get => Entity.Author;
			set {
				Entity.Author = value;
				Save();

                _events.SendToAll(
                    PamelloEvent.SongAuthorUpdated(Id, Author)
                );
            }
		}
		public string CoverUrl { //remove
			get => Entity.CoverUrl;
			set {
				Entity.CoverUrl = value;
				Save();
            }
		}
		public string YoutubeId { //change
			get => Entity.SourceUrl;
			set {
				Entity.SourceUrl = value;
				Save();
			}
		}
		public int PlayCount {
			get => Entity.PlayCount;
			set {
                Entity.PlayCount = value;
				Save();

                _events.SendToAll(
                    PamelloEvent.SongPlayCountUpdated(Id, PlayCount)
                );
            }
		}

		public List<PamelloEpisode> Episodes {
			get => Entity.Episodes.Select(episodesEntity => _episodes.Get(episodesEntity.Id)
				?? throw new Exception("Attempted to get song that doesnt exist")).ToList();
		}
		public List<PamelloPlaylist> Playlists {
			get => Entity.Playlists.Select(playlistEntity => _playlists.Get(playlistEntity.Id)
				?? throw new Exception("Attempted to get song that doesnt exist")).ToList();
		}

		public bool IsDownloaded {
			get => Entity.IsDownloaded && File.Exists(@$"C:\.PamelloV6Data\Music\{Id}.mp4") && !IsDownloading;
		}
		public bool IsDownloading {
			get => _downloadTask is not null;
		}

		public PamelloSong(SongEntity songEntity,
			IServiceProvider services
		) : base(services) {
			Entity = songEntity;

			_downloader = new SongDownloader(this);
			_downloader.OnStart += Downloader_OnStart;
			_downloader.OnEnd += Downloader_OnEnd;
		}

		private void Downloader_OnStart() {
			Entity.IsDownloaded = false;
			Save();

			Console.WriteLine($"Starting download of song {this}");

            _events.SendToAll(PamelloEvent.SongDownloadStarted(Id));
        }

		private void Downloader_OnEnd(DownloadResult downloadResult) {
			Entity.IsDownloaded = downloadResult == DownloadResult.Success;
			_downloadTask = null;
			Save();

            _events.SendToAll(PamelloEvent.SongDownloadEnded(Id, downloadResult));
        }

		public Task<DownloadResult> StartDownload(bool forceDownload = false) {
			if (Entity.IsDownloaded && !forceDownload) {
				return Task.FromResult(DownloadResult.Success);
			}

			if (_downloadTask is not null) {
				return _downloadTask;
			}

			var souceUri = new Uri(YoutubeId);
			if (!(souceUri.Host is "www.youtube.com" or "youtu.be")) {
				throw new Exception("Only youtube source are suported for auto downloading");
			}
			
			var youtubeId = HttpUtility.ParseQueryString(souceUri.Query)["v"]
				?? throw new Exception("Cant find youtube id in url");

			_downloadTask = Task.Run(() => _downloader.DownloadFromYoutubeAsync(youtubeId));
			return _downloadTask;
		}

		public PamelloEpisode CreateEpisode(string name, int start, bool skip = false) {
			return _episodes.Add(name, start, skip, this);
		}

		public override string ToString() {
			return $"[S: {Id}] {Name}";
		}

		public override object GetDTO() {
			return new SongDTO() {
				Id = Id,
				Title = Name,
				Author = Author,
				CoverUrl = CoverUrl,
				SourceUrl = YoutubeId,
				PlayCount = PlayCount,
				IsDownloaded = IsDownloaded,

				EpisodeIds = Episodes.Select(episodes => episodes.Id).ToList(),
				PlaylistIds = Playlists.Select(playlist => playlist.Id).ToList()
			};
		}
	}
}
