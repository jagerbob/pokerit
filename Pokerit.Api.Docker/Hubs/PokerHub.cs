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

        public async Task Leave(string hubId)
        {
            var retrievedSession = GetSession(hubId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, retrievedSession.Id);
            RemovePlayer(retrievedSession, Context.ConnectionId);
            _sessions.TryGetValue(retrievedSession.Id, out GameSession? session);
            NotifySessionUpdate(session.Id, session);
        }

        public async Task SetVote(string hubId, int vote)
        {
            var retrievedSession = GetSession(hubId);
            SetVote(retrievedSession, Context.ConnectionId, vote);
            _sessions.TryGetValue(retrievedSession.Id, out GameSession? session);
            NotifySessionUpdate(session.Id, session);
        }

        public async Task SetUsername(string hubId, string username)
        {
            var retrievedSession = GetSession(hubId);
            SetUsername(retrievedSession, Context.ConnectionId, username);
            _sessions.TryGetValue(retrievedSession.Id, out GameSession? session);
            NotifySessionUpdate(session.Id, session);
        }

        public async Task StartVote(string hubId)
        {
            var retrievedSession = GetSession(hubId);
            SetPhase(retrievedSession, GamePhases.VOTING);
            _sessions.TryGetValue(retrievedSession.Id, out GameSession? session);
            NotifySessionUpdate(session.Id, session);
        }

        public async Task StopVote(string hubId)
        {
            var retrievedSession = GetSession(hubId);
            SetPhase(retrievedSession, GamePhases.SHOWING);
            _sessions.TryGetValue(retrievedSession.Id, out GameSession? session);
            NotifySessionUpdate(session.Id, session);
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
                Phase = GamePhases.IDLE,
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

        private void RemovePlayer(GameSession session, string connectionId)
        {
            var newSession = session.Clone();
            var player = newSession.Players.RemoveAll((p) => p.Id == connectionId);
            _sessions.TryUpdate(session.Id, newSession, session);
        }

        private void SetPhase(GameSession session, string phase)
        {
            var newSession = session.Clone();
            newSession.Phase = phase;
            if(phase == GamePhases.VOTING)
            {
                newSession.Players.ForEach((p) => {
                    p.IsReady = false;
                    p.Vote = -1;
                });
            }
            _sessions.TryUpdate(session.Id, newSession, session);
        }

        private void SetVote(GameSession session, string connectionId, int vote)
        {
            var newSession = session.Clone();
            var player = newSession.Players.First((p) => p.Id == connectionId);
            player.Vote = vote;
            player.IsReady = true;
            _sessions.TryUpdate(session.Id, newSession, session);
        }

        private void SetUsername(GameSession session, string connectionId, string username)
        {
            var newSession = session.Clone();
            var player = newSession.Players.First((p) => p.Id == connectionId);
            player.Username = username;
            _sessions.TryUpdate(session.Id, newSession, session);
        }

        private async void NotifySessionUpdate(string sessionId, GameSession updatedSession)
        {
            await Clients.Group(sessionId).SendAsync("SessionUpdated", updatedSession);
        }
    }
}
