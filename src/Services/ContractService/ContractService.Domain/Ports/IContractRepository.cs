using ContractService.Domain.Entities;

namespace ContractService.Domain.Ports;

/// <summary>
/// Repository interface for contract persistence
/// </summary>
public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Contract?> GetByProposalIdAsync(Guid proposalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contract>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Contract contract, CancellationToken cancellationToken = default);
}
