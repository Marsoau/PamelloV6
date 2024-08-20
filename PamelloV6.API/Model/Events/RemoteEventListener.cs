using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using PamelloV6.Server.Model;
using System.Text;
using System.Text.Json;

namespace PamelloV6.API.Model.Events
{
    public class RemoteEventListener
    {
        public readonly Guid Key;

        private readonly HttpResponse _response;

        private PamelloUser? _user;
        public PamelloUser? User {
            get {
                return _user;
            } 
            set {
                var oldUser = _user;
                _user = value;

                if (User is null) {
                    Console.WriteLine($"{{\"{Key}\" events UNauthroized user {oldUser}}}");
                    SendEvent(PamelloEvent.Unauthorized());
                }
                else {
                    Console.WriteLine($"{{\"{Key}\" events Authroized user {User}}}");
                    SendEvent(PamelloEvent.Authorized(User.Token));
                }
            }
        }

        public RemoteEventListener(HttpResponse response)
        {
            Key = Guid.NewGuid();

            _response = response;

            Console.WriteLine($"{{Created new events \"{Key}\"}}");
            SendEvent(PamelloEvent.EventsConnected(Key));
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
