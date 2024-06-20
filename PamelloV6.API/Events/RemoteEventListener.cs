using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using PamelloV6.Server.Model;
using System.Text;
using System.Text.Json;

namespace PamelloV6.API.Events
{
    public class RemoteEventListener
    {
        private readonly HttpResponse _response;
        public readonly PamelloUser User;

        public RemoteEventListener(HttpResponse response, PamelloUser user) {
            _response = response;
            User = user;
        }

        public void SendEvent(string header, object? data) {
            Task.Run(() => SendEventAsync(header, data));
        }
        public async Task SendEventAsync(string header, object? data) {
            await _response.WriteAsync($"event: {header}\rdata: {JsonSerializer.Serialize(data)}\r\r");
            /*await _response.WriteAsJsonAsync();
            await _response.WriteAsync("\r\r");*/
            await _response.Body.FlushAsync();
        }
    }
}
