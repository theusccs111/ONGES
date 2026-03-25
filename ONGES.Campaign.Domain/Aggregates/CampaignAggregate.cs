namespace ONGES.Campaign.Domain.Aggregates;

using Entities;
using ValueObjects;
using Events;

/// <summary>
/// Aggregate representing a Campaign.
/// This is the Campaign aggregate root and contains all business logic.
/// </summary>
public sealed class CampaignAggregate : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Money FinancialTarget { get; private set; }
    public decimal AmountRaised { get; private set; }
    public CampaignStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid CreatorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool GoalAchieved => AmountRaised >= FinancialTarget.Amount;

    private CampaignAggregate() { }

    /// <summary>
    /// Factory method to create a new Campaign.
    /// </summary>
    public static CampaignAggregate Create(
        string title,
        string description,
        Money financialTarget,
        DateTime startDate,
        DateTime endDate,
        Guid creatorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("The campaign title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("The campaign description is required.", nameof(description));

        if (endDate <= DateTime.UtcNow)
            throw new ArgumentException("The campaign end date cannot be in the past.", nameof(endDate));

        if (startDate >= endDate)
            throw new ArgumentException("The start date must be before the end date.", nameof(startDate));

        if (creatorId == Guid.Empty)
            throw new ArgumentException("The creator ID is required.", nameof(creatorId));

        var campaign = new CampaignAggregate
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            FinancialTarget = financialTarget,
            AmountRaised = 0,
            Status = CampaignStatus.Active,
            StartDate = startDate,
            EndDate = endDate,
            CreatorId = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        // Raise domain event
        campaign.AddDomainEvent(new CampaignCreatedDomainEvent(
            campaign.Id,
            campaign.Title,
            campaign.Description,
            campaign.FinancialTarget.Amount,
            campaign.StartDate,
            campaign.EndDate,
            campaign.CreatorId));

        return campaign;
    }

    /// <summary>
    /// Updates the campaign data.
    /// Only active campaigns can be edited.
    /// </summary>
    public void Update(string title, string description, Money financialTarget, DateTime startDate, DateTime endDate)
    {
        if (!Status.IsActive)
            throw new InvalidOperationException("Cannot edit a campaign that is not active.");

        if (endDate <= DateTime.UtcNow)
            throw new ArgumentException("The campaign end date cannot be in the past.", nameof(endDate));

        if (startDate >= endDate)
            throw new ArgumentException("The start date must be before the end date.", nameof(startDate));

        Title = title;
        Description = description;
        FinancialTarget = financialTarget;
        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignUpdatedDomainEvent(
            Id,
            Title,
            Description,
            FinancialTarget.Amount,
            StartDate,
            EndDate));
    }

    /// <summary>
    /// Cancels the campaign.
    /// A cancelled campaign cannot receive more donations.
    /// </summary>
    public void Cancel()
    {
        if (!Status.IsActive)
            throw new InvalidOperationException("Only active campaigns can be cancelled.");

        Status = CampaignStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignCancelledDomainEvent(Id));
    }

    /// <summary>
    /// Updates the amount raised in the campaign.
    /// This method is called by the Worker after processing a donation via messaging.
    /// </summary>
    public void UpdateAmountRaised(decimal newAmount)
    {
        if (newAmount < 0)
            throw new ArgumentException("The amount raised cannot be negative.", nameof(newAmount));

        AmountRaised = newAmount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignAmountRaisedUpdatedDomainEvent(
            Id,
            AmountRaised,
            FinancialTarget.Amount));

        // Check if the goal was achieved
        if (GoalAchieved && Status.IsActive)
        {
            Status = CampaignStatus.Completed;

            AddDomainEvent(new CampaignCompletedDomainEvent(
                Id,
                AmountRaised,
                FinancialTarget.Amount));
        }
    }

    /// <summary>
    /// Checks if the campaign can receive donations.
    /// </summary>
    public bool CanReceiveDonations()
    {
        return Status.IsActive && DateTime.UtcNow <= EndDate;
    }
}
