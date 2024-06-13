using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace PamelloV6.Server.Handlers
{
	public class SocketPamelloInteractionContext : SocketInteractionContext
	{
		public object PamelloUser;

		public SocketPamelloInteractionContext(object user, DiscordSocketClient client, SocketInteraction interaction) : base(client, interaction) {
			PamelloUser = user;
		}
	}

	public class InteractionHander {
		private readonly DiscordSocketClient _client;
		private readonly InteractionService _commands;
		private readonly IServiceProvider _services;

		public InteractionHander(DiscordSocketClient client, InteractionService commands, IServiceProvider services) {
			_client = client;
			_commands = commands;
			_services = services;
		}

		public async Task InitializeAsync() {
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			_client.InteractionCreated += HandleInteraction;
		}

		private async Task HandleInteraction(SocketInteraction interaction) {
			var context = new SocketPamelloInteractionContext(
				"",
				_client,
				interaction
			);
			await _commands.ExecuteCommandAsync(context, _services);
		}
	}
}
