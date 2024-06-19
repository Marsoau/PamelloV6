using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PamelloV6.API.Model;
using PamelloV6.API.Repositories;
using PamelloV6.Core.Abstract;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;

namespace PamelloV6.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class DataController : ControllerBase
	{
		protected readonly PamelloUserRepository _users;
		protected readonly PamelloSongRepository _songs;
		protected readonly PamelloEpisodeRepository _episodes;
		protected readonly PamelloPlaylistRepository _playlists;
		protected readonly PamelloPlayerRepository _players;

		public DataController(
			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists,
			PamelloPlayerRepository player
		) {
			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
			_players = player;
		}

		[HttpGet("User")]
		public IActionResult GetUser() {
			return HandleGetRequest(_users);
		}
		[HttpGet("Song")]
		public IActionResult GetSong() {
			return HandleGetRequest(_songs);
		}
		[HttpGet("Episode")]
		public IActionResult GetEpisode() {
			return HandleGetRequest(_episodes);
		}
		[HttpGet("Playlist")]
		public IActionResult GetPlaylist() {
			return HandleGetRequest(_playlists);
		}
		[HttpGet("Player")]
		public IActionResult GetPlayer() {
			return HandleGetRequest(_players);
		}

		private IActionResult HandleGetRequest<T>(PamelloRepository<T> _repository) where T : PamelloEntity {
			var qId = Request.Query["id"].FirstOrDefault();
			if (qId is null) {
				return BadRequest("Id required");
			}

			if (!int.TryParse(qId, out int id)) {
				return BadRequest("Id must me an integer number");
			}

			var pamelloEntity = _repository.Get(id);
			if (pamelloEntity is null) return NotFound();

			return Ok(pamelloEntity.GetDTO());
		}
	}
}
