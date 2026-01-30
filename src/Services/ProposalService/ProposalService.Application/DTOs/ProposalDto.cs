namespace ProposalService.Application.DTOs;

/// <summary>
/// Data transfer object for proposal
/// </summary>
public sealed record ProposalDto
{
    public Guid Id { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerCPF { get; init; } = string.Empty;
    public decimal InsuranceValue { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? RejectionReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
