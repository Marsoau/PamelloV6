using Discord;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Repositories;
using PamelloV6.Core.DTO;
using PamelloV6.Server.Model;
using System.Diagnostics.CodeAnalysis;

namespace PamelloV6.API.Modules
{
	public class PamelloCommandsModule
	{
		protected readonly PamelloUserRepository _users;
		protected readonly PamelloSongRepository _songs;
		protected readonly PamelloEpisodeRepository _episodes;
		protected readonly PamelloPlaylistRepository _playlists;
		protected readonly PamelloPlayerRepository _players;

		private PamelloUser? _user;
		public PamelloUser User {
			get => _user ?? throw new Exception("User required");
			set => _user = value;
		}

		public PamelloPlayer updatedPlayer {
			get => (_user ?? throw new Exception("User required"))
				.selectedPlayer ?? throw new Exception("Selected player required");
		}

		public PamelloCommandsModule(
			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists,
			PamelloPlayerRepository players
		) {
			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
			_players = players;
		}

		public async Task<PamelloPlayer> PlayerCreate(string playerName) {
			RequireUser();

			return _players.Create(playerName);
		}
		public async Task PlayerSelect(int? playerId) {
			RequireUser();

			if (playerId is null) {
                User.selectedPlayer = null;
                return;
			}

			var player = _players.GetRequired(playerId.Value);
			User.selectedPlayer = player;
		}
		public async Task PlayerRename(string newName) {
			RequireUser();

			updatedPlayer.Name = newName;
		}
		public async Task PlayerDelete(int playerId) => throw new NotImplementedException();

		public async Task PlayerNext() {
			RequireUser();

			updatedPlayer.Queue.GoToSong(updatedPlayer.Queue.Position + 1);
		}
		public async Task PlayerPrev() {
			RequireUser();

			updatedPlayer.Queue.GoToSong(updatedPlayer.Queue.Position - 1);
		}
		public async Task PlayerSkip() {
			RequireUser();

			updatedPlayer.Queue.GoToNextSong();
		}
		public async Task PlayerGoToSong(int songPosition, bool returnBack) {
			RequireUser();

			updatedPlayer.Queue.GoToSong(songPosition, returnBack);
		}

		public async Task PlayerPause() {
			RequireUser();

			updatedPlayer.IsPaused = true;
        }
        public async Task PlayerResume() {
            RequireUser();

            updatedPlayer.IsPaused = false;
        }
        public async Task PlayerRewind(int seconds) {
            RequireUser();

            updatedPlayer.Queue.Current?.RewindTo(new AudioTime(seconds));
        }
        public async Task PlayerRewindToEpisode(int episodePosition) {
            RequireUser();

            updatedPlayer.Queue.Current?.RewindToEpisode(episodePosition);
        }

        public async Task PlayerQueueShuffle() => throw new NotImplementedException();
		public async Task PlayerQueueRandom(bool value) {
			RequireUser();

			updatedPlayer.Queue.IsRandom = value;
		}
		public async Task PlayerQueueReversed(bool value) {
			RequireUser();

			updatedPlayer.Queue.IsReversed = value;
		}
		public async Task PlayerQueueNoLeftovers(bool value) {
			RequireUser();

			updatedPlayer.Queue.IsNoLeftovers = value;
		}
		public async Task PlayerQueueClear() {
			RequireUser();

			updatedPlayer.Queue.Clear();
		}

		public async Task PlayerQueueAddSong(int songId) {
			RequireUser();

			updatedPlayer.Queue.AddSong(songId);
		}
		public async Task PlayerQueueInsertSong(int queuePosition, int songId) {
			RequireUser();

			updatedPlayer.Queue.InsertSong(queuePosition, songId);
		}
		public async Task PlayerQueueRemoveSong(int songPosition) {
			RequireUser();

			updatedPlayer.Queue.RemoveSong(songPosition);
		}
		public async Task PlayerQueueRequestNext(int? position) {
			RequireUser();

			updatedPlayer.Queue.NextPositionRequest = position;
		}
		public async Task PlayerQueueSwap(int fromPosition, int withPosition) {
			RequireUser();

			updatedPlayer.Queue.SwapSongs(fromPosition, withPosition);
		}
		public async Task PlayerQueueMove(int fromPosition, int toPosition) {
			RequireUser();

			updatedPlayer.Queue.MoveSong(fromPosition, toPosition);
		}

		public async Task<PamelloSong?> SongAddYoutube(string youtubeId) {
			var song = await _songs.Add(youtubeId);
			song?.StartDownload();

			return song;
		}
		public async Task SongEdit(int songId, string propertyName, string newValue) {
			RequireUser();

			var song = _songs.GetRequired(songId);

			var songType = song.GetType();

			foreach (var property in songType.GetProperties()) {
				if (property.Name == propertyName) {
					if (!(property.SetMethod?.IsPublic ?? false)) {
						throw new Exception("This property doesnt have public setter");
					}

					property.SetValue(song, newValue);
					return;
				}
			}
		}
		public async Task SongDelete(int songId) => throw new NotImplementedException();

		public async Task PlaylistAdd(string playlistName, bool isProtected) {
			RequireUser();

			_playlists.Add(playlistName, isProtected, User);
		}
		public async Task PlaylistRename(int playlistId, string newName) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			playlist.Name = newName;
		}
		public async Task PlaylistChangeProtection(int playlistId, bool protection) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			playlist.IsProtected = protection;
		}
		public async Task PlaylistDelete(int playlistId) => throw new NotImplementedException();

		public async Task PlaylistAddSong(int playlistId, int songId) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			var song = _songs.GetRequired(songId);

			playlist.AddSong(song);
		}
		public async Task PlaylistRemoveSong(int playlistId, int position) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);

			playlist.RemoveSong(position);
		}

		public async Task EpisodeAdd(int songId, string episodeName, int startSeconds, bool skip) {
			RequireUser();

			var song = _songs.GetRequired(songId);

			song.CreateEpisode(episodeName, startSeconds, skip);
		}
		public async Task EpisodeRename(int episodeId, string newName) {
			RequireUser();

			var episode = _episodes.GetRequired(episodeId);

			episode.Name = newName;
		}
		public async Task EpisodeChangeStart(int episodeId, int newStart) {
			RequireUser();

			var episode = _episodes.GetRequired(episodeId);

			episode.Start = new AudioTime(newStart);
		}
		public async Task EpisodeDelete(int episodeId) => throw new NotImplementedException();

		private void RequireUser(bool mustBeAdmin = false) {
			bool isAdministractor = User.IsAdministrator;
			if (mustBeAdmin && !isAdministractor) {
				throw new Exception("Administrator user required");
			}
		}
	}
}
