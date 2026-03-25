namespace ONGES.Campaign.Application.Services;

using AutoMapper;
using FluentValidation;
using Domain.Aggregates;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using DTOs;
using Interfaces;

/// <summary>
/// Service responsible for campaign business operations.
/// </summary>
public class CampaignService : ICampaignService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCampaignRequest> _createValidator;
    private readonly IValidator<UpdateCampaignRequest> _updateValidator;

    public CampaignService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateCampaignRequest> createValidator,
        IValidator<UpdateCampaignRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public Result<CampaignResponse> GetById(Guid id)
    {
        var campaign = _unitOfWork.Campaigns.GetByIdAsync(id).GetAwaiter().GetResult()
            ?? throw new NotFoundException("Campaign", id);

        return new Result<CampaignResponse>(_mapper.Map<CampaignResponse>(campaign));
    }

    public Result<IEnumerable<CampaignResponse>> GetAll()
    {
        var campaigns = _unitOfWork.Campaigns.GetAllAsync().GetAwaiter().GetResult();
        return new Result<IEnumerable<CampaignResponse>>(_mapper.Map<IEnumerable<CampaignResponse>>(campaigns));
    }

    public Result<IEnumerable<TransparencyPanelResponse>> GetAllActive()
    {
        var campaigns = _unitOfWork.Campaigns.GetAllActiveAsync().GetAwaiter().GetResult();
        return new Result<IEnumerable<TransparencyPanelResponse>>(_mapper.Map<IEnumerable<TransparencyPanelResponse>>(campaigns));
    }

    public Result<CampaignResponse> Create(CreateCampaignRequest request, Guid creatorId)
    {
        var validationResult = _createValidator.Validate(request);
        if (!validationResult.IsValid)
            throw new Domain.Exceptions.ValidationException(validationResult.Errors.ToList());

        var financialTarget = new Money(request.FinancialTarget);
        var campaign = CampaignAggregate.Create(
            request.Title,
            request.Description,
            financialTarget,
            request.StartDate,
            request.EndDate,
            creatorId);

        _unitOfWork.Campaigns.AddAsync(campaign).GetAwaiter().GetResult();

        return new Result<CampaignResponse>(_mapper.Map<CampaignResponse>(campaign));
    }

    public Result<CampaignResponse> Update(Guid id, UpdateCampaignRequest request)
    {
        var validationResult = _updateValidator.Validate(request);
        if (!validationResult.IsValid)
            throw new Domain.Exceptions.ValidationException(validationResult.Errors.ToList());

        var campaign = _unitOfWork.Campaigns.GetByIdAsync(id).GetAwaiter().GetResult()
            ?? throw new NotFoundException("Campaign", id);

        var financialTarget = new Money(request.FinancialTarget);
        campaign.Update(
            request.Title,
            request.Description,
            financialTarget,
            request.StartDate,
            request.EndDate);

        _unitOfWork.Campaigns.UpdateAsync(campaign).GetAwaiter().GetResult();

        return new Result<CampaignResponse>(_mapper.Map<CampaignResponse>(campaign));
    }

    public Result<CampaignResponse> Cancel(Guid id)
    {
        var campaign = _unitOfWork.Campaigns.GetByIdAsync(id).GetAwaiter().GetResult()
            ?? throw new NotFoundException("Campaign", id);

        campaign.Cancel();
        _unitOfWork.Campaigns.UpdateAsync(campaign).GetAwaiter().GetResult();

        return new Result<CampaignResponse>(_mapper.Map<CampaignResponse>(campaign));
    }

    public Result<CampaignResponse> UpdateAmountRaised(Guid campaignId, decimal newAmount)
    {
        var campaign = _unitOfWork.Campaigns.GetByIdAsync(campaignId).GetAwaiter().GetResult()
            ?? throw new NotFoundException("Campaign", campaignId);

        campaign.UpdateAmountRaised(newAmount);
        _unitOfWork.Campaigns.UpdateAsync(campaign).GetAwaiter().GetResult();

        return new Result<CampaignResponse>(_mapper.Map<CampaignResponse>(campaign));
    }

    public async Task CompleteAsync()
    {
        await _unitOfWork.SaveChangesAsync();
    }
}
