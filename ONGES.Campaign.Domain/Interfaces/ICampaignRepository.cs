namespace ONGES.Campaign.Domain.Interfaces;

using Aggregates;

/// <summary>
/// Campaign repository interface.
/// Defines the contract for Campaign aggregate persistence.
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
