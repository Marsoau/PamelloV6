using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using PamelloV6.Server.Model;
using System.Text;
using System.Text.Json;

namespace PamelloV6.API.Model.Events
{
    public class RemoteEventListener
    {
        private readonly HttpResponse _response;
        public readonly PamelloUser User;

        public RemoteEventListener(HttpResponse response, PamelloUser user)
        {
            _response = response;
            User = user;
        }

        public void SendEvent(PamelloEvent pammelloEvent)
        {
            Task.Run(() => SendEventAsync(pammelloEvent));
        }
        public async Task SendEventAsync(PamelloEvent pammelloEvent)
        {
            await _response.WriteAsync($"event: {pammelloEvent.Header}\rdata: {JsonSerializer.Serialize(pammelloEvent.Data)}\r\r");
            await _response.Body.FlushAsync();
        }
    }
}
