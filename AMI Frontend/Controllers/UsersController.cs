using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AMI_Frontend.Controllers
{
    [Route("Users")]
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7199/api/users";

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index() => View();

        [HttpGet("User")]
        public IActionResult UserRedirect() => RedirectToAction("Index");

        // ✅ New: Profile page for a user
        [HttpGet("Profile/{id}")]
        public IActionResult Profile(long id)
        {
            // Just return the Razor view; AJAX will fetch data
            return View();
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] object dto)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "JWT token missing" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}/{id}", dto);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "JWT token missing" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
