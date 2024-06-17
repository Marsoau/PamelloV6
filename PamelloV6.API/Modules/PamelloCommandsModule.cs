using Discord;
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

		private PamelloUser? _user;
		public PamelloUser User {
			get => _user ?? throw new Exception("User required");
			set => _user = value;
		}

		public PamelloCommandsModule(
			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists
		) {
			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
		}

		public void PlayerCreate(string name) => throw new NotImplementedException();
		public void PlayerSelect(int playerId) {
			RequireUser();

			User.SelectedPlayerId = playerId;
		}
		public void PlayerRename(int playerId, string newName) => throw new NotImplementedException();
		public void PlayerDelete(int playerId) => throw new NotImplementedException();

		public void PlayerNext() => throw new NotImplementedException();
		public void PlayerPrev() => throw new NotImplementedException();
		public void PlayerRandom() => throw new NotImplementedException();

		public void PlayerPause() => throw new NotImplementedException();
		public void PlayerResume() => throw new NotImplementedException();
		
		public void PlayerQueueShuffle() => throw new NotImplementedException();
		public void PlayerQueueRandom() => throw new NotImplementedException();
		public void PlayerQueueRepeat() => throw new NotImplementedException();
		public void PlayerQueueClear() => throw new NotImplementedException();

		public void PlayerQueueAddSong(int songId) => throw new NotImplementedException();
		public void PlayerQueueInsertSong(int songId, int queuePosition) => throw new NotImplementedException();
		public void PlayerQueueRemoveSong(int queuePosition) => throw new NotImplementedException();

		public void SongAddYoutube(string youtubeId) => throw new NotImplementedException();
		public void SongAdd(string name, string author, string coverUrl, string souceUrl) => throw new NotImplementedException();
		public void SongEdit(int songId, string propertyName, string newValue) => throw new NotImplementedException();
		public void SongDelete(int songId) => throw new NotImplementedException();

		public void PlaylistAdd(string playlistName, bool isProtected) => throw new NotImplementedException();
		public void PlaylistRename(int playlistId, string newName) => throw new NotImplementedException();
		public void PlaylistChangeProtection(int playlistId, bool protection) => throw new NotImplementedException();
		public void PlaylistDelete(int playlistId) => throw new NotImplementedException();

		public void PlaylistAddSong(int playlistId, int songId) => throw new NotImplementedException();
		public void PlaylistRemoveSong(int playlistId, int position) => throw new NotImplementedException();

		public void EpisodeAdd(int songId, string episodeName, int startSeconds) => throw new NotImplementedException();
		public void EpisodeRename(int episodeId, string newName) => throw new NotImplementedException();
		public void EpisodeChangeStart(int episodeId, int newStart) => throw new NotImplementedException();
		public void EpisodeDelete(int episodeId) => throw new NotImplementedException();

		private void RequireUser(bool mustBeAdmin = false) {
			bool isAdministractor = User.IsAdministrator;
			if (mustBeAdmin && !isAdministractor) {
				throw new Exception("Administrator user required");
			}
		}
	}
}
