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
    public class PamelloSongRepository
    {
        private readonly DatabaseContext _database;
        private readonly YoutubeInfoService _youtube;

        private readonly List<PamelloSong> _songs;

		private List<SongEntity> _databaseSongs {
			get => _database.Songs
				.Include(song => song.Playlists)
				.Include(song => song.Episodes)
                .ToList();
		}

		public PamelloSongRepository(DatabaseContext database, YoutubeInfoService youtube)
        {
            _database = database;
            _youtube = youtube;

            _songs = new List<PamelloSong>();
        }

		public PamelloSong AddSong(SongDTO songDTO) {
			var entity = new SongEntity() {
				Title = songDTO.Title,
				Author = songDTO.Author,
				CoverUrl = songDTO.CoverUrl,
				SourceUrl = songDTO.SourceUrl,
				PlayCount = 0,
				IsDownloaded = false,
				Playlists = [],
				Episodes = [],
			};

			_database.Songs.Add(entity);
			_database.SaveChanges();

			return AddSong(entity);
		}

		public async Task<PamelloSong?> AddSong(string youtubeId)
        {
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

            return AddSong(entity);
		}

        public PamelloSong? GetSong(int songId)
        {
            var song = _songs.FirstOrDefault(song => song.Entity.Id == songId);
            if (song is not null) return song;

            var entity = _databaseSongs.FirstOrDefault(song => song.Id == songId);
            if (entity is null) return null;

            return AddSong(entity);
        }

        public void RemoveSong(int songId)
        {
            var song = _songs.FirstOrDefault(song => song.Entity.Id == songId);
            if (song is null) return;

            _database.Songs.Remove(song.Entity);
            _songs.Remove(song);
        }

        private void LoadSongs()
        {
            foreach (var songEntity in _databaseSongs)
            {
                AddSong(songEntity);
            }
        }

        private PamelloSong AddSong(SongEntity songEntity)
        {
            var song = _songs.FirstOrDefault(song => song.Entity.Id == songEntity.Id);
            if (song is not null) return song;

            song = new PamelloSong(songEntity, _database);
            _songs.Add(song);

            return song;
        }
    }
}
