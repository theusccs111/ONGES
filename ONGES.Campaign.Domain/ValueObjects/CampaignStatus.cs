namespace ONGES.Campaign.Domain.ValueObjects;

/// <summary>
/// Value Object representing the status of a campaign.
/// </summary>
public class CampaignStatus
{
    public static readonly CampaignStatus Active = new(StatusEnum.Active);
    public static readonly CampaignStatus Completed = new(StatusEnum.Completed);
    public static readonly CampaignStatus Cancelled = new(StatusEnum.Cancelled);

    public enum StatusEnum
    {
        Active = 1,
        Completed = 2,
        Cancelled = 3
    }

    public StatusEnum Value { get; }

    private CampaignStatus(StatusEnum value)
    {
        Value = value;
    }

    public static CampaignStatus FromValue(int value)
    {
        return value switch
        {
            (int)StatusEnum.Active => Active,
            (int)StatusEnum.Completed => Completed,
            (int)StatusEnum.Cancelled => Cancelled,
            _ => throw new ArgumentException($"Status '{value}' is invalid.", nameof(value))
        };
    }

    public static CampaignStatus FromEnum(StatusEnum status)
    {
        return status switch
        {
            StatusEnum.Active => Active,
            StatusEnum.Completed => Completed,
            StatusEnum.Cancelled => Cancelled,
            _ => throw new ArgumentException($"Status '{status}' is invalid.", nameof(status))
        };
    }

    public bool IsActive => Value == StatusEnum.Active;
    public bool IsCompleted => Value == StatusEnum.Completed;
    public bool IsCancelled => Value == StatusEnum.Cancelled;

    public override bool Equals(object? obj)
    {
        return obj is CampaignStatus other && other.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
