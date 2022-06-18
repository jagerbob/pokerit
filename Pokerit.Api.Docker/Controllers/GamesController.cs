using Microsoft.AspNetCore.Mvc;

namespace Pokerit.Api.Docker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {

        [HttpGet]
        public IActionResult HelloWorld()
        {
            return Ok("Hello world");
        }
    }
}