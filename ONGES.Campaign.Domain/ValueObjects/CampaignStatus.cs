namespace ONGES.Campaign.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o status de uma campanha.
/// </summary>
public class CampaignStatus
{
    public static readonly CampaignStatus Ativa = new(StatusEnum.Ativa);
    public static readonly CampaignStatus Concluida = new(StatusEnum.Concluida);
    public static readonly CampaignStatus Cancelada = new(StatusEnum.Cancelada);

    public enum StatusEnum
    {
        Ativa = 1,
        Concluida = 2,
        Cancelada = 3
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
            (int)StatusEnum.Ativa => Ativa,
            (int)StatusEnum.Concluida => Concluida,
            (int)StatusEnum.Cancelada => Cancelada,
            _ => throw new ArgumentException($"Status '{value}' é inválido.", nameof(value))
        };
    }

    public static CampaignStatus FromEnum(StatusEnum status)
    {
        return status switch
        {
            StatusEnum.Ativa => Ativa,
            StatusEnum.Concluida => Concluida,
            StatusEnum.Cancelada => Cancelada,
            _ => throw new ArgumentException($"Status '{status}' é inválido.", nameof(status))
        };
    }

    public bool IsActive => Value == StatusEnum.Ativa;
    public bool IsCompleted => Value == StatusEnum.Concluida;
    public bool IsCancelled => Value == StatusEnum.Cancelada;

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
