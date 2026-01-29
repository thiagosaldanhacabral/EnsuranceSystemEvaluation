using ContractService.Application.DTOs;
using MediatR;

namespace ContractService.Application.Queries.ListContracts;

/// <summary>
/// Query to list contracts with pagination
/// </summary>
public sealed record ListContractsQuery : IRequest<PagedResult<ContractDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Paged result wrapper
/// </summary>
public sealed record PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
