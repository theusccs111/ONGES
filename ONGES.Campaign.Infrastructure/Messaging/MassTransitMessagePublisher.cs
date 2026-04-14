namespace ONGES.Campaign.Infrastructure.Messaging;

using MassTransit;

public sealed class MassTransitMessagePublisher(
    ISendEndpointProvider sendEndpointProvider,
    ILogger<MassTransitMessagePublisher> logger) : IMessagePublisher
{
    public async Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default) where T : class
    {
        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queue}"));
        await endpoint.Send(message, cancellationToken);

        logger.LogInformation("Mensagem publicada na fila '{Queue}' via MassTransit.", queue);
    }
}
