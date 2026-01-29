namespace ContractService.Application.DTOs;

/// <summary>
/// Data transfer object for contract
/// </summary>
public sealed record ContractDto
{
    public Guid Id { get; init; }
    public Guid ProposalId { get; init; }
    public string ContractNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerCPF { get; init; } = string.Empty;
    public decimal Premium { get; init; }
    public DateTime ContractDate { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
