using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV6.API.Model.Interactions;
using PamelloV6.API.Modules;
using PamelloV6.API.Repositories;
using PamelloV6.Server.Model;
using System.Reflection;
using PamelloV6.API.Extensions;
using PamelloV6.API.Model.Interactions.Builders;
using PamelloV6.API.Exceptions;
using Discord;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;

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

			_client.InteractionCreated += InteractionCreated;
            _commands.SlashCommandExecuted += SlashCommandExecuted;
        }

        private async Task InteractionCreated(SocketInteraction interaction) {
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
			await interaction.RespondAsync(embed: PamelloEmbedBuilder.BuildWait(), ephemeral: true);

			var pamelloUser = _users.Get(interaction.User.Id);
			if (pamelloUser is null) {
                await interaction.RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError("Unexpected error occured"));
				throw new PamelloException($"Cant execute discord command with null PamelloUser, discord user id: {interaction.User.Id}");
			}

			var context = new SocketPamelloInteractionContext(
				pamelloUser,
				_client,
				interaction,
				_services
			);

			await _commands.ExecuteCommandAsync(context, _services);
        }

        private async Task SlashCommandExecuted(SlashCommandInfo commandInfo, IInteractionContext context, Discord.Interactions.IResult result) {
			SocketInteraction interaction = context.Interaction as SocketInteraction ?? throw new Exception("tf");

			if (!result.IsSuccess && result is ExecuteResult executionResult) {
                if (executionResult.Exception?.InnerException is PamelloException pamelloException) {
                    if (context is SocketPamelloInteractionContext pamelloContext && pamelloContext.lastFollowupResponce is not null) {
                        await pamelloContext.lastFollowupResponce.ModifyAsync(message =>
							message.Embed = PamelloEmbedBuilder.BuildError(pamelloException.Message)
                        );
                    }
                    else await interaction.RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(pamelloException.Message));
                }
                else {
                    await interaction.RespondWithEmbedAsync(PamelloEmbedBuilder.BuildException("Unidentified error occured"));
                    Console.WriteLine(executionResult.Exception?.InnerException);
                }
            }
        }
    }
}
