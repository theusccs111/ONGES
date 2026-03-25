namespace ONGES.Campaign.Domain.Events;

using Entities;

/// <summary>
/// Evento disparado quando uma campanha é criada.
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
/// Evento disparado quando uma campanha é atualizada.
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
/// Evento disparado quando uma campanha é cancelada.
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
/// Evento disparado quando o valor arrecadado de uma campanha é atualizado.
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
/// Evento disparado quando uma campanha é concluída (meta atingida).
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
