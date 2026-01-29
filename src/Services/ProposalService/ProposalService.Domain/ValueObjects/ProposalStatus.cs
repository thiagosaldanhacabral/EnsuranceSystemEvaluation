namespace ProposalService.Domain.ValueObjects;

/// <summary>
/// Represents the status of a proposal in its lifecycle
/// </summary>
public enum ProposalStatus
{
    /// <summary>
    /// Proposal is under analysis
    /// </summary>
    InAnalysis = 0,

    /// <summary>
    /// Proposal has been approved
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Proposal has been rejected
    /// </summary>
    Rejected = 2
}
