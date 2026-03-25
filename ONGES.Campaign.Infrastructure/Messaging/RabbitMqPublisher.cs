namespace ONGES.Campaign.Infrastructure.Messaging;

using System.Text;
using System.Text.Json;

/// <summary>
/// RabbitMQ event publisher.
/// This is a stub to demonstrate the architecture.
/// In production, use RabbitMQ.Client or MassTransit.
/// </summary>
public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // TODO: Implement real publishing via RabbitMQ.Client
            _logger.LogInformation("Published message to queue '{Queue}': {Message}", queue, json);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to queue '{Queue}'", queue);
            throw;
        }
    }
}
