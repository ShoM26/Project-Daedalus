using Microsoft.AspNetCore.Mvc;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;


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