using Discord.WebSocket;
using PamelloV6.DAL.Entity;

namespace PamelloV6.Server.Model
{
	public class PamelloUser
	{
		public readonly UserEntity UserEntity;
		public readonly SocketUser DiscordUser;

		public int SelectedPlayerId { get; set; }

		public PamelloUser(UserEntity userEntity, SocketUser discordUser) {
			UserEntity = userEntity;
			DiscordUser = discordUser;
		}
	}
}
