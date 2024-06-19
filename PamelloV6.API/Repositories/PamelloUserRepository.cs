using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;
using PamelloV6.API.Model;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;
using PamelloV6.Server.Modules;
using PamelloV6.Server.Services;

namespace PamelloV6.API.Repositories
{
    public class PamelloUserRepository : PamelloRepository<PamelloUser>
    {
        private readonly DiscordClientService _discordClientService;

        private List<UserEntity> _databaseUsers {
            get => _database.Users.Include(user => user.OwnedPlaylists).ToList();
		}

        public PamelloUserRepository(DiscordClientService discordClientService,
            IServiceProvider services
        ) : base(services) {
            _discordClientService = discordClientService;

            //LoadAll();
        }

		public override PamelloUser? Get(int id)
        {
            var user = _list.Find(user => user.Entity.Id == id);
            if (user is not null) return user;

            var userEntity = _databaseUsers.FirstOrDefault(user => user.Id == id);
            if (userEntity is null) return null;

            return Load(userEntity);
        }

        public PamelloUser? Get(Guid token)
        {
            var user = _list.Find(user => user.Entity.Token == token);
            if (user is not null) return user;

            var userEntity = _databaseUsers.FirstOrDefault(user => user.Token == token);
            if (userEntity is null) return null;

            return Load(userEntity);
        }

        public PamelloUser? Get(ulong discordId)
        {
            var user = _list.Find(user => user.Entity.DiscordId == discordId);
            if (user is not null) return user;

            var userEntity = _databaseUsers.FirstOrDefault(user => user.DiscordId == discordId);
            if (userEntity is not null) return Load(userEntity);

            userEntity = new UserEntity()
            {
                DiscordId = discordId,
                Token = Guid.NewGuid(),
                OwnedPlaylists = [],
                IsAdministrator = false,
            };
            _database.Users.Add(userEntity);
            _database.SaveChanges();

            return Load(userEntity);
        }

		public override void Delete(int id) => throw new NotImplementedException();

		private void LoadAll()
        {
            foreach (var userEntity in _databaseUsers)
            {
                Load(userEntity);
            }
        }

        private PamelloUser? Load(UserEntity userEntity)
        {
            var discordUser = _discordClientService.MainDiscordClient.GetUser(userEntity.DiscordId);
            if (discordUser is null) return null;

            var user = new PamelloUser(userEntity, discordUser, _services);
            _list.Add(user);

			Console.WriteLine($"Loaded user: {user}");

			return user;
        }
    }
}
