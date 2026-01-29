using MediatR;
using ProposalService.Application.DTOs;
using ProposalService.Domain.Ports;

namespace ProposalService.Application.Queries.GetProposalById;

/// <summary>
/// Handler for getting a proposal by ID
/// </summary>
public sealed class GetProposalByIdQueryHandler : IRequestHandler<GetProposalByIdQuery, ProposalDto?>
{
    private readonly IProposalRepository _repository;

    public GetProposalByIdQueryHandler(IProposalRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProposalDto?> Handle(GetProposalByIdQuery request, CancellationToken cancellationToken)
    {
        var proposal = await _repository.GetByIdAsync(request.ProposalId, cancellationToken);

        if (proposal == null)
            return null;

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
