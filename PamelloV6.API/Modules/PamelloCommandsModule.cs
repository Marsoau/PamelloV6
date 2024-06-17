using Discord;
using PamelloV6.API.Model;
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

		public async Task PlayerCreate(string name) => throw new NotImplementedException();
		public async Task PlayerSelect(int playerId) {
			RequireUser();

			User.SelectedPlayerId = playerId;
		}
		public async Task PlayerRename(int playerId, string newName) => throw new NotImplementedException();
		public async Task PlayerDelete(int playerId) => throw new NotImplementedException();

		public async Task PlayerNext() => throw new NotImplementedException();
		public async Task PlayerPrev() => throw new NotImplementedException();
		public async Task PlayerRandom() => throw new NotImplementedException();

		public async Task PlayerPause() => throw new NotImplementedException();
		public async Task PlayerResume() => throw new NotImplementedException();
		
		public async Task PlayerQueueShuffle() => throw new NotImplementedException();
		public async Task PlayerQueueRandom() => throw new NotImplementedException();
		public async Task PlayerQueueRepeat() => throw new NotImplementedException();
		public async Task PlayerQueueClear() => throw new NotImplementedException();

		public async Task PlayerQueueAddSong(int songId) => throw new NotImplementedException();
		public async Task PlayerQueueInsertSong(int songId, int queuePosition) => throw new NotImplementedException();
		public async Task PlayerQueueRemoveSong(int queuePosition) => throw new NotImplementedException();

		public async Task<PamelloSong?> SongAddYoutube(string youtubeId) {
			var song = await _songs.Add(youtubeId);
			song?.StartDownload();

			return song;
		}
		public async Task SongAdd(string name, string author, string coverUrl, string souceUrl) => throw new NotImplementedException();
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

			episode.Start = newStart;
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
