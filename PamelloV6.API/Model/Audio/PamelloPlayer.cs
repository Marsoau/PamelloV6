using AngleSharp.Dom.Events;
using Discord.WebSocket;
using PamelloV6.API.Model.Events;
using PamelloV6.Core.DTO;

namespace PamelloV6.API.Model.Audio
{
    public class PamelloPlayer : PamelloEntity
    {
        private static int nextId = 1;
        public override int Id { get; }

        private string _name;
        public override string Name {
            get => _name;
            set {
                _name = value;

                _events.SendToAll(PamelloEvent.PlayerNameUpdated(Id, Name));
            }
        }

		public readonly PamelloSpeaker Speaker;
		public readonly PamelloQueue Queue;

        private bool _isPaused;
        public bool IsPaused {
            get => _isPaused;
            set {
                _isPaused = value;

                _events.SendToAllWithSelectedPlayer(Id, PamelloEvent.PlayerPauseUpdated(IsPaused));
            }
        }

        public PamelloPlayer(string name, IServiceProvider services) : base(services) {
            Id = nextId++;
            _name = name;

            var discordClient = services.GetRequiredService<DiscordSocketClient>();

            var guild = discordClient.GetGuild(1250768227542241450);
            var vc = guild.GetVoiceChannel(1250768228137959512);

            Speaker = new PamelloSpeaker(discordClient);
            Queue = new PamelloQueue(this, services);

            Task.Run(MusicLoop);
        }

        public async Task MusicLoop() {
            byte[]? audioBytes;

            while (true) {
                if (IsPaused ||
                    Queue.Current is null ||
                    !Speaker.IsConnected
                ) {
                    await Task.Delay(200);
                    continue;
                }

                if (!Queue.Current.IsInitialized) {
                    await Queue.Current.TryInitialize();
                }

                audioBytes = Queue.Current?.NextBytes();

                if (audioBytes is not null) Speaker.PlayBytes(audioBytes);
                else Queue.GoToNextSong();
            }
        }

        public override object GetDTO() {
            return new PlayerDTO() {
                Id = Id,
                Name = Name,

                IsPaused = IsPaused,

                CurrentSongTimePassed = Queue.Current?.Position.TotalSeconds ?? 0,
                CurrentSongTimeTotal = Queue.Current?.Duration.TotalSeconds ?? 0,

                CurrentSongId = Queue.Current?.Song.Id,
                QueueSongIds = Queue.SongAudios.Select(audio => audio.Song.Id),
                QueuePosition = Queue.Position,
				NextPositionRequest = Queue.NextPositionRequest,

				QueueIsRandom = Queue.IsRandom,
				QueueIsReversed = Queue.IsReversed,
				QueueIsNoLeftovers = Queue.IsNoLeftovers,
			};
		}
    }
}
