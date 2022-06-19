using Pokerit.Api.Docker.Model;

namespace Pokerit.Api.Docker.Extensions
{
    public static class GameSessionExtensions
    {
        public static GameSession Clone(this GameSession session) => new()
        {
            GameState = new()
            {
                GameId = session.GameState.GameId,
                Phase = session.GameState.Phase,
                Players = session.GameState.Players
            },
            CreationTime = session.CreationTime,
        };
    }
}
