using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PamelloV6.Server
{
	public class Program
	{
		public static Task Main() => new Program().MainAsync();
		public async Task MainAsync()
		{
			var hostBuilder = Host.CreateDefaultBuilder();

			hostBuilder.ConfigureServices(services => {
				services.AddSingleton(x => new DiscordSocketClient(
					new DiscordSocketConfig() {
						GatewayIntents = GatewayIntents.All,
						AlwaysDownloadUsers = true
					}
				));
			});

			await RunPamelloAsync(hostBuilder.Build());
		}

		public async Task RunPamelloAsync(IHost host) {
			using var serviceScope = host.Services.CreateScope();
			var provider = serviceScope.ServiceProvider;

			await RunBotsAsync(provider);

			await Task.Delay(-1);
		}

		public async Task RunBotsAsync(IServiceProvider provider) {
			var MainDiscordClient = provider.GetRequiredService<DiscordSocketClient>();

			MainDiscordClient.Log += async (message) => {
				Console.WriteLine(message);
			};

			await MainDiscordClient.LoginAsync(TokenType.Bot, "MTI1MDc2MzM0NjcxNDY5MzYzMg.GqF3b4.OVu84ru-0_-RtKUwcrQchAppjZgxaHUgnu_5yw");
			await MainDiscordClient.StartAsync();

		}
	}
}
