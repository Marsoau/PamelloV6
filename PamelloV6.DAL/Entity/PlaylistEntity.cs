using PamelloV6.Core.Abstract;
using PamelloV6.Core.DTO;

namespace PamelloV6.DAL.Entity
{
    public class PlaylistEntity : ITransformableToDTO
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public UserEntity Owner { get; set; }
        public bool IsProtected { get; set; }

        public List<SongEntity> Songs { get; set; }

		public object ToDTO() {
			return new PlaylistDTO() {
				Id = Id,
				Name = Name,
				OwnerId = Owner.Id,
				IsProtected = IsProtected,

				SongIds = Songs.Select(song => song.Id)
			};
		}
	}
}
