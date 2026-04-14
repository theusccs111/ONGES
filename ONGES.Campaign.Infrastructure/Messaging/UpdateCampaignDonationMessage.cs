using MassTransit;

namespace ONGES.Campaign.Infrastructure.Messaging
{
    [EntityName("update-campaign-donation")]
    public class UpdateCampaignDonationMessage
    {
        public Guid CampaignId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonatedAt { get; set; }
    }
}
