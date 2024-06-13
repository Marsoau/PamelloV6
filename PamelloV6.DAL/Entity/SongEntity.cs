namespace PamelloV6.DAL.Entity
{
    public class SongEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverUrl { get; set; }
        public string SourceUrl { get; set; }
        public int PlayCount { get; set; }
        public bool IsDownloaded { get; set; }

        public List<EpisodeEntity> Episodes { get; set; }
        public List<PlaylistEntity> Playlists { get; set; }
    }
}
