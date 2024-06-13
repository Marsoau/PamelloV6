using Discord.Interactions;
using Discord.WebSocket;
using PamelloV6.Server.Model;
using PamelloV6.Server.Services;
using System.Reflection;

namespace PamelloV6.Server.Handlers
{
	public class SocketPamelloInteractionContext : SocketInteractionContext
	{
		public readonly PamelloUser PamelloUser;

		public readonly IServiceProvider Services;

		public SocketPamelloInteractionContext(
			PamelloUser pamelloUser,
			DiscordSocketClient client,
			SocketInteraction interaction,
			
			IServiceProvider services
		) : base(client, interaction) {
			PamelloUser = pamelloUser;

			Services = services;
		}
	}

	public class InteractionHandler {
		private readonly DiscordSocketClient _client;
		private readonly InteractionService _commands;
		private readonly PamelloUserService _users;

		private readonly IServiceProvider _services;

		public InteractionHandler(
			DiscordSocketClient client,
			InteractionService commands,
			PamelloUserService users,

			IServiceProvider services
		) {
			_client = client;
			_commands = commands;
			_users = users;

			_services = services;
		}

		public async Task InitializeAsync() {
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			_client.InteractionCreated += HandleInteraction;
		}

		private async Task HandleInteraction(SocketInteraction interaction) {
			var pamelloUser = _users.GetUser(interaction.User.Id);
			if (pamelloUser is null) {
                Console.WriteLine($"Cant execute command with null PamelloUser, discord user id: {interaction.User.Id}");

                return;
			}

			var context = new SocketPamelloInteractionContext(
				pamelloUser,
				_client,
				interaction,
				_services
			);

			await _commands.ExecuteCommandAsync(context, _services);
		}
	}
}
