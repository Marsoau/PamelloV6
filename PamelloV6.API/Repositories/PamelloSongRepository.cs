using Discord;
using Microsoft.EntityFrameworkCore;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Services;
using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;
using PamelloV6.Server.Services;

namespace PamelloV6.API.Repositories
{
    public class PamelloSongRepository : PamelloRepository<PamelloSong>
    {
        private readonly YoutubeInfoService _youtube;

		private List<SongEntity> _databaseSongs {
			get => _database.Songs
				.Include(song => song.Playlists)
				.Include(song => song.Episodes)
                .ToList();
		}

		public PamelloSongRepository(YoutubeInfoService youtube,
			IServiceProvider services
		) : base(services) {
            _youtube = youtube;
		}

		public override PamelloSong? Get(int songId)
        {
            var song = _list.FirstOrDefault(song => song.Id == songId);
            if (song is not null) return song;

            var entity = _databaseSongs.FirstOrDefault(song => song.Id == songId);
            if (entity is null) return null;

            return Load(entity);
		}

		public PamelloSong? GetByName(string name) {
			var song = _list.FirstOrDefault(song => song.Name == name);
			if (song is not null) return song;

			var entity = _databaseSongs.FirstOrDefault(song => song.Title == name);
			if (entity is null) return null;

			return Load(entity);
		}
		public PamelloSong? GetByYoutubeId(string youtubeId) {
			var song = _list.FirstOrDefault(song => song.YoutubeId == youtubeId);
			if (song is not null) return song;

			var entity = _databaseSongs.FirstOrDefault(song => song.YoutubeId == youtubeId);
			if (entity is null) return null;

			return Load(entity);
        }
        public async Task<PamelloSong> GetByValue(string songValue, bool useYoutubeId = true, bool createIfUrlProvided = false) {
            PamelloSong song;

			if (int.TryParse(songValue, out var songId)) {
                var s = Get(songId);
                if (s is null) throw new Exception($"Song with id \"{songId}\" not found in database");

                song = s;
            }
            else if (useYoutubeId && songValue.StartsWith("http")) {
                var youtubeId = _youtube.GetVideoIdFromUrl(songValue);

                var s = GetByYoutubeId(youtubeId);
                if (s is null) {
                    if (createIfUrlProvided) {
                        s = await Add(youtubeId);
                    }
                    else throw new Exception($"Song with youtube id \"{youtubeId}\" not found in database");
                }

                song = s;
            }
            else {
                var s = GetByName(songValue);
                if (s is null) throw new Exception($"Song with name \"{songValue}\" not found in database");

                song = s;
            }

            return song;
        }

        public async Task<PamelloSong> Add(string youtubeId) {
			if (_list.Any(song => song.YoutubeId == youtubeId)) throw new Exception("This song is already in database");

			YoutubeVideoInfo youtubeInfo;
			try {
				youtubeInfo = await _youtube.GetVideoInfo(youtubeId);
			}
			catch (Exception x) {
				throw new Exception($"Error occured while attempting to get youtube video {youtubeId} info");
			}

			var entity = new SongEntity() {
				Title = youtubeInfo.Name,
				Author = youtubeInfo.Author,
				CoverUrl = $"https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg",
				YoutubeId = youtubeId,
				PlayCount = 0,
				IsDownloaded = false,
				Playlists = []
			};

			entity.Episodes = youtubeInfo.Episodes.Select(episodeInfo => new EpisodeEntity() {
				Name = episodeInfo.Name,
				Song = entity,
				Start = episodeInfo.Start,
			}).ToList();

			_database.Songs.Add(entity);
			_database.SaveChanges();

            var song = Load(entity);

            _events.SendToAll(PamelloEvent.SongCreated(song.Id));

            return song;
		}

		public override void Delete(int songId) => throw new NotImplementedException();

        public void LoadAll() {
            foreach (var songEntity in _databaseSongs) {
                Load(songEntity);
            }
        }

        private PamelloSong Load(SongEntity songEntity) {
            var song = _list.FirstOrDefault(song => song.Entity.Id == songEntity.Id);
            if (song is not null) return song;

            song = new PamelloSong(songEntity, _services);
			_list.Add(song);

            Console.WriteLine($"Loaded song: {song}");

            return song;
        }
    }
}
