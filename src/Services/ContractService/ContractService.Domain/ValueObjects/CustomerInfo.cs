using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace ContractService.Domain.ValueObjects;

/// <summary>
/// Value object containing customer information
/// </summary>
public sealed class CustomerInfo : ValueObject
{
    public string Name { get; }
    public string CPF { get; }
    public string? Email { get; }

    private CustomerInfo(string name, string cpf, string? email)
    {
        Name = name;
        CPF = cpf;
        Email = email;
    }

    public static CustomerInfo Create(string name, string cpf, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required");

        if (name.Length > 200)
            throw new DomainException("Customer name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("Customer CPF is required");

        // Clean CPF
        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (cleanCpf.Length != 11)
            throw new DomainException("CPF must have 11 digits");

        return new CustomerInfo(name, cleanCpf, email);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return CPF;
        yield return Email;
    }

    public override string ToString() => $"{Name} ({CPF})";
}
