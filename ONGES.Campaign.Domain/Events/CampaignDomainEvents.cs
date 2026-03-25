namespace ONGES.Campaign.Domain.Events;

using Entities;

/// <summary>
/// Event raised when a campaign is created.
/// </summary>
public sealed class CampaignCreatedDomainEvent : BaseDomainEvent
{
    public Guid CampaignId { get; }
    public string Title { get; }
    public string Description { get; }
    public decimal FinancialTarget { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public Guid CreatorId { get; }

    public CampaignCreatedDomainEvent(
        Guid campaignId,
        string title,
        string description,
        decimal financialTarget,
        DateTime startDate,
        DateTime endDate,
        Guid creatorId)
    {
        CampaignId = campaignId;
        Title = title;
        Description = description;
        FinancialTarget = financialTarget;
        StartDate = startDate;
        EndDate = endDate;
        CreatorId = creatorId;
    }
}

/// <summary>
/// Event raised when a campaign is updated.
/// </summary>
public sealed class CampaignUpdatedDomainEvent : BaseDomainEvent
{
    public Guid CampaignId { get; }
    public string Title { get; }
    public string Description { get; }
    public decimal FinancialTarget { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public CampaignUpdatedDomainEvent(
        Guid campaignId,
        string title,
        string description,
        decimal financialTarget,
        DateTime startDate,
        DateTime endDate)
    {
        CampaignId = campaignId;
        Title = title;
        Description = description;
        FinancialTarget = financialTarget;
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// Event raised when a campaign is cancelled.
/// </summary>
public sealed class CampaignCancelledDomainEvent : BaseDomainEvent
{
    public Guid CampaignId { get; }

    public CampaignCancelledDomainEvent(Guid campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// Event raised when the amount raised of a campaign is updated.
/// </summary>
public sealed class CampaignAmountRaisedUpdatedDomainEvent : BaseDomainEvent
{
    public Guid CampaignId { get; }
    public decimal TotalRaised { get; }
    public decimal FinancialTarget { get; }

    public CampaignAmountRaisedUpdatedDomainEvent(Guid campaignId, decimal totalRaised, decimal financialTarget)
    {
        CampaignId = campaignId;
        TotalRaised = totalRaised;
        FinancialTarget = financialTarget;
    }
}

/// <summary>
/// Event raised when a campaign is completed (goal achieved).
/// </summary>
public sealed class CampaignCompletedDomainEvent : BaseDomainEvent
{
    public Guid CampaignId { get; }
    public decimal TotalRaised { get; }
    public decimal FinancialTarget { get; }

    public CampaignCompletedDomainEvent(Guid campaignId, decimal totalRaised, decimal financialTarget)
    {
        CampaignId = campaignId;
        TotalRaised = totalRaised;
        FinancialTarget = financialTarget;
    }
}
