using Discord.Audio;

namespace PamelloV6.API.Model.Audio
{
	public class PamelloSpeaker
	{
		public Stream OutputStream;

		public PamelloSpeaker(IAudioClient ac) {
			OutputStream = ac.CreatePCMStream(AudioApplication.Mixed);
		}

		public void PlayBytes(byte[] audioBytes) {
			OutputStream.Write(audioBytes);
		}
	}
}
