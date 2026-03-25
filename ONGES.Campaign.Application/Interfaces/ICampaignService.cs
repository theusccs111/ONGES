namespace ONGES.Campaign.Application.Interfaces;

using DTOs;

/// <summary>
/// Service interface for campaign operations.
/// </summary>
public interface ICampaignService
{
    Result<CampaignResponse> GetById(Guid id);
    Result<IEnumerable<CampaignResponse>> GetAll();
    Result<IEnumerable<TransparencyPanelResponse>> GetAllActive();
    Result<CampaignResponse> Create(CreateCampaignRequest request, Guid creatorId);
    Result<CampaignResponse> Update(Guid id, UpdateCampaignRequest request);
    Result<CampaignResponse> Cancel(Guid id);
    Result<CampaignResponse> UpdateAmountRaised(Guid campaignId, decimal newAmount);
    Task CompleteAsync();
}
