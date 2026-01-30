using FluentValidation;

namespace ContractService.Application.Commands.ContractProposal;

/// <summary>
/// Validator for ContractProposalCommand
/// </summary>
public sealed class ContractProposalCommandValidator : AbstractValidator<ContractProposalCommand>
{
    public ContractProposalCommandValidator()
    {
        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("Proposal ID is required");

        RuleFor(x => x.DurationMonths)
            .GreaterThan(0).WithMessage("Duration must be at least 1 month")
            .LessThanOrEqualTo(60).WithMessage("Duration cannot exceed 60 months");
    }
}
