using MediatR;
using ProposalService.Application.DTOs;
using ProposalService.Domain.Events;
using ProposalService.Domain.Exceptions;
using ProposalService.Domain.Ports;

namespace ProposalService.Application.Commands.UpdateProposalStatus;

/// <summary>
/// Handler for updating proposal status
/// </summary>
public sealed class UpdateProposalStatusCommandHandler : IRequestHandler<UpdateProposalStatusCommand, ProposalDto>
{
    private readonly IProposalRepository _repository;
    private readonly IProposalEventPublisher _eventPublisher;

    public UpdateProposalStatusCommandHandler(
        IProposalRepository repository,
        IProposalEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<ProposalDto> Handle(UpdateProposalStatusCommand request, CancellationToken cancellationToken)
    {
        var proposal = await _repository.GetByIdAsync(request.ProposalId, cancellationToken);

        if (proposal == null)
            throw new ProposalDomainException($"Proposal with ID {request.ProposalId} not found");

        // Apply status change
        if (request.NewStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase))
        {
            proposal.Approve();

            // Publish approved event
            var approvedEvent = new ProposalApprovedEvent
            {
                ProposalId = proposal.Id,
                ProposalNumber = proposal.ProposalNumber,
                InsuranceValue = proposal.InsuranceValue.Amount,
                ApprovedAt = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(approvedEvent, cancellationToken);
        }
        else if (request.NewStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
        {
            proposal.Reject(request.RejectionReason ?? "No reason provided");

            // Publish rejected event
            var rejectedEvent = new ProposalRejectedEvent
            {
                ProposalId = proposal.Id,
                ProposalNumber = proposal.ProposalNumber,
                RejectionReason = proposal.RejectionReason ?? "No reason provided",
                RejectedAt = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(rejectedEvent, cancellationToken);
        }
        else
        {
            throw new ProposalDomainException($"Invalid status: {request.NewStatus}. Must be 'Approved' or 'Rejected'");
        }

        // Update in repository
        await _repository.UpdateAsync(proposal, cancellationToken);

        // Map to DTO
        return new ProposalDto
        {
            Id = proposal.Id,
            ProposalNumber = proposal.ProposalNumber,
            CustomerName = proposal.CustomerName,
            CustomerCPF = proposal.CustomerCPF.Formatted,
            InsuranceValue = proposal.InsuranceValue.Amount,
            Status = proposal.Status.ToString(),
            RejectionReason = proposal.RejectionReason,
            CreatedAt = proposal.CreatedAt,
            UpdatedAt = proposal.UpdatedAt
        };
    }
}
