namespace Financial.Domain.Enums;

/// <summary>
/// Represents the status of an insurance claim
/// </summary>
public enum ClaimStatus
{
    /// <summary>
    /// Claim has been submitted and is awaiting review
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// Claim is under investigation
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Additional documentation is required
    /// </summary>
    DocumentationRequired = 2,

    /// <summary>
    /// Claim has been approved for payment
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Claim has been rejected
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Claim has been paid out
    /// </summary>
    Paid = 5,

    /// <summary>
    /// Claim has been closed
    /// </summary>
    Closed = 6
}
