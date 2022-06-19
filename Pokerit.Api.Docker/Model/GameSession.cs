namespace Pokerit.Api.Docker.Model
{
    public class GameSession
    {
        public string Id { get; set; }
        public GamePhase Phase { get; set; }
        public List<Player> Players { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
