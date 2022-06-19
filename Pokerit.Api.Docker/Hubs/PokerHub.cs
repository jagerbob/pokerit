using Microsoft.AspNetCore.SignalR;
using Pokerit.Api.Docker.Model;
using Pokerit.Api.Docker.Extensions;
using System.Collections.Concurrent;

namespace Pokerit.Api.Docker.Hubs
{
    public class PokerHub : Hub
    {
        private readonly ConcurrentDictionary<string, GameSession> _sessions;

        public PokerHub()
        {
            _sessions = new ConcurrentDictionary<string, GameSession>();
        }

        public async Task Join(string username, string hubId)
        {
            var retrievedSession = GetSession(hubId);
            var newSession = retrievedSession.Clone();
            newSession.GameState.Players.Add(new()
            {
                Id = Context.ConnectionId,
                Username = username,
            });
            _sessions.TryUpdate(retrievedSession.GameState.GameId, newSession, retrievedSession);
            _sessions.TryGetValue(retrievedSession.GameState.GameId, out GameSession? session);
            Console.WriteLine(session);
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
                GameState = new()
                {
                    GameId = Guid.NewGuid().ToString(),
                    Phase = GamePhase.IDLE,
                    Players = new List<Player>(),
                },
                CreationTime = DateTime.Now
            };
            _sessions.TryAdd(session.GameState.GameId, session);
            return session;
        }
    }
}
