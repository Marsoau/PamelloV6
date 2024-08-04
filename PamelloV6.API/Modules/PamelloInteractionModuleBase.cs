using AngleSharp.Dom;
using AngleSharp.Io;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Model.Interactions;
using PamelloV6.API.Model.Interactions.Builders;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.Core.DTO;
using PamelloV6.Server.Services;
using System.Text;
using System.Text.Json.Nodes;

namespace PamelloV6.API.Modules
{
	public class PamelloInteractionModuleBase : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		protected readonly PamelloUserRepository _users;
		protected readonly PamelloSongRepository _songs;
		protected readonly PamelloEpisodeRepository _episodes;
		protected readonly PamelloPlaylistRepository _playlists;
		protected readonly PamelloPlayerRepository _players;
		protected readonly YoutubeInfoService _youtube;
        protected readonly UserAuthorizationService _authorization;

        public PamelloInteractionModuleBase(
			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists,
			PamelloPlayerRepository players,

			YoutubeInfoService youtube,
            UserAuthorizationService authorization
        ) {
			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
			_players = players;

			_youtube = youtube;
			_authorization = authorization;
		}

		protected async Task RespondWithEmbedAsync(Embed embed) {
			await ModifyOriginalResponseAsync(message => message.Embed = embed);
		}

		protected async Task SearchForPamelloEntity<T>(PamelloRepository<T> repository, string request, int page) where T : PamelloEntity {
			var searchResult = repository.Search(page - 1, 10, request);

			var pageContent = GeneratePageFromCollection(searchResult.Results);

            await RespondWithEmbedPageAsync("Results", (
                    pageContent.Length == 0 ? "Empty" : pageContent.ToString()
            ), page, searchResult.PagesCount);
        }

		protected string GeneratePageFromCollection<T>(IEnumerable<T> collection) where T : PamelloEntity {
            StringBuilder sb = new StringBuilder();

            foreach (var entity in collection) {
                sb.Append($"`[{entity.Id}]` {entity.Name}");

                if (typeof(T) == typeof(PamelloPlayer) &&
                    Context.User.SelectedPlayer?.Id == entity.Id) sb.AppendLine(" **< selected**");
                else sb.AppendLine();
            }

			return sb.ToString();
        }

		protected async Task RespondWithEmbedPageAsync(string header, string content, int? page = null, int? totalPages = null) {
            await RespondWithEmbedAsync(
                PamelloEmbedBuilder.Info(header, content)
                .WithFooter($"{
					(page is not null ? $"page {page}" : "")
				}{
					(totalPages is not null ? $" / {totalPages}" : "")
				}")
                .Build()
            );
        }

        //general
        public async Task Add(string songValue) {
            if (Context.User.SelectedPlayer is null) {
                await PlayerCreate("Player");
            }
            await PlayerQueueSongAdd(songValue);
			if (!(Context.User.SelectedPlayer?.Speakers.IsAnyConnected ?? false)) {
                await Context.Commands.PlayerConnect();
            }
        }
        public async Task AddPlaylist(string playlistValue) {
            if (Context.User.SelectedPlayer is null) {
                await PlayerCreate("Player");
            }
            await PlayerQueuePlaylistAdd(playlistValue);
        }
        public async Task Connect() {
            if (Context.User.SelectedPlayer is null) {
				await PlayerCreate("Player");
			}
            await PlayerConnect();
        }
        public async Task GetCode() {
			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Authroization code", _authorization.GetCode(Context.User.DiscordUser.Id).ToString()));
        }

