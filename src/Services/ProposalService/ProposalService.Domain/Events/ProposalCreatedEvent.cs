using SharedKernel.Domain;

namespace ProposalService.Domain.Events;

/// <summary>
/// Event raised when a new proposal is created
/// </summary>
public sealed record ProposalCreatedEvent : DomainEvent
{
    public Guid ProposalId { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerCPF { get; init; } = string.Empty;
    public decimal InsuranceValue { get; init; }
}
