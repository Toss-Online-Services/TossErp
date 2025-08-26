using TossErp.CRM.Domain.SeedWork;

namespace TossErp.CRM.Domain.ValueObjects;

/// <summary>
/// Represents a lead pipeline stage with visual progression tracking
/// Supports customizable pipeline stages and automated stage progression
/// </summary>
public class LeadPipelineStage : ValueObject
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public int Order { get; private set; }
    public string Color { get; private set; }
    public string Icon { get; private set; }
    public bool IsActive { get; private set; }
    public int MinimumScore { get; private set; }
    public int MaximumDaysInStage { get; private set; }
    public string? Description { get; private set; }
    public DateTime EnteredAt { get; private set; }
    public string EnteredBy { get; private set; }

    // Standard pipeline stages
    public static readonly LeadPipelineStage New = new("New", "New Lead", 1, "#94a3b8", "user-plus", true, 0, 7, "Newly created lead awaiting initial contact");
    public static readonly LeadPipelineStage Contacted = new("Contacted", "Initial Contact", 2, "#3b82f6", "phone", true, 20, 14, "First contact has been made with the lead");
    public static readonly LeadPipelineStage Qualified = new("Qualified", "Qualified", 3, "#10b981", "check-circle", true, 60, 30, "Lead meets qualification criteria and shows genuine interest");
    public static readonly LeadPipelineStage Proposal = new("Proposal", "Proposal Sent", 4, "#f59e0b", "document-text", true, 70, 21, "Proposal or quote has been provided to the lead");
    public static readonly LeadPipelineStage Negotiation = new("Negotiation", "In Negotiation", 5, "#ef4444", "chat-bubble-left-right", true, 80, 14, "Actively discussing terms and conditions");
    public static readonly LeadPipelineStage Won = new("Won", "Converted", 6, "#22c55e", "trophy", false, 90, 0, "Lead successfully converted to customer");
    public static readonly LeadPipelineStage Lost = new("Lost", "Lost/Disqualified", 7, "#6b7280", "x-circle", false, 0, 0, "Lead was lost or disqualified");

    private LeadPipelineStage() 
    { 
        Name = null!;
        DisplayName = null!;
        Color = null!;
        Icon = null!;
        EnteredBy = null!;
    } // EF Core

    public LeadPipelineStage(
        string name,
        string displayName,
        int order,
        string color,
        string icon,
        bool isActive,
        int minimumScore,
        int maximumDaysInStage,
        string? description = null)
    {
        Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName?.Trim() ?? throw new ArgumentNullException(nameof(displayName));
        Order = order;
        Color = color?.Trim() ?? throw new ArgumentNullException(nameof(color));
        Icon = icon?.Trim() ?? throw new ArgumentNullException(nameof(icon));
        IsActive = isActive;
        MinimumScore = minimumScore;
        MaximumDaysInStage = maximumDaysInStage;
        Description = description?.Trim();
        EnteredAt = DateTime.UtcNow;
        EnteredBy = "System";
    }

    private LeadPipelineStage(
        string name,
        string displayName,
        int order,
        string color,
        string icon,
        bool isActive,
        int minimumScore,
        int maximumDaysInStage,
        string? description,
        DateTime enteredAt,
        string enteredBy)
    {
        Name = name;
        DisplayName = displayName;
        Order = order;
        Color = color;
        Icon = icon;
        IsActive = isActive;
        MinimumScore = minimumScore;
        MaximumDaysInStage = maximumDaysInStage;
        Description = description;
        EnteredAt = enteredAt;
        EnteredBy = enteredBy;
    }

    public bool CanProgressTo(LeadPipelineStage nextStage, int currentScore)
    {
        if (!IsActive || !nextStage.IsActive)
            return false;

        // Can't go backwards unless it's to Lost
        if (nextStage.Order < Order && nextStage != Lost)
            return false;

        // Check if lead meets minimum score requirements
        return currentScore >= nextStage.MinimumScore;
    }

    public bool IsOverdue => IsActive && MaximumDaysInStage > 0 && DaysInStage > MaximumDaysInStage;
    
    public int DaysInStage => (int)(DateTime.UtcNow - EnteredAt).TotalDays;

    public LeadPipelineStage EnterStage(string enteredBy)
    {
        return new LeadPipelineStage(
            Name,
            DisplayName,
            Order,
            Color,
            Icon,
            IsActive,
            MinimumScore,
            MaximumDaysInStage,
            Description,
            DateTime.UtcNow,
            enteredBy);
    }

    public static List<LeadPipelineStage> GetStandardPipeline()
    {
        return new List<LeadPipelineStage>
        {
            New,
            Contacted,
            Qualified,
            Proposal,
            Negotiation,
            Won,
            Lost
        };
    }

    public static LeadPipelineStage? GetByName(string name)
    {
        return GetStandardPipeline().FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static LeadPipelineStage GetNextStage(LeadPipelineStage currentStage)
    {
        var pipeline = GetStandardPipeline().Where(s => s.IsActive).OrderBy(s => s.Order).ToList();
        var currentIndex = pipeline.FindIndex(s => s.Name == currentStage.Name);
        
        if (currentIndex == -1 || currentIndex == pipeline.Count - 1)
            return currentStage;
        
        return pipeline[currentIndex + 1];
    }

    public static LeadPipelineStage GetPreviousStage(LeadPipelineStage currentStage)
    {
        var pipeline = GetStandardPipeline().Where(s => s.IsActive).OrderBy(s => s.Order).ToList();
        var currentIndex = pipeline.FindIndex(s => s.Name == currentStage.Name);
        
        if (currentIndex <= 0)
            return currentStage;
        
        return pipeline[currentIndex - 1];
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return DisplayName;
        yield return Order;
        yield return Color;
        yield return Icon;
        yield return IsActive;
        yield return MinimumScore;
        yield return MaximumDaysInStage;
        yield return Description ?? string.Empty;
    }

    public override string ToString()
    {
        return $"{DisplayName} (Order: {Order}, Min Score: {MinimumScore})";
    }
}
