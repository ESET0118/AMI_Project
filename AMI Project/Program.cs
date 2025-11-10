using AMI.Extensions;
using AMI_Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Database
// ----------------------
builder.Services.ConfigureSqlContext(builder.Configuration);

// ----------------------
// Application Services
// ----------------------
builder.Services.RegisterServices();

// ----------------------
// Controllers
// ----------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 5;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ----------------------
// JWT Authentication
// ----------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

// ----------------------
// Swagger
// ----------------------
builder.Services.ConfigureSwagger();
builder.Services.AddEndpointsApiExplorer();

// ----------------------
// CORS
// ----------------------
builder.Services.ConfigureCors();

// ----------------------
// Build App
// ----------------------
var app = builder.Build();

// ----------------------
// Middleware Order Matters!
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication(); // ✅ Must come BEFORE UseAuthorization
app.UseAuthorization();

// ----------------------
// Swagger UI
// ----------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AMI API v1");
});

// ----------------------
// Map Controllers
// ----------------------
app.MapControllers();

app.Run();
