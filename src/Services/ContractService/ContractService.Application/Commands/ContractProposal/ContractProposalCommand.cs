using ContractService.Application.DTOs;
using MediatR;

namespace ContractService.Application.Commands.ContractProposal;

/// <summary>
/// Command to contract an approved proposal
/// </summary>
public sealed record ContractProposalCommand : IRequest<ContractDto>
{
    public Guid ProposalId { get; init; }
    public int DurationMonths { get; init; } = 12;
}
