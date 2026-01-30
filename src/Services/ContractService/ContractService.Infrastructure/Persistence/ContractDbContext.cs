using ContractService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ContractService.Infrastructure.Persistence;

/// <summary>
/// Database context for Contract service
/// </summary>
public class ContractDbContext : DbContext
{
    public ContractDbContext(DbContextOptions<ContractDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
