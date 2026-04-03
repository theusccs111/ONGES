namespace ONGES.Campaign.Infrastructure.Services;

using FluentValidation;
using Domain.Aggregates;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.Shared.Results;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Messaging;

public class CampaignService(
    IUnitOfWork unitOfWork,
    IMessagePublisher messagePublisher,
    IValidator<CreateCampaignRequest> createValidator,
    IValidator<UpdateCampaignRequest> updateValidator) : ICampaignService
{
    public async Task<Result<CampaignResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await unitOfWork.Campaigns.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return Result.Failure<CampaignResponse>(new Error("404", "Campanha não encontrada."));

        return Result.Success(MapToResponse(campaign));
    }

    public async Task<Result<IEnumerable<CampaignResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var campaigns = await unitOfWork.Campaigns.GetAllAsync(cancellationToken);
        var responses = campaigns.Select(MapToResponse);
        return Result.Success(responses);
    }

    public async Task<Result<IEnumerable<TransparencyPanelResponse>>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var campaigns = await unitOfWork.Campaigns.GetAllActiveAsync(cancellationToken);
        var responses = campaigns.Select(c => new TransparencyPanelResponse(
            c.Id, c.Title, c.FinancialTarget.Amount, c.AmountRaised));
        return Result.Success(responses);
    }

    public async Task<Result<CampaignResponse>> CreateAsync(CreateCampaignRequest request, Guid creatorId, CancellationToken cancellationToken = default)
    {
        var validation = createValidator.Validate(request);

        if (!validation.IsValid)
            return Result.Failure<CampaignResponse>(new Error("400", string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));

        var financialTarget = new Money(request.FinancialTarget);
        var campaign = CampaignAggregate.Create(
            request.Title, request.Description, financialTarget,
            request.StartDate, request.EndDate, creatorId);

        await unitOfWork.Campaigns.AddAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(campaign, cancellationToken);

        return Result.Success(MapToResponse(campaign));
    }

    public async Task<Result<CampaignResponse>> UpdateAsync(Guid id, UpdateCampaignRequest request, CancellationToken cancellationToken = default)
    {
        var validation = updateValidator.Validate(request);

        if (!validation.IsValid)
            return Result.Failure<CampaignResponse>(new Error("400", string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));

        var campaign = await unitOfWork.Campaigns.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return Result.Failure<CampaignResponse>(new Error("404", "Campanha não encontrada."));

        var financialTarget = new Money(request.FinancialTarget);
        campaign.Update(request.Title, request.Description, financialTarget, request.StartDate, request.EndDate);

        await unitOfWork.Campaigns.UpdateAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(campaign, cancellationToken);

        return Result.Success(MapToResponse(campaign));
    }

    public async Task<Result<CampaignResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await unitOfWork.Campaigns.GetByIdAsync(id, cancellationToken);

        if (campaign is null)
            return Result.Failure<CampaignResponse>(new Error("404", "Campanha não encontrada."));

        campaign.Cancel();

        await unitOfWork.Campaigns.UpdateAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(campaign, cancellationToken);

        return Result.Success(MapToResponse(campaign));
    }

    public async Task<Result<CampaignResponse>> UpdateAmountRaisedAsync(Guid campaignId, decimal amount, CancellationToken cancellationToken = default)
    {
        var campaign = await unitOfWork.Campaigns.GetByIdAsync(campaignId, cancellationToken);

        if (campaign is null)
            return Result.Failure<CampaignResponse>(new Error("404", "Campanha não encontrada."));

        campaign.UpdateAmountRaised(amount);

        await unitOfWork.Campaigns.UpdateAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(campaign, cancellationToken);

        return Result.Success(MapToResponse(campaign));
    }

    private async Task PublishDomainEventsAsync(CampaignAggregate campaign, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in campaign.DomainEvents)
        {
            var queueName = domainEvent.GetType().Name;
            await messagePublisher.PublishAsync(queueName, domainEvent, cancellationToken);
        }
        campaign.ClearDomainEvents();
    }

    private static CampaignResponse MapToResponse(CampaignAggregate campaign) =>
        new(campaign.Id, campaign.Title, campaign.Description,
            campaign.FinancialTarget.Amount, campaign.AmountRaised,
            campaign.GoalAchieved, campaign.Status.Value.ToString(),
            campaign.StartDate, campaign.EndDate, campaign.CreatorId,
            campaign.CreatedAt, campaign.UpdatedAt);
}
