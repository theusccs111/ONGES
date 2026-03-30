namespace ONGES.Campaign.Application.DTOs.Responses;

public sealed record CampaignResponse(
    Guid Id,
    string Title,
    string Description,
    decimal FinancialTarget,
    decimal AmountRaised,
    bool GoalAchieved,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    Guid CreatorId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
