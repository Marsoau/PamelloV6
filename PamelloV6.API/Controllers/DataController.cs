using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PamelloV6.Core.Abstract;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;

namespace PamelloV6.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class DataController : ControllerBase
	{
		private readonly DatabaseContext _database;

		public DataController(DatabaseContext database) {
			_database = database;
		}

		[HttpGet("User")]
		public async Task<IActionResult> GetUser() {
			return await GetDTO<UserEntity>();
		}
		[HttpGet("Song")]
		public async Task<IActionResult> GetSong() {
			return await GetDTO<SongEntity>();
		}
		[HttpGet("Playlist")]
		public async Task<IActionResult> GetPlaylist() {
			return await GetDTO<PlaylistEntity>();
		}
		[HttpGet("Episode")]
		public async Task<IActionResult> GetEpisode() {
			return await GetDTO<EpisodeEntity>();
		}

		private async Task<IActionResult> GetDTO<T>() where T : class, ITransformableToDTO {
			var qId = Request.Query["id"].FirstOrDefault();
			if (qId is null) {
				return BadRequest("Id required");
			}

			if (!int.TryParse(qId, out int id)) {
				return BadRequest("Id must me an integer number");
			}

			var entity = await _database.Set<T>().FindAsync(id);
			if (entity is null) {
				return NotFound();
			}

			return Ok(entity.ToDTO());
		}
	}
}
