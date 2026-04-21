using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ONGES.Campaign.Infrastructure.Configuration;
using ONGES.Campaign.Infrastructure.Persistence;
using Prometheus;

namespace ONGES.Campaign.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddControllersWithViews();
        builder.Services.AddOpenApi();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ONGES.Campaign.API",
                Version = "v1",
                Description = "API de gestão de campanhas - ONG Esperança Solidária"
            });

            c.CustomSchemaIds(n => n.FullName);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Informe o token JWT obtido na autenticação do ONGES.Users.Api"
            });

            c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    []
                }
            });
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Convert.FromBase64String(builder.Configuration["Jwt:Key"]!))
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SomenteGestor", policy =>
                policy.RequireRole("Gestor"));
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CampaignDbContext>();

                var retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        db.Database.Migrate();
                        break;
                    }
                    catch
                    {
                        retries--;
                        Thread.Sleep(2000);
                    }
                }
            }
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseHttpMetrics();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapMetrics();
        app.MapControllers();

        app.Run();
    }
}
