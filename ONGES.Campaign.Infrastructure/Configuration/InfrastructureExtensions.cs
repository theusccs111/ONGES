namespace ONGES.Campaign.Infrastructure.Configuration;

using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Interfaces;
using FluentValidation;
using MassTransit;
using Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using Services;
using Validators;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureMassTransitRegistration = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureRabbitMq = null)
    {
        services.AddDbContext<CampaignDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();

        services.AddScoped<IValidator<CreateCampaignRequest>, CreateCampaignRequestValidator>();
        services.AddScoped<IValidator<UpdateCampaignRequest>, UpdateCampaignRequestValidator>();

        services.Configure<MessageBrokerOptions>(configuration.GetSection(MessageBrokerOptions.SectionName));

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            configureMassTransitRegistration?.Invoke(busConfigurator);

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<MessageBrokerOptions>>().Value;
                cfg.Host(options.Host, options.Port, options.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(options.Username);
                    hostConfigurator.Password(options.Password);
                });
                cfg.UseRawJsonSerializer();
                cfg.UseRawJsonDeserializer();

                configureRabbitMq?.Invoke(context, cfg);

                if (configureRabbitMq is null)
                    cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

