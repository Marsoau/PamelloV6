﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Responses;
using PamelloV6.API.Model.Youtube;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
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

		protected readonly PamelloYoutubeService _youtube;

		public DataController(
			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists,
			PamelloPlayerRepository player,

            PamelloYoutubeService youtube
        ) {
			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
			_players = player;

			_youtube = youtube;
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
		[HttpGet("Users/Search")]
		public IActionResult UsersSeach() {
			return HandleGetSearchRequest(_users);
		}
		[HttpGet("Users/Info")]
		public IActionResult UsersInfo() {
			return HandleGetInfoRequest(_users);
		}
		//
		[HttpGet("Song")]
		public IActionResult GetSong() {
			return HandleGetByIdRequest(_songs);
        }
        [HttpGet("Songs/Search")]
        public IActionResult SongsSearch() {
            return HandleGetSearchRequest(_songs);
        }
        [HttpGet("Songs/SearchYoutube")]
        public IActionResult SongsSearcYoutubeh() {
            return HandleGetYoutubeSearchRequest();
        }
        [HttpGet("Songs/Info")]
		public IActionResult SongsInfo() {
			return HandleGetInfoRequest(_songs);
		}
		[HttpGet("Episode")]
		public IActionResult GetEpisode() {
			return HandleGetByIdRequest(_episodes);
		}
		[HttpGet("Episodes/Search")]
		public IActionResult EpisodesSearch() {
			return HandleGetSearchRequest(_episodes);
		}
		[HttpGet("Episodes/Info")]
		public IActionResult EpisodesInfo() {
			return HandleGetInfoRequest(_episodes);
		}
		[HttpGet("Playlist")]
		public IActionResult GetPlaylist() {
			return HandleGetByIdRequest(_playlists);
		}
		[HttpGet("Playlists/Search")]
		public IActionResult PlaylistsSearch() {
			return HandleGetSearchRequest(_playlists);
		}
		[HttpGet("Playlists/Info")]
		public IActionResult PlaylistsInfo() {
			return HandleGetInfoRequest(_playlists);
		}
		[HttpGet("Player")]
		public IActionResult GetPlayer() {
			return HandleGetByIdRequest(_players);
		}
		[HttpGet("Players/Search")]
		public IActionResult PlayersSearch() {
			return HandleGetSearchRequest(_players);
		}
		[HttpGet("Players/Info")]
		public IActionResult PlayersInfo() {
			return HandleGetInfoRequest(_players);
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

		private IActionResult HandleGetSearchRequest<T>(PamelloRepository<T> _repository) where T : PamelloEntity {
			var qSearch = Request.Query["q"].FirstOrDefault();
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

			var pamelloEntities = _repository.Search(page, count, qSearch);
			return Ok(new {
				page = page,
				pagesCount = pamelloEntities.PagesCount,
				results = pamelloEntities.Results.Select(pamelloEntity => pamelloEntity.GetDTO()),
                query = qSearch,
            });
		}

		private IActionResult HandleGetInfoRequest<T>(PamelloRepository<T> _repository) where T : PamelloEntity {
			return Ok(new {
				count = _repository.Size
			});
		}

		private IActionResult HandleGetUserByTokenRequest(string qToken) {
			if (!Guid.TryParse(qToken, out var token)) {
				return BadRequest("Invalid token format");
			}

			var user = _users.Get(token);
			if (user is null) return NotFound();

			return Ok(user.GetDTO());
		}

		private IActionResult HandleGetYoutubeSearchRequest() {
            var qSearch = Request.Query["q"].FirstOrDefault();
            var qCount = Request.Query["count"].FirstOrDefault();
            if (qCount is null) {
                return BadRequest("Count required");
            }

            if (!int.TryParse(qCount, out int count)) {
                return BadRequest("Count must be an integer number");
            }

            var searchResult = _youtube.Search(count, qSearch).Result;
            return Ok(new {
                resultsCount = searchResult.ResultsCount,
                pamelloSongs = searchResult.PamelloSongs.Select(song => song.GetDTO()),
                youtubeVideos = searchResult.YoutubeVideos,
            });
        }
    }
}
