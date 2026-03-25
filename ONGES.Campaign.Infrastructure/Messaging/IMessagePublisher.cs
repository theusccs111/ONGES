namespace ONGES.Campaign.Infrastructure.Messaging;

/// <summary>
/// Interface for publishing events via messaging.
/// </summary>
public interface IMessagePublisher
{
    Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default) where T : class;
}
