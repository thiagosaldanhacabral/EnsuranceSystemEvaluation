using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContractService.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for ContractDbContext (used by EF Core tools)
/// </summary>
public class ContractDbContextFactory : IDesignTimeDbContextFactory<ContractDbContext>
{
    public ContractDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractDbContext>();

        // Use a temporary connection string for migrations
        optionsBuilder.UseSqlServer("Server=localhost;Database=ContractDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True");

        return new ContractDbContext(optionsBuilder.Options);
    }
}
