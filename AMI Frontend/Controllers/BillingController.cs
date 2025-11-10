using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    public class BillingController : Controller
    {
        // Serve the Razor view
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
