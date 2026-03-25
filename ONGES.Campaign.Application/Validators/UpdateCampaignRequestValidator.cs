namespace ONGES.Campaign.Application.Validators;

using FluentValidation;
using DTOs;

/// <summary>
/// Validator for UpdateCampaignRequest.
/// </summary>
public class UpdateCampaignRequestValidator : AbstractValidator<UpdateCampaignRequest>
{
    public UpdateCampaignRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The title is required.")
            .MaximumLength(255).WithMessage("The title cannot exceed 255 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required.")
            .MaximumLength(1000).WithMessage("The description cannot exceed 1000 characters.");

        RuleFor(x => x.FinancialTarget)
            .GreaterThan(0).WithMessage("The financial target must be greater than zero.");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("The start date must be before the end date.");

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("The end date cannot be in the past.");
    }
}
