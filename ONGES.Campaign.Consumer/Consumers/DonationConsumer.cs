using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ONGES.Campaign.Application.Interfaces;
using ONGES.Campaign.Infrastructure.Configuration;
using System.Text.Json;

namespace ONGES.Campaign.Consumer.Consumers;

public sealed class DonationConsumer(
    ServiceBusClient serviceBusClient,
    IOptions<ServiceBusOptions> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DonationConsumer> logger) : BackgroundService
{
    private ServiceBusProcessor? _processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = serviceBusClient.CreateProcessor(
            options.Value.TopicName,
            options.Value.SubscriptionName,
            new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);

        logger.LogInformation("DonationConsumer iniciado. Escutando mensagens no tópico {TopicName}", options.Value.TopicName);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        logger.LogInformation("DonationConsumer parado.");
        await base.StopAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var payload = args.Message.Body.ToString();
        var message = JsonSerializer.Deserialize<DonationMessage>(payload);

        if (message is null)
        {
            logger.LogWarning("Mensagem de doação inválida recebida.");
            await args.DeadLetterMessageAsync(args.Message, cancellationToken: args.CancellationToken);
            return;
        }

        using var scope = serviceScopeFactory.CreateScope();
        var campaignService = scope.ServiceProvider.GetRequiredService<ICampaignService>();

        var result = await campaignService.UpdateAmountRaisedAsync(
            message.CampaignId,
            message.Amount);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Valor arrecadado atualizado com sucesso. CampaignId={CampaignId} Amount={Amount}",
                message.CampaignId,
                message.Amount);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        logger.LogError(
            "Falha ao atualizar valor arrecadado da campanha. CampaignId={CampaignId} Error={Error}",
            message.CampaignId,
            result);

        await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(
            args.Exception,
            "Erro no consumer de doações. Entity={EntityPath} Source={ErrorSource}",
            args.EntityPath,
            args.ErrorSource);

        return Task.CompletedTask;
    }
}

public class DonationMessage
{
    public Guid CampaignId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DonatedAt { get; set; }
}
