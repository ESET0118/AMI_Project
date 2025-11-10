using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    public class MeterReadingsController : Controller
    {
        // Load meter readings page for a specific meter
        public IActionResult Index(string serialNo)
        {
            if (string.IsNullOrEmpty(serialNo))
                return RedirectToAction("Index", "Meters");

            ViewBag.SerialNo = serialNo; // pass serial number to the view
            return View();
        }
    }
}
