using Discord.WebSocket;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Services;
using PamelloV6.Server.Model;
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
			get => _speakers.Any(s => s.IsConnected);
		}

		public PamelloSpeakerCollection(PamelloPlayer parentPlayer, IServiceProvider services) {
			_clients = services.GetRequiredService<DiscordClientService>();
			_events = services.GetRequiredService<PamelloEventsService>();

			_parentPlayer = parentPlayer;
			_speakers = new List<PamelloSpeaker>();
		}

        /*
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

			throw new PamelloException("User is not connected to the voice channel");
		}

		public async Task Connect(ulong guildId, ulong vcId) {
			var freeClient = GetNonBusyDiscordClient(guildId);

			foreach (var speaker in _speakers) {
				if (speaker.DiscordClientUserId == freeClient.CurrentUser.Id && speaker.Guild.Id == guildId) {
					if (speaker.VoiceChannel is not null) throw new PamelloException($"Unexpected speaker discord client error");
					
					await speaker.Connect(vcId);
					return;
				}

				if (speaker.VoiceChannel?.Id == vcId) {
					throw new PamelloException($"Player \"{_parentPlayer.Name}\" already connected to this voice channel");
				}
			}

			var newSpreaker = new PamelloSpeaker(_parentPlayer, freeClient, guildId);
			_speakers.Add(newSpreaker);

            newSpreaker.Connected += async (speaker) => {
                SendSpeakersUpdatedEvent();
            };
            newSpreaker.Disconnected += async (speaker) => {
				_speakers.Remove(speaker);

				SendSpeakersUpdatedEvent();
            };

			await newSpreaker.Connect(vcId);
        }

		*/

        public async Task ConnectSpeakerToUserVc(PamelloUser user) {
            var mutualGuilds = _clients.MainDiscordClient.GetUser(user.DiscordUser.Id).MutualGuilds ?? [];

            SocketGuildUser guildUser;
            foreach (var guild in mutualGuilds) {
                guildUser = guild.GetUser(user.DiscordUser.Id);
                if (guildUser.VoiceChannel is not null) {
                    await ConnectSpeakerToVc(guild.Id, guildUser.VoiceChannel.Id);
                    return;
                }
            }

            throw new PamelloException("User is not connected to the voice channel");
        }
        public async Task ConnectSpeakerToVc(ulong guildId, ulong vcId) {
            var freeClient = GetFreeDiscordClient(guildId);

			await ConnectSpeakerClientToVc(freeClient, guildId, vcId);
        }

        public async Task ConnectSpeakerClientToVc(DiscordSocketClient speakerClient, ulong guildId, ulong vcId) {
            foreach (var speaker in _speakers) {
                if (speaker.DiscordClientUserId == speakerClient.CurrentUser.Id && speaker.Guild.Id == guildId) {
                    if (speaker.VoiceChannel is not null) throw new PamelloException($"Unexpected speaker discord client error");

                    await speaker.Connect(vcId);
                    return;
                }

                if (speaker.VoiceChannel?.Id == vcId) {
                    throw new PamelloException($"Player \"{_parentPlayer.Name}\" already connected to this voice channel");
                }
            }

            var newSpreaker = new PamelloSpeaker(_parentPlayer, speakerClient, guildId);
            _speakers.Add(newSpreaker);

            newSpreaker.Connected += async (speaker) => {
                SendSpeakersUpdatedEvent();
            };
            newSpreaker.Disconnected += async (speaker) => {
                _speakers.Remove(speaker);

                SendSpeakersUpdatedEvent();
            };

            await newSpreaker.Connect(vcId);
        }

        public async Task Disconnect(int speakerPosition) {
            if (speakerPosition < 0 || speakerPosition >= _speakers.Count) {
                throw new PamelloException("Invalid speaker position");
            }

            var speaker = _speakers[speakerPosition];

            await speaker.Disconnect();
            _speakers.Remove(speaker);
        }

        private void SendSpeakersUpdatedEvent() => _events.SendToAllWithSelectedPlayer(_parentPlayer.Id,
            PamelloEvent.PlayerSpeakersUpdated(Speakers.Select(speaker => speaker.GetDTO()))
        );

        private DiscordSocketClient GetFreeDiscordClient(ulong guildId) {
			SocketGuild? guild;

			foreach (var discordClient in _clients.DiscordClients) {
				guild = discordClient.GetGuild(guildId);
				if (guild is null) continue;

				if (guild.AudioClient is null) return discordClient;
			}

			throw new PamelloException($"No free speakers found in guild {guildId}");
		}

		public void PlayBytes(byte[] audioBytes) {
			foreach (var speaker in _speakers) {
				speaker.PlayBytes(audioBytes);
			}
		}
	}
}
