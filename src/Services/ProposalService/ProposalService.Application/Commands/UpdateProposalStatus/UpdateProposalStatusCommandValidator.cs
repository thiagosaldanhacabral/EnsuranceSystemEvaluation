using FluentValidation;

namespace ProposalService.Application.Commands.UpdateProposalStatus;

/// <summary>
/// Validator for UpdateProposalStatusCommand
/// </summary>
public sealed class UpdateProposalStatusCommandValidator : AbstractValidator<UpdateProposalStatusCommand>
{
    public UpdateProposalStatusCommandValidator()
    {
        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("Proposal ID is required");

        RuleFor(x => x.NewStatus)
            .NotEmpty().WithMessage("New status is required")
            .Must(BeValidStatus).WithMessage("Status must be either 'Approved' or 'Rejected'");

        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .When(x => x.NewStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Rejection reason is required when rejecting a proposal");
    }

    private static bool BeValidStatus(string status)
    {
        return status.Equals("Approved", StringComparison.OrdinalIgnoreCase) ||
               status.Equals("Rejected", StringComparison.OrdinalIgnoreCase);
    }
}
