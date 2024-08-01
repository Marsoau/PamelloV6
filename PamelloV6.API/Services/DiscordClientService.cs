using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
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
			DiscordClients = new DiscordSocketClient[2];

            DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
            DiscordClients[1] = services.GetRequiredKeyedService<DiscordSocketClient>("Speaker1");
        }
	}
}
