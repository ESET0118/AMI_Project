using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    public class TODRulesController : Controller
    {
        public IActionResult Index()
        {
            // Just render the view; no API calls.
            return View();
        }
    }
}
