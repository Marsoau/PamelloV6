using Discord.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV6.API.Modules;
using PamelloV6.API.Repositories;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;
using System.ComponentModel;
using System.Reflection;

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

			var user = _users.Get(userToken);
			if (user is null) {
				return NotFound($"Cant get user by {userToken} token");
			}

			_commands.User = user;

			var command = typeof(PamelloCommandsModule).GetMethod(queriedCommand);
			if (command is null) {
				return NotFound($"Cant find command named \"{queriedCommand}\"");
			}

			var argsInfo = command.GetParameters();
			var args = new object?[argsInfo.Length];

			string? argKey;
			string? argStringValue;
			object? argValue;
            for (int i = 0; i < argsInfo.Length; i++) {
				argKey = argsInfo[i].Name;
				if (argKey is null) {
					continue;
				}

				argStringValue = Request.Query[argKey].FirstOrDefault();
				if (argStringValue is null) {
					return BadRequest($"Cant find argument {argsInfo[i].Name} in query");
				}
				try {
					argValue = TypeDescriptor.GetConverter(argsInfo[i].ParameterType).ConvertFromInvariantString(argStringValue);
				}
				catch {
					return BadRequest($"Cant convert \"{argsInfo[i].Name}\" argument \"{argStringValue}\" value to \"{argsInfo[i].ParameterType}\" type");
				}

				args[i] = argValue;
			}

            Console.Write($"[{_commands.User}] Executing network command /{command.Name} ");
            for (int i = 0; i < argsInfo.Length; i++) {
                Console.Write($"{argsInfo[i].Name}: {args[i]}");
                if (i != argsInfo.Length - 1) {
                    Console.Write(", ");
                }
            }
            Console.WriteLine();

            try {
                command.Invoke(_commands, args);
			}
			catch (TargetInvocationException tie) {
				return BadRequest($"Execution of command interrupted by exception, message: {tie.InnerException?.Message}");
			}

			return Ok();
        }
	}
}
