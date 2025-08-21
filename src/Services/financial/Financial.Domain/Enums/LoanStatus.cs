namespace Financial.Domain.Enums;

/// <summary>
/// Represents the status of a loan application or active loan
/// </summary>
public enum LoanStatus
{
    /// <summary>
    /// Loan application is pending review
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Loan application is under review
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Loan has been approved
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Loan application has been rejected
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Loan is active and payments are being made
    /// </summary>
    Active = 4,

    /// <summary>
    /// Loan has been fully paid off
    /// </summary>
    PaidOff = 5,

    /// <summary>
    /// Loan is in default (overdue payments)
    /// </summary>
    Default = 6,

    /// <summary>
    /// Loan has been cancelled
    /// </summary>
    Cancelled = 7
}
