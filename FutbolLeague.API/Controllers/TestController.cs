using Microsoft.AspNetCore.Mvc;

namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [Obsolete("Este endpoint es solo para pruebas y no debe ser utilizado en producción.")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("FutbolLeague API funcionando 🚀");
        }
    }
}