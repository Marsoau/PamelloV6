using Discord.WebSocket;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
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

                _events.SendToOne(Id, "updatedIsAdministrator", selectedPlayer?.Id);
            }
		}

		public List<PamelloPlaylist> OwnedPlaylists {
			get => Entity.OwnedPlaylists.Select(playlistEntity => _playlists.Get(playlistEntity.Id)
				?? throw new Exception("Attempted to get song that doesnt exist")).ToList();
		}

		private PamelloPlayer? _selectedPlayer;
		public PamelloPlayer? selectedPlayer {
			get => _selectedPlayer;
			set {
				_selectedPlayer = value;
				_events.SendToOne(Id, "updatedSelectedPlayer", selectedPlayer?.Id);
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
				Name = DiscordUser.GlobalName,
				DiscordId = DiscordUser.Id,
				IsAdministrator = IsAdministrator,

				OwnedPlaylistIds = OwnedPlaylists?.Select(playlist => playlist.Id) ?? [],
			};
		}
	}
}
