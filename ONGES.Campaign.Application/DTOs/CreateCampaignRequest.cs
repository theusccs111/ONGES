namespace ONGES.Campaign.Application.DTOs;

/// <summary>
/// Request DTO for creating a campaign.
/// </summary>
public record CreateCampaignRequest(
    string Title,
    string Description,
    decimal FinancialTarget,
    DateTime StartDate,
    DateTime EndDate);
