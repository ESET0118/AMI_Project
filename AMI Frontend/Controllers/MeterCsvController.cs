using Microsoft.AspNetCore.Mvc;

namespace AMI_Frontend.Controllers
{
    [Route("UploadMeter")]
    public class MetersCsvController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7199/api/meters/csv";

        public MetersCsvController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /UploadMeter
        [HttpGet("")]
        public IActionResult Index()
        {
            // Returns the view Upload.cshtml located in /Views/MetersCsv/Upload.cshtml
            return View("Upload");
        }

        // POST: /UploadMeter/Upload
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadMeterCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file selected." });

            var client = _httpClientFactory.CreateClient();

            // Optional: attach JWT token from session if needed
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // Prepare multipart content
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

            // Send request to backend API
            var response = await client.PostAsync($"{_apiBaseUrl}/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = errorMsg });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return Content(responseBody, "application/json");
        }
    }
}
