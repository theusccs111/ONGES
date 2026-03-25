namespace ONGES.Campaign.Infrastructure.Messaging;

using System.Text;
using System.Text.Json;
using Domain.Events;

/// <summary>
/// Interface para publicar eventos via mensageria.
/// </summary>
public interface IMessagePublisher
{
    Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default) where T : class;
}

/// <summary>
/// Publicador de eventos para RabbitMQ.
/// Este é um stub para demonstrar a arquitetura.
/// Na prática, você usará RabbitMQ.Client ou MassTransit.
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

            // TODO: Implementar publicação real via RabbitMQ.Client
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
