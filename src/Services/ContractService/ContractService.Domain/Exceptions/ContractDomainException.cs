using SharedKernel.Exceptions;

namespace ContractService.Domain.Exceptions;

/// <summary>
/// Exception for contract business rule violations
/// </summary>
public class ContractDomainException : DomainException
{
    public ContractDomainException(string message) : base(message)
    {
    }

    public ContractDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
