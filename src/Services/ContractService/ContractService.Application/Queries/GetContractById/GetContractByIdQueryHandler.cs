using ContractService.Application.DTOs;
using ContractService.Domain.Ports;
using MediatR;

namespace ContractService.Application.Queries.GetContractById;

/// <summary>
/// Handler for getting a contract by ID
/// </summary>
public sealed class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ContractDto?>
{
    private readonly IContractRepository _repository;

    public GetContractByIdQueryHandler(IContractRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContractDto?> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await _repository.GetByIdAsync(request.ContractId, cancellationToken);

        if (contract == null)
            return null;

        return new ContractDto
        {
            Id = contract.Id,
            ProposalId = contract.ProposalId,
            ContractNumber = contract.ContractNumber,
            CustomerName = contract.Customer.Name,
            CustomerCPF = contract.Customer.CPF,
            Premium = contract.Premium.Amount,
            ContractDate = contract.ContractDate,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate
        };
    }
}
