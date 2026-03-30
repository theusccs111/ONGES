namespace ONGES.Campaign.Application.DTOs.Responses;

public sealed record TransparencyPanelResponse(
    Guid Id,
    string Title,
    decimal FinancialTarget,
    decimal AmountRaised);
