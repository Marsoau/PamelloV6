using Discord.WebSocket;

namespace PamelloV6.API.Model.Audio
{
    public class PamelloSpeakerCollection
    {
        private readonly DiscordSocketClient _client;
        private List<PamelloSpeaker> _speakers;

        public PamelloSpeakerCollection(IServiceProvider services) {
            _client = services.GetRequiredService<DiscordSocketClient>();
        }

        public void Disconnect(uint guildId, uint vcId) {
            var guild = _client.GetGuild(guildId);
            var speakerUser = guild.GetUser(_client.CurrentUser.Id);
            var currentVc = speakerUser.VoiceChannel;

            if (currentVc is null) {
                throw new Exception("All speakers in this guild is already used");
            }
        }
        public void Connect(uint guildId, uint vcId) {
            var guild = _client.GetGuild(guildId);
            var speakerUser = guild.GetUser(_client.CurrentUser.Id);
            var currentVc = speakerUser.VoiceChannel;

            if (currentVc is not null) {
                throw new Exception("All speakers in this guild is already used");
            }

            //var newSpeaker = new PamelloSpeaker(_client, guildId, vcId);
            //_speakers.Add(newSpeaker);
        }
    }
}
