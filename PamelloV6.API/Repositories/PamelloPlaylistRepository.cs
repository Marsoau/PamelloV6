using PamelloV6.DAL.Entity;
using PamelloV6.DAL;
using Microsoft.EntityFrameworkCore;
using PamelloV6.API.Model;
using PamelloV6.Server.Model;
using Discord;
using AngleSharp.Dom.Events;
using PamelloV6.API.Model.Events;

namespace PamelloV6.API.Repositories
{
	public class PamelloPlaylistRepository : PamelloRepository<PamelloPlaylist>
	{
		private List<PlaylistEntity> _databasePlaylists {
			get => _database.Playlists
				.Include(playlist => playlist.Songs)
				.Include(playlist => playlist.Owner)
				.ToList();
		}

		public PamelloPlaylistRepository(IServiceProvider services) : base(services) {

		}

		public override PamelloPlaylist? Get(int id) {
			var playlist = _list.FirstOrDefault(playlist => playlist.Id == id);
			if (playlist is not null) return playlist;

			var entity = _databasePlaylists.FirstOrDefault(playlist => playlist.Id == id);
			if (entity is null) return null;

			return Load(entity);
		}

		public PamelloPlaylist Add(string name, bool isProtected, PamelloUser owner) {
			var playlistEntity = new PlaylistEntity() {
				Name = name,
				Owner = owner.Entity,
				IsProtected = isProtected,
				Songs = [],
			};

			_database.Playlists.Add(playlistEntity);
			_database.SaveChanges();

            var playlist = Load(playlistEntity);

			_events.SendToAll(PamelloEvent.PlaylistCreated(playlist.Id));

            return playlist;
		}

        public override void Delete(int id) => throw new NotImplementedException();

        public void LoadAll() {
			foreach (var playlist in _databasePlaylists) {
				Load(playlist);
			}
		}

		private PamelloPlaylist Load(PlaylistEntity playlistEntity) {
			var playlist = _list.FirstOrDefault(playlist => playlist.Id == playlistEntity.Id);
			if (playlist is not null) return playlist;

			playlist = new PamelloPlaylist(playlistEntity, _services);
			_list.Add(playlist);

			Console.WriteLine($"Loaded playlist: {playlist}");

			return playlist;
		}
	}
}
