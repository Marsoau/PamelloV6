using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV6.Server.Handlers;
using PamelloV6.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Server.Modules
{
	public class InteractionModule : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		private readonly UserAuthorizationService _authtorization;

		public InteractionModule(UserAuthorizationService authtorization) {
			_authtorization = authtorization;
		}

		[SlashCommand("ping", "Check if bot is alive")]
		public async Task Ping() {
			await RespondAsync("Pong", ephemeral: true);
		}

		[SlashCommand("get-code", "Get authorisation code")]
		public async Task GetCode() {
			await RespondAsync($"Authrozation Code: {_authtorization.GetCode(Context.Interaction.User.Id)}", ephemeral: true);
		}
	}
}
