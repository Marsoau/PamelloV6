using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Abstract;
using PamelloV6.Server.Model;
using PamelloV6.Server.Modules;

namespace PamelloV6.Server.Services
{
	public class PamelloUserService : IPamelloUserService
	{
		private readonly DiscordClientService _discordClientService;
		private readonly DatabaseContext _databaseContext;

		private readonly Dictionary<int, ulong> _userCodes;
		private readonly List<PamelloUser> _users;

		public PamelloUserService(DiscordClientService discordClientService, DatabaseContext databaseContext) {
			_discordClientService = discordClientService;
			_databaseContext = databaseContext;

			_userCodes = new Dictionary<int, ulong>();
			_users = new List<PamelloUser>();
		}

		public PamelloUser? GetUser(int id) {
			var user = _users.Find(user => user.UserEntity.Id == id);
			if (user is not null) return user;

			var userEntity = _databaseContext.Users.Find(id);
			if (userEntity is null) return null;

			return AddUser(userEntity);
		}

		public PamelloUser? GetUser(Guid token) {
			var user = _users.Find(user => user.UserEntity.Token == token);
			if (user is not null) return user;

			var userEntity = _databaseContext.Users.FirstOrDefault(user => user.Token == token);
			if (userEntity is null) return null;

			return AddUser(userEntity);
		}
		public PamelloUser? GetUser(ulong discordId) {
			var user = _users.Find(user => user.UserEntity.DiscordId == discordId);
			if (user is not null) return user;

			var userEntity = _databaseContext.Users.FirstOrDefault(user => user.DiscordId == discordId);
			if (userEntity is not null) return AddUser(userEntity);

			userEntity = new UserEntity() {
				DiscordId = discordId,
				Token = Guid.NewGuid(),
				OwnedPlaylists = [],
				IsAdministrator = false,
			};
			_databaseContext.Users.Add(userEntity);
			_databaseContext.SaveChanges();

			return AddUser(userEntity);
		}

		private void LoadUsers() {
			foreach (var userEntity in _databaseContext.Users) {
				AddUser(userEntity);
			}
		}

		private PamelloUser? AddUser(UserEntity userEntity) {
			var discordUser = _discordClientService.MainDiscordClient.GetUser(userEntity.DiscordId);
			if (discordUser is null) return null;

			var user = new PamelloUser(userEntity, discordUser);
			_users.Add(user);

			return user;
		}
	}
}
