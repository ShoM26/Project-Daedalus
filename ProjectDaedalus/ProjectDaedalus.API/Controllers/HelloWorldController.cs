using Microsoft.AspNetCore.Mvc;

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string SayHello()
        {
            return "Hello World!";
        }
    }
}