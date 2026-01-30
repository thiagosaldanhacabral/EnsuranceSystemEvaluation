using FluentAssertions;
using ContractService.Domain.ValueObjects;
using SharedKernel.Exceptions;

namespace ContractService.UnitTests.Domain.ValueObjects;

public class CustomerInfoTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCustomerInfo()
    {
        // Arrange
        var name = "John Doe";
        var cpf = "11144477735";
        var email = "john@example.com";

        // Act
        var customerInfo = CustomerInfo.Create(name, cpf, email);

        // Assert
        customerInfo.Should().NotBeNull();
        customerInfo.Name.Should().Be(name);
        customerInfo.CPF.Should().Be(cpf);
        customerInfo.Email.Should().Be(email);
    }

    [Fact]
    public void Create_WithoutEmail_ShouldCreateCustomerInfo()
    {
        // Arrange
        var name = "John Doe";
        var cpf = "11144477735";

        // Act
        var customerInfo = CustomerInfo.Create(name, cpf);

        // Assert
        customerInfo.Name.Should().Be(name);
        customerInfo.CPF.Should().Be(cpf);
        customerInfo.Email.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldThrowDomainException(string? name)
    {
        // Act
        var act = () => CustomerInfo.Create(name!, "11144477735");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Customer name is required");
    }

    [Fact]
    public void Create_WithLongName_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 201);

        // Act
        var act = () => CustomerInfo.Create(longName, "11144477735");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Customer name cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyCPF_ShouldThrowDomainException(string? cpf)
    {
        // Act
        var act = () => CustomerInfo.Create("John Doe", cpf!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Customer CPF is required");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345678")]
    [InlineData("111444777352")]
    public void Create_WithInvalidCPFLength_ShouldThrowDomainException(string cpf)
    {
        // Act
        var act = () => CustomerInfo.Create("John Doe", cpf);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CPF must have 11 digits");
    }

    [Theory]
    [InlineData("123.456.789-01", "12345678901")]
    [InlineData("987.654.321-00", "98765432100")]
    public void Create_WithFormattedCPF_ShouldRemoveFormatting(string formattedCpf, string expectedCpf)
    {
        // Act
        var customerInfo = CustomerInfo.Create("John Doe", formattedCpf);

        // Assert
        customerInfo.CPF.Should().Be(expectedCpf);
    }

    [Fact]
    public void ToString_ShouldReturnNameAndCPF()
    {
        // Arrange
        var customerInfo = CustomerInfo.Create("John Doe", "11144477735");

        // Act
        var result = customerInfo.ToString();

        // Assert
        result.Should().Be("John Doe (11144477735)");
    }

    [Fact]
    public void Equals_WithSameData_ShouldReturnTrue()
    {
        // Arrange
        var customer1 = CustomerInfo.Create("John Doe", "11144477735", "john@example.com");
        var customer2 = CustomerInfo.Create("John Doe", "11144477735", "john@example.com");

        // Act & Assert
        customer1.Should().Be(customer2);
        (customer1 == customer2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentName_ShouldReturnFalse()
    {
        // Arrange
        var customer1 = CustomerInfo.Create("John Doe", "11144477735");
        var customer2 = CustomerInfo.Create("Jane Doe", "11144477735");

        // Act & Assert
        customer1.Should().NotBe(customer2);
    }

    [Fact]
    public void Equals_WithDifferentCPF_ShouldReturnFalse()
    {
        // Arrange
        var customer1 = CustomerInfo.Create("John Doe", "11144477735");
        var customer2 = CustomerInfo.Create("John Doe", "98765432100");

        // Act & Assert
        customer1.Should().NotBe(customer2);
    }
}
