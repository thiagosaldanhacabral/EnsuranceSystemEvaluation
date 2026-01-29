using ContractService.Domain.Exceptions;
using ContractService.Domain.ValueObjects;
using SharedKernel.Domain;

namespace ContractService.Domain.Entities;

/// <summary>
/// Represents an insurance contract aggregate root
/// </summary>
public class Contract : Entity
{
    public Guid ProposalId { get; private set; }
    public string ContractNumber { get; private set; } = string.Empty;
    public CustomerInfo Customer { get; private set; } = null!;
    public Money Premium { get; private set; } = null!;
    public DateTime ContractDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    // EF Core constructor
    private Contract() { }

    private Contract(
        Guid proposalId,
        string contractNumber,
        CustomerInfo customer,
        Money premium,
        DateTime startDate,
        DateTime endDate)
    {
        if (proposalId == Guid.Empty)
            throw new ContractDomainException("Proposal ID is required");

        if (string.IsNullOrWhiteSpace(contractNumber))
            throw new ContractDomainException("Contract number is required");

        if (startDate >= endDate)
            throw new ContractDomainException("End date must be after start date");

        ProposalId = proposalId;
        ContractNumber = contractNumber;
        Customer = customer ?? throw new ContractDomainException("Customer information is required");
        Premium = premium ?? throw new ContractDomainException("Premium is required");
        ContractDate = DateTime.UtcNow;
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Factory method to create a new contract
    /// </summary>
    public static Contract Create(
        Guid proposalId,
        string contractNumber,
        CustomerInfo customer,
        Money premium,
        DateTime startDate,
        DateTime endDate)
    {
        if (premium.Amount <= 0)
            throw new ContractDomainException("Premium must be greater than zero");

        return new Contract(proposalId, contractNumber, customer, premium, startDate, endDate);
    }

    /// <summary>
    /// Checks if contract is currently active
    /// </summary>
    public bool IsActive()
    {
        var now = DateTime.UtcNow.Date;
        return now >= StartDate.Date && now <= EndDate.Date;
    }

    /// <summary>
    /// Gets the contract duration in days
    /// </summary>
    public int GetDurationInDays() => (EndDate - StartDate).Days;
}
