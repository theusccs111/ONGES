namespace ONGES.Campaign.Application.DTOs;

/// <summary>
/// DTO de resposta para dados de campanha.
/// </summary>
public record CreateCampaignRequest(
    string Title,
    string Description,
    decimal FinancialTarget,
    DateTime StartDate,
    DateTime EndDate);

/// <summary>
/// DTO de resposta para atualização de campanha.
/// </summary>
public record UpdateCampaignRequest(
    string Title,
    string Description,
    decimal FinancialTarget,
    DateTime StartDate,
    DateTime EndDate);

/// <summary>
/// DTO de resposta para dados de campanha.
/// </summary>
public record CampaignResponse(
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

/// <summary>
/// DTO para painel de transparência (apenas campanhas ativas).
/// </summary>
public record TransparencyPanelResponse(
    Guid Id,
    string Title,
    decimal FinancialTarget,
    decimal AmountRaised);

/// <summary>
/// DTO de resposta genérica.
/// </summary>
public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default,
    List<string>? Errors = null);
