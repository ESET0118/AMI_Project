using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")) &&
                string.IsNullOrEmpty(Request.Cookies["jwtToken"]))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
