using ContractService.Application.DTOs;
using ContractService.Domain.Ports;
using MediatR;

namespace ContractService.Application.Queries.ListContracts;

/// <summary>
/// Handler for listing contracts with pagination
/// </summary>
public sealed class ListContractsQueryHandler : IRequestHandler<ListContractsQuery, PagedResult<ContractDto>>
{
    private const int MAX_PAGE_SIZE = 100;
    private readonly IContractRepository _repository;

    public ListContractsQueryHandler(IContractRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ContractDto>> Handle(ListContractsQuery request, CancellationToken cancellationToken)
    {
        // Validate and adjust page size
        var pageSize = Math.Min(request.PageSize, MAX_PAGE_SIZE);
        var pageNumber = Math.Max(request.PageNumber, 1);

        var contracts = await _repository.GetAllAsync(pageNumber, pageSize, cancellationToken);
        var totalCount = await _repository.CountAsync(cancellationToken);

        var items = contracts.Select(c => new ContractDto
        {
            Id = c.Id,
            ProposalId = c.ProposalId,
            ContractNumber = c.ContractNumber,
            CustomerName = c.Customer.Name,
            CustomerCPF = c.Customer.CPF,
            Premium = c.Premium.Amount,
            ContractDate = c.ContractDate,
            StartDate = c.StartDate,
            EndDate = c.EndDate
        });

        return new PagedResult<ContractDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
