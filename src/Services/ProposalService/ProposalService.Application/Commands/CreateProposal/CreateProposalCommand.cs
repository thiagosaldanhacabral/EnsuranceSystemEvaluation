using MediatR;
using ProposalService.Application.DTOs;

namespace ProposalService.Application.Commands.CreateProposal;

/// <summary>
/// Command to create a new proposal
/// </summary>
public sealed record CreateProposalCommand : IRequest<ProposalDto>
{
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerCPF { get; init; } = string.Empty;
    public decimal InsuranceValue { get; init; }
}
