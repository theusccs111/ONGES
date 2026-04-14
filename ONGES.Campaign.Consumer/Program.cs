using MassTransit;
using Microsoft.Extensions.Options;
using ONGES.Campaign.Consumer.Consumers;
using ONGES.Campaign.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(
    builder.Configuration,
    configureMassTransitRegistration: busConfigurator =>
    {
        busConfigurator.AddConsumer<DonationConsumer>();
    },
    configureRabbitMq: (context, cfg) =>
    {
        var options = context.GetRequiredService<IOptions<MessageBrokerOptions>>().Value;

        cfg.ReceiveEndpoint(options.CampaignUpdatesQueue, endpoint =>
        {
            endpoint.ConfigureConsumer<DonationConsumer>(context);
        });
    });

var host = builder.Build();
host.Run();
