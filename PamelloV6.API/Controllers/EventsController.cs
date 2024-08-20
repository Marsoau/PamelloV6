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
            Response.Headers.ContentType = "text/event-stream";
            Response.Headers.CacheControl = "no-cache";
            //Response.Headers.Connection = "keep-alive";
            await Response.Body.FlushAsync();

            _events.AddListener(Response);

            await Task.Delay(-1);
        }
    }
}
