using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Core.DTO
{
	public class PlaylistDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int OwnerId { get; set; }
		public bool IsProtected { get; set; }

		public IEnumerable<int> SongIds { get; set; }
	}
}
