namespace ONGES.Campaign.Infrastructure.Messaging;

using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

public sealed class AzureServiceBusPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ILogger<AzureServiceBusPublisher> _logger;

    public AzureServiceBusPublisher(IConfiguration configuration, ILogger<AzureServiceBusPublisher> logger)
    {
        _logger = logger;
        var connectionString = configuration["AzureServiceBus:ConnectionString"]
            ?? throw new InvalidOperationException("AzureServiceBus:ConnectionString não configurada.");
        _client = new ServiceBusClient(connectionString);
    }

    public async Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default) where T : class
    {
        await using var sender = _client.CreateSender(queue);

        var json = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(json)
        {
            ContentType = "application/json"
        };

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        _logger.LogInformation("Mensagem publicada na fila '{Queue}': {Message}", queue, json);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
    }
}
