using SharedKernel.Domain;

namespace ProposalService.Domain.Events;

/// <summary>
/// Event raised when a proposal is rejected
/// </summary>
public sealed record ProposalRejectedEvent : DomainEvent
{
    public Guid ProposalId { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public string RejectionReason { get; init; } = string.Empty;
    public DateTime RejectedAt { get; init; }
}