        //player
        public async Task PlayerSelect(string? value = null) {
			if (value is null) {
				Context.Commands.PlayerSelect(null);
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Select player", "Deselected player"));
				return;
			}
			int id;

			if (!int.TryParse(value, out id)) {
				var player = _players.GetByName(value);
				if (player is null) {
					throw new PamelloException($"Cant find player with name \"{value}\"");
				}

				id = player.Id;
			}

            Context.Commands.PlayerSelect(id);

            if (Context.User.SelectedPlayer is not null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Select player", $"Selected player \"{Context.User.SelectedPlayer.Name}\""));
			}
			else {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Select player", "Deselected player"));
			}
		}

		public async Task PlayerCreate(string name) {
			var newPlayerId = Context.Commands.PlayerCreate(name);
			Context.Commands.PlayerSelect(newPlayerId);

			var newPlayer = _players.Get(newPlayerId);

			if (newPlayer is null) {
                throw new PamelloException($"Cant find newly created played by id {newPlayerId}");
            }

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Create player", $"Created{(
                Context.User.SelectedPlayer?.Id == newPlayer.Id ? " and selected " : " "
            )}new player \"{newPlayer.Name}\""));
        }
		public async Task PlayerConnect() {
            await Context.Commands.PlayerConnect();

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Connect player", "Selected player connected to the voice channel"));
		}
        public async Task PlayerDelete(string value) {
			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Delete player", "This command is unavailable yet"));
		}
		public async Task PlayerList(int page = 1) {
			await SearchForPamelloEntity(_players, "", page - 1);
		}
		public async Task PlayerGoTo(int songPosition, bool returnBack = false) {
			int newSongId;

            newSongId = Context.Commands.PlayerGoToSong(songPosition, returnBack);

            var newSong = _songs.Get(newSongId);

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Go to song", $"Playing song \"{newSong?.Name}\""));
		}
		public async Task PlayerPrev() {
			int newSongId;

            newSongId = Context.Commands.PlayerPrev();

            var newSong = _songs.Get(newSongId);

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Player revious song", $"Playing song \"{newSong?.Name}\""));
		}
		public async Task PlayerNext() {
			int newSongId;

            newSongId = Context.Commands.PlayerNext();

            var newSong = _songs.Get(newSongId);

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Player next song", $"Playing song \"{newSong?.Name}\""));
		}
		public async Task PlayerSkip() {
            Context.Commands.PlayerSkip();

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Player skip current song", $"Skipped"));
		}

		//queue
		public async Task QueueInsertSong(string songValue, bool createIfUrlProvided, int? position = null) {
			var song = await _songs.GetByValue(songValue, true, createIfUrlProvided);

            Context.Commands.PlayerQueueAddSong(song.Id);

            await RespondWithEmbedAsync(
				PamelloEmbedBuilder.Info($"Added song to \"{Context.User.SelectedPlayer?.Name}\" queue", song.Name)
					.WithThumbnailUrl(song.CoverUrl)
					.Build()
			);
		}

		//
		public async Task PlayerQueueSongAdd(string songValue) {
			await QueueInsertSong(songValue, false);
		}
		public async Task PlayerQueueSongInsert(int position, string songValue) {
			await QueueInsertSong(songValue, false, position);
		}
		public async Task PlayerQueuePlaylistAdd(string playlistValue) {
            var playlist = _playlists.GetByValue(playlistValue);
            Context.Commands.PlayerQueueAddPlaylist(playlist.Id);

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Add playlist", $"Songs from playlist added to current queue"));
        }
		public async Task PlayerQueueSongRemove(int position) {
            var removedSongId = Context.Commands.PlayerQueueRemoveSong(position);

            var removedSong = _songs.Get(removedSongId);
			if (removedSong is null) {
				throw new PamelloException("Removed non existent song successfully?");
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Remove song", $"Song {removedSong.Name} removed"));
		}
		public async Task PlayerQueueSongMove(int fromPosition, int toPosition) {
            Context.Commands.PlayerQueueMove(fromPosition, toPosition);

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Move song", $"Moved song from position {fromPosition} to position {toPosition}"));
		}
		public async Task PlayerQueueSongSwap(int inPosition, int withPosition) {
            Context.Commands.PlayerQueueSwap(inPosition, withPosition);

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Move song", $"Swapped song in position {inPosition} with position {withPosition}"));
		}
		public async Task PlayerQueueSongRequestNext(int? position) {
            Context.Commands.PlayerQueueRequestNext(position);

            if (position is not null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Request next", $"Song in position {position} requested to be next"));
			}
			else {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Request next", $"Next song request removed"));
			}
		}
		public async Task PlayerQueueRandom(bool state) {
            Context.Commands.PlayerQueueRandom(state);

            var newState = Context.User.SelectedPlayer?.Queue.IsRandom
                ?? throw new PamelloException("Queue was not found?");

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Queue random", $"Queue is{(newState ? "" : " not")} random now"));
        }
		public async Task PlayerQueueReversed(bool state) {
            Context.Commands.PlayerQueueReversed(state);

            var newState = Context.User.SelectedPlayer?.Queue.IsReversed
                ?? throw new PamelloException("Queue was not found?");

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Queue reversed", $"Queue is{(newState ? "" : " not")} reversed now"));
        }
		public async Task PlayerQueueNoLeftovers(bool state) {
            Context.Commands.PlayerQueueNoLeftovers(state);

            var newState = Context.User.SelectedPlayer?.Queue.IsNoLeftovers
                ?? throw new PamelloException("Queue was not found?");

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Queue no leftovers", $"Queue has{(!newState ? "" : " no")} leftovers now"));
        }
		public async Task PlayerQueueShuffle() {

        }
		public async Task PlayerQueueClear() {
            Context.Commands.PlayerQueueClear();

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Clear queue", "Queue cleared"));
		}

		public async Task SongAdd(string url) {
            var youtubeId = _youtube.GetVideoIdFromUrl(url);
            var song = await _songs.Add(youtubeId);

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Add new song", $"New song {song.Name} added to database"));
        }
		public async Task SongEditName(string songValue, string newName) {
            var song = await _songs.GetByValue(songValue);
            Context.Commands.SongEditName(song.Id, newName);

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Rename song", $"Song name changed to {song.Name}"));
        }
		public async Task SongEditAuthor(string songValue, string newAuthor) {
            var song = await _songs.GetByValue(songValue);
            Context.Commands.SongEditAuthor(song.Id, newAuthor);

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Rename song", $"Song author changed to {song.Name}"));
        }
		public async Task SongSearch(string request, int page) {
            await SearchForPamelloEntity(_songs, request, page - 1);
        }
		public async Task SongInfo(string songValue) {
            var song = await _songs.GetByValue(songValue);

            await RespondWithEmbedAsync(
				PamelloEmbedBuilder.Info(song.Name, song.Author)
					.WithThumbnailUrl(song.CoverUrl)
					.Build()
			);
		}

		//song episodes
		public async Task EpisodeAdd(string songValue, string episodeName, int start) {
            var song = await _songs.GetByValue(songValue);
            Context.Commands.EpisodeAdd(song.Id, episodeName, start, false);
        }
		public async Task EpisodeRename(int episodeId, string newName) {
            Context.Commands.EpisodeRename(episodeId, newName);
        }
		public async Task EpisodeDelete(int episodeId) {
            Context.Commands.EpisodeDelete(episodeId);
        }
		public async Task EpisodeList(string songValue) {
            var song = await _songs.GetByValue(songValue);
            var episodes = song.Episodes;

            var pageContent = GeneratePageFromCollection(episodes);

            await RespondWithEmbedPageAsync($"Song [{song.Id}] episodes list",
                pageContent.Length == 0 ? "Empty" : pageContent
            );
        }

		//playlist
		public async Task PlaylistAdd(string playlistName) {
            Context.Commands.PlaylistAdd(playlistName, false);
        }
		public async Task PlaylistRename(string playlistValue, string newName) {
            var playlist = _playlists.GetByValue(playlistValue);
            Context.Commands.PlaylistRename(playlist.Id, newName);
        }
        public async Task PlaylistAddSong(string playlistValue, string songValue) {
            var playlist = _playlists.GetByValue(playlistValue);
            var song = await _songs.GetByValue(songValue);
            Context.Commands.PlaylistAddSong(playlist.Id, song.Id);
        }
        public async Task PlaylistRemoveSong(string playlistValue, int songPosition) {
            var playlist = _playlists.GetByValue(playlistValue);
            Context.Commands.PlaylistRemoveSong(playlist.Id, songPosition);
        }
        public async Task PlaylistShowMine() {
            var playlists = Context.User.OwnedPlaylists;

            var pageContent = GeneratePageFromCollection(playlists);

            await RespondWithEmbedPageAsync("Player list",
                pageContent.Length == 0 ? "Empty" : pageContent
            );
        }
		public async Task PlaylistDelete(string playlistValue) {
            var playlist = _playlists.GetByValue(playlistValue);
            Context.Commands.EpisodeDelete(playlist.Id);
        }
		public async Task PlaylistSearch(string request, int page) {
            await SearchForPamelloEntity(_playlists, request, page - 1);
        }
		public async Task PlaylistInfo(string playlistValue) {
            var playlist = _playlists.GetByValue(playlistValue);

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo(playlist.Name, $"Contains {playlist.Songs.Count} songs"));
        }
    }
}
