using FluentAssertions;
using ProposalService.Domain.ValueObjects;
using SharedKernel.Exceptions;

namespace ProposalService.UnitTests.Domain.ValueObjects;

public class CPFTests
{
    // Valid CPF numbers for testing
    private const string ValidCPF1 = "11144477735";
    private const string ValidCPF2 = "52998224725";

    [Theory]
    [InlineData("11144477735")]
    [InlineData("52998224725")]
    public void Create_WithValidCPF_ShouldCreateCPF(string cpfNumber)
    {
        // Act
        var cpf = CPF.Create(cpfNumber);

        // Assert
        cpf.Should().NotBeNull();
        cpf.Number.Should().Be(cpfNumber);
    }

    [Theory]
    [InlineData("111.444.777-35", "11144477735")]
    [InlineData("529.982.247-25", "52998224725")]
    public void Create_WithFormattedCPF_ShouldRemoveFormatting(string formattedCpf, string expectedNumber)
    {
        // Act
        var cpf = CPF.Create(formattedCpf);

        // Assert
        cpf.Number.Should().Be(expectedNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyCPF_ShouldThrowDomainException(string? cpfNumber)
    {
        // Act
        var act = () => CPF.Create(cpfNumber!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CPF cannot be empty");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345678")]
    [InlineData("111444777352")]
    public void Create_WithInvalidLength_ShouldThrowDomainException(string cpfNumber)
    {
        // Act
        var act = () => CPF.Create(cpfNumber);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CPF must have 11 digits");
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("12345678900")]
    public void Create_WithInvalidCheckDigit_ShouldThrowDomainException(string cpfNumber)
    {
        // Act
        var act = () => CPF.Create(cpfNumber);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid CPF*");
    }

    [Fact]
    public void Formatted_ShouldReturnFormattedCPF()
    {
        // Arrange
        var cpf = CPF.Create(ValidCPF1);

        // Act
        var formatted = cpf.Formatted;

        // Assert
        formatted.Should().Be("111.444.777-35");
    }

    [Fact]
    public void Equals_WithSameCPF_ShouldReturnTrue()
    {
        // Arrange
        var cpf1 = CPF.Create(ValidCPF1);
        var cpf2 = CPF.Create(ValidCPF1);

        // Act & Assert
        cpf1.Should().Be(cpf2);
        (cpf1 == cpf2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCPF_ShouldReturnFalse()
    {
        // Arrange
        var cpf1 = CPF.Create(ValidCPF1);
        var cpf2 = CPF.Create(ValidCPF2);

        // Act & Assert
        cpf1.Should().NotBe(cpf2);
        (cpf1 != cpf2).Should().BeTrue();
    }
}
