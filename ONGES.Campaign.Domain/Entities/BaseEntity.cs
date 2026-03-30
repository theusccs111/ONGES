namespace ONGES.Campaign.Domain.Entities;

public abstract class BaseEntity(Guid id) : IEquatable<BaseEntity>
{
    public Guid Id { get; init; } = id;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    private readonly List<BaseDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public void UpdateChangesDate() => UpdatedAt = DateTime.UtcNow;

    public bool Equals(BaseEntity? other)
    {
        if (other is null) return false;
        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((BaseEntity)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();
    public static bool operator ==(BaseEntity? left, BaseEntity? right) => Equals(left, right);
    public static bool operator !=(BaseEntity? left, BaseEntity? right) => !Equals(left, right);
}

public abstract class BaseDomainEvent
{
    public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
}
