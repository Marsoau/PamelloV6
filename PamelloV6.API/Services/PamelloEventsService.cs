using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.PortableExecutable;
using PamelloV6.Server.Model;
using PamelloV6.API.Model.Events;
using PamelloV6.API.Exceptions;

namespace PamelloV6.API.Services
{
    public class PamelloEventsService
    {
        private readonly List<RemoteEventListener> _listeners;

        public PamelloEventsService() {
            _listeners = new List<RemoteEventListener>();
        }

        public RemoteEventListener AddListener(HttpResponse response) {
            var listener = new RemoteEventListener(response);
            _listeners.Add(listener);

            return listener;
        }
        public void RemoveListener(RemoteEventListener listener) {
            _listeners.Remove(listener);
        }

        public void AuthorizeListener(Guid eventsKey, PamelloUser? user) {
            foreach (var listener in _listeners) {
                if (listener.Key == eventsKey) {
                    listener.User = user;
                    return;
                }
            }

            throw new PamelloException($"Cant find event listener with key {eventsKey}");
        }
        public void UnauthorizeListener(Guid eventsKey) {
            AuthorizeListener(eventsKey, null);
        }

        public void SendToAll(PamelloEvent pamelloEvent) {
            Console.WriteLine($"[Event To All] {pamelloEvent}");
            foreach (var listener in _listeners) {
                listener.SendEvent(pamelloEvent);
            }
        }
        public void SendToOne(int userId, PamelloEvent pamelloEvent) {
            Console.WriteLine($"[Event To One | userId: {userId}] {pamelloEvent}");
            foreach (var listener in _listeners) {
                if (listener.User?.Id == userId) listener.SendEvent(pamelloEvent);
            }
        }
        public void SendToAllWithSelectedPlayer(int selectedPlayerId, PamelloEvent pamelloEvent) {
            Console.WriteLine($"[Event To Selected Player | playerId: {selectedPlayerId}] {pamelloEvent}");
            foreach (var listener in _listeners) {
                if (listener.User?.SelectedPlayer?.Id == selectedPlayerId) listener.SendEvent(pamelloEvent);
            }
        }
    }
}
