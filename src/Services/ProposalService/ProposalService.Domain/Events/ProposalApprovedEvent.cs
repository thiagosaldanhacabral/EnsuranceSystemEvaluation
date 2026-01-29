using SharedKernel.Domain;

namespace ProposalService.Domain.Events;

/// <summary>
/// Event raised when a proposal is approved
/// </summary>
public sealed record ProposalApprovedEvent : DomainEvent
{
    public Guid ProposalId { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public decimal InsuranceValue { get; init; }
    public DateTime ApprovedAt { get; init; }
}
