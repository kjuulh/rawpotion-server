using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RawPotionServer.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly ILogger logger;
        public HealthController(ILogger<HealthController> logger)
        {
            this.logger = logger;
        }

        [Produces("application/json")]
        [HttpGet("ping")]
        public ActionResult Ping()
        {
            return Ok(new { Message = "pong!" });
        }
    }
}