using TossErp.Projects.Domain.Enums;
using TossErp.Projects.Domain.SeedWork;

namespace TossErp.Projects.Domain.ValueObjects;

/// <summary>
/// Duration value object for time-based measurements
/// </summary>
public class Duration : ValueObject
{
    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public TimeSpan TimeSpan => new(Hours, Minutes, 0);

    private Duration() { } // EF Core

    public Duration(int hours, int minutes = 0)
    {
        if (hours < 0)
            throw new ArgumentException("Hours cannot be negative");
        if (minutes < 0 || minutes >= 60)
            throw new ArgumentException("Minutes must be between 0 and 59");

        Hours = hours;
        Minutes = minutes;
    }

    public Duration(TimeSpan timeSpan)
    {
        if (timeSpan < TimeSpan.Zero)
            throw new ArgumentException("Duration cannot be negative");

        Hours = (int)timeSpan.TotalHours;
        Minutes = timeSpan.Minutes;
    }

    public Duration Add(Duration other)
    {
        var totalMinutes = (Hours * 60 + Minutes) + (other.Hours * 60 + other.Minutes);
        return new Duration(totalMinutes / 60, totalMinutes % 60);
    }

    public Duration Subtract(Duration other)
    {
        var totalMinutes = (Hours * 60 + Minutes) - (other.Hours * 60 + other.Minutes);
        if (totalMinutes < 0)
            throw new ArgumentException("Result would be negative");
        return new Duration(totalMinutes / 60, totalMinutes % 60);
    }

    public decimal ToHours() => Hours + (decimal)Minutes / 60;

    public bool IsZero => Hours == 0 && Minutes == 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hours;
        yield return Minutes;
    }

    public override string ToString() => $"{Hours:D2}:{Minutes:D2}";

    public static Duration FromHours(decimal hours)
    {
        var totalMinutes = (int)(hours * 60);
        return new Duration(totalMinutes / 60, totalMinutes % 60);
    }

    public static Duration Zero => new(0, 0);
}

/// <summary>
/// Date range value object for project planning
/// </summary>
public class DateRange : ValueObject
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int DurationInDays => (EndDate - StartDate).Days + 1;

    private DateRange() { } // EF Core

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date");

        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }

    public bool Contains(DateTime date) => date.Date >= StartDate && date.Date <= EndDate;

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public DateRange Extend(int days)
    {
        if (days < 0)
            throw new ArgumentException("Days cannot be negative");
        return new DateRange(StartDate, EndDate.AddDays(days));
    }

    public bool IsActive => DateTime.Today >= StartDate && DateTime.Today <= EndDate;
    public bool IsUpcoming => DateTime.Today < StartDate;
    public bool IsCompleted => DateTime.Today > EndDate;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}

/// <summary>
/// Progress value object for tracking completion
/// </summary>
public class Progress : ValueObject
{
    public decimal Percentage { get; private set; }

    private Progress() { } // EF Core

    public Progress(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Percentage must be between 0 and 100");

        Percentage = Math.Round(percentage, 2);
    }

    public Progress(int completed, int total)
    {
        if (total <= 0)
            throw new ArgumentException("Total must be greater than zero");
        if (completed < 0 || completed > total)
            throw new ArgumentException("Completed must be between 0 and total");

        Percentage = total == 0 ? 0 : Math.Round((decimal)completed / total * 100, 2);
    }

    public bool IsComplete => Percentage >= 100;
    public bool IsStarted => Percentage > 0;
    public bool IsNotStarted => Percentage == 0;

    public Progress Add(Progress other)
    {
        var newPercentage = Math.Min(100, Percentage + other.Percentage);
        return new Progress(newPercentage);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Percentage;
    }

    public override string ToString() => $"{Percentage:F1}%";

    public static Progress Zero => new(0);
    public static Progress Complete => new(100);
}

