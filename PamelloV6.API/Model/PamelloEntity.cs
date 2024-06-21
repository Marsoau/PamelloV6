using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.DAL;

namespace PamelloV6.API.Model
{
	public abstract class PamelloEntity
    {
        private readonly DatabaseContext _database;

        protected readonly PamelloEventsService _events;

        protected readonly PamelloUserRepository _users;
		protected readonly PamelloSongRepository _songs;
		protected readonly PamelloEpisodeRepository _episodes;
		protected readonly PamelloPlaylistRepository _playlists;

		public abstract int Id { get; }

		public PamelloEntity(IServiceProvider services) {
			_database = services.GetRequiredService<DatabaseContext>();

			_events = services.GetRequiredService<PamelloEventsService>();

			_users = services.GetRequiredService<PamelloUserRepository>();
			_songs = services.GetRequiredService<PamelloSongRepository>();
			_episodes = services.GetRequiredService<PamelloEpisodeRepository>();
			_playlists = services.GetRequiredService<PamelloPlaylistRepository>();
		}

		protected void Save() => _database.SaveChanges();
		protected void SendUpdate(string header) => _events.SendToAll(header, Id);

		public abstract object GetDTO();
	}
}
