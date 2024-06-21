using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV6.API.Repositories;
using PamelloV6.API.Services;
using PamelloV6.Server.Services;

namespace PamelloV6.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly PamelloEventsService _events;
        private readonly PamelloUserRepository _users;

        public EventsController(
            PamelloEventsService events,
            PamelloUserRepository users
        ) {
            _events = events;
            _users = users;
        }

        public async Task Get() {
            var queriedToken = Request.Headers["user-token"].FirstOrDefault();
            if (queriedToken is null) {
                BadRequest("User token required");
                return;
            }

            if (!Guid.TryParse(queriedToken, out Guid userToken)) {
                BadRequest("Wrong user token format");
                return;
            }

            var user = _users.Get(userToken);
            if (user is null) {
                NotFound($"Cant get user by {userToken} token");
                return;
            }

            Response.Headers.ContentType = "text/event-stream";
            Response.Headers.CacheControl = "no-cache";
            Response.Headers.Connection = "keep-alive";
            await Response.Body.FlushAsync();

            _events.AddListener(Response, user);

            await Task.Delay(-1);
        }
    }
}
