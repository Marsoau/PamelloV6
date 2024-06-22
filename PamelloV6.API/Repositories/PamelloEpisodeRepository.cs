using PamelloV6.API.Model;
using PamelloV6.API.Services;
using PamelloV6.DAL.Entity;
using PamelloV6.DAL;
using Microsoft.EntityFrameworkCore;

namespace PamelloV6.API.Repositories
{
	public class PamelloEpisodeRepository : PamelloRepository<PamelloEpisode>
	{
		private List<EpisodeEntity> _databaseEpisodes {
			get => _database.Episodes
				.Include(episode => episode.Song)
				.ToList();
		}

		public PamelloEpisodeRepository(IServiceProvider services) : base(services) {

		}

		public override PamelloEpisode? Get(int id) {
			var episode = _list.FirstOrDefault(episode => episode.Id == id);
			if (episode is not null) return episode;

			var entity = _databaseEpisodes.FirstOrDefault(episode => episode.Id == id);
			if (entity is null) return null;
			
			return Load(entity);
		}

		public PamelloEpisode Add(string name, int start, bool skip, PamelloSong song) {
			var episodeEntity = new EpisodeEntity() {
				Name = name,
				Start = start,
				Song = song.Entity,
				Skip = skip
			};

			_database.Episodes.Add(episodeEntity);
			_database.SaveChanges();

			var episode = Load(episodeEntity);

            _events.SendToAll("episodeCreated", episode.Id);

            return episode;
		}

		public override void Delete(int id) => throw new NotImplementedException();

		public void LoadAll() {
			foreach (var episode in _databaseEpisodes) {
				Load(episode);
			}
		}

		private PamelloEpisode Load(EpisodeEntity entity) {
			var episode = _list.FirstOrDefault(episode => episode.Entity.Id == entity.Id);
			if (episode is not null) return episode;

			episode = new PamelloEpisode(entity, _services);
			_list.Add(episode);

			Console.WriteLine($"Loaded episode: {episode}");

			return episode;
		}
	}
}
