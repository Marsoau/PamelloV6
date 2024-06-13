using PamelloV6.Server.Model;

namespace PamelloV6.Server.Abstract
{
	public interface IPamelloUserService
	{
		public PamelloUser? GetUser(int id);
		public PamelloUser? GetUser(Guid token);
		public PamelloUser? GetUser(ulong discordId);
	}
}
