using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV6.API.Model.Interactions;
using PamelloV6.API.Modules;
using PamelloV6.API.Repositories;
using PamelloV6.Server.Model;
using System.Reflection;

namespace PamelloV6.Server.Handlers
{
	public class InteractionHandler {
		private readonly DiscordSocketClient _client;
		private readonly InteractionService _commands;
		private readonly PamelloUserRepository _users;

		private readonly IServiceProvider _services;

		public InteractionHandler(
			DiscordSocketClient client,
			InteractionService discordCommands,
			PamelloUserRepository users,

			IServiceProvider services
		) {
			_client = client;
			_commands = discordCommands;
			_users = users;

			_services = services;
		}

		public async Task InitializeAsync() {
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			_client.InteractionCreated += _client_InteractionCreated;
		}

		private async Task _client_InteractionCreated(SocketInteraction interaction) {
			try {
				await HandleInteraction(interaction);
			}
			catch (Exception x) {
				Console.WriteLine("ERROR with interaction");
                Console.WriteLine($"Message: {x.Message}");
                Console.WriteLine($"More: {x}");
                await interaction.RespondAsync("An error occured, check the console for more info", ephemeral: true);
			}
		}

		private async Task HandleInteraction(SocketInteraction interaction) {
			await interaction.DeferAsync(true);

			var pamelloUser = _users.Get(interaction.User.Id);
			if (pamelloUser is null) {
				await interaction.ModifyOriginalResponseAsync(message => message.Content = $"Unexpected error");
				throw new Exception($"Cant execute discord command with null PamelloUser, discord user id: {interaction.User.Id}");
			}

			var context = new SocketPamelloInteractionContext(
				pamelloUser,
				_client,
				interaction,
				_services
			);

            if ((await _commands.ExecuteCommandAsync(context, _services)).IsSuccess) {
				//await interaction.ModifyOriginalResponseAsync(message => message.Content = $"Command executed");
			}
			else {
				await interaction.ModifyOriginalResponseAsync(message => message.Content = $"Error ocured");
            }
		}
	}
}
