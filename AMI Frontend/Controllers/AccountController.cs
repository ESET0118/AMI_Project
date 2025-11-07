using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AMI_Frontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _apiBaseUrl = "https://localhost:7199";
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login() => View();

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> SaveToken([FromBody] JsonElement data)
        {
            if (data.TryGetProperty("token", out var tokenElement))
            {
                HttpContext.Session.SetString("JWTToken", tokenElement.GetString() ?? "");
                return Ok(new { message = "Token saved successfully" });
            }

            return BadRequest(new { message = "Token missing" });
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(new { email, password });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_apiBaseUrl}/api/auth/login", content);

            if (!response.IsSuccessStatusCode)
                return BadRequest("Login failed");

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var token = jsonDoc.RootElement.GetProperty("token").GetString();

            HttpContext.Session.SetString("JWTToken", token ?? "");
            return Ok(new { token });
        }
    }
}
