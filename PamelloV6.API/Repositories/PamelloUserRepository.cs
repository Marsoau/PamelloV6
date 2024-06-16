using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;
using PamelloV6.Server.Modules;
using PamelloV6.Server.Services;

namespace PamelloV6.API.Repositories
{
    public class PamelloUserRepository
    {
        private readonly DiscordClientService _discordClientService;
        private readonly DatabaseContext _database;

        private readonly Dictionary<int, ulong> _userCodes;
        private readonly List<PamelloUser> _users;

        private List<UserEntity> _databaseUsers {
            get => _database.Users.Include(user => user.OwnedPlaylists).ToList();
		}

        public PamelloUserRepository(DiscordClientService discordClientService, DatabaseContext databaseContext)
        {
            _discordClientService = discordClientService;
            _database = databaseContext;

            _userCodes = new Dictionary<int, ulong>();
            _users = new List<PamelloUser>();

            LoadUsers();
        }

        public PamelloUser? GetUser(int id)
        {
            var user = _users.Find(user => user.Entity.Id == id);
            if (user is not null) return user;

            var userEntity = _databaseUsers.FirstOrDefault(user => user.Id == id);
            if (userEntity is null) return null;

            return AddUser(userEntity);
        }

        public PamelloUser? GetUser(Guid token)
        {
            var user = _users.Find(user => user.Entity.Token == token);
            if (user is not null) return user;

            var userEntity = _databaseUsers.FirstOrDefault(user => user.Token == token);
            if (userEntity is null) return null;

            return AddUser(userEntity);
        }

        public PamelloUser? GetUser(ulong discordId)
        {
            var user = _users.Find(user => user.Entity.DiscordId == discordId);
            if (user is not null) return user;

            var userEntity = _databaseUsers.FirstOrDefault(user => user.DiscordId == discordId);
            if (userEntity is not null) return AddUser(userEntity);

            userEntity = new UserEntity()
            {
                DiscordId = discordId,
                Token = Guid.NewGuid(),
                OwnedPlaylists = [],
                IsAdministrator = false,
            };
            _database.Users.Add(userEntity);
            _database.SaveChanges();

            return AddUser(userEntity);
        }

        private void LoadUsers()
        {
            foreach (var userEntity in _databaseUsers)
            {
                AddUser(userEntity);
            }
        }

        private PamelloUser? AddUser(UserEntity userEntity)
        {
            var discordUser = _discordClientService.MainDiscordClient.GetUser(userEntity.DiscordId);
            if (discordUser is null) return null;

            var user = new PamelloUser(userEntity, discordUser, _database);
            _users.Add(user);

            return user;
        }
    }
}
