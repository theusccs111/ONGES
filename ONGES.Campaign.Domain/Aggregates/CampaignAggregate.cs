namespace ONGES.Campaign.Domain.Aggregates;

using Entities;
using ValueObjects;
using Events;

public sealed class CampaignAggregate : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money FinancialTarget { get; private set; } = null!;
    public decimal AmountRaised { get; private set; }
    public CampaignStatus Status { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid CreatorId { get; private set; }

    public bool GoalAchieved => AmountRaised >= FinancialTarget.Amount;

    private CampaignAggregate() : base(Guid.Empty) { }

    private CampaignAggregate(
        Guid id,
        string title,
        string description,
        Money financialTarget,
        DateTime startDate,
        DateTime endDate,
        Guid creatorId) : base(id)
    {
        Title = title;
        Description = description;
        FinancialTarget = financialTarget;
        AmountRaised = 0;
        Status = CampaignStatus.Active;
        StartDate = startDate;
        EndDate = endDate;
        CreatorId = creatorId;
    }

    public static CampaignAggregate Create(
        string title,
        string description,
        Money financialTarget,
        DateTime startDate,
        DateTime endDate,
        Guid creatorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("O título da campanha é obrigatório.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição da campanha é obrigatória.", nameof(description));

        if (endDate <= DateTime.UtcNow)
            throw new ArgumentException("A data de término não pode ser no passado.", nameof(endDate));

        if (startDate >= endDate)
            throw new ArgumentException("A data de início deve ser anterior à data de término.", nameof(startDate));

        if (creatorId == Guid.Empty)
            throw new ArgumentException("O ID do criador é obrigatório.", nameof(creatorId));

        var campaign = new CampaignAggregate(
            Guid.NewGuid(), title, description, financialTarget,
            startDate, endDate, creatorId);

        campaign.AddDomainEvent(new CampaignCreatedDomainEvent(
            campaign.Id, campaign.Title, campaign.Description,
            campaign.FinancialTarget.Amount, campaign.StartDate,
            campaign.EndDate, campaign.CreatorId));

        return campaign;
    }

    public void Update(string title, string description, Money financialTarget, DateTime startDate, DateTime endDate)
    {
        if (!Status.IsActive)
            throw new InvalidOperationException("Não é possível editar uma campanha que não está ativa.");

        if (endDate <= DateTime.UtcNow)
            throw new ArgumentException("A data de término não pode ser no passado.", nameof(endDate));

        if (startDate >= endDate)
            throw new ArgumentException("A data de início deve ser anterior à data de término.", nameof(startDate));

        Title = title;
        Description = description;
        FinancialTarget = financialTarget;
        StartDate = startDate;
        EndDate = endDate;
        UpdateChangesDate();

        AddDomainEvent(new CampaignUpdatedDomainEvent(
            Id, Title, Description, FinancialTarget.Amount, StartDate, EndDate));
    }

    public void Cancel()
    {
        if (!Status.IsActive)
            throw new InvalidOperationException("Somente campanhas ativas podem ser canceladas.");

        Status = CampaignStatus.Cancelled;
        UpdateChangesDate();

        AddDomainEvent(new CampaignCancelledDomainEvent(Id));
    }

    public void UpdateAmountRaised(decimal donationAmount)
    {
        if (donationAmount <= 0)
            throw new ArgumentException("O valor da doação deve ser maior que zero.", nameof(donationAmount));

        AmountRaised += donationAmount;
        UpdateChangesDate();

        AddDomainEvent(new CampaignAmountRaisedUpdatedDomainEvent(
            Id, AmountRaised, FinancialTarget.Amount));

        if (GoalAchieved && Status.IsActive)
        {
            Status = CampaignStatus.Completed;

            AddDomainEvent(new CampaignCompletedDomainEvent(
                Id, AmountRaised, FinancialTarget.Amount));
        }
    }

    public bool CanReceiveDonations()
    {
        return Status.IsActive && DateTime.UtcNow <= EndDate;
    }
}
