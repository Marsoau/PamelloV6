using Discord;
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

		public void PlayerCreate() => throw new NotImplementedException();
		public void PlayerSelect(int playerId) {
			RequireUser();

			User.SelectedPlayerId = playerId;
		}
		public void PlayerDelete() => throw new NotImplementedException();

		public void PlayerNext() => throw new NotImplementedException();
		public void PlayerPrev() => throw new NotImplementedException();
		public void PlayerPause() => throw new NotImplementedException();
		public void PlayerResume() => throw new NotImplementedException();
		
		public void PlayerQueueShuffle() => throw new NotImplementedException();
		public void PlayerQueueRepeat() => throw new NotImplementedException();
		public void PlayerQueueClear() => throw new NotImplementedException();

		private void RequireUser(bool mustBeAdmin = false) {
			if (User is null) {
				throw new Exception("This command/action requires user");
			}
			if (mustBeAdmin && !User.UserEntity.IsAdministrator) {
				throw new Exception("This command/action requires administrator user");
			}
		}
	}
}
