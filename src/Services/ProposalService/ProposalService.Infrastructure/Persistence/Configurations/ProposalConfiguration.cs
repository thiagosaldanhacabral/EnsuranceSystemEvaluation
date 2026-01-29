using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalService.Domain.Entities;
using ProposalService.Domain.ValueObjects;
using SharedKernel.Domain;

namespace ProposalService.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Proposal entity
/// </summary>
public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
{
    public void Configure(EntityTypeBuilder<Proposal> builder)
    {
        builder.ToTable("Proposals");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.ProposalNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ProposalNumber)
            .IsUnique();

        builder.Property(p => p.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        // Configure CPF value object
        builder.OwnsOne(p => p.CustomerCPF, cpf =>
        {
            cpf.Property(c => c.Number)
                .IsRequired()
                .HasMaxLength(11)
                .HasColumnName("CustomerCPF");

            cpf.HasIndex(c => c.Number)
                .HasDatabaseName("IX_Proposals_CustomerCPF");
        });

        // Configure Money value object
        builder.OwnsOne(p => p.InsuranceValue, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasColumnName("InsuranceValue");

            money.Property(m => m.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasColumnName("Currency")
                .HasDefaultValue("BRL");
        });

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(p => p.Status);

        builder.Property(p => p.RejectionReason)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);
    }
}
