using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("UploadMeter")]
    public class MetersCsvController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7199/api/metercsv";

        public MetersCsvController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /UploadMeter
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Upload");
        }

        // POST: /UploadMeter/Upload
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadMeterCsv([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file selected." });

            var client = _httpClientFactory.CreateClient();

            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

            var response = await client.PostAsync($"{_apiBaseUrl}/upload", content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { message = responseBody });

            return Content(responseBody, "application/json");
        }
    }
}
