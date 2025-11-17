using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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


            // GET: /Account/ConsumerLogin
            [HttpGet]
            public IActionResult ConsumerLogin()
            {
                return View(); // will return Views/Account/ConsumerLogin.cshtml
            }

        // LoginUser receives { email, password } JSON and proxies to API, returns token + roles[]
        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto login)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(new { email = login.Email, password = login.Password });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_apiBaseUrl}/api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return BadRequest(new { message = "Login failed: " + errorMsg });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            // token must exist
            string token = "";
            if (root.TryGetProperty("token", out var tokenProp) && tokenProp.ValueKind == JsonValueKind.String)
                token = tokenProp.GetString() ?? "";

            // try to extract roles from response.user.Roles (handles strings or objects)
            var roles = new List<string>();
            if (root.TryGetProperty("user", out var userProp))
            {
                if (userProp.TryGetProperty("Roles", out var rolesProp) && rolesProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var r in rolesProp.EnumerateArray())
                    {
                        if (r.ValueKind == JsonValueKind.String)
                        {
                            var s = r.GetString();
                            if (!string.IsNullOrWhiteSpace(s)) roles.Add(s);
                        }
                        else if (r.ValueKind == JsonValueKind.Object)
                        {
                            // common shape: { "Name": "Admin" } or { "name": "Admin" }
                            if (r.TryGetProperty("Name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String)
                            {
                                var s = nameProp.GetString();
                                if (!string.IsNullOrWhiteSpace(s)) roles.Add(s);
                            }
                            else if (r.TryGetProperty("name", out var nameLowerProp) && nameLowerProp.ValueKind == JsonValueKind.String)
                            {
                                var s = nameLowerProp.GetString();
                                if (!string.IsNullOrWhiteSpace(s)) roles.Add(s);
                            }
                        }
                    }
                }
            }

            // If no roles found in user object, decode JWT and extract role claims
            if (!roles.Any() && !string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);

                    // Role claim types may be: ClaimTypes.Role, "role", "roles", or "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    var claimRoles = jwt.Claims
                        .Where(c => string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(c.Type, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.OrdinalIgnoreCase)
                                 || c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase) // fallback
                                 ).Select(c => c.Value)
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .ToList();

                    if (claimRoles.Any()) roles.AddRange(claimRoles);
                }
                catch
                {
                    // ignore decode errors and continue; roles remains empty
                }
            }

            // Normalize roles (trim + as-is)
            roles = roles.Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            // Save JWT (and roles) into session so server-side pages can use it
            HttpContext.Session.SetString("JWTToken", token ?? "");
            HttpContext.Session.SetString("UserRoles", string.Join(",", roles));

            return Ok(new
            {
                token,
                roles
            });
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
