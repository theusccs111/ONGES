namespace ONGES.Campaign.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Persistence;
using Messaging;

/// <summary>
/// Extensões para configurar a infrastructura no container de DI.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Registra os serviços de infraestrutura.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        // Configurar DbContext
        services.AddDbContext<CampaignDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(CampaignDbContext).Assembly.FullName)));

        // Registrar Unit of Work e Repositórios
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Registrar Message Publisher
        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
}
