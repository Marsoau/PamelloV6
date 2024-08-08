using AngleSharp.Dom.Events;
using Discord;
using Discord.WebSocket;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Repositories;
using PamelloV6.Core.DTO;
using PamelloV6.Core.Enumerators;
using PamelloV6.Server.Model;
using System.Numerics;

namespace PamelloV6.API.Model.Audio
{
    public class PamelloPlayer : PamelloEntity
    {
        private readonly PamelloUserRepository _users;

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

		public readonly PamelloSpeakerCollection Speakers;
		public readonly PamelloQueue Queue;

        private bool _isPaused;
        public bool IsPaused {
            get => _isPaused;
            set {
                _isPaused = value;

                _events.SendToAllWithSelectedPlayer(Id, PamelloEvent.PlayerPauseUpdated(IsPaused));
            }
        }

        private PamelloPlayerState _state;
        public PamelloPlayerState State {
            get => _state;
            set {
                _state = value;

                _events.SendToAllWithSelectedPlayer(Id, PamelloEvent.PlayerStateUpdated(State));
            }
        }

        public List<PamelloUser> Users {
            get => _users.GetAllWithSelectedPlayer(Id);
        }

        private bool _deleted;

        public PamelloPlayer(string name, IServiceProvider services) : base(services) {
            Id = nextId++;
            _name = name;

            _users = services.GetRequiredService<PamelloUserRepository>();

            var discordClient = services.GetRequiredService<DiscordSocketClient>();

            Speakers = new PamelloSpeakerCollection(this, services);
            Queue = new PamelloQueue(this, services);

            Task.Run(MusicLoop);
        }

        public async Task MusicLoop() {
            byte[]? audioBytes;

            while (true) {
                if (_deleted) return;

                if (IsPaused) {
                    await Task.Delay(1000);
                    continue;
                }
                if (Queue.Current is null) {
                    if (State != PamelloPlayerState.AwaitingSong) {
                        State = PamelloPlayerState.AwaitingSong;
                    }

                    await Task.Delay(1000);
                    continue;
                }
                if (!Speakers.IsAnyConnected) {
                    if (State != PamelloPlayerState.AwaitingSpeaker) {
                        State = PamelloPlayerState.AwaitingSpeaker;
                    }

                    await Task.Delay(1000);
                    continue;
                }

                if (!Queue.Current.IsInitialized) {
                    Console.WriteLine($"Initialzing {Queue.Current.Song.Name}");
                    if (!await Queue.Current.TryInitialize()) {
                        Console.WriteLine($"Failed to initialize {Queue.Current.Song.Name}");
                        Queue.RemoveSong(Queue.Position);
                        continue;
                    }
                    Console.WriteLine($"Initialzed {Queue.Current.Song.Name}");
                }

                if (State != PamelloPlayerState.Ready) {
                    State = PamelloPlayerState.Ready;
                }

                audioBytes = Queue.Current?.NextBytes();

                try {
                    if (audioBytes is not null) Speakers.PlayBytes(audioBytes);
                    else Queue.GoToNextSong();
                }
                catch (Exception x) {
                    Console.WriteLine($"PlayBytes Catch: {x}");
                }
            }
        }

        public async Task Delete() {
            var currentUsers = Users;
            foreach (var user in currentUsers) {
                user.SelectedPlayer = null;
            }

            _deleted = true;
            await Speakers.DisconnectAll();
            Queue.Clear();
        }

        public override object GetDTO() {
            return new PlayerDTO() {
                Id = Id,
                Name = Name,

                IsPaused = IsPaused,
                State = State,
                Speakers = Speakers.Speakers.Select(speaker => speaker.GetDTO()),

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
