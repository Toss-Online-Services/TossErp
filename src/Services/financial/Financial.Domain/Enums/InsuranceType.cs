namespace Financial.Domain.Enums;

/// <summary>
/// Represents different types of insurance policies
/// </summary>
public enum InsuranceType
{
    /// <summary>
    /// General business liability insurance
    /// </summary>
    BusinessLiability = 0,

    /// <summary>
    /// Property insurance for business assets
    /// </summary>
    Property = 1,

    /// <summary>
    /// Professional indemnity insurance
    /// </summary>
    ProfessionalIndemnity = 2,

    /// <summary>
    /// Workers compensation insurance
    /// </summary>
    WorkersCompensation = 3,

    /// <summary>
    /// Commercial vehicle insurance
    /// </summary>
    Vehicle = 4,

    /// <summary>
    /// Health insurance for employees
    /// </summary>
    Health = 5,

    /// <summary>
    /// Business interruption insurance
    /// </summary>
    BusinessInterruption = 6,

    /// <summary>
    /// Cyber liability insurance
    /// </summary>
    CyberLiability = 7
}
