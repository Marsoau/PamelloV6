using Discord;
using Discord.Audio;
using Discord.WebSocket;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Services;

namespace PamelloV6.API.Model.Audio
{
	public class PamelloSpeaker
    {
        private readonly PamelloPlayer _parentPlayer;
        public readonly DiscordSocketClient DiscordClient;
        public readonly SocketGuild Guild;

		public SocketVoiceChannel? VoiceChannel { get; private set; }

        private Stream? _audioOutput;

        public ulong DiscordClientUserId {
            get => DiscordClient.CurrentUser.Id;
        }

		public bool IsConnected {
			get =>
                VoiceChannel is not null &&
                Guild.AudioClient is not null &&
                Guild.AudioClient.ConnectionState == ConnectionState.Connected &&
                _audioOutput is not null;
		}

        public event Func<Task>? Connected;
        public event Func<Task>? Disconected;

        public PamelloSpeaker(PamelloPlayer parentPlayer, DiscordSocketClient discordClient, ulong guildId) {
            _parentPlayer = parentPlayer;
            DiscordClient = discordClient;
            Guild = discordClient.GetGuild(guildId) ??
                throw new Exception($"Attempted to create speaker in guild without specified discord client");

            DiscordClient.UserVoiceStateUpdated += UserVoiceStateUpdated;
        }

        private async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState fromVC, SocketVoiceState toVC) {
            if (user.Id == DiscordClient.CurrentUser.Id) {
                Console.WriteLine($"Player \"{_parentPlayer.Name}\" speaker in {Guild.Name} guild: {fromVC} -> {toVC}");

                VoiceChannel = toVC.VoiceChannel;

                if (VoiceChannel is not null) {
                    SubscribeACEvents();
                }
            }
        }

        public void SubscribeACEvents() {
            if (Guild.AudioClient is null) return;
            Console.WriteLine("Subscribing");

            Guild.AudioClient.Connected += AudioClient_Connected;
            Guild.AudioClient.Disconnected += AudioClient_Disconnected;
        }

        private async Task AudioClient_Connected() {
            _audioOutput = Guild.AudioClient.CreatePCMStream(AudioApplication.Mixed);
            Connected?.Invoke();
        }
        private async Task AudioClient_Disconnected(Exception arg) {
            _audioOutput = null;
            Disconected?.Invoke();
        }

        public async Task Connect(ulong vcId) {
            VoiceChannel = Guild.GetVoiceChannel(vcId);
            if (VoiceChannel is null) return;

            await VoiceChannel.ConnectAsync();
        }
        public async Task Disconnect() {

        }

        public void PlayBytes(byte[] audioBytes) {
            try {
                _audioOutput?.Write(audioBytes);
            }
            catch {
                _audioOutput = null;
            }
        }
	}
}
