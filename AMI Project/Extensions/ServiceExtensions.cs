using AMI_Project.Data;
using AMI_Project.Repositories;
using AMI_Project.Repositories.Interfaces;
using AMI_Project.Services;
using AMI_Project.Services.Implementations;
using AMI_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AMI.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder
                        .WithOrigins("https://localhost:7264")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("❌ Missing DB connection string 'DefaultConnection'");

            services.AddDbContext<AMIDbContext>(options =>
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.EnableRetryOnFailure(5);
                    sql.CommandTimeout(60);
                }));
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AMI API",
                    Version = "v1",
                    Description = "Advanced Metering Infrastructure API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token as: Bearer {your token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IConsumerRepository, ConsumerRepository>();
            services.AddScoped<IMeterRepository, MeterRepository>();
            services.AddScoped<IOrgUnitRepository, OrgUnitRepository>();
            services.AddScoped<ITariffRepository, TariffRepository>();
            services.AddScoped<ITariffSlabRepository, TariffSlabRepository>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Services
            services.AddScoped<IConsumerService, ConsumerService>();
            services.AddScoped<IMeterService, MeterService>();
            services.AddScoped<IOrgUnitService, OrgUnitService>();
            services.AddScoped<ITariffService, TariffService>();
            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
