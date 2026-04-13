using ONGES.Campaign.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<ONGES.Campaign.Consumer.Consumers.DonationConsumer>();

var host = builder.Build();
host.Run();
