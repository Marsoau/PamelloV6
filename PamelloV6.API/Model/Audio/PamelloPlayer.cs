using PamelloV6.Core.DTO;

namespace PamelloV6.API.Model.Audio
{
    public class PamelloPlayer : PamelloEntity
    {
        private static int nextId = 1;
        public override int Id { get; }
        public string Name { get; set; }

		public readonly PamelloSpeaker Speaker;
		public readonly PamelloQueue Queue;

        public bool IsPaused { get; set; }

        public PamelloPlayer(string name, IServiceProvider services) : base(services) {
            Id = nextId++;
            Name = name;

            Speaker = new PamelloSpeaker();
            Queue = new PamelloQueue(services);

            Task.Run(MusicLoop);
        }

        public async Task MusicLoop() {
            byte[]? audioBytes;

            while (true) {
                if (IsPaused ||
                    Queue.Current is null
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
