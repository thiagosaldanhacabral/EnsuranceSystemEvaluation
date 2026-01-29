using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Ports;
using ProposalService.Domain.ValueObjects;

namespace ProposalService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Proposal aggregate
/// </summary>
public class ProposalRepository : IProposalRepository
{
    private readonly ProposalDbContext _context;

    public ProposalRepository(ProposalDbContext context)
    {
        _context = context;
    }

    public async Task<Proposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Proposal?> GetByProposalNumberAsync(string proposalNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProposalNumber == proposalNumber, cancellationToken);
    }

    public async Task<IEnumerable<Proposal>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Proposal>> GetByStatusAsync(
        ProposalStatus status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Proposals.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(ProposalStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .Where(p => p.Status == status)
            .CountAsync(cancellationToken);
    }

    public async Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default)
    {
        await _context.Proposals.AddAsync(proposal, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Proposal proposal, CancellationToken cancellationToken = default)
    {
        _context.Proposals.Update(proposal);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
