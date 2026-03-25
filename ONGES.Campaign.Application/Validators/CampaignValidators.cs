namespace ONGES.Campaign.Application.Validators;

using FluentValidation;
using Commands;

/// <summary>
/// Validador para CreateCampaignCommand.
/// </summary>
public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
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

        RuleFor(x => x.CreatorId)
            .NotEqual(Guid.Empty).WithMessage("O ID do criador é obrigatório.");
    }
}

/// <summary>
/// Validador para UpdateCampaignCommand.
/// </summary>
public class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("O ID é obrigatório.");

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

/// <summary>
/// Validador para CancelCampaignCommand.
/// </summary>
public class CancelCampaignCommandValidator : AbstractValidator<CancelCampaignCommand>
{
    public CancelCampaignCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("O ID é obrigatório.");
    }
}

/// <summary>
/// Validador para UpdateCampaignAmountRaisedCommand.
/// </summary>
public class UpdateCampaignAmountRaisedCommandValidator : AbstractValidator<UpdateCampaignAmountRaisedCommand>
{
    public UpdateCampaignAmountRaisedCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEqual(Guid.Empty).WithMessage("O ID da campanha é obrigatório.");

        RuleFor(x => x.NewAmount)
            .GreaterThanOrEqualTo(0).WithMessage("O valor não pode ser negativo.");
    }
}
