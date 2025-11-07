using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        // GET: api/Dashboard
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("GET request to DashboardController");
        }

        // POST: api/Dashboard
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("POST request to DashboardController");
        }
    }
}
