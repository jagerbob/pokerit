using Pokerit.Api.Docker.Model;

namespace Pokerit.Api.Docker.Extensions
{
    public static class GameSessionExtensions
    {
        public static GameSession Clone(this GameSession session) => new()
        {
            Id = session.Id,
            Phase = session.Phase,
            Players = session.Players,
            CreationTime = session.CreationTime,
        };
    }
}
