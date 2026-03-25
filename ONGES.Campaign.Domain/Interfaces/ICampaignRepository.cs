namespace ONGES.Campaign.Domain.Interfaces;

using Aggregates;

/// <summary>
/// Interface do repositório de campanhas.
/// Define contrato para persistência de agregados Campaign.
/// </summary>
public interface ICampaignRepository
{
    Task<CampaignAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<CampaignAggregate>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    
    Task<List<CampaignAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<CampaignAggregate> AddAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default);
    
    Task<CampaignAggregate> UpdateAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
