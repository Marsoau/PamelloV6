using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PamelloV6.API.Repositories;
using PamelloV6.Core.Abstract;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;

namespace PamelloV6.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class DataController : ControllerBase
	{
		private readonly PamelloUserRepository _userRepository;
		private readonly PamelloSongRepository _songRepository;

		public DataController(
			PamelloUserRepository userRepository,
			PamelloSongRepository songRepository
		) {
			_userRepository = userRepository;
			_songRepository = songRepository;
		}

		[HttpGet("User")]
		public async Task<IActionResult> GetUser() {
			int requestedId;
			try {
				requestedId = GetRequestedId();
			}
			catch (Exception x) {
				return BadRequest(x);
			}

			var user = _userRepository.GetUser(requestedId);
			if (user is null) {
				return NotFound();
			}

			return Ok(user.Entity.ToDTO());
		}
		[HttpGet("Song")]
		public async Task<IActionResult> GetSong() {
			int requestedId;
			try {
				requestedId = GetRequestedId();
			}
			catch (Exception x) {
				return BadRequest(x);
			}

			var song = _songRepository.GetSong(requestedId);
			if (song is null) {
				return NotFound();
			}

			return Ok(song.Entity.ToDTO());
		}
		[HttpGet("Playlist")]
		public async Task<IActionResult> GetPlaylist() {
			throw new NotImplementedException();
		}
		[HttpGet("Episode")]
		public async Task<IActionResult> GetEpisode() {
			throw new NotImplementedException();
		}

		private int GetRequestedId() {
			var qId = Request.Query["id"].FirstOrDefault();
			if (qId is null) {
				throw new Exception("Id required");
			}

			if (!int.TryParse(qId, out int id)) {
				throw new Exception("Id must me an integer number");
			}

			return id;
		}
	}
}
