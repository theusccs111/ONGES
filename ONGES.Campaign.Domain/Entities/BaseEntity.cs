namespace ONGES.Campaign.Domain.Entities;

/// <summary>
/// Base class for all domain entities.
/// Provides common functionality for IDs and domain events.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    private readonly List<BaseDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract class BaseDomainEvent
{
    public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
}
