using PamelloV6.Core.Abstract;
using PamelloV6.Core.DTO;

namespace PamelloV6.DAL.Entity
{
    public class EpisodeEntity : ITransformableToDTO
	{
        public int Id { get; set; }
        public SongEntity Song { get; set; }
        public string Name { get; set; }
		public int Start { get; set; }
		public bool Skip { get; set; }

		public object ToDTO() {
            return new EpisodeDTO() {
                Id = Id,
                SongId = Song.Id,
                Name = Name,
                Start = Start,
            };
		}
	}
}
