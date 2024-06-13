using Discord.Interactions;
using PamelloV6.Server.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Server.Modules
{
	public class InteractionModule : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		[SlashCommand("ping", "Check if bot is alive")]
		public async Task Ping() {
			await RespondAsync("Pong", ephemeral: true);
		}
	}
}