/// <summary>
/// Money value object for project budgets and costs
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money()
    {
        Currency = null!;
    } // EF Core

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        Amount = Math.Round(amount, 2);
        Currency = currency?.ToUpper() ?? throw new ArgumentNullException(nameof(currency));
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new ArgumentException($"Cannot add different currencies: {Currency} and {other.Currency}");
        
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new ArgumentException($"Cannot subtract different currencies: {Currency} and {other.Currency}");
        
        var result = Amount - other.Amount;
        if (result < 0)
            throw new ArgumentException("Result would be negative");
        
        return new Money(result, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Factor cannot be negative");
        
        return new Money(Amount * factor, Currency);
    }

    public decimal Divide(Money other)
    {
        if (Currency != other.Currency)
            throw new ArgumentException($"Cannot divide different currencies: {Currency} and {other.Currency}");
        
        if (other.Amount == 0)
            throw new DivideByZeroException("Cannot divide by zero amount");
        
        return Amount / other.Amount;
    }

    public bool IsZero => Amount == 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:C} {Currency}";

    public static Money Zero(string currency = "USD") => new(0, currency);
}

/// <summary>
/// Priority value object with urgency and importance scoring
/// </summary>
public class Priority : ValueObject
{
    public int UrgencyScore { get; private set; }
    public int ImportanceScore { get; private set; }
    public int TotalScore => UrgencyScore + ImportanceScore;
    public ProjectPriority Level { get; private set; }

    private Priority() { } // EF Core

    public Priority(int urgencyScore, int importanceScore)
    {
        if (urgencyScore < 1 || urgencyScore > 5)
            throw new ArgumentException("Urgency score must be between 1 and 5");
        if (importanceScore < 1 || importanceScore > 5)
            throw new ArgumentException("Importance score must be between 1 and 5");

        UrgencyScore = urgencyScore;
        ImportanceScore = importanceScore;
        Level = CalculateLevel();
    }

    public Priority(ProjectPriority level)
    {
        Level = level;
        (UrgencyScore, ImportanceScore) = level switch
        {
            ProjectPriority.Low => (2, 2),
            ProjectPriority.Medium => (3, 3),
            ProjectPriority.High => (4, 4),
            ProjectPriority.Critical => (5, 5),
            _ => throw new ArgumentException($"Invalid priority level: {level}")
        };
    }

    private ProjectPriority CalculateLevel()
    {
        return TotalScore switch
        {
            <= 4 => ProjectPriority.Low,
            <= 6 => ProjectPriority.Medium,
            <= 8 => ProjectPriority.High,
            _ => ProjectPriority.Critical
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UrgencyScore;
        yield return ImportanceScore;
    }

    public override string ToString() => $"{Level} (U:{UrgencyScore}, I:{ImportanceScore})";
}

/// <summary>
/// Project code value object for unique identification
/// </summary>
public class ProjectCode : ValueObject
{
    public string Value { get; private set; }

    private ProjectCode()
    {
        Value = null!;
    } // EF Core

    public ProjectCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Project code cannot be empty");
        
        value = value.Trim().ToUpper();
        
        if (!IsValidFormat(value))
            throw new ArgumentException("Project code must be 3-10 alphanumeric characters");

        Value = value;
    }

    private static bool IsValidFormat(string code)
    {
        return code.Length >= 3 && code.Length <= 10 && 
               code.All(c => char.IsLetterOrDigit(c));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ProjectCode projectCode) => projectCode.Value;
}

/// <summary>
/// Effort estimation value object
/// </summary>
public class Effort : ValueObject
{
    public Duration EstimatedDuration { get; private set; }
    public Duration ActualDuration { get; private set; }
    public decimal EstimatedCost { get; private set; }
    public decimal ActualCost { get; private set; }
    public string Currency { get; private set; }

    private Effort()
    {
        EstimatedDuration = null!;
        ActualDuration = null!;
        Currency = null!;
    } // EF Core

    public Effort(Duration estimatedDuration, decimal estimatedCost, string currency = "USD")
    {
        EstimatedDuration = estimatedDuration ?? throw new ArgumentNullException(nameof(estimatedDuration));
        ActualDuration = Duration.Zero;
        EstimatedCost = estimatedCost >= 0 ? estimatedCost : throw new ArgumentException("Estimated cost cannot be negative");
        ActualCost = 0;
        Currency = currency?.ToUpper() ?? throw new ArgumentNullException(nameof(currency));
    }

    public void UpdateActuals(Duration actualDuration, decimal actualCost)
    {
        ActualDuration = actualDuration ?? throw new ArgumentNullException(nameof(actualDuration));
        ActualCost = actualCost >= 0 ? actualCost : throw new ArgumentException("Actual cost cannot be negative");
    }

