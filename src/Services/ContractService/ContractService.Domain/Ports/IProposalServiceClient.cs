namespace ContractService.Domain.Ports;

/// <summary>
/// Interface for communicating with ProposalService
/// </summary>
public interface IProposalServiceClient
{
    Task<ProposalDto?> GetProposalByIdAsync(Guid proposalId, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO for proposal data from external service
/// </summary>
public record ProposalDto
{
    public Guid Id { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerCPF { get; init; } = string.Empty;
    public decimal InsuranceValue { get; init; }
    public string Status { get; init; } = string.Empty;
}
