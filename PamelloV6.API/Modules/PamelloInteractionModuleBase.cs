using AngleSharp.Dom;
using AngleSharp.Io;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Model.Interactions;
using PamelloV6.API.Model.Interactions.Builders;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.Core.DTO;
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

		public PamelloInteractionModuleBase(
			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists,
			PamelloPlayerRepository players,

			YoutubeInfoService youtube
		) {
			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
			_players = players;

			_youtube = youtube;
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
			if (!(Context.User.SelectedPlayer?.Speaker.IsConnected ?? false)) {
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
        public async Task Disconnect() {
            await PlayerDisconnect();
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
					await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError($"Cant find player with name \"{value}\""));
					return;
				}

				id = player.Id;
			}

			try {
				Context.Commands.PlayerSelect(id);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

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

			if (newPlayer is not null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Create player", $"Created{(
					Context.User.SelectedPlayer?.Id == newPlayer.Id ? " and selected " : " "
				)}new player \"{newPlayer.Name}\""));
			}
			else {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError($"Cant find newly created played by id {newPlayerId}"));
			}
		}
		public async Task PlayerConnect() {
			try {
				await Context.Commands.PlayerConnect();
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Connect player", "Selected player connected to the voice channel"));
		}
        public async Task PlayerDisconnect() {
            try {
                await Context.Commands.PlayerDisconnect();
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }

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

			try {
				newSongId = Context.Commands.PlayerGoToSong(songPosition, returnBack);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			var newSong = _songs.Get(newSongId);

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Go to song", $"Playing song \"{newSong?.Name}\""));
		}
		public async Task PlayerPrev() {
			int newSongId;

			try {
				newSongId = Context.Commands.PlayerPrev();
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			var newSong = _songs.Get(newSongId);

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Player revious song", $"Playing song \"{newSong?.Name}\""));
		}
		public async Task PlayerNext() {
			int newSongId;

			try {
				newSongId = Context.Commands.PlayerNext();
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			var newSong = _songs.Get(newSongId);

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Player next song", $"Playing song \"{newSong?.Name}\""));
		}
		public async Task PlayerSkip() {
			try {
				Context.Commands.PlayerSkip();
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Player skip current song", $"Skipped"));
		}

		//queue
		public async Task QueueInsertSong(string songValue, bool createIfUrlProvided, int? position = null) {
			var song = await _songs.GetByValue(songValue, true, createIfUrlProvided);

			try {
				Context.Commands.PlayerQueueAddSong(song.Id);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

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
            try {
                var playlist = _playlists.GetByValue(playlistValue);
                Context.Commands.PlayerQueueAddPlaylist(playlist.Id);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Add playlist", $"Songs from playlist added to current queue"));
        }
		public async Task PlayerQueueSongRemove(int position) {
			int removedSongId;

			try {
				removedSongId = Context.Commands.PlayerQueueRemoveSong(position);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			var removedSong = _songs.Get(removedSongId);
			if (removedSong is null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError("Removed non existent song successfully?"));
				return;
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Remove song", $"Song {removedSong.Name} removed"));
		}
		public async Task PlayerQueueSongMove(int fromPosition, int toPosition) {
			try {
				Context.Commands.PlayerQueueMove(fromPosition, toPosition);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Move song", $"Moved song from position {fromPosition} to position {toPosition}"));
		}
		public async Task PlayerQueueSongSwap(int inPosition, int withPosition) {
			try {
				Context.Commands.PlayerQueueSwap(inPosition, withPosition);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Move song", $"Swapped song in position {inPosition} with position {withPosition}"));
		}
		public async Task PlayerQueueSongRequestNext(int? position) {
			try {
				Context.Commands.PlayerQueueRequestNext(position);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			if (position is not null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Request next", $"Song in position {position} requested to be next"));
			}
			else {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Request next", $"Next song request removed"));
			}
		}
		public async Task PlayerQueueRandom(bool state) {
			try {
				Context.Commands.PlayerQueueRandom(state);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			var newState = Context.User.SelectedPlayer?.Queue.IsRandom;

			if (newState is null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError("Queue was not found?"));
			}
			else {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Queue random", $"Queue is{(newState.Value ? "" : " not")} random now"));
			}
		}
		public async Task PlayerQueueReversed(bool state) {
			try {
				Context.Commands.PlayerQueueReversed(state);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			var newState = Context.User.SelectedPlayer?.Queue.IsReversed;

			if (newState is null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError("Queue was not found?"));
			}
			else {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Queue reversed", $"Queue is{(newState.Value ? "" : " not")} reversed now"));
			}
		}
		public async Task PlayerQueueNoLeftovers(bool state) {
			try {
				Context.Commands.PlayerQueueNoLeftovers(state);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			var newState = Context.User.SelectedPlayer?.Queue.IsNoLeftovers;

			if (newState is null) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError("Queue was not found?"));
			}
			else {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Queue no leftovers", $"Queue has{(newState.Value ? "" : " not")} no leftovers now"));
			}
		}
		public async Task PlayerQueueShuffle() {
			
		}
		public async Task PlayerQueueClear() {
			try {
				Context.Commands.PlayerQueueClear();
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Clear queue", "Queue cleared"));
		}

		public async Task SongAdd(string url) {
			try {
				var youtubeId = _youtube.GetVideoIdFromUrl(url);
				var song = await _songs.Add(youtubeId);

				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Add new song", $"New song {song.Name} added to database"));
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}
		}
		public async Task SongEditName(string songValue, string newName) {
			try {
				var song = await _songs.GetByValue(songValue);
				Context.Commands.SongEditName(song.Id, newName);

				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Rename song", $"Song name changed to {song.Name}"));
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}
		}
		public async Task SongEditAuthor(string songValue, string newAuthor) {
			try {
				var song = await _songs.GetByValue(songValue);
				Context.Commands.SongEditAuthor(song.Id, newAuthor);

				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Rename song", $"Song author changed to {song.Name}"));
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}
		}
		public async Task SongDelete(string songValue) {
			throw new NotImplementedException();
		}
		public async Task SongSearch(string request, int page) {
			try {
                await SearchForPamelloEntity(_songs, request, page - 1);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
		public async Task SongInfo(string songValue) {
			PamelloSong song;
			try {
				song = await _songs.GetByValue(songValue);
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			await RespondWithEmbedAsync(
				PamelloEmbedBuilder.Info(song.Name, song.Author)
					.WithThumbnailUrl(song.CoverUrl)
					.Build()
			);
		}

		//song episodes
		public async Task EpisodeAdd(string songValue, string episodeName, int start) {
            try {
                var song = await _songs.GetByValue(songValue);
                Context.Commands.EpisodeAdd(song.Id, episodeName, start, false);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
		}
		public async Task EpisodeRename(int episodeId, string newName) {
            try {
                Context.Commands.EpisodeRename(episodeId, newName);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
		public async Task EpisodeDelete(int episodeId) {
            try {
                Context.Commands.EpisodeDelete(episodeId);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
		public async Task EpisodeList(string songValue) {
            try {
				var song = await _songs.GetByValue(songValue);
				var episodes = song.Episodes;

                var pageContent = GeneratePageFromCollection(episodes);

                await RespondWithEmbedPageAsync($"Song [{song.Id}] episodes list",
                    pageContent.Length == 0 ? "Empty" : pageContent
                );
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }

		//playlist
		public async Task PlaylistAdd(string playlistName) {
            try {
                Context.Commands.PlaylistAdd(playlistName, false);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
		public async Task PlaylistRename(string playlistValue, string newName) {
            try {
                var playlist = _playlists.GetByValue(playlistValue);
                Context.Commands.PlaylistRename(playlist.Id, newName);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
        public async Task PlaylistAddSong(string playlistValue, string songValue) {
            try {
                var playlist = _playlists.GetByValue(playlistValue);
                var song = await _songs.GetByValue(songValue);
                Context.Commands.PlaylistAddSong(playlist.Id, song.Id);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
        public async Task PlaylistRemoveSong(string playlistValue, int songPosition) {
            try {
                var playlist = _playlists.GetByValue(playlistValue);
                Context.Commands.PlaylistRemoveSong(playlist.Id, songPosition);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
        public async Task PlaylistShowMine() {
            try {
                var playlists = Context.User.OwnedPlaylists;

                var pageContent = GeneratePageFromCollection(playlists);

                await RespondWithEmbedPageAsync("Player list",
                    pageContent.Length == 0 ? "Empty" : pageContent
                );
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
		public async Task PlaylistDelete(string playlistValue) {
            try {
				var playlist = _playlists.GetByValue(playlistValue);
                Context.Commands.EpisodeDelete(playlist.Id);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
		public async Task PlaylistSearch(string request, int page) {
            try {
                await SearchForPamelloEntity(_playlists, request, page - 1);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }
        }
		public async Task PlaylistInfo(string playlistValue) {
            PamelloPlaylist playlist;
            try {
                playlist = _playlists.GetByValue(playlistValue);
            }
            catch (Exception x) {
                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                return;
            }

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo(playlist.Name, $"Contains {playlist.Songs.Count} songs"));
        }
    }
}
