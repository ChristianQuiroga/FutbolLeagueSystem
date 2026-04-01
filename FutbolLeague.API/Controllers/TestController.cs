using Microsoft.AspNetCore.Mvc;

namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("FutbolLeague API funcionando 🚀");
        }
    }
}