namespace ONGES.Campaign.Application.DTOs;

/// <summary>
/// Response DTO for the transparency panel (active campaigns only).
/// </summary>
public record TransparencyPanelResponse(
    Guid Id,
    string Title,
    decimal FinancialTarget,
    decimal AmountRaised);
