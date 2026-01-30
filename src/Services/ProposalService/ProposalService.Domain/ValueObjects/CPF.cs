using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace ProposalService.Domain.ValueObjects;

/// <summary>
/// Value object representing a Brazilian CPF (Cadastro de Pessoas Físicas)
/// </summary>
public sealed class CPF : ValueObject
{
    public string Number { get; }

    private CPF(string number)
    {
        Number = number;
    }

    public static CPF Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("CPF cannot be empty");

        // Remove formatting characters
        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cleanCpf.Length != 11)
            throw new DomainException("CPF must have 11 digits");

        // Check for known invalid CPFs (all same digits)
        if (cleanCpf.Distinct().Count() == 1)
            throw new DomainException("Invalid CPF");

        if (!IsValidCPF(cleanCpf))
            throw new DomainException("Invalid CPF check digits");

        return new CPF(cleanCpf);
    }

    /// <summary>
    /// Validates CPF check digits
    /// </summary>
    private static bool IsValidCPF(string cpf)
    {
        // Calculate first check digit
        var sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (cpf[i] - '0') * (10 - i);
        }

        var remainder = sum % 11;
        var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        if (firstCheckDigit != (cpf[9] - '0'))
            return false;

        // Calculate second check digit
        sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += (cpf[i] - '0') * (11 - i);
        }

        remainder = sum % 11;
        var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        return secondCheckDigit == (cpf[10] - '0');
    }

    /// <summary>
    /// Returns formatted CPF (###.###.###-##)
    /// </summary>
    public string Formatted =>
        $"{Number.Substring(0, 3)}.{Number.Substring(3, 3)}.{Number.Substring(6, 3)}-{Number.Substring(9, 2)}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Number;
    }

    public override string ToString() => Formatted;
}
