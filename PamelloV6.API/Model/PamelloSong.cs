using AngleSharp.Dom;
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
		}
		public string YoutubeId { //change
			get => Entity.YoutubeId;
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

			_downloadTask = Task.Run(() => _downloader.DownloadFromYoutubeAsync(YoutubeId));
			return _downloadTask;
		}

		public PamelloEpisode CreateEpisode(string name, int start, bool skip = false) {
			var episode = _episodes.Add(name, start, skip, this);

            _events.SendToAll(
				PamelloEvent.SongEpisodesUpdated(Id, Episodes.Select(episode => episode.Id))
			);

			return episode;
        }

        public void MoveEpisode(int fromPosition, int toPosition) {
            if (toPosition > fromPosition) toPosition--;

            var episode = Entity.Episodes[fromPosition];
            Entity.Episodes.RemoveAt(fromPosition);
            Entity.Episodes.Insert(toPosition, episode);

            Save();

            _events.SendToAll(
                PamelloEvent.SongEpisodesUpdated(Id, Episodes.Select(episode => episode.Id))
            );
        }
        public void SwapEpisode(int fromPosition, int withPosition) {
            var buffer = Entity.Episodes[fromPosition];
            Entity.Episodes[fromPosition] = Entity.Episodes[withPosition];
            Entity.Episodes[withPosition] = buffer;

            Save();

            _events.SendToAll(
                PamelloEvent.SongEpisodesUpdated(Id, Episodes.Select(episode => episode.Id))
            );
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
