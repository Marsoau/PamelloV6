using PamelloV6.API.Model.Audio;
using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;

namespace PamelloV6.API.Model
{
    public class PamelloEpisode : PamelloEntity
	{
		internal readonly EpisodeEntity Entity;

		public override int Id {
			get => Entity.Id;
		}

		public string Name {
			get => Entity.Name;
			set {
				Entity.Name = value;
				Save();
			}
		}

		public AudioTime Start {
			get => new AudioTime(Entity.Start);
			set {
				Entity.Start = value.TotalSeconds;
				Save();
			}
		}

		public PamelloSong Song {
			get => _songs.Get(Entity.Song.Id) ?? throw new Exception("Attempted to get song that doesnt exist");
			set {
				Entity.Song = value.Entity;
				Save();
			}
		}

		public bool Skip {
			get => Entity.Skip;
			set {
				Entity.Skip = value;
				Save();
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
