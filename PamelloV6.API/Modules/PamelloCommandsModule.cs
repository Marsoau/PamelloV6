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
				.SelectedPlayer ?? throw new Exception("Selected player required");
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

		public int PlayerCreate(string playerName) {
			RequireUser();

			return _players.Create(playerName).Id;
		}
		public bool PlayerSelect(int? playerId) {
			RequireUser();

			if (playerId is null) {
                User.SelectedPlayer = null;
                return true;
			}

			var player = _players.GetRequired(playerId.Value);
			User.SelectedPlayer = player;

			return true;
		}
		public void PlayerRename(string newName) {
			RequireUser();

			updatedPlayer.Name = newName;
		}
		public void PlayerDelete(int playerId) => throw new NotImplementedException();

		public void PlayerNext() {
			RequireUser();

			updatedPlayer.Queue.GoToSong(updatedPlayer.Queue.Position + 1);
		}
		public void PlayerPrev() {
			RequireUser();

			updatedPlayer.Queue.GoToSong(updatedPlayer.Queue.Position - 1);
		}
		public void PlayerSkip() {
			RequireUser();

			updatedPlayer.Queue.GoToNextSong();
		}
		public void PlayerGoToSong(int songPosition, bool returnBack) {
			RequireUser();

			updatedPlayer.Queue.GoToSong(songPosition, returnBack);
		}

		public void PlayerPause() {
			RequireUser();

			updatedPlayer.IsPaused = true;
        }
        public void PlayerResume() {
            RequireUser();

            updatedPlayer.IsPaused = false;
        }
        public void PlayerRewind(int seconds) {
            RequireUser();

            updatedPlayer.Queue.Current?.RewindTo(new AudioTime(seconds));
        }
        public void PlayerRewindToEpisode(int episodePosition) {
            RequireUser();

            updatedPlayer.Queue.Current?.RewindToEpisode(episodePosition);
        }

        public void PlayerQueueShuffle() => throw new NotImplementedException();
		public void PlayerQueueRandom(bool value) {
			RequireUser();

			updatedPlayer.Queue.IsRandom = value;
		}
		public void PlayerQueueReversed(bool value) {
			RequireUser();

			updatedPlayer.Queue.IsReversed = value;
		}
		public void PlayerQueueNoLeftovers(bool value) {
			RequireUser();

			updatedPlayer.Queue.IsNoLeftovers = value;
		}
		public void PlayerQueueClear() {
			RequireUser();

			updatedPlayer.Queue.Clear();
		}

		public void PlayerQueueAddSong(int songId) {
			RequireUser();

			updatedPlayer.Queue.AddSong(songId);
		}
		public void PlayerQueueInsertSong(int queuePosition, int songId) {
			RequireUser();

			updatedPlayer.Queue.InsertSong(queuePosition, songId);
		}
		public void PlayerQueueRemoveSong(int songPosition) {
			RequireUser();

			updatedPlayer.Queue.RemoveSong(songPosition);
		}
		public void PlayerQueueRequestNext(int? position) {
			RequireUser();

			updatedPlayer.Queue.NextPositionRequest = position;
		}
		public void PlayerQueueSwap(int fromPosition, int withPosition) {
			RequireUser();

			updatedPlayer.Queue.SwapSongs(fromPosition, withPosition);
		}
		public void PlayerQueueMove(int fromPosition, int toPosition) {
			RequireUser();

			updatedPlayer.Queue.MoveSong(fromPosition, toPosition);
		}

		public async void SongAddYoutube(string youtubeId) {
			var song = await _songs.Add(youtubeId);
			song?.StartDownload();

			return; //song;
		}
		public void SongEdit(int songId, string propertyName, string newValue) {
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
		public void SongDelete(int songId) => throw new NotImplementedException();

		public void PlaylistAdd(string playlistName, bool isProtected) {
			RequireUser();

			_playlists.Add(playlistName, isProtected, User);
		}
		public void PlaylistRename(int playlistId, string newName) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			playlist.Name = newName;
		}
		public void PlaylistChangeProtection(int playlistId, bool protection) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			playlist.IsProtected = protection;
		}
		public void PlaylistDelete(int playlistId) => throw new NotImplementedException();

		public void PlaylistAddSong(int playlistId, int songId) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);
			var song = _songs.GetRequired(songId);

			playlist.AddSong(song);
		}
		public void PlaylistRemoveSong(int playlistId, int position) {
			RequireUser();

			var playlist = _playlists.GetRequired(playlistId);

			playlist.RemoveSong(position);
		}

		public void EpisodeAdd(int songId, string episodeName, int startSeconds, bool skip) {
			RequireUser();

			var song = _songs.GetRequired(songId);

			song.CreateEpisode(episodeName, startSeconds, skip);
		}
		public void EpisodeRename(int episodeId, string newName) {
			RequireUser();

			var episode = _episodes.GetRequired(episodeId);

			episode.Name = newName;
		}
		public void EpisodeChangeStart(int episodeId, int newStart) {
			RequireUser();

			var episode = _episodes.GetRequired(episodeId);

			episode.Start = new AudioTime(newStart);
		}
		public void EpisodeDelete(int episodeId) => throw new NotImplementedException();

		private void RequireUser(bool mustBeAdmin = false) {
			bool isAdministractor = User.IsAdministrator;
			if (mustBeAdmin && !isAdministractor) {
				throw new Exception("Administrator user required");
			}
		}
	}
}
