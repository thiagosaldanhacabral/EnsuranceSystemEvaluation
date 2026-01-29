using SharedKernel.Domain;

namespace ContractService.Domain.Events;

/// <summary>
/// Event raised when a new contract is created
/// </summary>
public sealed record ContractCreatedEvent : DomainEvent
{
    public Guid ContractId { get; init; }
    public Guid ProposalId { get; init; }
    public string ContractNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public decimal Premium { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
