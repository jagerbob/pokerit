namespace Pokerit.Api.Docker.Model
{
    public class Player
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public int Vote { get; set; }
        public bool IsReady { get; set; }
    }
}
