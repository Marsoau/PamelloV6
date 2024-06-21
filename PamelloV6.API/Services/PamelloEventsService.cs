﻿using PamelloV6.API.Events;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.PortableExecutable;
using PamelloV6.Server.Model;

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

        public void SendToAll(string header, object? data) {
            foreach (var listener in _listeners) {
                listener.SendEvent(header, data);
            }
        }
        public void SendToOne(int userId, string header, object? data) {
            foreach (var listener in _listeners) {
                if (listener.User.Id == userId) listener.SendEvent(header, data);
            }
        }
        public void SendToAllWithSelectedPlayer(int updatedPlayerId, string header, object? data) {
            foreach (var listener in _listeners) {
                if (listener.User.selectedPlayer?.Id == updatedPlayerId) listener.SendEvent(header, data);
            }
        }
    }
}
