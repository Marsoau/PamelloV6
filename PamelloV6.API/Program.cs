
using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using PamelloV6.Server.Handlers;
using PamelloV6.Server.Services;
using PamelloV6.DAL;
using PamelloV6.API.Modules;
using Microsoft.EntityFrameworkCore;
using PamelloV6.DAL.Entity;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using PamelloV6.API.Config;

namespace PamelloV6.API
{
    public class Program
	{
		public static Task Main(string[] args) => new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			ConfigureAPIServices(builder.Services);
            ConfigureDatabaseServices(builder.Services);
            ConfigureDiscordServices(builder.Services);
			ConfigurePamelloServices(builder.Services);

			var app = builder.Build();

			StartupDatabaseServices(app.Services);

            await StartupPamelloServicesAsync(app.Services);

            await StartupDiscordServicesAsync(app.Services);

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment()) {
				/*
				app.UseSwagger();
				app.UseSwaggerUI();
				*/
			}

			app.UseHttpsRedirection();
            //app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();
			app.Run();
		}

		public void ConfigureAPIServices(IServiceCollection services) {
			// Add services to the container.
			services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.AddHttpClient();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
        }

        public void ConfigureDiscordServices(IServiceCollection services) {
            var discordConfig = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

            services.AddSingleton(new DiscordSocketClient(discordConfig));
            services.AddKeyedSingleton("Speaker1", new DiscordSocketClient(discordConfig));

            services.AddSingleton(services => new InteractionService(
				services.GetRequiredService<DiscordSocketClient>(),
				new InteractionServiceConfig() {
					//DefaultRunMode = RunMode.Sync,
					//ThrowOnError = true,
				}
			));
			services.AddSingleton<InteractionHandler>();
		}

		public void ConfigurePamelloServices(IServiceCollection services) {
			services.AddSingleton<InteractionHandler>();

			services.AddSingleton<DiscordClientService>();
			services.AddSingleton<UserAuthorizationService>();
			services.AddSingleton<PamelloEventsService>();

			services.AddSingleton<PamelloUserRepository>();
			services.AddSingleton<PamelloSongRepository>();
			services.AddSingleton<PamelloEpisodeRepository>();
			services.AddSingleton<PamelloPlaylistRepository>();
			services.AddSingleton<PamelloPlayerRepository>();

			services.AddTransient<PamelloCommandsModule>();

			services.AddTransient<YoutubeInfoService>();
		}

		public void ConfigureDatabaseServices(IServiceCollection services) {
			services.AddSingleton<DatabaseContext>();
		}

		public async Task StartupDiscordServicesAsync(IServiceProvider services) {
			var MainDiscordClient = services.GetRequiredService<DiscordSocketClient>();
            var S1DiscordClient = services.GetRequiredKeyedService<DiscordSocketClient>("Speaker1");

            var interactionService = services.GetRequiredService<InteractionService>();

			await services.GetRequiredService<InteractionHandler>().InitializeAsync();

			MainDiscordClient.Log += async (message) => {
				Console.WriteLine(message);
			};

			var discordReady = new TaskCompletionSource();
			MainDiscordClient.Ready += async () => {
                /*
				var guild = MainDiscordClient.GetGuild(PamelloConfig.TestGuildId);

				foreach (var command in await guild.GetApplicationCommandsAsync()) {
                    Console.WriteLine($"Deleting {command.Name} command");
                    //await command.DeleteAsync();
                }
				*/
                Console.WriteLine($"Registering commands");
                await interactionService.RegisterCommandsGloballyAsync();

                discordReady.SetResult();
			};

			await MainDiscordClient.LoginAsync(TokenType.Bot, PamelloConfig.BotToken);
			await MainDiscordClient.StartAsync();

            await S1DiscordClient.LoginAsync(TokenType.Bot, PamelloConfig.Speaker1Token);
            await S1DiscordClient.StartAsync();

            await discordReady.Task;
		}

		public async Task StartupPamelloServicesAsync(IServiceProvider services) {
			var users = services.GetRequiredService<PamelloUserRepository>();
			var songs = services.GetRequiredService<PamelloSongRepository>();
			var episodes = services.GetRequiredService<PamelloEpisodeRepository>();
			var playlists = services.GetRequiredService<PamelloPlaylistRepository>();

			users.LoadAll();
			songs.LoadAll();
			episodes.LoadAll();
			playlists.LoadAll();

            var authorisation = services.GetRequiredService<UserAuthorizationService>();
            var commands = services.GetRequiredService<PamelloCommandsModule>();

            Console.WriteLine(authorisation.GetCode(544933092503060509));

			//Console.WriteLine("=======================================");
            //Console.WriteLine(commands.GetTSString());
            //Console.WriteLine("=======================================");
        }

		public void StartupDatabaseServices(IServiceProvider services) {
			var database = services.GetRequiredService<DatabaseContext>();
		}
	}
}

/*

freeorder-list component: 40min
song/playlist name/author edit in inspector: 20min



*/