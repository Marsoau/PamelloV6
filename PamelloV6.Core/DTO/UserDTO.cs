using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Core.DTO
{
	public class UserDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string CoverUrl { get; set; }
		public ulong DiscordId { get; set; }
		public int? SelectedPlayerId { get; set; }
		public bool IsAdministrator { get; set; }

		public IEnumerable<int> OwnedPlaylistIds { get; set; }
	}
}
