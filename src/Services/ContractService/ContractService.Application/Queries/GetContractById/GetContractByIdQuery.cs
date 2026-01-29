using ContractService.Application.DTOs;
using MediatR;

namespace ContractService.Application.Queries.GetContractById;

/// <summary>
/// Query to get a contract by ID
/// </summary>
public sealed record GetContractByIdQuery : IRequest<ContractDto?>
{
    public Guid ContractId { get; init; }
}
