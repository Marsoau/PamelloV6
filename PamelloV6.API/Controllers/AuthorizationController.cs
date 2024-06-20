using Microsoft.AspNetCore.Mvc;
using PamelloV6.API.Repositories;
using PamelloV6.Server.Services;

namespace PamelloV6.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserAuthorizationService _authtorization;
        private readonly PamelloUserRepository _users;

        public AuthorizationController(
            UserAuthorizationService authtorization,
            PamelloUserRepository users
        ) {
            _authtorization = authtorization;
            _users = users;
        }

        [HttpGet("GetToken")]
        public IActionResult GetToken() {
            var qCode = Request.Query["code"].FirstOrDefault();
            if (qCode is null) {
                return BadRequest("Authorization code required");
            }

            if (!int.TryParse(qCode, out int code) || !(100000 <= code && code <= 999999)) {
                return BadRequest("Authorization code must me an 6 digit integer number");
            }

            var discordId = _authtorization.GetDiscordId(code);
            if (discordId is null) {
                return BadRequest("Invalid code");
            }

            var user = _users.Get(discordId.Value);
            if (user is null) {
                return NotFound($"Code is correct, but cant find user with discord id {discordId} for some reason");
            }

            return Ok(user.Token);
        }
    }
}
