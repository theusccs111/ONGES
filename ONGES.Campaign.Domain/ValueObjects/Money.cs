namespace ONGES.Campaign.Domain.ValueObjects;

/// <summary>
/// Value Object representing monetary values.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("The financial target must be greater than zero.", nameof(amount));
        }

        Amount = amount;
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Amount.GetHashCode();
    }

    public override string ToString()
    {
        return $"R$ {Amount:F2}";
    }

    public static bool operator ==(Money left, Money right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Money left, Money right)
    {
        return !(left == right);
    }

    public static Money operator +(Money left, Money right)
    {
        return new Money(left.Amount + right.Amount);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Amount < right.Amount)
            throw new InvalidOperationException("Cannot subtract a value greater than the available amount.");
        
        return new Money(left.Amount - right.Amount);
    }
}
