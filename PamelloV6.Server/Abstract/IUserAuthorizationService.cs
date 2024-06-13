using PamelloV6.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV6.Server.Abstract
{
	public interface IUserAuthorizationService
	{
		public int GetCode(ulong discordId);
		public ulong? GetDiscordId(int code);
	}
}
