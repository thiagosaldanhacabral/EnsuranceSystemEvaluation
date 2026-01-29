using ProposalService.Domain.Exceptions;
using ProposalService.Domain.ValueObjects;
using SharedKernel.Domain;

namespace ProposalService.Domain.Entities;

/// <summary>
/// Represents an insurance proposal aggregate root
/// </summary>
public class Proposal : Entity
{
    public string ProposalNumber { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public CPF CustomerCPF { get; private set; } = null!;
    public Money InsuranceValue { get; private set; } = null!;
    public ProposalStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core constructor
    private Proposal() { }

    private Proposal(string proposalNumber, string customerName, CPF customerCPF, Money insuranceValue)
    {
        if (string.IsNullOrWhiteSpace(proposalNumber))
            throw new ProposalDomainException("Proposal number is required");

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ProposalDomainException("Customer name is required");

        if (customerName.Length > 200)
            throw new ProposalDomainException("Customer name cannot exceed 200 characters");

        ProposalNumber = proposalNumber;
        CustomerName = customerName;
        CustomerCPF = customerCPF ?? throw new ProposalDomainException("Customer CPF is required");
        InsuranceValue = insuranceValue ?? throw new ProposalDomainException("Insurance value is required");
        Status = ProposalStatus.InAnalysis;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new proposal
    /// </summary>
    public static Proposal Create(string proposalNumber, string customerName, CPF customerCPF, Money insuranceValue)
    {
        if (insuranceValue.Amount <= 0)
            throw new ProposalDomainException("Insurance value must be greater than zero");

        if (insuranceValue.Amount > 1_000_000)
            throw new ProposalDomainException("Insurance value cannot exceed 1,000,000");

        return new Proposal(proposalNumber, customerName, customerCPF, insuranceValue);
    }

    /// <summary>
    /// Approves the proposal
    /// </summary>
    public void Approve()
    {
        if (Status == ProposalStatus.Approved)
            throw new ProposalDomainException("Proposal is already approved");

        if (Status == ProposalStatus.Rejected)
            throw new ProposalDomainException("Cannot approve a rejected proposal");

        Status = ProposalStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejects the proposal with a reason
    /// </summary>
    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ProposalDomainException("Rejection reason is required");

        if (Status == ProposalStatus.Rejected)
            throw new ProposalDomainException("Proposal is already rejected");

        if (Status == ProposalStatus.Approved)
            throw new ProposalDomainException("Cannot reject an approved proposal");

        Status = ProposalStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if proposal is eligible for contracting
    /// </summary>
    public bool IsEligibleForContract() => Status == ProposalStatus.Approved;
}
