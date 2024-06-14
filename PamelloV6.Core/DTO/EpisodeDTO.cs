using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Core.DTO
{
	public class EpisodeDTO
	{
		public int Id { get; set; }
		public int SongId { get; set; }
		public string Name { get; set; }
		public int Start { get; set; }
	}
}
