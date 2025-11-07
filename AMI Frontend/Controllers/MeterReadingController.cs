using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    public class MeterReadingsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7199/api/meterreading";

        public MeterReadingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string serialNo)
        {
            if (string.IsNullOrEmpty(serialNo))
                return RedirectToAction("Index", "Meters");

            var client = _httpClientFactory.CreateClient();

            // (Optional) Add token if authentication is enabled
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"{_apiBaseUrl}/meter/{serialNo}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to load readings");

            var json = await response.Content.ReadAsStringAsync();
            ViewBag.SerialNo = serialNo;
            return View("Index", json);
        }
    }
}
