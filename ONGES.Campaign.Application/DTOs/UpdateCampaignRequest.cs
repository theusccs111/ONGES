namespace ONGES.Campaign.Application.DTOs;

/// <summary>
/// Request DTO for updating a campaign.
/// </summary>
public record UpdateCampaignRequest(
    string Title,
    string Description,
    decimal FinancialTarget,
    DateTime StartDate,
    DateTime EndDate);
