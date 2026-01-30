using ContractService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractService.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Contract entity
/// </summary>
public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.ProposalId)
            .IsRequired();

        builder.Property(c => c.ContractNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ContractDate)
            .IsRequired();

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        // Configure CustomerInfo value object as owned entity
        builder.OwnsOne(c => c.Customer, customer =>
        {
            customer.Property(ci => ci.Name)
                .HasColumnName("CustomerName")
                .IsRequired()
                .HasMaxLength(200);

            customer.Property(ci => ci.CPF)
                .HasColumnName("CustomerCPF")
                .IsRequired()
                .HasMaxLength(11);

            customer.Property(ci => ci.Email)
                .HasColumnName("CustomerEmail")
                .HasMaxLength(100);

            // Index on CustomerCPF for performance
            customer.HasIndex(ci => ci.CPF)
                .HasDatabaseName("IX_Contracts_CustomerCPF");
        });

        // Configure Money value object as owned entity
        builder.OwnsOne(c => c.Premium, premium =>
        {
            premium.Property(m => m.Amount)
                .HasColumnName("Premium")
                .IsRequired()
                .HasPrecision(18, 2);

            premium.Property(m => m.Currency)
                .HasColumnName("PremiumCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("BRL");
        });

        // Indexes for performance
        builder.HasIndex(c => c.ContractNumber)
            .IsUnique()
            .HasDatabaseName("IX_Contracts_ContractNumber");

        builder.HasIndex(c => c.ProposalId)
            .HasDatabaseName("IX_Contracts_ProposalId");
    }
}
