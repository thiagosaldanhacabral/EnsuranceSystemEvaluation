using ContractService.Domain.Entities;
using ContractService.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Contract aggregate
/// </summary>
public class ContractRepository : IContractRepository
{
    private readonly ContractDbContext _context;

    public ContractRepository(ContractDbContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Contract?> GetByProposalIdAsync(Guid proposalId, CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ProposalId == proposalId, cancellationToken);
    }

    public async Task<IEnumerable<Contract>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .AsNoTracking()
            .OrderByDescending(c => c.ContractDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .CountAsync(cancellationToken);
    }

    public async Task AddAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        await _context.Contracts.AddAsync(contract, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByProposalIdAsync(Guid proposalId, CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .AsNoTracking()
            .AnyAsync(c => c.ProposalId == proposalId, cancellationToken);
    }
}
