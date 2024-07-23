using Discord.Audio;
using Discord.WebSocket;

namespace PamelloV6.API.Model.Audio
{
	public class PamelloSpeaker
    {
        private readonly DiscordSocketClient _discordClient;

		private IAudioClient? _ac;
        private Stream? _audioOutputStream;

		public bool IsConnected {
			get =>
				_audioOutputStream is not null &&
				_ac is not null &&
				_ac.ConnectionState == Discord.ConnectionState.Connected;
		}

        public PamelloSpeaker(DiscordSocketClient discordClient) {
			_discordClient = discordClient;
		}

		public async Task Connect(SocketVoiceChannel voiceChannel) {
			_ac = await voiceChannel.ConnectAsync();
			_audioOutputStream = _ac.CreatePCMStream(AudioApplication.Mixed);
		}

		public void PlayBytes(byte[] audioBytes) {
			if (!IsConnected) return;
			_audioOutputStream?.Write(audioBytes);
        }
	}
}
