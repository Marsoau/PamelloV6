using Discord;
using Discord.Audio;
using Discord.Interactions;
using Discord.WebSocket;
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

	public class GeneralInteractionsModuleGroup : PamelloInteractionModuleBase
    {
        public GeneralInteractionsModuleGroup(
            PamelloUserRepository users,
            PamelloSongRepository songs,
            PamelloEpisodeRepository episodes,
            PamelloPlaylistRepository playlists,
            PamelloPlayerRepository players,

            YoutubeInfoService youtube
        ) : base(
            users,
            songs,
            episodes,
            playlists,
            players,
            youtube
        ) {

        }

        [SlashCommand("add", "Add song to selected player queue")]
        public async Task AddHandler(
            [Summary("song", "Song id, (exact) name, or youtube url")] string songValue
        ) => await Add(songValue);

        [SlashCommand("connect", "Connect player to your voice channel")]
        public async Task ConnectHander()
            => await Connect();

        [SlashCommand("report-problem", "Report a problem")]
        public async Task ReportProblemHandler(
            [Summary("problem-description", "Describe the problem so i can understand and fix it")] string problemDescription
        ) {
			var file = File.Open(@"C:\.PamelloV6Data\problems.txt", FileMode.OpenOrCreate);

            file.Seek(0, SeekOrigin.End);
            Console.WriteLine($"pos: {file.Position}");
            var sw = new StreamWriter(file);
            sw.WriteLine($"{Context.User} - {DateTime.Now}\n{problemDescription}\n");
            sw.Flush();
            sw.Close();

            await RespondWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Problem reported", problemDescription));
        }
	}

	[Group("player", "Commands to manipulate player")]
	public class PlayerInteractionsModuleGroup : PamelloInteractionModuleBase
    {
        public PlayerInteractionsModuleGroup(
            PamelloUserRepository users,
            PamelloSongRepository songs,
            PamelloEpisodeRepository episodes,
            PamelloPlaylistRepository playlists,
            PamelloPlayerRepository players,

            YoutubeInfoService youtube
        ) : base(
			users,
			songs,
			episodes,
			playlists,
			players,
            youtube
        ) {

		}

		[SlashCommand("select", "Select a player to work with")]
		public async Task PlayerSelectHandler(
			[Summary("player", "Player id or (exact) name")] string? value = null
		) => await PlayerSelect(value);

        [SlashCommand("create", "Create new player")]
		public async Task PlayerCreateHandler(
			[Summary("name", "Name of the new player")] string name
		) => await PlayerCreate(name);

		[SlashCommand("connect", "Create new player")]
		public async Task PlayerConnectHandler()
			=> await PlayerConnect();

        [SlashCommand("delete", "Delete player")]
		public async Task PlayerDeleteHandler(
			[Summary("player", "Player id or (exact) name")] string value
		) => await PlayerDelete(value);

		[SlashCommand("list", "Get a list of available players")]
		public async Task PlayerListHandler(
			[Summary("page", "Page of players list")] int page = 1
		) => await PlayerList(page);

		[SlashCommand("go-to", "Go to specific song from queue")]
		public async Task PlayerGoToHandler(
			[Summary("song-position", "Song position in queue")] int songPosition,
			[Summary("return-back", "Return back after song ends")] bool returnBack = false
		) => await PlayerGoTo(songPosition, returnBack);

		[SlashCommand("prev", "Go to previous song in queue")]
		public async Task PlayerPrevHandler()
			=> await PlayerPrev();

		[SlashCommand("next", "Go to next song in queue")]
		public async Task PlayerNextHandler()
			=> await PlayerNext();

		[SlashCommand("skip", "Skip current song")]
		public async Task PlayerSkipHandler()
			=> await PlayerSkip();

		[Group("queue", "Commands to manage selected player queue")]
		public class PlayerQueueInteractionsModuleGroup : PamelloInteractionModuleBase
        {
            public PlayerQueueInteractionsModuleGroup(
                PamelloUserRepository users,
                PamelloSongRepository songs,
                PamelloEpisodeRepository episodes,
                PamelloPlaylistRepository playlists,
                PamelloPlayerRepository players,

				YoutubeInfoService youtube
            ) : base(
                users,
                songs,
                episodes,
                playlists,
                players,
                youtube
            ) {

            }

            [SlashCommand("song-add", "Add song to queue")]
			public async Task PlayerQueueSongAddHandler(
				[Summary("song", "Song id, (exact) name, or youtube url")] string songValue
			) => await PlayerQueueSongAdd(songValue);

			[SlashCommand("song-insert", "Isert song into specific place in queue")]
			public async Task PlayerQueueSongInsertHandler(
				[Summary("position", "Where song should be inserted")] int position,
				[Summary("song", "Song id or youtube url")] string songValue
			) => await PlayerQueueSongInsert(position, songValue);

            [SlashCommand("playlist-add", "Add song to queue")]
            public async Task PlayerQueuePlaylistAddHandler(
            [Summary("playlist", "Playlist id or (exact) name")] string playlistValue
            ) => await PlayerQueuePlaylistAdd(playlistValue);

            [SlashCommand("song-remove", "Remove song from queue")]
			public async Task PlayerQueueSongRemoveHandler(
				[Summary("position", "Position of song that should be removed")] int position
			) => await PlayerQueueSongRemove(position);

			[SlashCommand("song-move", "Move song to another place in queue")]
			public async Task PlayerQueueSongMoveHandler(
				[Summary("from-position", "Position of song that should be moved")] int fromPosition,
				[Summary("to-position", "Position where song should be puted")] int toPosition
			) => await PlayerQueueSongMove(fromPosition, toPosition);

			[SlashCommand("song-swap", "Swap position in queue of one song with another")]
			public async Task PlayerQueueSongSwapHandler(
				[Summary("from-position", "Position of song that should be swaped with another song")] int fromPosition,
				[Summary("with-position", "Position of another song")] int withPosition
			) => await PlayerQueueSongSwap(fromPosition, withPosition);

			[SlashCommand("song-request-next", "Request song to be played next")]
			public async Task PlayerQueueSongRequestNextHandler(
				[Summary("position", "Position of the song")] int position
			) => await PlayerQueueSongRequestNext(position);

			[SlashCommand("random", "Enable/Disable random playback of queue songs")]
			public async Task PlayerQueueRandomHandler(
				[Summary("state")] EnableDisable state
			) => await PlayerQueueRandom(state == EnableDisable.Enable);

			[SlashCommand("reversed", "Enable/Disable reversed playback of queue songs")]
			public async Task PlayerQueueReversedHandler(
				[Summary("state")] EnableDisable state
            ) => await PlayerQueueReversed(state == EnableDisable.Enable);

            [SlashCommand("no-leftovers", "Enable/Disable removal of played songs")]
			public async Task PlayerQueueNoLeftoversHandler(
				[Summary("state")] EnableDisable state
            ) => await PlayerQueueNoLeftovers(state == EnableDisable.Enable);

			[SlashCommand("shuffle", "Shuffle queue")]
			public async Task PlayerQueueShuffleHandler()
				=> await PlayerQueueShuffle();

			[SlashCommand("clear", "Clear queue")]
			public async Task PlayerQueueClearHandler()
				=> await PlayerQueueClear();
		}
	}

	[Group("song", "Commands to manage songs")]
	public class SongInteractionsModuleGroup : PamelloInteractionModuleBase
    {
        public SongInteractionsModuleGroup(
            PamelloUserRepository users,
            PamelloSongRepository songs,
            PamelloEpisodeRepository episodes,
            PamelloPlaylistRepository playlists,
            PamelloPlayerRepository players,

            YoutubeInfoService youtube
        ) : base(
            users,
            songs,
            episodes,
            playlists,
            players,
            youtube
        ) {

        }

        [SlashCommand("add", "Add new song to pamello database")]
		public async Task SongAddHandler(
			[Summary("url", "Youtube url of song")] string url
		) => await SongAdd(url);

        [SlashCommand("edit-name", "Edit song field values")]
        public async Task SongEditNameHandler(
            [Summary("song", "Song id or Youtube url")] string songValue,
            [Summary("name", "New name")] string newName
        ) => await SongEditName(songValue, newName);
        
		[SlashCommand("edit-author", "Edit song field values")]
        public async Task SongEditAuthorHandler(
            [Summary("song", "Song id or Youtube url")] string songValue,
            [Summary("author", "New author")] string newAuthor
        ) => await SongEditAuthor(songValue, newAuthor);

        [SlashCommand("delete", "Delete song from the database")]
		public async Task SongDeleteHandler(
			[Summary("song", "Song id or Youtube url")] string songValue
        ) => await SongDelete(songValue);

		[SlashCommand("search", "Search for songs in the database")]
		public async Task SongSearchHandler(
			[Summary("request", "Song name")] string request,
            [Summary("page", "Results page")] int page = 1

        ) => await SongSearch(request, page);

		[SlashCommand("info", "Get info about song")]
		public async Task SongInfoHandler(
			[Summary("song", "Song id or Youtube url")] string songValue
		) => await SongInfo(songValue);

		[Group("episode", "Commands to manage song episodes")]
		public class EpisodeInteractionsModuleGroup : PamelloInteractionModuleBase
        {
            public EpisodeInteractionsModuleGroup(
                PamelloUserRepository users,
                PamelloSongRepository songs,
                PamelloEpisodeRepository episodes,
                PamelloPlaylistRepository playlists,
                PamelloPlayerRepository players,

                YoutubeInfoService youtube
            ) : base(
                users,
                songs,
                episodes,
                playlists,
                players,
				youtube
            ) {

            }

            [SlashCommand("add", "Add episode to the song")]
			public async Task EpisodeAddHandler(
				[Summary("song", "Song id or Youtube url")] string songValue,
				[Summary("name", "New episode name")] string episodeName,
				[Summary("start", "Episode starting position (in seconds)")] int start
			) => await EpisodeAdd(songValue, episodeName, start);

			[SlashCommand("rename", "Edit episode of the song")]
			public async Task EpisodeRenameHandler(
                [Summary("episode", "Episode id (look up in song episodes list)")] int episodeId,
                [Summary("name", "New episode name")] string newName
            ) => await EpisodeRename(episodeId, newName);

			[SlashCommand("delete", "Delete episode from the song")]
			public async Task EpisodeDeleteHandler(
                [Summary("episode", "Episode id (look up in song episodes list)")] int episodeId
			) => await EpisodeDelete(episodeId);

			[SlashCommand("list", "Show all episodes of the song")]
			public async Task EpisodeListHandler(
				[Summary("song", "Song id or Youtube url")] string songValue
            ) => await EpisodeList(songValue);
		}
	}

	[Group("playlist", "Commands to manage playlists")]
	public class PlaylistInteractionsModuleGroup : PamelloInteractionModuleBase
    {
        public PlaylistInteractionsModuleGroup(
            PamelloUserRepository users,
            PamelloSongRepository songs,
            PamelloEpisodeRepository episodes,
            PamelloPlaylistRepository playlists,
            PamelloPlayerRepository players,

            YoutubeInfoService youtube
        ) : base(
            users,
            songs,
            episodes,
            playlists,
            players,
            youtube
        ) {

        }

        [SlashCommand("create", "Create new playlist")]
		public async Task PlaylistAddHandler(
            [Summary("name", "New playlist name")] string playlistName
        ) => await PlaylistAdd(playlistName);

		[SlashCommand("rename", "Edit playlist field values")]
		public async Task PlaylistRenameHandler(
            [Summary("playlist", "Playlist id or (exact) name")] string playlistValue,
            [Summary("name", "New playlist name")] string newName
        ) => await PlaylistRename(playlistValue, newName);

		[SlashCommand("add-song", "Add song to playlist")]
		public async Task PlaylistAddSongHandler(
            [Summary("playlist", "Playlist id or (exact) name")] string playlistValue,
            [Summary("song", "Song id, (exact) name, or youtube url")] string songValue
        ) => await PlaylistAddSong(playlistValue, songValue);

		[SlashCommand("show-mine", "Show list of all playlist you own")]
		public async Task PlaylistShowMineHandler()
			=> await PlaylistShowMine();

		[SlashCommand("search", "Search for song in database")]
		public async Task PlaylistSearchHandler(
            [Summary("request", "Song name")] string request,
            [Summary("page", "Results page")] int page = 1
        ) => await PlaylistSearch(request, page);

        [SlashCommand("delete", "Delete playlist from the database")]
        public async Task PlaylistDeleteHandler(
            [Summary("playlist", "Playlist id or (exact) name")] string playlistValue
        ) => await PlaylistDelete(playlistValue);

        [SlashCommand("info", "Get info about song")]
		public async Task PlaylistInfoHandler(
            [Summary("playlist", "Playlist id or (exact) name")] string playlistValue
        ) => await PlaylistInfo(playlistValue);
	}
}
