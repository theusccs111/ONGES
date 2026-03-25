namespace ONGES.Campaign.API.Configuration;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using Application.Validators;
using Application.Mappers;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Configuration;

/// <summary>
/// Extensions for configuring application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // JWT Configuration
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Controllers
        services.AddControllers();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateCampaignRequestValidator>();

        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CampaignMappingProfile).Assembly));

        // Services
        services.AddScoped<ICampaignService, CampaignService>();

        // Infrastructure
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured");
        services.AddInfrastructureServices(connectionString);

        // Swagger
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "ONGES - Campaign API",
                Version = "v1",
                Description = "Campaign management API for ONG Esperança Solidária",
                Contact = new()
                {
                    Name = "ONGES",
                    Url = new Uri("https://github.com/onges")
                }
            });

            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme"
            });

            options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
                    new List<string>()
                }
            });
        });

        return services;
    }
}
