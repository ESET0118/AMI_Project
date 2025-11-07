using AMI.Extensions;
using AMI_Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// 🧱 1️⃣ Configure Services
// --------------------------------------------------
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.RegisterServices();
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Prevent circular references in Swagger JSON
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 3;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(AMI_Project.Mappings.MappingProfile));

// CORS (Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://localhost:7264")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// --------------------------------------------------
// 🚀 2️⃣ Build Application
// --------------------------------------------------
var app = builder.Build();

// --------------------------------------------------
// 🧩 3️⃣ Middleware Pipeline
// --------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // Developer exception page for detailed errors
    app.UseDeveloperExceptionPage();

    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AMI API v1");
        c.RoutePrefix = "swagger"; // Swagger UI served at /swagger
    });

    // Auto-migrate DB (optional)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AMIDbContext>();
        await db.Database.MigrateAsync();
    }
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

//Optional: enable global exception middleware if needed
 app.UseMiddleware<AMI.Middleware.ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
