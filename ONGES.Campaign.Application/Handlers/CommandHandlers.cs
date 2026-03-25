namespace ONGES.Campaign.Application.Handlers;

using AutoMapper;
using Domain.ValueObjects;
using Domain.Interfaces;
using MediatR;
using Commands;
using DTOs;

/// <summary>
/// Handler para CreateCampaignCommand.
/// Cria uma nova campanha no banco de dados.
/// </summary>
public sealed class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, CampaignResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCampaignCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<CampaignResponse> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Criar o agregado com as validações de negócio
            var financialTarget = new Money(request.FinancialTarget);
            var campaign = Domain.Aggregates.CampaignAggregate.Create(
                request.Title,
                request.Description,
                financialTarget,
                request.StartDate,
                request.EndDate,
                request.CreatorId);

            // Adicionar ao repositório
            await _unitOfWork.Campaigns.AddAsync(campaign, cancellationToken);

            // Salvar e publicar eventos
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CampaignResponse>(campaign);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"Erro ao criar campanha: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Handler para UpdateCampaignCommand.
/// Atualiza uma campanha existente.
/// </summary>
public sealed class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, CampaignResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCampaignCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<CampaignResponse> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
        {
            throw new KeyNotFoundException($"Campanha com ID {request.Id} não encontrada.");
        }

        try
        {
            var financialTarget = new Money(request.FinancialTarget);
            campaign.Update(
                request.Title,
                request.Description,
                financialTarget,
                request.StartDate,
                request.EndDate);

            await _unitOfWork.Campaigns.UpdateAsync(campaign, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CampaignResponse>(campaign);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Erro ao atualizar campanha: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Handler para CancelCampaignCommand.
/// Cancela uma campanha.
/// </summary>
public sealed class CancelCampaignCommandHandler : IRequestHandler<CancelCampaignCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelCampaignCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(CancelCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
        {
            throw new KeyNotFoundException($"Campanha com ID {request.Id} não encontrada.");
        }

        campaign.Cancel();
        await _unitOfWork.Campaigns.UpdateAsync(campaign, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

/// <summary>
/// Handler para UpdateCampaignAmountRaisedCommand.
/// Atualiza o valor arrecadado de uma campanha.
/// Chamado pelo Worker ao processar eventos de doação.
/// </summary>
public sealed class UpdateCampaignAmountRaisedCommandHandler : IRequestHandler<UpdateCampaignAmountRaisedCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCampaignAmountRaisedCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(UpdateCampaignAmountRaisedCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(request.CampaignId, cancellationToken);
        if (campaign is null)
        {
            throw new KeyNotFoundException($"Campanha com ID {request.CampaignId} não encontrada.");
        }

        campaign.UpdateAmountRaised(request.NewAmount);
        await _unitOfWork.Campaigns.UpdateAsync(campaign, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
