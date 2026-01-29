using MediatR;
using ProposalService.Application.DTOs;

namespace ProposalService.Application.Queries.ListProposals;

/// <summary>
/// Query to list proposals with pagination
/// </summary>
public sealed record ListProposalsQuery : IRequest<PagedResult<ProposalDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
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
