using Discord;
using PamelloV6.Core.DTO;
using PamelloV6.Server.Model;
using System.Diagnostics.CodeAnalysis;

namespace PamelloV6.API.Modules
{
	public class PamelloCommandsModule
	{
		public PamelloUser User { get; private set; }

		public PamelloCommandsModule() {

		}

		public void SetUser(PamelloUser user) {
			if (User is not null) {
				throw new Exception("User already set");
			}

			User = user;
		}

		public void PlayerCreate(string name) => throw new NotImplementedException();
		public void PlayerSelect(int playerId) {
			RequireUser();

			User.SelectedPlayerId = playerId;
		}
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

		public void SongAdd(string name, string author, string coverUrl, string souceUrl) => throw new NotImplementedException();
		public void SongDelete(int songId) => throw new NotImplementedException();

		public void PlaylistAdd(string playlistName, bool isProtected, int[] songIds) => throw new NotImplementedException();
		public void PlaylistDelete(int playlistId) => throw new NotImplementedException();

		public void EpisodeAdd(int songId, string episodeName, int startSeconds) => throw new NotImplementedException();
		public void EpisodeDelete(int episodeId) => throw new NotImplementedException();

		private void RequireUser(bool mustBeAdmin = false) {
			if (User is null) {
				throw new Exception("This command/action requires user");
			}
			if (mustBeAdmin && !User.Entity.IsAdministrator) {
				throw new Exception("This command/action requires administrator user");
			}
		}
	}
}
