namespace ONGES.Campaign.Application.Handlers;

using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Queries;
using DTOs;

/// <summary>
/// Handler para GetCampaignByIdQuery.
/// Obtém uma campanha pelo ID.
/// </summary>
public sealed class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, CampaignResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCampaignByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<CampaignResponse?> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
    {
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(request.Id, cancellationToken);
        
        if (campaign is null)
        {
            return null;
        }

        return _mapper.Map<CampaignResponse>(campaign);
    }
}

/// <summary>
/// Handler para GetActiveCampaignsQuery.
/// Obtém todas as campanhas ativas para o painel de transparência.
/// </summary>
public sealed class GetActiveCampaignsQueryHandler : IRequestHandler<GetActiveCampaignsQuery, List<TransparencyPanelResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActiveCampaignsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<TransparencyPanelResponse>> Handle(GetActiveCampaignsQuery request, CancellationToken cancellationToken)
    {
        var campaigns = await _unitOfWork.Campaigns.GetAllActiveAsync(cancellationToken);
        return campaigns
            .Select(c => _mapper.Map<TransparencyPanelResponse>(c))
            .ToList();
    }
}

/// <summary>
/// Handler para GetAllCampaignsQuery.
/// Obtém todas as campanhas (apenas para GestorONG).
/// </summary>
public sealed class GetAllCampaignsQueryHandler : IRequestHandler<GetAllCampaignsQuery, List<CampaignResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCampaignsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<CampaignResponse>> Handle(GetAllCampaignsQuery request, CancellationToken cancellationToken)
    {
        var campaigns = await _unitOfWork.Campaigns.GetAllAsync(cancellationToken);
        return campaigns
            .Select(c => _mapper.Map<CampaignResponse>(c))
            .ToList();
    }
}
