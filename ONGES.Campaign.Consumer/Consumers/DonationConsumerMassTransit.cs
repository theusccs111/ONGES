using MassTransit;
using ONGES.Campaign.Application.Interfaces;
using ONGES.Contracts.DTOs;

namespace ONGES.Campaign.Consumer.Consumers;

public sealed class DonationConsumer(
    ICampaignService campaignService,
    ILogger<DonationConsumer> logger) : IConsumer<DonationMessage>
{
    public async Task Consume(ConsumeContext<DonationMessage> context)
    {
        var result = await campaignService.UpdateAmountRaisedAsync(
            context.Message.CampaignId,
            context.Message.Amount,
            context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Valor arrecadado atualizado com sucesso. CampaignId={CampaignId} Amount={Amount}",
                context.Message.CampaignId,
                context.Message.Amount);
            return;
        }

        logger.LogError(
            "Falha ao atualizar valor arrecadado da campanha. CampaignId={CampaignId} Error={Error}",
            context.Message.CampaignId,
            result);

        throw new InvalidOperationException("Falha ao atualizar valor arrecadado da campanha.");
    }
}
