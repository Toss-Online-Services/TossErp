using Toss.Domain.Common;

namespace Toss.Domain.Entities.Onboarding;

/// <summary>
/// Tracks onboarding completion status for different user roles (Retailer, Supplier, Driver)
/// </summary>
public class OnboardingStatus : BaseAuditableEntity
{
    public OnboardingStatus()
    {
        UserId = string.Empty;
        Role = string.Empty;
        CompletedSteps = new List<string>();
    }

    /// <summary>
    /// Gets or sets the user ID (foreign key to Identity User)
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the role this onboarding is for (Retailer, Supplier, Driver)
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Gets or sets whether onboarding is completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the list of completed step identifiers
    /// </summary>
    public List<string> CompletedSteps { get; set; }

    /// <summary>
    /// Gets or sets the current step the user is on (for resuming)
    /// </summary>
    public int CurrentStep { get; set; }

    /// <summary>
    /// Gets or sets any additional onboarding data stored as JSON
    /// </summary>
    public string? OnboardingData { get; set; }
}