    public decimal DurationVariance => ActualDuration.ToHours() - EstimatedDuration.ToHours();
    public decimal CostVariance => ActualCost - EstimatedCost;
    public decimal DurationVariancePercentage => EstimatedDuration.ToHours() > 0 ? 
        (DurationVariance / EstimatedDuration.ToHours()) * 100 : 0;
    public decimal CostVariancePercentage => EstimatedCost > 0 ? 
        (CostVariance / EstimatedCost) * 100 : 0;

    public bool IsOverEstimate => DurationVariance > 0 || CostVariance > 0;
    public bool IsUnderEstimate => DurationVariance < 0 || CostVariance < 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EstimatedDuration;
        yield return ActualDuration;
        yield return EstimatedCost;
        yield return ActualCost;
        yield return Currency;
    }

    public override string ToString() => 
        $"Est: {EstimatedDuration} / {EstimatedCost:C} {Currency}, " +
        $"Act: {ActualDuration} / {ActualCost:C} {Currency}";
}

/// <summary>
/// Risk score value object
/// </summary>
public class RiskScore : ValueObject
{
    public RiskProbability Probability { get; private set; }
    public RiskImpact Impact { get; private set; }
    public int NumericScore { get; private set; }
    public string RiskLevel { get; private set; }

    private RiskScore()
    {
        RiskLevel = null!;
    } // EF Core

    public RiskScore(RiskProbability probability, RiskImpact impact)
    {
        Probability = probability;
        Impact = impact;
        NumericScore = CalculateScore();
        RiskLevel = CalculateLevel();
    }

    private int CalculateScore()
    {
        var probValue = (int)Probability + 1; // 1-5
        var impactValue = (int)Impact + 1; // 1-5
        return probValue * impactValue; // 1-25
    }

    private string CalculateLevel()
    {
        return NumericScore switch
        {
            <= 5 => "Low",
            <= 10 => "Medium",
            <= 15 => "High",
            <= 20 => "Very High",
            _ => "Critical"
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Probability;
        yield return Impact;
    }

    public override string ToString() => $"{RiskLevel} ({NumericScore}) - P:{Probability}, I:{Impact}";
}

/// <summary>
/// Team capacity value object
/// </summary>
public class TeamCapacity : ValueObject
{
    public int TeamSize { get; private set; }
    public Duration AvailableHoursPerDay { get; private set; }
    public int WorkingDaysPerWeek { get; private set; }
    public decimal UtilizationRate { get; private set; }

    private TeamCapacity()
    {
        AvailableHoursPerDay = null!;
    } // EF Core

    public TeamCapacity(int teamSize, Duration availableHoursPerDay, int workingDaysPerWeek = 5, decimal utilizationRate = 0.8m)
    {
        if (teamSize <= 0)
            throw new ArgumentException("Team size must be positive");
        if (workingDaysPerWeek <= 0 || workingDaysPerWeek > 7)
            throw new ArgumentException("Working days per week must be between 1 and 7");
        if (utilizationRate <= 0 || utilizationRate > 1)
            throw new ArgumentException("Utilization rate must be between 0 and 1");

        TeamSize = teamSize;
        AvailableHoursPerDay = availableHoursPerDay ?? throw new ArgumentNullException(nameof(availableHoursPerDay));
        WorkingDaysPerWeek = workingDaysPerWeek;
        UtilizationRate = utilizationRate;
    }

    public Duration CalculateWeeklyCapacity()
    {
        var dailyHours = AvailableHoursPerDay.ToHours() * TeamSize * UtilizationRate;
        var weeklyHours = dailyHours * WorkingDaysPerWeek;
        return Duration.FromHours(weeklyHours);
    }

    public Duration CalculateCapacityForDays(int days)
    {
        var dailyHours = AvailableHoursPerDay.ToHours() * TeamSize * UtilizationRate;
        var totalHours = dailyHours * days;
        return Duration.FromHours(totalHours);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TeamSize;
        yield return AvailableHoursPerDay;
        yield return WorkingDaysPerWeek;
        yield return UtilizationRate;
    }

    public override string ToString() => 
        $"{TeamSize} people × {AvailableHoursPerDay}/day × {WorkingDaysPerWeek} days/week × {UtilizationRate:P0}";
}
