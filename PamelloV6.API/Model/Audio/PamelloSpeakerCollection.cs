using Discord.WebSocket;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Services;
using PamelloV6.Server.Services;

namespace PamelloV6.API.Model.Audio
{
	public class PamelloSpeakerCollection
	{
		private readonly DiscordClientService _clients;
		private readonly PamelloEventsService _events;

		private readonly PamelloPlayer _parentPlayer;
		private readonly List<PamelloSpeaker> _speakers;
		public IReadOnlyList<PamelloSpeaker> Speakers {
			get => _speakers;
		}

		public bool IsAnyConnected {
			get {
				foreach (var speaker in _speakers) {
					if (speaker.IsConnected) return true;
				}
				return false;
			}
		}

		public PamelloSpeakerCollection(PamelloPlayer parentPlayer, IServiceProvider services) {
			_clients = services.GetRequiredService<DiscordClientService>();
			_events = services.GetRequiredService<PamelloEventsService>();

			_parentPlayer = parentPlayer;
			_speakers = new List<PamelloSpeaker>();
		}

		public async Task ConnectToUser(ulong userId) {
			var user = _clients.MainDiscordClient.GetUser(userId);
			var guilds = user.MutualGuilds;

			SocketGuildUser guildUser;
			foreach (var guild in guilds) {
				guildUser = guild.GetUser(userId);
				if (guildUser.VoiceChannel is not null) {
					await Connect(guild.Id, guildUser.VoiceChannel.Id);
					return;
				}
			}
		}

		public async Task Connect(ulong guildId, ulong vcId) {
			var freeClient = GetNonBusyDiscordClient(guildId);

			foreach (var speaker in _speakers) {
				if (speaker.DiscordClientUserId == freeClient.CurrentUser.Id && speaker.Guild.Id == guildId) {
					if (speaker.VoiceChannel is not null) throw new Exception($"Unexpected speaker discord client error");
					
					await speaker.Connect(vcId);
					return;
				}

				if (speaker.VoiceChannel?.Id == vcId) {
					throw new Exception($"Player \"{_parentPlayer.Name}\" already connected to this voice channel");
				}
			}

			var newSpreaker = new PamelloSpeaker(_parentPlayer, freeClient, guildId);
			_speakers.Add(newSpreaker);
            SendSpeakersUpdatedEvent();

            newSpreaker.Connected += async (speaker) => {
                SendSpeakersUpdatedEvent();
            };
            newSpreaker.Disconnected += async (speaker) => {
				_speakers.Remove(speaker);

				SendSpeakersUpdatedEvent();
            };

			await newSpreaker.Connect(vcId);
		}

		private void SendSpeakersUpdatedEvent() => _events.SendToAllWithSelectedPlayer(_parentPlayer.Id,
            PamelloEvent.PlayerSpeakersUpdated(Speakers.Select(speaker => speaker.GetDTO()))
        );

        private DiscordSocketClient GetNonBusyDiscordClient(ulong guildId) {
			SocketGuild? guild;
			SocketVoiceChannel? vc;

			foreach (var discordClient in _clients.DiscordClients) {
				guild = discordClient.GetGuild(guildId);
				if (guild is null) continue;

				vc = guild.GetUser(discordClient.CurrentUser.Id)?.VoiceChannel;
				if (vc is null) return discordClient;
			}

			throw new Exception($"No free speakers found in guild {guildId}");
		}

		public void PlayBytes(byte[] audioBytes) {
			foreach (var speaker in _speakers) {
				speaker.PlayBytes(audioBytes);
			}
		}
	}
}
