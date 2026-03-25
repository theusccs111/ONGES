namespace ONGES.Campaign.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Fornece funcionalidade comum para IDs e eventos de domínio.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    
    /// <summary>
    /// Lista de eventos de domínio que ocorreram nesta entidade.
    /// </summary>
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
/// Classe base para eventos de domínio.
/// </summary>
public abstract class BaseDomainEvent
{
    public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
}
