using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    public class MetersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7199/api/meters";

        public MetersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index() => View();

        [HttpGet("/api/meters")]
        public async Task<IActionResult> GetMeters()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_apiBaseUrl);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch meters");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpPost("/api/meters/upload")]
        public async Task<IActionResult> UploadMeter(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent
            {
                { new StreamContent(file.OpenReadStream()), "file", file.FileName }
            };

            var response = await client.PostAsync($"{_apiBaseUrl}/upload", content);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Upload failed");

            return Ok("File uploaded successfully");
        }

        [HttpGet("/api/meters/{id}/readings")]
        public async Task<IActionResult> GetMeterReadings(string id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_apiBaseUrl}/{id}/readings");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch meter readings");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

    }
}
