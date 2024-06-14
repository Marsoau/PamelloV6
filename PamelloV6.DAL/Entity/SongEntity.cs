using PamelloV6.Core.Abstract;
using PamelloV6.Core.DTO;

namespace PamelloV6.DAL.Entity
{
    public class SongEntity : ITransformableToDTO
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

		public object ToDTO() {
			return new SongDTO() {
                Id = Id,
                Title = Title,
                Author = Author,
                CoverUrl = CoverUrl,
                SourceUrl = SourceUrl,
                PlayCount = PlayCount,
                IsDownloaded = IsDownloaded,

                EpisodeIds = Episodes.Select(episodes => episodes.Id),
                PlaylistIds = Playlists.Select(playlist => playlist.Id),
			};
		}
	}
}
