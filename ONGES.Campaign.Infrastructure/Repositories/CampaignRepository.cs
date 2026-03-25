namespace ONGES.Campaign.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Aggregates;
using Domain.Interfaces;
using Domain.ValueObjects;
using Persistence;

/// <summary>
/// Implementação do repositório de campanhas usando Entity Framework Core.
/// </summary>
public sealed class CampaignRepository : ICampaignRepository
{
    private readonly CampaignDbContext _context;

    public CampaignRepository(CampaignDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CampaignAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<CampaignAggregate>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .Where(c => c.Status.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CampaignAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<CampaignAggregate> AddAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default)
    {
        await _context.Campaigns.AddAsync(campaign, cancellationToken);
        return campaign;
    }

    public async Task<CampaignAggregate> UpdateAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default)
    {
        _context.Campaigns.Update(campaign);
        return await Task.FromResult(campaign);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await GetByIdAsync(id, cancellationToken);
        if (campaign is not null)
        {
            _context.Campaigns.Remove(campaign);
        }
    }
}
