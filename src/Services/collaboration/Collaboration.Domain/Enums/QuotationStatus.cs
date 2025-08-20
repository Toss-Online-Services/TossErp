namespace Collaboration.Domain.Enums;

/// <summary>
/// Represents the status of a supplier quotation
/// </summary>
public enum QuotationStatus
{
    /// <summary>
    /// Quotation has been submitted and is under review
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// Quotation has been accepted
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// Quotation has been rejected
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Quotation has been withdrawn by the supplier
    /// </summary>
    Withdrawn = 3,

    /// <summary>
    /// Quotation has expired
    /// </summary>
    Expired = 4,

    /// <summary>
    /// Quotation is under negotiation
    /// </summary>
    UnderNegotiation = 5
}
