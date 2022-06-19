using Microsoft.AspNetCore.SignalR;
using Pokerit.Api.Docker.Model;
using Pokerit.Api.Docker.Extensions;
using System.Collections.Concurrent;

namespace Pokerit.Api.Docker.Hubs
{
    public class PokerHub : Hub
    {
        private static readonly ConcurrentDictionary<string, GameSession> _sessions = new ConcurrentDictionary<string, GameSession>();

        public async Task Join(string username, string hubId)
        {
            var retrievedSession = GetSession(hubId);
            await Groups.AddToGroupAsync(Context.ConnectionId, retrievedSession.Id);
            AddPlayer(retrievedSession, username);
            _sessions.TryGetValue(retrievedSession.Id, out GameSession? session);
            NotifySessionUpdate(session.Id, session);

        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        private GameSession GetSession(string hubId)
        {
            GameSession? session;
            if (string.IsNullOrEmpty(hubId))
            {
                session = CreateNewSession();
            } 
            else
            {
                _sessions.TryGetValue(hubId, out session);
                if(session == null)
                {
                    session = CreateNewSession();
                }
            }
            return session;
        }

        private GameSession CreateNewSession() {
            GameSession session = new()
            {
                Id = Guid.NewGuid().ToString(),
                Phase = GamePhase.IDLE,
                Players = new List<Player>(),
                CreationTime = DateTime.Now
            };
            _sessions.TryAdd(session.Id, session);
            return session;
        }

        private void AddPlayer(GameSession session, string username)
        {
            var newSession = session.Clone();
            newSession.Players.Add(new()
            {
                Id = Context.ConnectionId,
                Username = username,
            });
            _sessions.TryUpdate(session.Id, newSession, session);
        }

        private async void NotifySessionUpdate(string sessionId, GameSession updatedSession)
        {
            await Clients.Group(sessionId).SendAsync("SessionUpdated", updatedSession);

        }
    }
}
