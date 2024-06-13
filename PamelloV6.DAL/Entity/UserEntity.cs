namespace PamelloV6.DAL.Entity
{
    public class UserEntity
    {
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public Guid Token { get; set; }

        public List<PlaylistEntity> OwnedPlaylists { get; set; }
    }
}
