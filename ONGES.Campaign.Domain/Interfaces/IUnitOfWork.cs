namespace ONGES.Campaign.Domain.Interfaces;

/// <summary>
/// Interface for managing transactions and data persistence.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    ICampaignRepository Campaigns { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
