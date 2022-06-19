namespace Pokerit.Api.Docker.Model
{
    public class GameState
    {
        public string GameId { get; set; }
        public GamePhase Phase { get; set; }
        public List<Player> Players { get; set; }
    }
}
