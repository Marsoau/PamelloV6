using Discord.WebSocket;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;

namespace PamelloV6.Server.Model
{
	public class PamelloUser
	{
		private readonly DatabaseContext _database;

		public readonly UserEntity UserEntity;
		public readonly SocketUser DiscordUser;

		private int _selectedPlayerId;
		public int SelectedPlayerId {
			get => _selectedPlayerId;
			set {
				_selectedPlayerId = value;
                Console.WriteLine($"[{this}] selected player {SelectedPlayerId}");
            }
		}

		public PamelloUser(UserEntity userEntity, SocketUser discordUser,
			DatabaseContext database
		) {
			_database = database;

			UserEntity = userEntity;
			DiscordUser = discordUser;
		}

		public void Save() => _database.SaveChanges();

		public override string ToString() {
			return $"{UserEntity.Id}: {DiscordUser.Username}";
		}
	}
}
