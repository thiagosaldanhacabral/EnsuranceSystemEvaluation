using SharedKernel.Exceptions;

namespace ProposalService.Domain.Exceptions;

/// <summary>
/// Exception for proposal business rule violations
/// </summary>
public class ProposalDomainException : DomainException
{
    public ProposalDomainException(string message) : base(message)
    {
    }

    public ProposalDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
