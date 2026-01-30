using FluentAssertions;
using ProposalService.Domain.Entities;
using ProposalService.Domain.ValueObjects;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace ProposalService.UnitTests.Domain.Entities;

public class ProposalTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProposal()
    {
        // Arrange
        var proposalNumber = "PROP-20260129-00001";
        var customerName = "John Doe";
        var cpf = CPF.Create("11144477735");
        var insuranceValue = Money.Create(50000m);

        // Act
        var proposal = Proposal.Create(proposalNumber, customerName, cpf, insuranceValue);

        // Assert
        proposal.Should().NotBeNull();
        proposal.ProposalNumber.Should().Be(proposalNumber);
        proposal.CustomerName.Should().Be(customerName);
        proposal.CustomerCPF.Should().Be(cpf);
        proposal.InsuranceValue.Should().Be(insuranceValue);
        proposal.Status.Should().Be(ProposalStatus.InAnalysis);
        proposal.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        proposal.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyProposalNumber_ShouldThrowDomainException()
    {
        // Arrange
        var cpf = CPF.Create("11144477735");
        var insuranceValue = Money.Create(50000m);

        // Act
        var act = () => Proposal.Create("", "John Doe", cpf, insuranceValue);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Proposal number is required");
    }

    [Fact]
    public void Create_WithEmptyCustomerName_ShouldThrowDomainException()
    {
        // Arrange
        var cpf = CPF.Create("11144477735");
        var insuranceValue = Money.Create(50000m);

        // Act
        var act = () => Proposal.Create("PROP-20260129-00001", "", cpf, insuranceValue);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Customer name is required");
    }

    [Fact]
    public void Create_WithLongCustomerName_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 201);
        var cpf = CPF.Create("11144477735");
        var insuranceValue = Money.Create(50000m);

        // Act
        var act = () => Proposal.Create("PROP-20260129-00001", longName, cpf, insuranceValue);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Customer name cannot exceed 200 characters");
    }

    [Fact]
    public void Approve_WhenInAnalysis_ShouldChangeStatusToApproved()
    {
        // Arrange
        var proposal = CreateValidProposal();

        // Act
        proposal.Approve();

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Approved);
        proposal.UpdatedAt.Should().NotBeNull();
        proposal.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Approve_WhenAlreadyApproved_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = CreateValidProposal();
        proposal.Approve();

        // Act
        var act = () => proposal.Approve();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Proposal is already approved");
    }

    [Fact]
    public void Approve_WhenRejected_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = CreateValidProposal();
        proposal.Reject("Test reason");

        // Act
        var act = () => proposal.Approve();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot approve a rejected proposal");
    }

    [Fact]
    public void Reject_WithValidReason_ShouldChangeStatusToRejected()
    {
        // Arrange
        var proposal = CreateValidProposal();
        var reason = "Insufficient documentation";

        // Act
        proposal.Reject(reason);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Rejected);
        proposal.RejectionReason.Should().Be(reason);
        proposal.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reject_WithEmptyReason_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = CreateValidProposal();

        // Act
        var act = () => proposal.Reject("");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Rejection reason is required");
    }

    [Fact]
    public void Reject_WhenAlreadyApproved_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = CreateValidProposal();
        proposal.Approve();

        // Act
        var act = () => proposal.Reject("Test reason");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot reject an approved proposal");
    }

    private static Proposal CreateValidProposal()
    {
        var cpf = CPF.Create("11144477735");
        var insuranceValue = Money.Create(50000m);
        return Proposal.Create("PROP-20260129-00001", "John Doe", cpf, insuranceValue);
    }
}
