namespace ONGES.Campaign.Application.Queries;

using MediatR;
using DTOs;

/// <summary>
/// Query para obter uma campanha por ID.
/// </summary>
public sealed record GetCampaignByIdQuery(Guid Id) : IRequest<CampaignResponse?>;

/// <summary>
/// Query para obter todas as campanhas ativas.
/// Usada para o painel de transparência.
/// </summary>
public sealed record GetActiveCampaignsQuery : IRequest<List<TransparencyPanelResponse>>;

/// <summary>
/// Query para obter todas as campanhas.
/// Acesso apenas para usuários com role GestorONG.
/// </summary>
public sealed record GetAllCampaignsQuery : IRequest<List<CampaignResponse>>;
