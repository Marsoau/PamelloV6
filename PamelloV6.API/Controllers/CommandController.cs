using Discord.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV6.API.Modules;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;
using PamelloV6.Server.Services;
using System.ComponentModel;

namespace PamelloV6.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class CommandController : ControllerBase
	{
		private readonly PamelloUserService _users;
		private readonly PamelloCommandsModule _commands;

		public CommandController(
			PamelloUserService users,
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

			command.Invoke(_commands, args);

			return Ok();
        }
	}
}
