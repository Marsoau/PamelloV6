using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Core.DTO
{
	public class SongDTO
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string CoverUrl { get; set; }
		public string SourceUrl { get; set; }
		public int PlayCount { get; set; }
		public bool IsDownloaded { get; set; }

		public IEnumerable<int> EpisodeIds { get; set; }
		public IEnumerable<int> PlaylistIds { get; set; }
	}
}
