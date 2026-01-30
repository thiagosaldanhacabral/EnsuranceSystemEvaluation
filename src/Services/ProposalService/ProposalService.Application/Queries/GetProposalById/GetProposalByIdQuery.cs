using MediatR;
using ProposalService.Application.DTOs;

namespace ProposalService.Application.Queries.GetProposalById;

/// <summary>
/// Query to get a proposal by ID
/// </summary>
public sealed record GetProposalByIdQuery : IRequest<ProposalDto?>
{
    public Guid ProposalId { get; init; }
}
