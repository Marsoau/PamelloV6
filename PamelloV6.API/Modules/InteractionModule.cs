using Discord;
using Discord.Audio;
using Discord.Interactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Expressions;
using Newtonsoft.Json.Linq;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Model.Interactions;
using PamelloV6.API.Model.Interactions.Builders;
using PamelloV6.API.Modules;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.Server.Handlers;
using PamelloV6.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PamelloV6.Server.Modules
{
	public enum EnableDisable {
		Enable,
		Disable
	}

	public class GeneralInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		private async Task RespondWithEmbedAsync(Embed embed) {
			await ModifyOriginalResponseAsync(message => message.Embed = embed);
        }

        [SlashCommand("add", "Add song to selected player queue")]
        public async Task Add() {

		}

        [SlashCommand("connect", "Connect player to your voice channel")]
		public async Task Connect() {
			/*
			try {
				if (Context.User.SelectedPlayer is null) {
					var newPlayerId = Context.Commands.PlayerCreate("Player");
					Context.Commands.PlayerSelect(newPlayerId);
				}
				await Context.Commands.PlayerConnect();
			}
			catch (Exception x) {
				await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
				return;
			}

			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo($"Created, selected, and connected new player \"{Context.User.SelectedPlayer?.Name}\""));
			*/
		}
	}

	[Group("player", "Commands to maipulate player")]
	public class PlayerInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		private readonly PamelloPlayerRepository _players;
		private readonly PamelloSongRepository _songs;

		public PlayerInteractionsModuleGroup(
			PamelloPlayerRepository players,
			PamelloSongRepository songs
		) : base() {
			_players = players;
			_songs = songs;
		}

		private void MakeSureCorrectPlayerSelected() {
			if (Context.User.SelectedPlayer is null) throw new Exception("Player must be selected");
			//if (!Context.User.SelectedPlayer.InVCWith(Context.User)) throw new Exception("You must be in the same voice channel as the selected player speaker");
		}

		private async Task RespondWithEmbedAsync(Embed embed) {
			await ModifyOriginalResponseAsync(message => message.Embed = embed);
		}

		[SlashCommand("select", "Select a player to work with")]
		public async Task PlayerSelect(
			[Summary("player", "Player id or (exact) name")] string? value = null
		) {
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

		[SlashCommand("create", "Create new player")]
		public async Task PlayerCreate(
			[Summary("name", "Name of the new player")] string name
		) {
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

		[SlashCommand("connect", "Create new player")]
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

		[SlashCommand("delete", "Delete player")]
		public async Task PlayerDelete(
			[Summary("player", "Player id or (exact) name")] string value
		) {
			await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Delete player", "This command is unavailable yet"));
		}

		[SlashCommand("list", "Get a list of available players")]
		public async Task PlayerList(
			[Summary("page", "Page of players list")] int page = 1
		) {
			StringBuilder sb = new StringBuilder();

			var searchResult = _players.Search(page - 1, 10, "");

			foreach (var player in searchResult.Results) {
				sb.Append($"`[{player.Id}]` {player.Name}");

				if (Context.User.SelectedPlayer?.Id == player.Id) sb.AppendLine(" **< selected**");
				else sb.AppendLine();
			}

			await RespondWithEmbedAsync(
				PamelloEmbedBuilder.Info("Player list", (
					sb.Length == 0 ? "Empty" : sb.ToString()
				))
					.WithFooter($"page {page} / {searchResult.PagesCount}")
					.Build()
			);
		}

		[SlashCommand("go-to", "Go to specific song from queue")]
		public async Task PlayerGoTo(
			[Summary("song-position", "Song position in queue")] int songPosition,
			[Summary("return-back", "Return back after song ends")] bool returnBack = false
		) {
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

		[SlashCommand("prev", "Go to previous song in queue")]
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

		[SlashCommand("next", "Go to next song in queue")]
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

		[SlashCommand("skip", "Skip current song")]
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

		[Group("queue", "Commands to manage selected player queue")]
		public class PlayerQueueInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext>
        {
            private readonly PamelloPlayerRepository _players;
            private readonly PamelloSongRepository _songs;
			private readonly YoutubeInfoService _youtube;

            public PlayerQueueInteractionsModuleGroup(
                PamelloPlayerRepository players,
                PamelloSongRepository songs,
                YoutubeInfoService youtube
            ) : base() {
                _players = players;
                _songs = songs;
				_youtube = youtube;
            }

            private async Task RespondWithEmbedAsync(Embed embed) {
				await ModifyOriginalResponseAsync(message => message.Embed = embed);
			}

			[SlashCommand("song-add", "Add song to queue")]
			public async Task PlayerQueueSongAdd(
				[Summary("song", "Song id, (exact) name, or youtube url")] string songValue
			) {
				int songId;

                if (!int.TryParse(songValue, out songId)) {
					if (songValue.StartsWith("http")) {
						string youtubeId;
						try {
							youtubeId = _youtube.GetVideoIdFromUrl(songValue);
                        }
						catch (Exception x) {
                            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
                            return;
                        }

                        Console.WriteLine("asdasdddddds start");
                        var sId = _songs.GetByYoutubeId(youtubeId)?.Id;
						if (sId is null) {
							try {
                                Console.WriteLine("start");
                                sId = (await _songs.Add(youtubeId)).Id;
                                Console.WriteLine("ebd");
                            }
							catch (Exception x) {
                                await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
								return;
                            }
                        }
                        Console.WriteLine("aaaaaaaess");

                        songId = sId.Value;
                    }
					else {
                        var sId = _songs.GetByName(songValue)?.Id;
						if (sId is null) {
                            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError($"Cant find song with name \"{songValue}\""));
                            return;
                        }

                        songId = sId.Value;
                    }
				}

				try {
                    Context.Commands.PlayerQueueAddSong(songId);
                }
                catch (Exception x) {
                    await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError(x.Message));
					return;
                }

				var song = _songs.Get(songId);

				if (song is null) {
                    await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildError("Added unexistent song succesfully?"));
					return;
                }

                await RespondWithEmbedAsync(
                    PamelloEmbedBuilder.Info($"Added song to \"{Context.User.SelectedPlayer?.Name}\" queue", song.Name)
						.WithThumbnailUrl(song.CoverUrl)
						.Build()
				);
            }

			[SlashCommand("song-insert", "Isert song into specific place in queue")]
			public async Task PlayerQueueSongInsert(
				[Summary("position", "Where song should be inserted")] int position,
				[Summary("song", "Song id or youtube url")] string songValue
			) {
				if (!int.TryParse(songValue, out var songId)) throw new Exception("Only id supported for now");
				Context.Commands.PlayerQueueInsertSong(position, songId);
			}

			[SlashCommand("song-remove", "Remove song from queue")]
			public async Task PlayerQueueSongRemove(
				[Summary("position", "Position of song that should be removed")] int position
			) {
				Context.Commands.PlayerQueueRemoveSong(position);
			}

			[SlashCommand("song-move", "Move song to another place in queue")]
			public async Task PlayerQueueSongMove(
				[Summary("from-position", "Position of song that should be moved")] int fromPosition,
				[Summary("to-position", "Position where song should be puted")] int toPosition
			) {
				Context.Commands.PlayerQueueMove(fromPosition, toPosition);
			}

			[SlashCommand("song-swap", "Swap position in queue of one song with another")]
			public async Task PlayerQueueSongSwap(
				[Summary("from-position", "Position of song that should be swaped with another song")] int fromPosition,
				[Summary("with-position", "Position of another song")] int withPosition
			) {
				Context.Commands.PlayerQueueSwap(fromPosition, withPosition);
			}

			[SlashCommand("song-request-next", "Request song to be played next")]
			public async Task PlayerQueueSongRequestNext(
				[Summary("position", "Position of the song")] int position
			) {
				Context.Commands.PlayerQueueRequestNext(position);
			}

			[SlashCommand("random", "Enable/Disable random playback of queue songs")]
			public async Task PlayerQueueRandom(
				[Summary("state")] EnableDisable state
			) {
				Context.Commands.PlayerQueueRandom(state == EnableDisable.Enable);
			}

			[SlashCommand("reversed", "Enable/Disable reversed playback of queue songs")]
			public async Task PlayerQueueReversed(
				[Summary("state")] EnableDisable state
			) {
				Context.Commands.PlayerQueueReversed(state == EnableDisable.Enable);
			}

			[SlashCommand("no-leftovers", "Enable/Disable removal of played songs")]
			public async Task PlayerQueueNoLeftovers(
				[Summary("state")] EnableDisable state
			) {
				Context.Commands.PlayerQueueNoLeftovers(state == EnableDisable.Enable);
			}

			[SlashCommand("shuffle", "Shuffle queue")]
			public async Task PlayerQueueShuffle() => throw new NotImplementedException();

			[SlashCommand("clear", "Clear queue")]
			public async Task PlayerQueueClear() => throw new NotImplementedException();
		}
	}

	[Group("song", "Commands to manage songs")]
	public class SongInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		private async Task RespondWithEmbedAsync(Embed embed) {
			await ModifyOriginalResponseAsync(message => message.Embed = embed);
		}

		private readonly PamelloSongRepository _songs;

		public SongInteractionsModuleGroup(PamelloSongRepository songs) : base() {
			_songs = songs;
		}

		[SlashCommand("add", "Add new song to pamello database")]
		public async Task SongAdd(
			[Summary("url", "Youtube url of song")] string url
		) {
			

			var song = await _songs.Add(url);
			if (song is null) throw new Exception("That song already present in the database");

			await ModifyOriginalResponseAsync(message => message.Content = $"Added new song `{song}` to the database");
		}

		[SlashCommand("edit", "Edit song field values")]
		public async Task SongEdit(
			[Summary("song", "Song id or Youtube url")] string songValue
		) => throw new NotImplementedException();

		[SlashCommand("delete", "Delete song from the database")]
		public async Task SongDelete(
			[Summary("song", "Song id or Youtube url")] string songValue
		) => throw new NotImplementedException();

		[SlashCommand("search", "Search for song in the database")]
		public async Task SongSearch(
			[Summary("request", "Song name")] string request
		) => throw new NotImplementedException();

		[SlashCommand("info", "Get info about song")]
		public async Task SongInfo(
			[Summary("song", "Song id or Youtube url")] string songValue
		) => throw new NotImplementedException();

		[Group("episode", "Commands to manage song episodes")]
		public class EpisodeInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext>
		{
			[SlashCommand("add", "Add episode to the song")]
			public async Task EpisodeAdd(
				[Summary("song", "Song id or Youtube url")] string songValue,
				[Summary("name", "New episode name")] string episodeName,
				[Summary("start", "Episode starting position (in seconds)")] int start
			) => throw new NotImplementedException();

			[SlashCommand("edit", "Edit episode of the song")]
			public async Task EpisodeEdit() => throw new NotImplementedException();

			[SlashCommand("delete", "Delete episode from the song")]
			public async Task EpisodeDelete() => throw new NotImplementedException();

			[SlashCommand("list", "Show all episodes of the song")]
			public async Task EpisodeList() => throw new NotImplementedException();
		}
	}

	[Group("playlist", "Commands to manage playlists")]
	public class PlaylistInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		private async Task RespondWithEmbedAsync(Embed embed) {
			await ModifyOriginalResponseAsync(message => message.Embed = embed);
		}

		[SlashCommand("create", "Create new playlist")]
		public async Task PlaylistAdd() => throw new NotImplementedException();

		[SlashCommand("edit", "Edit playlist field values")]
		public async Task PlaylistEdit() => throw new NotImplementedException();

		[SlashCommand("add-song", "Add song to playlist")]
		public async Task PlaylistAddSong() => throw new NotImplementedException();

		[SlashCommand("show-mine", "Show list of all playlist you own")]
		public async Task PlaylistShowMine() => throw new NotImplementedException();

		[SlashCommand("delete", "Delete playlist from the database")]
		public async Task PlaylistDelete() => throw new NotImplementedException();

		[SlashCommand("search", "Search for song in database")]
		public async Task PlaylistSearch() => throw new NotImplementedException();

		[SlashCommand("info", "Get info about song")]
		public async Task PlaylistInfo() => throw new NotImplementedException();
	}
}
