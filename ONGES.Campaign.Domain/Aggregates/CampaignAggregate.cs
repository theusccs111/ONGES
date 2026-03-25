namespace ONGES.Campaign.Domain.Aggregates;

using Entities;
using ValueObjects;
using Events;

/// <summary>
/// Agregado que representa uma Campanha.
/// Esta é a raiz do agregado Campaign e contém toda a lógica de negócio.
/// </summary>
public sealed class CampaignAggregate : BaseEntity
{
    // Propriedades
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

    // Propriedade calculada
    public bool GoalAchieved => AmountRaised >= FinancialTarget.Amount;

    // Construtor privado para ser usado apenas por factory methods
    private CampaignAggregate() { }

    /// <summary>
    /// Factory method para criar uma nova Campanha.
    /// </summary>
    public static CampaignAggregate Create(
        string title,
        string description,
        Money financialTarget,
        DateTime startDate,
        DateTime endDate,
        Guid creatorId)
    {
        // Validações de negócio
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("O título da campanha é obrigatório.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição da campanha é obrigatória.", nameof(description));

        if (endDate <= DateTime.UtcNow)
            throw new ArgumentException("A data de término da campanha não pode ser no passado.", nameof(endDate));

        if (startDate >= endDate)
            throw new ArgumentException("A data de início deve ser anterior à data de término.", nameof(startDate));

        if (creatorId == Guid.Empty)
            throw new ArgumentException("O ID do criador é obrigatório.", nameof(creatorId));

        var campaign = new CampaignAggregate
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            FinancialTarget = financialTarget,
            AmountRaised = 0,
            Status = CampaignStatus.Ativa,
            StartDate = startDate,
            EndDate = endDate,
            CreatorId = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        // Disparar evento de domínio
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
    /// Atualiza os dados da campanha.
    /// Apenas campanha Ativa podem ser editadas.
    /// </summary>
    public void Update(string title, string description, Money financialTarget, DateTime startDate, DateTime endDate)
    {
        if (!Status.IsActive)
            throw new InvalidOperationException("Não é possível editar uma campanha que não está ativa.");

        if (endDate <= DateTime.UtcNow)
            throw new ArgumentException("A data de término da campanha não pode ser no passado.", nameof(endDate));

        if (startDate >= endDate)
            throw new ArgumentException("A data de início deve ser anterior à data de término.", nameof(startDate));

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
    /// Cancela a campanha.
    /// Uma campanha cancelada não pode receber mais doações.
    /// </summary>
    public void Cancel()
    {
        if (!Status.IsActive)
            throw new InvalidOperationException("Apenas campanhas ativas podem ser canceladas.");

        Status = CampaignStatus.Cancelada;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignCancelledDomainEvent(Id));
    }

    /// <summary>
    /// Atualiza o valor arrecadado na campanha.
    /// Este método é chamado pelo Worker após processar uma doação via mensageria.
    /// </summary>
    public void UpdateAmountRaised(decimal newAmount)
    {
        if (newAmount < 0)
            throw new ArgumentException("O valor arrecadado não pode ser negativo.", nameof(newAmount));

        AmountRaised = newAmount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignAmountRaisedUpdatedDomainEvent(
            Id,
            AmountRaised,
            FinancialTarget.Amount));

        // Verificar se a meta foi atingida
        if (GoalAchieved && Status.IsActive)
        {
            Status = CampaignStatus.Concluida;

            AddDomainEvent(new CampaignCompletedDomainEvent(
                Id,
                AmountRaised,
                FinancialTarget.Amount));
        }
    }

    /// <summary>
    /// Verifica se a campanha pode receber doações.
    /// </summary>
    public bool CanReceiveDonations()
    {
        return Status.IsActive && DateTime.UtcNow <= EndDate;
    }
}
