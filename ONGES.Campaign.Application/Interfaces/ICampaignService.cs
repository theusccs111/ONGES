namespace ONGES.Campaign.Application.Interfaces;

using DTOs.Requests;
using DTOs.Responses;
using Domain.Shared.Results;

public interface ICampaignService
{
    Task<Result<CampaignResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CampaignResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TransparencyPanelResponse>>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<Result<CampaignResponse>> CreateAsync(CreateCampaignRequest request, Guid creatorId, CancellationToken cancellationToken = default);
    Task<Result<CampaignResponse>> UpdateAsync(Guid id, UpdateCampaignRequest request, CancellationToken cancellationToken = default);
    Task<Result<CampaignResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
