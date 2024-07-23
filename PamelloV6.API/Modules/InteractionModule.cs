using Discord.Audio;
using Discord.Interactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Expressions;
using Newtonsoft.Json.Linq;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Modules;
using PamelloV6.API.Repositories;
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
        [SlashCommand("connect", "Connect player to your voice channel")]
        public async Task Connect() {
            Context.Commands.PlayerCreate("test");
            Context.Commands.PlayerSelect(1);
            await Context.Commands.PlayerConnect();
        }
    }

	[Group("player", "Commands to maipulate player")]
	public class PlayerInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext> {
		[SlashCommand("select", "Select a player to work with")]
		public async Task PlayerSelect(
			[Summary("player", "Player id or (exact) name")] string value
		) {
			if (!int.TryParse(value, out var id)) throw new Exception("Only id supported for now");
			Context.Commands.PlayerSelect(id);
		}

		[SlashCommand("create", "Create new player")]
		public async Task PlayerCreate(
			[Summary("name", "Name of the new player")] string name
		) {
			Context.Commands.PlayerCreate(name);
		}

		[SlashCommand("connect", "Create new player")]
		public async Task PlayerConnect() {
			await Context.Commands.PlayerConnect();
		}

		[SlashCommand("delete", "Delete player")]
		public async Task PlayerDelete(
			[Summary("player", "Player id or (exact) name")] string value
		) => throw new NotImplementedException();

		[SlashCommand("list", "Get a list of available players")]
		public async Task PlayerList(
			[Summary("page", "Page of players list")] int page = 1
		) => throw new NotImplementedException();

		[SlashCommand("go-to", "Go to specific song from queue")]
		public async Task PlayerGoTo(
            [Summary("song-position", "Song position in queue")] int songPosition,
            [Summary("return-back", "Return back after song ends")] bool returnBack = false
        ) {
			Context.Commands.PlayerGoToSong(songPosition, returnBack);
		}

		[SlashCommand("prev", "Go to previous song in queue")]
		public async Task PlayerPrev() {
            Context.Commands.PlayerPrev();
        }

        [SlashCommand("next", "Go to next song in queue")]
		public async Task PlayerNext() {
            Context.Commands.PlayerNext();
        }

        [SlashCommand("skip", "Skip current song")]
		public async Task PlayerSkip() {
            Context.Commands.PlayerSkip();
        }

        [Group("queue", "Commands to manage selected player queue")]
		public class EpisodeInteractionsModuleGroup : InteractionModuleBase<SocketPamelloInteractionContext>
		{
			[SlashCommand("song-add", "Add song to queue")]
			public async Task PlayerQueueSongAdd(
				[Summary("song", "Song id or youtube url")] string songValue
			) {
                if (!int.TryParse(songValue, out var songId)) throw new Exception("Only id supported for now");
                Context.Commands.PlayerQueueAddSong(songId);
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
