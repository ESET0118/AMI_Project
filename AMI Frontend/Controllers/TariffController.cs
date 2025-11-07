//using Microsoft.AspNetCore.Mvc;

//namespace AMI_Frontend.Controllers
//{
//    public class TariffController : Controller
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly string _apiUrl = "https://localhost:7199/api/Tariff";

//        public TariffController(IHttpClientFactory httpClientFactory)
//        {
//            _httpClientFactory = httpClientFactory;
//        }

//        public IActionResult Index() => View();

//        [HttpGet("/Tariff/GetAll")]
//        public async Task<IActionResult> GetAll()
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync(_apiUrl);

//            if (!response.IsSuccessStatusCode)
//                return StatusCode((int)response.StatusCode, "Error loading tariffs");

//            var content = await response.Content.ReadAsStringAsync();
//            return Content(content, "application/json");
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    public class TariffController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl = "https://localhost:7199/api/tariffs"; // ✅ fixed: plural + lowercase

        public TariffController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index() => View();

        [HttpGet("/Tariff/GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error loading tariffs");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}

