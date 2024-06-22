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
            if (Request.Query.TryGetValue("token", out var tokenValues)) {
                return HandleGetUserByTokenRequest(tokenValues.FirstOrDefault() ?? "");
            }
            else {
                return HandleGetByIdRequest(_users);
            }
        }
        [HttpGet("AllUsers")]
        public IActionResult GetAllUsers() {
            return HandleGetAllRequest(_users);
        }
        [HttpGet("Song")]
        public IActionResult GetSong() {
            return HandleGetByIdRequest(_songs);
        }
        [HttpGet("AllSongs")]
        public IActionResult GetAllSongs() {
            return HandleGetAllRequest(_songs);
        }
        [HttpGet("Episode")]
        public IActionResult GetEpisode() {
            return HandleGetByIdRequest(_episodes);
        }
        [HttpGet("AllEpisodes")]
        public IActionResult GetAllEpisodes() {
            return HandleGetAllRequest(_episodes);
        }
        [HttpGet("Playlist")]
        public IActionResult GetPlaylist() {
            return HandleGetByIdRequest(_playlists);
        }
        [HttpGet("AllPlaylists")]
        public IActionResult GetAllPlaylists() {
            return HandleGetAllRequest(_playlists);
        }
        [HttpGet("Player")]
        public IActionResult GetPlayer() {
            return HandleGetByIdRequest(_players);
        }
        [HttpGet("AllPlayers")]
        public IActionResult GetAllPlayers() {
            return HandleGetAllRequest(_players);
        }

        private IActionResult HandleGetByIdRequest<T>(PamelloRepository<T> _repository) where T : PamelloEntity {
            var qId = Request.Query["id"].FirstOrDefault();
            if (qId is null) {
                return BadRequest("Id required");
            }

            if (!int.TryParse(qId, out int id)) {
                return BadRequest("Id must be an integer number");
            }

            var pamelloEntity = _repository.Get(id);
            if (pamelloEntity is null) return NotFound();

            return Ok(pamelloEntity.GetDTO());
        }

        private IActionResult HandleGetAllRequest<T>(PamelloRepository<T> _repository) where T : PamelloEntity {
            var qPage = Request.Query["page"].FirstOrDefault();
            if (qPage is null) {
                return BadRequest("Page required");
            }
            var qCount = Request.Query["count"].FirstOrDefault();
            if (qCount is null) {
                return BadRequest("Count required");
            }

            if (!int.TryParse(qPage, out int page)) {
                return BadRequest("Page must be an integer number");
            }
            if (!int.TryParse(qCount, out int count)) {
                return BadRequest("Count must be an integer number");
            }

            var pamelloEntities = _repository.GetAll(page, count);
            return Ok(pamelloEntities.Select(pamelloEntity => pamelloEntity.GetDTO()));
        }

        private IActionResult HandleGetUserByTokenRequest(string qToken) {
            if (!Guid.TryParse(qToken, out var token)) {
                return BadRequest("Invalid token format");
            }

            var user = _users.Get(token);
            if (user is null) return NotFound();

            return Ok(user.GetDTO());
        }
    }
}
