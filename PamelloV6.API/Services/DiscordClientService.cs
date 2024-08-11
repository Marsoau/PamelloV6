using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV6.API.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Server.Services
{
	public class DiscordClientService
	{
		public DiscordSocketClient[] DiscordClients;

		public DiscordSocketClient MainDiscordClient {
			get => DiscordClients[0];
		}

		public DiscordClientService(IServiceProvider services) {
			DiscordClients = new DiscordSocketClient[PamelloConfig.SpeakersTokens.Length + 1];

            DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
			for (int i = 0; i < PamelloConfig.SpeakersTokens.Length; i++) {
				DiscordClients[i + 1] = services.GetRequiredKeyedService<DiscordSocketClient>($"Speaker-{i}");
			}
        }
	}
}
