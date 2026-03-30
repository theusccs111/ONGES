namespace ONGES.Campaign.Infrastructure.Validators;

using FluentValidation;
using Application.DTOs.Requests;

public class UpdateCampaignRequestValidator : AbstractValidator<UpdateCampaignRequest>
{
    public UpdateCampaignRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(255).WithMessage("O título não pode exceder 255 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(1000).WithMessage("A descrição não pode exceder 1000 caracteres.");

        RuleFor(x => x.FinancialTarget)
            .GreaterThan(0).WithMessage("A meta financeira deve ser maior que zero.");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("A data de início deve ser anterior à data de término.");

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("A data de término não pode ser no passado.");
    }
}
