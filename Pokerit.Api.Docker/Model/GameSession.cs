namespace Pokerit.Api.Docker.Model
{
    public class GameSession
    {
        public string Id { get; set; }
        public string Phase { get; set; }
        public List<Player> Players { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
