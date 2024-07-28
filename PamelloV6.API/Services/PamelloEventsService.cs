using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.PortableExecutable;
using PamelloV6.Server.Model;
using PamelloV6.API.Model.Events;

namespace PamelloV6.API.Services
{
    public class PamelloEventsService
    {
        private readonly List<RemoteEventListener> _listeners;

        public PamelloEventsService() {
            _listeners = new List<RemoteEventListener>();
        }

        public RemoteEventListener AddListener(HttpResponse response, PamelloUser user) {
            var listener = new RemoteEventListener(response, user);
            _listeners.Add(listener);

            return listener;
        }
        public void RemoveListener(RemoteEventListener listener) {
            _listeners.Remove(listener);
        }

        public void SendToAll(PamelloEvent pamelloEvent) {
            foreach (var listener in _listeners) {
                listener.SendEvent(pamelloEvent);
            }
        }
        public void SendToOne(int userId, PamelloEvent pamelloEvent) {
            foreach (var listener in _listeners) {
                if (listener.User.Id == userId) listener.SendEvent(pamelloEvent);
            }
        }
        public void SendToAllWithSelectedPlayer(int selectedPlayerId, PamelloEvent pamelloEvent) {
            foreach (var listener in _listeners) {
                if (listener.User.SelectedPlayer?.Id == selectedPlayerId) listener.SendEvent(pamelloEvent);
            }
        }
    }
}
