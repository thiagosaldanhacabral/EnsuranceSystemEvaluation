using MediatR;
using ProposalService.Application.DTOs;
using ProposalService.Domain.Ports;
using ProposalService.Domain.ValueObjects;

namespace ProposalService.Application.Queries.ListProposals;

/// <summary>
/// Handler for listing proposals with pagination
/// </summary>
public sealed class ListProposalsQueryHandler : IRequestHandler<ListProposalsQuery, PagedResult<ProposalDto>>
{
    private const int MAX_PAGE_SIZE = 100;
    private readonly IProposalRepository _repository;

    public ListProposalsQueryHandler(IProposalRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProposalDto>> Handle(ListProposalsQuery request, CancellationToken cancellationToken)
    {
        // Validate and adjust page size
        var pageSize = Math.Min(request.PageSize, MAX_PAGE_SIZE);
        var pageNumber = Math.Max(request.PageNumber, 1);

        IEnumerable<ProposalService.Domain.Entities.Proposal> proposals;
        int totalCount;

        // Filter by status if provided
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ProposalStatus>(request.Status, true, out var status))
        {
            proposals = await _repository.GetByStatusAsync(status, pageNumber, pageSize, cancellationToken);
            totalCount = await _repository.CountByStatusAsync(status, cancellationToken);
        }
        else
        {
            proposals = await _repository.GetAllAsync(pageNumber, pageSize, cancellationToken);
            totalCount = await _repository.CountAsync(cancellationToken);
        }

        var items = proposals.Select(p => new ProposalDto
        {
            Id = p.Id,
            ProposalNumber = p.ProposalNumber,
            CustomerName = p.CustomerName,
            CustomerCPF = p.CustomerCPF.Formatted,
            InsuranceValue = p.InsuranceValue.Amount,
            Status = p.Status.ToString(),
            RejectionReason = p.RejectionReason,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });

        return new PagedResult<ProposalDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
