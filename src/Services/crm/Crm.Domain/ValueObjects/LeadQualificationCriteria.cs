using TossErp.CRM.Domain.SeedWork;

namespace TossErp.CRM.Domain.ValueObjects;

/// <summary>
/// Represents lead qualification criteria for automated lead scoring and qualification
/// Based on BANT (Budget, Authority, Need, Timeline) methodology
/// </summary>
public class LeadQualificationCriteria : ValueObject
{
    public bool HasBudget { get; private set; }
    public bool HasAuthority { get; private set; }
    public bool HasNeed { get; private set; }
    public bool HasTimeline { get; private set; }
    public int ScoreWeight { get; private set; }
    public string? QualificationNotes { get; private set; }
    public DateTime EvaluatedAt { get; private set; }
    public string EvaluatedBy { get; private set; }

    private LeadQualificationCriteria() 
    { 
        EvaluatedBy = null!;
    } // EF Core

    public LeadQualificationCriteria(
        bool hasBudget, 
        bool hasAuthority, 
        bool hasNeed, 
        bool hasTimeline,
        string evaluatedBy,
        string? qualificationNotes = null)
    {
        HasBudget = hasBudget;
        HasAuthority = hasAuthority;
        HasNeed = hasNeed;
        HasTimeline = hasTimeline;
        EvaluatedBy = evaluatedBy ?? throw new ArgumentNullException(nameof(evaluatedBy));
        QualificationNotes = qualificationNotes?.Trim();
        EvaluatedAt = DateTime.UtcNow;
        ScoreWeight = CalculateScoreWeight();
    }

    public bool IsQualified => HasBudget && HasAuthority && HasNeed && HasTimeline;
    public bool IsPartiallyQualified => BANTScore >= 2;
    public int BANTScore => (HasBudget ? 1 : 0) + (HasAuthority ? 1 : 0) + (HasNeed ? 1 : 0) + (HasTimeline ? 1 : 0);
    
    public string QualificationLevel => BANTScore switch
    {
        4 => "Fully Qualified",
        3 => "Highly Qualified", 
        2 => "Partially Qualified",
        1 => "Low Qualification",
        0 => "Not Qualified"
    };

    private int CalculateScoreWeight()
    {
        return BANTScore switch
        {
            4 => 40, // Fully qualified gets maximum points
            3 => 25,
            2 => 15,
            1 => 5,
            0 => -10 // Penalize unqualified leads
        };
    }

    public LeadQualificationCriteria UpdateBudget(bool hasBudget, string updatedBy, string? notes = null)
    {
        return new LeadQualificationCriteria(hasBudget, HasAuthority, HasNeed, HasTimeline, updatedBy, notes);
    }

    public LeadQualificationCriteria UpdateAuthority(bool hasAuthority, string updatedBy, string? notes = null)
    {
        return new LeadQualificationCriteria(HasBudget, hasAuthority, HasNeed, HasTimeline, updatedBy, notes);
    }

    public LeadQualificationCriteria UpdateNeed(bool hasNeed, string updatedBy, string? notes = null)
    {
        return new LeadQualificationCriteria(HasBudget, HasAuthority, hasNeed, HasTimeline, updatedBy, notes);
    }

    public LeadQualificationCriteria UpdateTimeline(bool hasTimeline, string updatedBy, string? notes = null)
    {
        return new LeadQualificationCriteria(HasBudget, HasAuthority, HasNeed, hasTimeline, updatedBy, notes);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HasBudget;
        yield return HasAuthority;
        yield return HasNeed;
        yield return HasTimeline;
        yield return ScoreWeight;
        yield return QualificationNotes ?? string.Empty;
        yield return EvaluatedAt;
        yield return EvaluatedBy;
    }

    public override string ToString()
    {
        return $"BANT Score: {BANTScore}/4 ({QualificationLevel}) - B:{HasBudget} A:{HasAuthority} N:{HasNeed} T:{HasTimeline}";
    }
}
