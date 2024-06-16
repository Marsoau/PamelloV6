using Discord.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV6.API.Modules;
using PamelloV6.API.Repositories;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;
using System.ComponentModel;

namespace PamelloV6.API.Controllers
{
    [Route("[controller]")]
	[ApiController]
	public class CommandController : ControllerBase
	{
		private readonly PamelloUserRepository _users;
		private readonly PamelloCommandsModule _commands;

		public CommandController(
			PamelloUserRepository users,
			PamelloCommandsModule commands
		) {
			_users = users;
			_commands = commands;
		}

		[HttpGet]
		public IActionResult Get() {
			var queriedCommand = Request.Query["name"].FirstOrDefault();
			if (queriedCommand is null) {
				return BadRequest("Command name in querry \"name\" required");
			}

			var queriedToken = Request.Headers["user-token"].FirstOrDefault();
			if (queriedToken is null) {
				return BadRequest("User token required");
			}

			if (!Guid.TryParse(queriedToken, out Guid userToken)) {
				return BadRequest("Wrong user token format");
			}

			var user = _users.GetUser(userToken);
			if (user is null) {
				return NotFound($"Cant get user by {userToken} token");
			}

			_commands.SetUser(user);

			var command = typeof(PamelloCommandsModule).GetMethod(queriedCommand);
			if (command is null) {
				return NotFound($"Cant find command named \"{queriedCommand}\"");
			}

			var argsInfo = command.GetParameters();
			var args = new object[argsInfo.Length];

			string argStringValue;
			object? argValue;
            for (int i = 0; i < argsInfo.Length; i++) {
				argStringValue = Request.Query[argsInfo[i].Name ?? ""].FirstOrDefault() ?? "";
				if (argStringValue is null) {
					return BadRequest($"Cant find argument {argsInfo[i].Name} in query");
				}
				argValue = TypeDescriptor.GetConverter(argsInfo[i].ParameterType).ConvertFromInvariantString(argStringValue);
				if (argValue is null) {
					return BadRequest($"Cant convert argument {argsInfo[i].Name} value");
				}

				args[i] = argValue;
			}

			try {
				command.Invoke(_commands, args);
			}
			catch (Exception x) {
				return BadRequest($"Execution of command interrupted by exception, message: {x.Message}");
			}

			return Ok();
        }
	}
}
