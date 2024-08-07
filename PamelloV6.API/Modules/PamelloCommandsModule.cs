using Discord;
using Discord.WebSocket;
using PamelloV6.API.Attributes;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.Core.DTO;
using PamelloV6.Server.Model;
using PamelloV6.Server.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PamelloV6.API.Modules
{
	public class PamelloCommandsModule
	{
		protected readonly PamelloUserRepository _users;
		protected readonly PamelloSongRepository _songs;
		protected readonly PamelloEpisodeRepository _episodes;
		protected readonly PamelloPlaylistRepository _playlists;
		protected readonly PamelloPlayerRepository _players;

		protected readonly DiscordSocketClient _discordClient;
        protected readonly YoutubeInfoService _youtube;


        private PamelloUser? _user;
		public PamelloUser User {
			get => _user ?? throw new PamelloException("User required");
			set => _user = value;
		}

		public PamelloPlayer SelectedPlayer {
			get => (_user ?? throw new PamelloException("User required"))
				.SelectedPlayer ?? throw new PamelloException("Selected player required");
		}

		public PamelloCommandsModule(
			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists,
			PamelloPlayerRepository players,

            DiscordSocketClient discordClient,
            YoutubeInfoService youtube
        ) {
			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
			_players = players;

			_discordClient = discordClient;
            _youtube = youtube;
        }


        [PamelloCommand]
        public int PlayerCreate(string playerName) {
			RequireUser();

			return _players.Create(playerName).Id;
        }
        [PamelloCommand]
        public void PlayerSelect(int? playerId) {
			RequireUser();

			if (playerId is null) {
                User.SelectedPlayer = null;
                return;
			}

			var player = _players.GetRequired(playerId.Value);
			User.SelectedPlayer = player;
        }
        [PamelloCommand]
        public async Task PlayerConnect() {
            RequireUser();

            await SelectedPlayer.Speakers.ConnectSpeakerToUserVc(User);
        }
        [PamelloCommand]
        public async Task PlayerDisconnect(int speakerPosition) {
            RequireUser();

            await SelectedPlayer.Speakers.Disconnect(speakerPosition);
        }
        [PamelloCommand]
        public void PlayerRename(string newName) {
            RequireUser();

            SelectedPlayer.Name = newName;
        }
        [PamelloCommand]
        public void PlayerDeleteSelected() {
            RequireUser();

            _players.Delete(SelectedPlayer.Id);
        }

        [PamelloCommand]
        public int PlayerNext() {
			RequireUser();

			return SelectedPlayer.Queue.GoToSong(SelectedPlayer.Queue.Position + 1).Id;
        }
        [PamelloCommand]
        public int PlayerPrev() {
			RequireUser();

            return SelectedPlayer.Queue.GoToSong(SelectedPlayer.Queue.Position - 1).Id;
        }
        [PamelloCommand]
        public int? PlayerSkip() {
			RequireUser();

			return SelectedPlayer.Queue.GoToNextSong()?.Id;
        }
        [PamelloCommand]
        public int PlayerGoToSong(int songPosition, bool returnBack) {
			RequireUser();

			return SelectedPlayer.Queue.GoToSong(songPosition, returnBack).Id;
		}

        [PamelloCommand]
        public void PlayerPause() {
			RequireUser();

			SelectedPlayer.IsPaused = true;
        }
        [PamelloCommand]
        public void PlayerResume() {
            RequireUser();

            SelectedPlayer.IsPaused = false;
        }
        [PamelloCommand]
        public void PlayerRewind(int seconds) {
            RequireUser();

            SelectedPlayer.Queue.Current?.RewindTo(new AudioTime(seconds));
        }
        [PamelloCommand]
        public void PlayerRewindToEpisode(int episodePosition) {
            RequireUser();

            SelectedPlayer.Queue.Current?.RewindToEpisode(episodePosition);
        }

        public void PlayerQueueShuffle() => throw new NotImplementedException();
        [PamelloCommand]
        public void PlayerQueueRandom(bool value) {
			RequireUser();

			SelectedPlayer.Queue.IsRandom = value;
        }
        [PamelloCommand]
        public void PlayerQueueReversed(bool value) {
			RequireUser();

			SelectedPlayer.Queue.IsReversed = value;
        }
        [PamelloCommand]
        public void PlayerQueueNoLeftovers(bool value) {
			RequireUser();

			SelectedPlayer.Queue.IsNoLeftovers = value;
        }
        [PamelloCommand]
        public void PlayerQueueClear() {
			RequireUser();

			SelectedPlayer.Queue.Clear();
		}

        [PamelloCommand]
        public void PlayerQueueAddSong(int songId) {
			RequireUser();

			var song = _songs.GetRequired(songId);
			SelectedPlayer.Queue.AddSong(song);
        }
        [PamelloCommand]
        public void PlayerQueueInsertSong(int queuePosition, int songId) {
			RequireUser();

            var song = _songs.GetRequired(songId);
            SelectedPlayer.Queue.InsertSong(queuePosition, song);
        }
        [PamelloCommand]
        public void PlayerQueueAddPlaylist(int playlistId) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);
            SelectedPlayer.Queue.AddPlaylist(playlist);
        }
        [PamelloCommand]
        public void PlayerQueueInsertPlaylist(int queuePosition, int playlistId) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);
            SelectedPlayer.Queue.InsertPlaylist(queuePosition, playlist);
        }
        [PamelloCommand]
        public int PlayerQueueRemoveSong(int songPosition) {
			RequireUser();

            return SelectedPlayer.Queue.RemoveSong(songPosition).Id;
        }
        [PamelloCommand]
        public void PlayerQueueRequestNext(int? position) {
			RequireUser();

			SelectedPlayer.Queue.NextPositionRequest = position;
        }
        [PamelloCommand]
        public void PlayerQueueSwap(int inPosition, int withPosition) {
			RequireUser();

			SelectedPlayer.Queue.SwapSongs(inPosition, withPosition);
        }
        [PamelloCommand]
        public void PlayerQueueMove(int fromPosition, int toPosition) {
			RequireUser();

			SelectedPlayer.Queue.MoveSong(fromPosition, toPosition);
		}
        [PamelloCommand]
        public async Task<int> SongAddYoutube(string youtubeId) {
			var song = await _songs.Add(youtubeId);
			song.StartDownload();

			return song.Id;
        }
        [PamelloCommand]
        public void SongEditName(int songId, string newName) {
			RequireUser();

            var song = _songs.GetRequired(songId);
			song.Name = newName;
        }
        [PamelloCommand]
        public void SongEditAuthor(int songId, string newAuthor) {
            RequireUser();

            var song = _songs.GetRequired(songId);
            song.Author = newAuthor;
        }
        [PamelloCommand]
        public void SongMoveEpisode(int songId, int fromPosition, int toPosition) {
            RequireUser();

            var song = _songs.GetRequired(songId);

            song.MoveEpisode(fromPosition, toPosition);
        }
        [PamelloCommand]
        public void SongSwapEpisode(int songId, int fromPosition, int withPosition) {
            RequireUser();

            var song = _songs.GetRequired(songId);

            song.SwapEpisode(fromPosition, withPosition);
        }

        [PamelloCommand]
        public int PlaylistAdd(string playlistName, bool isProtected) {
			RequireUser();

			return _playlists.Add(playlistName, isProtected, User).Id;
        }
        [PamelloCommand]
        public void PlaylistRename(int playlistId, string newName) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			playlist.Name = newName;
        }
        [PamelloCommand]
        public void PlaylistChangeProtection(int playlistId, bool protection) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			playlist.IsProtected = protection;
        }
        [PamelloCommand]
        public void PlaylistDelete(int playlistId) {
            RequireUser();

            _playlists.Delete(playlistId);
        }

        [PamelloCommand]
        public void PlaylistAddSong(int playlistId, int songId) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);
            var song = _songs.GetRequired(songId);

            playlist.AddSong(song);
        }
        [PamelloCommand]
        public void PlaylistInsertSong(int playlistId, int position, int songId) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);
            var song = _songs.GetRequired(songId);

            playlist.InsertSong(position, song);
        }
        [PamelloCommand]
        public void PlaylistAddPlaylistSongs(int toPlaylistId, int fromPlaylistId) {
            RequireUser();

            var toPlaylist = _playlists.GetRequired(toPlaylistId);
            var fromPlaylist = _playlists.GetRequired(fromPlaylistId);

            toPlaylist.AddSongs(fromPlaylist.Songs);
        }
        [PamelloCommand]
        public void PlaylistInsertPlaylistSongs(int toPlaylistId, int position, int fromPlaylistId) {
            RequireUser();

            var toPlaylist = _playlists.GetRequired(toPlaylistId);
            var fromPlaylist = _playlists.GetRequired(fromPlaylistId);

            toPlaylist.InsertSongs(position, fromPlaylist.Songs);
        }
        [PamelloCommand]
        public void PlaylistMoveSong(int playlistId, int fromPosition, int toPosition) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);

            playlist.MoveSong(fromPosition, toPosition);
        }
        [PamelloCommand]
        public void PlaylistSwapSong(int playlistId, int inPosition, int withPosition) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);

            playlist.SwapSong(inPosition, withPosition);
        }
        [PamelloCommand]
        public void PlaylistRemoveSong(int playlistId, int songId) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);
            var song = _songs.GetRequired(songId);

            playlist.RemoveSong(song);
        }
        [PamelloCommand]
        public void PlaylistRemoveSongAt(int playlistId, int songPosition) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);

            playlist.RemoveSongAt(songPosition);
        }

        [PamelloCommand]
        public int EpisodeAdd(int songId, string episodeName, int startSeconds, bool skip) {
			RequireUser();

			var song = _songs.GetRequired(songId);

			return song.CreateEpisode(episodeName, startSeconds, skip).Id;
        }
        [PamelloCommand]
        public void EpisodeRename(int episodeId, string newName) {
			RequireUser();

			var episode = _episodes.GetRequired(episodeId);

			episode.Name = newName;
        }
        [PamelloCommand]
        public void EpisodeChangeStart(int episodeId, int newStart) {
            RequireUser();

            var episode = _episodes.GetRequired(episodeId);

            episode.Start = new AudioTime(newStart);
        }
        [PamelloCommand]
        public void EpisodeChangeSkipState(int episodeId, bool newState) {
            RequireUser();

            var episode = _episodes.GetRequired(episodeId);

            episode.Skip = newState;
        }
        [PamelloCommand]
        public void EpisodeDelete(int episodeId) {
            RequireUser();

            _episodes.Delete(episodeId);
        }

		private void RequireUser(bool mustBeAdmin = false) {
			bool isAdministractor = User.IsAdministrator;
			if (mustBeAdmin && !isAdministractor) {
				throw new PamelloException("Administrator user required");
			}
		}

		//debug only
		public string GetTSString() {
			var sb = new StringBuilder();

            /*			
			public async PlayerPrev() {
				return await this.InvokeCommand(`PlayerPrev`);
			}
			*/

            foreach (var method in typeof(PamelloCommandsModule).GetMethods()) {
				if (method.CustomAttributes.Any(attr => attr.AttributeType == typeof(PamelloCommandAttribute))) {
                    sb.Append($"public async {method.Name}(");
					foreach (var parameter in method.GetParameters()) {
                        sb.Append($"{parameter.Name}: {parameter.ParameterType}, ");
                    }
                    sb.Append($"): Promise<{method.ReturnType.Name}> {{\n\treturn await this.InvokeCommand(`{method.Name}");
                    foreach (var parameter in method.GetParameters()) {
                        sb.Append($"&{parameter.Name}=${{{parameter.Name}}}");
                    }
                    sb.AppendLine($"`) as {method.ReturnType.Name};\n}}");
                }
			}

			return sb.ToString();
		}
	}
}
