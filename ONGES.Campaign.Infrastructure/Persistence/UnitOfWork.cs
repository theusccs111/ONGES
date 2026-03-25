namespace ONGES.Campaign.Infrastructure.Persistence;

using Domain.Interfaces;
using Repositories;

/// <summary>
/// Unit of Work implementation using Entity Framework Core.
/// Manages transactions and coordinates repositories.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CampaignDbContext _context;
    private ICampaignRepository? _campaignRepository;

    public UnitOfWork(CampaignDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ICampaignRepository Campaigns
    {
        get
        {
            _campaignRepository ??= new CampaignRepository(_context);
            return _campaignRepository;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
