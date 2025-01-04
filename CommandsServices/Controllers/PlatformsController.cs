using Microsoft.AspNetCore.Mvc;

namespace CommandsServices.Controllers
{
    [Route("api/commands/[controller]")]
    [ApiController]
    public class PlatformsController : Controller
    {
        public PlatformsController()
        {
            
        }
        
        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            return Ok("The platform controller is working from Commands");
        }

    }
}
