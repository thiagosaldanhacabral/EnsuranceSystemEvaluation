using FluentAssertions;
using ContractService.Domain.Entities;
using ContractService.Domain.ValueObjects;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace ContractService.UnitTests.Domain.Entities;

public class ContractTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateContract()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contractNumber = "CONT-20260129-00001";
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(1000m);
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = startDate.AddMonths(12);

        // Act
        var contract = Contract.Create(proposalId, contractNumber, customer, premium, startDate, endDate);

        // Assert
        contract.Should().NotBeNull();
        contract.ProposalId.Should().Be(proposalId);
        contract.ContractNumber.Should().Be(contractNumber);
        contract.Customer.Should().Be(customer);
        contract.Premium.Should().Be(premium);
        contract.StartDate.Should().Be(startDate);
        contract.EndDate.Should().Be(endDate);
        contract.ContractDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyContractNumber_ShouldThrowDomainException()
    {
        // Arrange
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(1000m);
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = startDate.AddMonths(12);

        // Act
        var act = () => Contract.Create(Guid.NewGuid(), "", customer, premium, startDate, endDate);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Contract number is required");
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldThrowDomainException()
    {
        // Arrange
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(1000m);
        var startDate = DateTime.UtcNow.Date.AddDays(10);
        var endDate = startDate.AddDays(-5);

        // Act
        var act = () => Contract.Create(
            Guid.NewGuid(),
            "CONT-20260129-00001",
            customer,
            premium,
            startDate,
            endDate);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("End date must be after start date");
    }

    [Fact]
    public void IsActive_WhenCurrentDateIsBetweenStartAndEnd_ShouldReturnTrue()
    {
        // Arrange
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(1000m);
        var startDate = DateTime.UtcNow.Date.AddDays(-10);
        var endDate = DateTime.UtcNow.Date.AddDays(10);

        var contract = Contract.Create(
            Guid.NewGuid(),
            "CONT-20260129-00001",
            customer,
            premium,
            startDate,
            endDate);

        // Act
        var isActive = contract.IsActive();

        // Assert
        isActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenCurrentDateIsBeforeStart_ShouldReturnFalse()
    {
        // Arrange
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(1000m);
        var startDate = DateTime.UtcNow.Date.AddDays(10);
        var endDate = startDate.AddDays(20);

        var contract = Contract.Create(
            Guid.NewGuid(),
            "CONT-20260129-00001",
            customer,
            premium,
            startDate,
            endDate);

        // Act
        var isActive = contract.IsActive();

        // Assert
        isActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenCurrentDateIsAfterEnd_ShouldReturnFalse()
    {
        // Arrange
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(1000m);
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date.AddDays(-10);

        var contract = Contract.Create(
            Guid.NewGuid(),
            "CONT-20260129-00001",
            customer,
            premium,
            startDate,
            endDate);

        // Act
        var isActive = contract.IsActive();

        // Assert
        isActive.Should().BeFalse();
    }

    [Fact]
    public void GetDurationInDays_ShouldReturnCorrectDuration()
    {
        // Arrange
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(1000m);
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 12, 31);

        var contract = Contract.Create(
            Guid.NewGuid(),
            "CONT-20260129-00001",
            customer,
            premium,
            startDate,
            endDate);

        // Act
        var duration = contract.GetDurationInDays();

        // Assert
        duration.Should().Be(364); // 2026 is not a leap year
    }
}
