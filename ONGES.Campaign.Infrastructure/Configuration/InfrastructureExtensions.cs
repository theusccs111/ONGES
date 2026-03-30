namespace ONGES.Campaign.Infrastructure.Configuration;

using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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

        return services;
    }
}
