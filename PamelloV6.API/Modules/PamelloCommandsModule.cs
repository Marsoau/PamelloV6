using Discord;
using Discord.WebSocket;
using PamelloV6.API.Attributes;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.Core.DTO;
using PamelloV6.Server.Model;
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
			get => _user ?? throw new Exception("User required");
			set => _user = value;
		}

		public PamelloPlayer selectedPlayer {
			get => (_user ?? throw new Exception("User required"))
				.SelectedPlayer ?? throw new Exception("Selected player required");
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

			var guild = _discordClient.GetGuild(1250768227542241450);
			var vc = guild.GetUser(User.DiscordUser.Id).VoiceChannel;
			if (vc is null) throw new Exception("Execting user must be in voice channel");

            await selectedPlayer.Speaker.Connect(vc);
        }
        [PamelloCommand]
        public void PlayerRename(string newName) {
            RequireUser();

            selectedPlayer.Name = newName;
        }
        public void PlayerDelete(int playerId) => throw new NotImplementedException();

        [PamelloCommand]
        public int PlayerNext() {
			RequireUser();

			return selectedPlayer.Queue.GoToSong(selectedPlayer.Queue.Position + 1).Id;
        }
        [PamelloCommand]
        public int PlayerPrev() {
			RequireUser();

            return selectedPlayer.Queue.GoToSong(selectedPlayer.Queue.Position - 1).Id;
        }
        [PamelloCommand]
        public int? PlayerSkip() {
			RequireUser();

			return selectedPlayer.Queue.GoToNextSong()?.Id;
        }
        [PamelloCommand]
        public int PlayerGoToSong(int songPosition, bool returnBack) {
			RequireUser();

			return selectedPlayer.Queue.GoToSong(songPosition, returnBack).Id;
		}

        [PamelloCommand]
        public void PlayerPause() {
			RequireUser();

			selectedPlayer.IsPaused = true;
        }
        [PamelloCommand]
        public void PlayerResume() {
            RequireUser();

            selectedPlayer.IsPaused = false;
        }
        [PamelloCommand]
        public void PlayerRewind(int seconds) {
            RequireUser();

            selectedPlayer.Queue.Current?.RewindTo(new AudioTime(seconds));
        }
        [PamelloCommand]
        public void PlayerRewindToEpisode(int episodePosition) {
            RequireUser();

            selectedPlayer.Queue.Current?.RewindToEpisode(episodePosition);
        }

        public void PlayerQueueShuffle() => throw new NotImplementedException();
        [PamelloCommand]
        public void PlayerQueueRandom(bool value) {
			RequireUser();

			selectedPlayer.Queue.IsRandom = value;
        }
        [PamelloCommand]
        public void PlayerQueueReversed(bool value) {
			RequireUser();

			selectedPlayer.Queue.IsReversed = value;
        }
        [PamelloCommand]
        public void PlayerQueueNoLeftovers(bool value) {
			RequireUser();

			selectedPlayer.Queue.IsNoLeftovers = value;
        }
        [PamelloCommand]
        public void PlayerQueueClear() {
			RequireUser();

			selectedPlayer.Queue.Clear();
		}

        [PamelloCommand]
        public void PlayerQueueAddSong(int songId) {
			RequireUser();

			var song = _songs.GetRequired(songId);
			selectedPlayer.Queue.AddSong(song);
        }
        [PamelloCommand]
        public void PlayerQueueAddPlaylist(int playlistId) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
            var songs = playlist.Songs;

            foreach (var song in songs) {
                selectedPlayer.Queue.AddSong(song);
            }
        }
        [PamelloCommand]
        public void PlayerQueueInsertSong(int queuePosition, int songId) {
			RequireUser();

            var song = _songs.GetRequired(songId);
            selectedPlayer.Queue.InsertSong(queuePosition, song);
        }
        [PamelloCommand]
        public int PlayerQueueRemoveSong(int songPosition) {
			RequireUser();

            return selectedPlayer.Queue.RemoveSong(songPosition).Id;
        }
        [PamelloCommand]
        public void PlayerQueueRequestNext(int? position) {
			RequireUser();

			selectedPlayer.Queue.NextPositionRequest = position;
        }
        [PamelloCommand]
        public void PlayerQueueSwap(int inPosition, int withPosition) {
			RequireUser();

			selectedPlayer.Queue.SwapSongs(inPosition, withPosition);
        }
        [PamelloCommand]
        public void PlayerQueueMove(int fromPosition, int toPosition) {
			RequireUser();

			selectedPlayer.Queue.MoveSong(fromPosition, toPosition);
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
        public void SongDelete(int songId) => throw new NotImplementedException();

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
		public void PlaylistDelete(int playlistId) => throw new NotImplementedException();

        [PamelloCommand]
        public void PlaylistAddSong(int playlistId, int songId) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);
            var song = _songs.GetRequired(songId);

            playlist.AddSong(song);
        }
        [PamelloCommand]
        public void PlaylistInsertSong(int playlistId, int songId, int position) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);
            var song = _songs.GetRequired(songId);

            playlist.InsertSong(position, song);
        }
        [PamelloCommand]
        public void PlaylistMoveSong(int playlistId, int fromPosition, int toPosition) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);

            playlist.MoveSong(fromPosition, toPosition);
        }
        [PamelloCommand]
        public void PlaylistSwapSong(int playlistId, int fromPosition, int withPosition) {
            RequireUser();

            var playlist = _playlists.GetRequired(playlistId);

            playlist.SwapSong(fromPosition, withPosition);
        }
        [PamelloCommand]
        public void PlaylistRemoveSong(int playlistId, int position) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);

			playlist.RemoveSong(position);
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
        public void EpisodeDelete(int episodeId) => throw new NotImplementedException();

		private void RequireUser(bool mustBeAdmin = false) {
			bool isAdministractor = User.IsAdministrator;
			if (mustBeAdmin && !isAdministractor) {
				throw new Exception("Administrator user required");
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
