using Discord;
using Microsoft.AspNetCore.Mvc;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.Server.Model;
using PamelloV6.Server.Services;

namespace PamelloV6.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserAuthorizationService _authtorization;
        private readonly PamelloUserRepository _users;
        private readonly PamelloEventsService _events;

        public AuthorizationController(
            UserAuthorizationService authtorization,
            PamelloUserRepository users,
            PamelloEventsService events
        ) {
            _authtorization = authtorization;
            _users = users;
            _events = events;
        }

        [HttpGet("Events")]
        public IActionResult Events() {
            var qKey = Request.Query["events-key"].FirstOrDefault();
            if (qKey is null) {
                return BadRequest("Events key required");
            }
            if (!Guid.TryParse(qKey, out var key)) {
                return BadRequest("Invalid key format");
            }

            var qCode = Request.Query["code"].FirstOrDefault();
            var qUserToken = Request.Query["user-token"].FirstOrDefault();
            PamelloUser? user;

            if (qCode is not null) {
                if (!int.TryParse(qCode, out var code) || !(100000 <= code && code <= 999999)) {
                    return BadRequest("Authorization code must me an 6 digit integer number");
                }

                var discordId = _authtorization.GetDiscordId(code);
                if (discordId is null) {
                    return BadRequest("Invalid code");
                }

                user = _users.Get(discordId.Value);
            }
            else if (qUserToken is not null) {
                if (!Guid.TryParse(qUserToken, out var userToken)) {
                    return BadRequest("Invalid user token format");
                }

                user = _users.Get(userToken);
            }
            else {
                return BadRequest("Authorization code or user token required");
            }

            if (user is null) {
                return NotFound($"Cant find user");
            }

            try {
                _events.AuthorizeListener(key, user);
            }
            catch (PamelloException x) {
                return NotFound(x.Message);
            }

            return Ok();
        }
        [HttpGet("Events/Close")]
        public IActionResult EventsClose() {
            var qKey = Request.Query["events-key"].FirstOrDefault();
            if (qKey is null) {
                return BadRequest("Events key required");
            }
            if (!Guid.TryParse(qKey, out var key)) {
                return BadRequest("Invalid key format");
            }

            try {
                _events.UnauthorizeListener(key);
            }
            catch (PamelloException x) {
                return NotFound(x.Message);
            }

            return Ok();
        }
    }
}
