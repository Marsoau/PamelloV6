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
		public PamelloSong? GetBySource(string sourceUrl) {
			var song = _list.FirstOrDefault(song => song.YoutubeId == sourceUrl);
			if (song is not null) return song;

			var entity = _databaseSongs.FirstOrDefault(song => song.SourceUrl == sourceUrl);
			if (entity is null) return null;

			return Load(entity);
		}

		public async Task<PamelloSong> Add(string youtubeId) {
			var song = _list.FirstOrDefault(song => song.YoutubeId == $"https://www.youtube.com/watch?v={youtubeId}");
			if (song is not null) return song;

			YoutubeVideoInfo youtubeInfo;
			try {
				youtubeInfo = await _youtube.GetVideoInfo(youtubeId);
			}
			catch (Exception x) {
				throw new Exception($"Error ocured while attempting to get youtube video info:\n{x}");
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

            song = Load(entity);

            _events.SendToAll("songCreated", song.Id);

            return Load(entity);
		}

		public override void Delete(int songId) => throw new NotImplementedException();

        public void LoadAll()
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
