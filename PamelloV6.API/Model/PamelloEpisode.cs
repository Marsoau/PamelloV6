using PamelloV6.API.Exceptions;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Model.Events;
using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using System.Xml.Linq;

namespace PamelloV6.API.Model
{
    public class PamelloEpisode : PamelloEntity
	{
		internal readonly EpisodeEntity Entity;

        public override int Id {
			get => Entity.Id;
		}

		public override string Name {
			get => Entity.Name;
			set {
				Entity.Name = value;

				Save();
				_events.SendToAll(
					PamelloEvent.EpisodeNameUpdated(Id, Name)
				);
            }
		}

		public AudioTime Start {
			get => new AudioTime(Entity.Start);
			set {
				Entity.Start = value.TotalSeconds;

                Save();
                _events.SendToAll(
                    PamelloEvent.EpisodeStartUpdated(Id, Start.TotalSeconds)
                );
            }
		}

		public PamelloSong Song {
			get => _songs.Get(Entity.Song.Id) ?? throw new PamelloException("Attempted to get song that doesnt exist");
		}

		public bool Skip {
			get => Entity.Skip;
			set {
				Entity.Skip = value;

                Save();
                _events.SendToAll(
                    PamelloEvent.EpisodeSkipStateUpdated(Id, Skip)
                );
            }
		}

		public PamelloEpisode(EpisodeEntity entity,
			IServiceProvider services
		) : base(services) {
			Entity = entity;
		}

		public override object GetDTO() {
			return new EpisodeDTO() {
				Id = Id,
				SongId = Song.Id,
				Name = Name,
				Start = Start.TotalSeconds,
				Skip = Skip,
			};
		}

		public override string ToString() {
			return $"[E: {Id}{(Skip ? " (S)" : "")}] {Start} - {Name}";
		}
	}
}
