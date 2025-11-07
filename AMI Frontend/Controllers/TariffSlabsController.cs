using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    public class TariffSlabsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7199/api/TariffSlab"; // ✅ Backend URL

        public TariffSlabsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index() => View();

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // ✅ GET all slabs
        [HttpGet("/api/tariffslab")]
        public async Task<IActionResult> GetSlabs()
        {
            var client = CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        // ✅ GET slab by ID
        [HttpGet("/api/tariffslab/{id:int}")]
        public async Task<IActionResult> GetSlabById(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        // ✅ POST new slab
        [HttpPost("/api/tariffslab")]
        public async Task<IActionResult> AddSlab([FromBody] object slab)
        {
            var client = CreateClient();
            var response = await client.PostAsync(_apiBaseUrl,
                new StringContent(slab.ToString(), System.Text.Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        // ✅ PUT (update)
        [HttpPut("/api/tariffslab/{id:int}")]
        public async Task<IActionResult> UpdateSlab(int id, [FromBody] object slab)
        {
            var client = CreateClient();
            var response = await client.PutAsync($"{_apiBaseUrl}/{id}",
                new StringContent(slab.ToString(), System.Text.Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        // ✅ DELETE
        [HttpDelete("/api/tariffslab/{id:int}")]
        public async Task<IActionResult> DeleteSlab(int id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");
            return StatusCode((int)response.StatusCode);
        }
    }
}
