using PamelloV6.Core.Abstract;
using PamelloV6.Core.DTO;

namespace PamelloV6.DAL.Entity
{
    public class UserEntity : ITransformableToDTO
	{
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public Guid Token { get; set; }
        public bool IsAdministrator { get; set; }

        public List<PlaylistEntity> OwnedPlaylists { get; set; }

		public object ToDTO() {
            return new UserDTO() {
                Id = Id,
                DiscordId = DiscordId,
                IsAdministrator = IsAdministrator,

                OwnedPlaylistIds = (OwnedPlaylists ?? []).Select(playlist => playlist.Id),
            };
		}
	}
}
