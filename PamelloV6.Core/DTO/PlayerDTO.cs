using PamelloV6.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Core.DTO
{
	public class PlayerDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public bool IsPaused { get; set; }
        public PamelloPlayerState State { get; set; }
        public IEnumerable<SpeakerDTO> Speakers { get; set; }
		
		public int CurrentSongTimePassed { get; set; }
		public int CurrentSongTimeTotal { get; set; }

		public int? CurrentSongId { get; set; }
		public IEnumerable<int> QueueSongIds { get; set; }
		public int QueuePosition { get; set; }
		public int? NextPositionRequest { get; set; }

		public bool QueueIsRandom { get; set; }
		public bool QueueIsReversed { get; set; }
		public bool QueueIsNoLeftovers { get; set; }
	}
}
