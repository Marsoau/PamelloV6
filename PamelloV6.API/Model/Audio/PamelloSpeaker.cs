using Discord;
using Discord.Audio;
using Discord.WebSocket;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Services;

namespace PamelloV6.API.Model.Audio
{
	public class PamelloSpeaker
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly PamelloEventsService _events;
        private readonly PamelloPlayer _parentPlayer;

        private IAudioClient? _ac;
        private Stream? _audioOutputStream;

		public SocketVoiceChannel? VoiceChannel { get; private set; }

		public bool IsConnected {
			get =>
                VoiceChannel is not null &&
                _audioOutputStream is not null &&
				_ac is not null &&
				_ac.ConnectionState == Discord.ConnectionState.Connected;
		}

        public PamelloSpeaker(PamelloPlayer player, IServiceProvider services) {
            _parentPlayer = player;

            _discordClient = services.GetRequiredService<DiscordSocketClient>();
            _events = services.GetRequiredService<PamelloEventsService>();
        }

        public async Task Connect(SocketVoiceChannel voiceChannel) {
            await Disconnect();

            VoiceChannel = voiceChannel;
            _ac = await VoiceChannel.ConnectAsync();
            _audioOutputStream = _ac.CreatePCMStream(AudioApplication.Mixed);
            
            _events.SendToAllWithSelectedPlayer(_parentPlayer.Id, PamelloEvent.PlayerSpeakerConnected(
                VoiceChannel.Guild.Name,
                VoiceChannel.Name
            ));
        }
        public async Task Disconnect() {
            if (VoiceChannel is null) return;

            await VoiceChannel.DisconnectAsync();
            VoiceChannel = null;
            _ac = null;
            _audioOutputStream = null;

            _events.SendToAllWithSelectedPlayer(_parentPlayer.Id, PamelloEvent.PlayerSpeakerDisconnected());
        }

        public void PlayBytes(byte[] audioBytes) {
			if (!IsConnected) return;
			_audioOutputStream?.Write(audioBytes);
        }
	}
}
