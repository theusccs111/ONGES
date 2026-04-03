namespace ONGES.Campaign.Infrastructure.Workers;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Application.Interfaces;

public class DonationWorker : BackgroundService
{
    private readonly ILogger<DonationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _connectionString;
    private readonly string _topicName;
    private readonly string _subscriptionName;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public DonationWorker(
        ILogger<DonationWorker> logger,
        IServiceProvider serviceProvider,
        string connectionString,
        string topicName,
        string subscriptionName)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _connectionString = connectionString;
        _topicName = topicName;
        _subscriptionName = subscriptionName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Iniciando DonationWorker...");

            _client = new ServiceBusClient(_connectionString);
            _processor = _client.CreateProcessor(_topicName, _subscriptionName);

            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("DonationWorker iniciado com sucesso. Escutando mensagens no tópico {TopicName}", _topicName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("DonationWorker foi cancelado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no DonationWorker");
            throw;
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            _logger.LogInformation("Mensagem recebida: {MessageBody}", body);

            var donationMessage = JsonSerializer.Deserialize<DonationMessage>(body);

            if (donationMessage == null)
            {
                _logger.LogWarning("Mensagem de doação inválida");
                await args.AbandonMessageAsync();
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var campaignService = scope.ServiceProvider.GetRequiredService<ICampaignService>();

            var result = await campaignService.UpdateAmountRaisedAsync(
                donationMessage.CampaignId,
                donationMessage.Amount);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Falha ao atualizar valor arrecadado da campanha {CampaignId}: {Error}",
                    donationMessage.CampaignId, result);
                await args.AbandonMessageAsync();
                return;
            }

            _logger.LogInformation("Valor arrecadado atualizado com sucesso para a campanha {CampaignId}",
                donationMessage.CampaignId);

            await args.CompleteMessageAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem de doação");
            await args.AbandonMessageAsync();
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Erro no processamento de mensagens do Service Bus");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        if (_client != null)
        {
            await _client.DisposeAsync();
        }

        _logger.LogInformation("DonationWorker parado.");

        await base.StopAsync(cancellationToken);
    }
}

public class DonationMessage
{
    public Guid CampaignId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DonatedAt { get; set; }
}
