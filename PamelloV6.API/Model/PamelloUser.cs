using Discord.WebSocket;
using PamelloV6.API.Model;
using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;

namespace PamelloV6.Server.Model
{
	public class PamelloUser : PamelloEntity {
		internal readonly UserEntity Entity;
		public readonly SocketUser DiscordUser;

		public override int Id {
			get => Entity.Id;
		}

		public Guid Token {
			get => Entity.Token;
		}

		public bool IsAdministrator {
			get => Entity.IsAdministrator;
			set {
				Entity.IsAdministrator = value;
				Save();
			}
		}

		public List<PamelloPlaylist> OwnedPlaylists {
			get => Entity.OwnedPlaylists.Select(playlistEntity => _playlists.Get(playlistEntity.Id)
				?? throw new Exception("Attempted to get song that doesnt exist")).ToList();
		}

		private int _selectedPlayerId;
		public int SelectedPlayerId {
			get => _selectedPlayerId;
			set {
				_selectedPlayerId = value;
                Console.WriteLine($"[{this}] selected player {SelectedPlayerId}");
            }
		}

		public PamelloUser(UserEntity userEntity, SocketUser discordUser,
			IServiceProvider services
		) : base(services) {
			Entity = userEntity;
			DiscordUser = discordUser;
		}

		public PamelloPlaylist CreatePlaylist(string name, bool isProtected = true) {
			return _playlists.Add(name, isProtected, this);
		}

		public override string ToString() {
			return $"[U: {Id} | {DiscordUser.Id}] {DiscordUser.GlobalName}";
		}

		public override object GetDTO() {
			return new UserDTO() {
				Id = Id,
				DiscordId = DiscordUser.Id,
				IsAdministrator = IsAdministrator,

				OwnedPlaylistIds = (OwnedPlaylists ?? []).Select(playlist => playlist.Id),
			};
		}
	}
}
