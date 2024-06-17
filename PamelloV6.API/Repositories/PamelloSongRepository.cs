using Discord;
using Microsoft.EntityFrameworkCore;
using PamelloV6.API.Model;
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

			//LoadAll();
		}

        public override PamelloSong? Get(int songId)
        {
            var song = _list.FirstOrDefault(song => song.Entity.Id == songId);
            if (song is not null) return song;

            var entity = _databaseSongs.FirstOrDefault(song => song.Id == songId);
            if (entity is null) return null;

            return Load(entity);
		}

		public async Task<PamelloSong?> Add(string youtubeId) {
			YoutubeVideoInfo youtubeInfo;
			try {
				youtubeInfo = await _youtube.GetVideoInfo(youtubeId);
			}
			catch (Exception x) {
				Console.WriteLine($"Error ocured while attempting to get youtube video info:\n{x}");
				return null;
			}

			var entity = new SongEntity() {
				Title = youtubeInfo.Name,
				Author = youtubeInfo.Author,
				CoverUrl = $"https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg",
				SourceUrl = $"https://www.youtube.com/watch?v={youtubeId}",
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

			return Load(entity);
		}

		public override void Delete(int songId) => throw new NotImplementedException();

        private void LoadAll()
        {
            foreach (var songEntity in _databaseSongs)
            {
                Load(songEntity);
            }
        }

        private PamelloSong Load(SongEntity songEntity)
        {
            var song = _list.FirstOrDefault(song => song.Entity.Id == songEntity.Id);
            if (song is not null) return song;

            song = new PamelloSong(songEntity, _services);
			_list.Add(song);

            Console.WriteLine($"Loaded song: {song}");

            return song;
        }
    }
}
