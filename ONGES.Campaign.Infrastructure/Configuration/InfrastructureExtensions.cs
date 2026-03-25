namespace ONGES.Campaign.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Persistence;
using Messaging;

/// <summary>
/// Extensions for configuring infrastructure in the DI container.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Registers infrastructure services.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        // DbContext
        services.AddDbContext<CampaignDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(CampaignDbContext).Assembly.FullName)));

        // Unit of Work & Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Message Publisher
        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
}
