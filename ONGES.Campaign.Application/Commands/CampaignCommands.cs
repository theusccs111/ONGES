namespace ONGES.Campaign.Application.Commands;

using MediatR;
using DTOs;

/// <summary>
/// Command para criar uma nova campanha.
/// </summary>
public sealed record CreateCampaignCommand(
    string Title,
    string Description,
    decimal FinancialTarget,
    DateTime StartDate,
    DateTime EndDate,
    Guid CreatorId) : IRequest<CampaignResponse>;

/// <summary>
/// Command para atualizar uma campanha existente.
/// </summary>
public sealed record UpdateCampaignCommand(
    Guid Id,
    string Title,
    string Description,
    decimal FinancialTarget,
    DateTime StartDate,
    DateTime EndDate) : IRequest<CampaignResponse>;

/// <summary>
/// Command para cancelar uma campanha.
/// </summary>
public sealed record CancelCampaignCommand(Guid Id) : IRequest<Unit>;

/// <summary>
/// Command para atualizar o valor arrecadado de uma campanha.
/// Chamado pelo Worker após processar doação via RabbitMQ.
/// </summary>
public sealed record UpdateCampaignAmountRaisedCommand(
    Guid CampaignId,
    decimal NewAmount) : IRequest<Unit>;
