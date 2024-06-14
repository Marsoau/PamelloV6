
using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using PamelloV6.Server.Handlers;
using PamelloV6.Server.Services;
using PamelloV6.DAL;
using PamelloV6.API.Modules;
using Microsoft.EntityFrameworkCore;
using PamelloV6.DAL.Entity;

namespace PamelloV6.API
{
    public class Program
	{
		public static Task Main(string[] args) => new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			ConfigureAPIServices(builder.Services);
			ConfigureDiscordServices(builder.Services);
			ConfigurePamelloServices(builder.Services);
			ConfigureDatabaseServices(builder.Services);

			var app = builder.Build();

			await StartupDiscordServicesAsync(app.Services);
			await StartupPamelloServicesAsync(app.Services);

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment()) {
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			
			app.UseHttpsRedirection();
			app.UseAuthorization();

			app.MapControllers();
			app.Run();
		}

		public void ConfigureAPIServices(IServiceCollection services) {
			// Add services to the container.
			services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
		}

		public void ConfigureDiscordServices(IServiceCollection services) {
            var discordConfig = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

			services.AddSingleton(services => new DiscordSocketClient(
				discordConfig
			));
			services.AddSingleton(services => new InteractionService(
				services.GetRequiredService<DiscordSocketClient>()
			));
			services.AddSingleton<InteractionHandler>();
		}

		public void ConfigurePamelloServices(IServiceCollection services) {
			services.AddSingleton<InteractionHandler>();

			services.AddSingleton<DiscordClientService>();
			services.AddSingleton<UserAuthorizationService>();
			services.AddSingleton<PamelloUserService>();

			services.AddTransient<PamelloCommandsModule>();
		}

		public void ConfigureDatabaseServices(IServiceCollection services) {
			services.AddSingleton<DatabaseContext>();
		}

		public async Task StartupDiscordServicesAsync(IServiceProvider services) {
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

		public async Task StartupPamelloServicesAsync(IServiceProvider services) {

		}

		public void StartupDatabaseServices(IServiceProvider services) {
			var database = services.GetRequiredService<DatabaseContext>();

			database.Users
				.Include(user => user.OwnedPlaylists);
			database.Playlists
				.Include(playlist => playlist.Songs)
				.Include(playlist => playlist.Owner);
			database.Songs
				.Include(song => song.Playlists)
				.Include(song => song.Episodes);
			database.Episodes
				.Include(episode => episode.Song);
		}
	}
}
