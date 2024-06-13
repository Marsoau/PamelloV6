using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV6.DAL;
using PamelloV6.Server.Handlers;
using PamelloV6.Server.Services;
using System;

namespace PamelloV6.Server
{
	public class Program
	{
		public static Task Main() => new Program().MainAsync();
		public async Task MainAsync()
		{
			var hostBuilder = Host.CreateDefaultBuilder();

			hostBuilder.ConfigureServices(services => {
				services.AddSingleton(services => new DiscordSocketClient(
					new DiscordSocketConfig() {
						GatewayIntents = GatewayIntents.All,
						AlwaysDownloadUsers = true
					}
				));

				services.AddSingleton(services => new InteractionService(
					services.GetRequiredService<DiscordSocketClient>()
				));
				services.AddSingleton<InteractionHandler>();

				services.AddSingleton<DiscordClientService>();
				services.AddSingleton<UserAuthorizationService>();
				services.AddSingleton<PamelloUserService>();

				services.AddSingleton<DatabaseContext>();
			});

			await RunPamelloAsync(hostBuilder.Build());
		}

		public async Task RunPamelloAsync(IHost host) {
			using var serviceScope = host.Services.CreateScope();
			var provider = serviceScope.ServiceProvider;

			await RunBotsAsync(provider);

			await Task.Delay(-1);
		}

		public async Task RunBotsAsync(IServiceProvider services) {
			var MainDiscordClient = services.GetRequiredService<DiscordSocketClient>();
			var interactionService = services.GetRequiredService<InteractionService>();

			await services.GetRequiredService<InteractionHandler>().InitializeAsync();

			MainDiscordClient.Log += async (message) => {
				Console.WriteLine(message);
			};
			MainDiscordClient.Ready += async () => {
				await interactionService.RegisterCommandsToGuildAsync(1250768227542241450);
			};

			await MainDiscordClient.LoginAsync(TokenType.Bot, "MTI1MDc2MzM0NjcxNDY5MzYzMg.GqF3b4.OVu84ru-0_-RtKUwcrQchAppjZgxaHUgnu_5yw");
			await MainDiscordClient.StartAsync();

		}
	}
}
