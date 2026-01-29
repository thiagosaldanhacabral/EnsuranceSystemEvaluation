using ProposalService.Domain.Entities;
using ProposalService.Domain.ValueObjects;

namespace ProposalService.Domain.Ports;

/// <summary>
/// Repository interface for proposal persistence
/// </summary>
public interface IProposalRepository
{
    Task<Proposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Proposal?> GetByProposalNumberAsync(string proposalNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Proposal>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Proposal>> GetByStatusAsync(ProposalStatus status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(ProposalStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default);
    Task UpdateAsync(Proposal proposal, CancellationToken cancellationToken = default);
}
