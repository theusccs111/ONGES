namespace ONGES.Campaign.Infrastructure.Configuration;

using Azure.Messaging.ServiceBus;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Domain.Interfaces;
using Application.DTOs.Requests;
using Application.Interfaces;
using Persistence;
using Messaging;
using Services;
using Validators;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CampaignDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddSingleton<IMessagePublisher, AzureServiceBusPublisher>();

        services.AddScoped<IValidator<CreateCampaignRequest>, CreateCampaignRequestValidator>();
        services.AddScoped<IValidator<UpdateCampaignRequest>, UpdateCampaignRequestValidator>();

        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.SectionName));

        services.AddSingleton(sp =>
        {
            var options = configuration.GetSection(ServiceBusOptions.SectionName).Get<ServiceBusOptions>()!;
            return new ServiceBusClient(options.ConnectionString);
        });

        return services;
    }
}

