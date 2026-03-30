namespace ONGES.Campaign.Application.DTOs.Requests;

public sealed record UpdateCampaignRequest(
    string Title,
    string Description,
    decimal FinancialTarget,
    DateTime StartDate,
    DateTime EndDate);
