using MediatR;
using ProposalService.Application.DTOs;

namespace ProposalService.Application.Commands.UpdateProposalStatus;

/// <summary>
/// Command to update proposal status (approve or reject)
/// </summary>
public sealed record UpdateProposalStatusCommand : IRequest<ProposalDto>
{
    public Guid ProposalId { get; init; }
    public string NewStatus { get; init; } = string.Empty; // "Approved" or "Rejected"
    public string? RejectionReason { get; init; }
}
