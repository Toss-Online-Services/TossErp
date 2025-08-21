using TossErp.HumanResources.Domain.SeedWork;
using TossErp.HumanResources.Domain.ValueObjects;
using TossErp.HumanResources.Domain.Enums;

namespace TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate.Entities;

/// <summary>
/// Employee Emergency Contact
/// </summary>
public class EmployeeContact : Entity
{
    public string ContactName { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public EmailAddress? Email { get; private set; }
    public bool IsPrimary { get; private set; }

    protected EmployeeContact() { } // For EF Core

    public EmployeeContact(
        string contactName,
        string relationship,
        PhoneNumber phoneNumber,
        EmailAddress? email = null)
    {
        if (string.IsNullOrWhiteSpace(contactName))
            throw new ArgumentException("Contact name cannot be empty", nameof(contactName));
        if (string.IsNullOrWhiteSpace(relationship))
            throw new ArgumentException("Relationship cannot be empty", nameof(relationship));

        ContactName = contactName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
        Email = email;
        IsPrimary = false;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
        MarkAsModified();
    }

    public void RemoveAsPrimary()
    {
        IsPrimary = false;
        MarkAsModified();
    }

    public void UpdateContact(PhoneNumber? phoneNumber = null, EmailAddress? email = null)
    {
        if (phoneNumber != null)
            PhoneNumber = phoneNumber;
        if (email != null)
            Email = email;

        MarkAsModified();
    }
}

/// <summary>
/// Employee Qualification/Education
/// </summary>
public class EmployeeQualification : Entity
{
    public string QualificationName { get; private set; } = string.Empty;
    public string Institution { get; private set; } = string.Empty;
    public int YearOfCompletion { get; private set; }
    public string? Grade { get; private set; }
    public string? FieldOfStudy { get; private set; }
    public bool IsVerified { get; private set; }

    protected EmployeeQualification() { } // For EF Core

    public EmployeeQualification(
        string qualificationName,
        string institution,
        int yearOfCompletion,
        string? grade = null,
        string? fieldOfStudy = null)
    {
        if (string.IsNullOrWhiteSpace(qualificationName))
            throw new ArgumentException("Qualification name cannot be empty", nameof(qualificationName));
        if (string.IsNullOrWhiteSpace(institution))
            throw new ArgumentException("Institution cannot be empty", nameof(institution));
        if (yearOfCompletion < 1900 || yearOfCompletion > DateTime.UtcNow.Year + 5)
            throw new ArgumentException("Invalid year of completion", nameof(yearOfCompletion));

        QualificationName = qualificationName;
        Institution = institution;
        YearOfCompletion = yearOfCompletion;
        Grade = grade;
        FieldOfStudy = fieldOfStudy;
        IsVerified = false;
    }

    public void Verify()
    {
        IsVerified = true;
        MarkAsModified();
    }

    public void UpdateGrade(string grade)
    {
        Grade = grade;
        MarkAsModified();
    }
}

/// <summary>
/// Employee Work Experience
/// </summary>
public class EmployeeExperience : Entity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string JobTitle { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Description { get; private set; }
    public string? Achievements { get; private set; }
    public bool IsCurrentJob { get; private set; }

    // Calculated property
    public TimeSpan Duration => (EndDate ?? DateTime.UtcNow) - StartDate;
    public double DurationInYears => Duration.TotalDays / 365.25;

    protected EmployeeExperience() { } // For EF Core

    public EmployeeExperience(
        string companyName,
        string jobTitle,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name cannot be empty", nameof(companyName));
        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new ArgumentException("Job title cannot be empty", nameof(jobTitle));
        if (startDate > DateTime.UtcNow)
            throw new ArgumentException("Start date cannot be in the future", nameof(startDate));
        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("End date cannot be before start date", nameof(endDate));

        CompanyName = companyName;
        JobTitle = jobTitle;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        IsCurrentJob = !endDate.HasValue;
    }

    public void UpdateEndDate(DateTime endDate)
    {
        if (endDate < StartDate)
            throw new ArgumentException("End date cannot be before start date", nameof(endDate));

        EndDate = endDate;
        IsCurrentJob = false;
        MarkAsModified();
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        MarkAsModified();
    }

    public void AddAchievements(string achievements)
    {
        Achievements = achievements;
        MarkAsModified();
    }
}

/// <summary>
/// Employee Skills
/// </summary>
public class EmployeeSkill : Entity
{
    public string SkillName { get; private set; } = string.Empty;
    public string? ProficiencyLevel { get; private set; }
    public DateTime? CertificationDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public bool IsVerified { get; private set; }

    protected EmployeeSkill() { } // For EF Core

    public EmployeeSkill(
        string skillName,
        string? proficiencyLevel = null,
        DateTime? certificationDate = null,
        DateTime? expiryDate = null)
    {
        if (string.IsNullOrWhiteSpace(skillName))
            throw new ArgumentException("Skill name cannot be empty", nameof(skillName));

        SkillName = skillName;
        ProficiencyLevel = proficiencyLevel;
        CertificationDate = certificationDate;
        ExpiryDate = expiryDate;
        IsVerified = false;
    }

    public void UpdateProficiency(string proficiencyLevel)
    {
        ProficiencyLevel = proficiencyLevel;
        MarkAsModified();
    }

    public void Certify(DateTime certificationDate, DateTime? expiryDate = null)
    {
        CertificationDate = certificationDate;
        ExpiryDate = expiryDate;
        IsVerified = true;
        MarkAsModified();
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
}

/// <summary>
/// Employee Documents
/// </summary>
public class EmployeeDocument : Entity
{
    public string DocumentType { get; private set; } = string.Empty;
    public string DocumentName { get; private set; } = string.Empty;
    public string? DocumentNumber { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public DateTime UploadDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public bool IsVerified { get; private set; }
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

    protected EmployeeDocument() { } // For EF Core

    public EmployeeDocument(
        string documentType,
        string documentName,
        string filePath,
        string? documentNumber = null,
        DateTime? expiryDate = null)
    {
        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException("Document type cannot be empty", nameof(documentType));
        if (string.IsNullOrWhiteSpace(documentName))
            throw new ArgumentException("Document name cannot be empty", nameof(documentName));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        DocumentType = documentType;
        DocumentName = documentName;
        FilePath = filePath;
        DocumentNumber = documentNumber;
        ExpiryDate = expiryDate;
        UploadDate = DateTime.UtcNow;
        IsVerified = false;
    }

    public void Verify()
    {
        IsVerified = true;
        MarkAsModified();
    }

    public void UpdateExpiry(DateTime expiryDate)
    {
        ExpiryDate = expiryDate;
        MarkAsModified();
    }
}

/// <summary>
/// Salary History
/// </summary>
public class SalaryHistory : Entity
{
    public Salary PreviousSalary { get; private set; } = null!;
    public Salary NewSalary { get; private set; } = null!;
    public DateTime EffectiveDate { get; private set; }
    public string? Reason { get; private set; }
    public decimal PercentageIncrease { get; private set; }

    protected SalaryHistory() { } // For EF Core

    public SalaryHistory(
        Salary previousSalary,
        Salary newSalary,
        DateTime effectiveDate,
        string? reason = null)
    {
        if (previousSalary.Currency != newSalary.Currency)
            throw new ArgumentException("Previous and new salary must have the same currency");

        PreviousSalary = previousSalary;
        NewSalary = newSalary;
        EffectiveDate = effectiveDate;
        Reason = reason;

        // Calculate percentage increase
        if (previousSalary.Amount > 0)
        {
            PercentageIncrease = ((newSalary.Amount - previousSalary.Amount) / previousSalary.Amount) * 100;
        }
    }

    public bool IsIncrease => NewSalary.Amount > PreviousSalary.Amount;
    public bool IsDecrease => NewSalary.Amount < PreviousSalary.Amount;
    public Salary AmountChange => NewSalary.Subtract(PreviousSalary);
}

/// <summary>
/// Performance Review
/// </summary>
public class PerformanceReview : Entity
{
    public DateTime ReviewDate { get; private set; }
    public DateTime ReviewPeriodStart { get; private set; }
    public DateTime ReviewPeriodEnd { get; private set; }
    public PerformanceRating Rating { get; private set; }
    public string? ReviewComments { get; private set; }
    public string? Goals { get; private set; }
    public string? Achievements { get; private set; }
    public string? AreasForImprovement { get; private set; }
    public string? ReviewerComments { get; private set; }
    public Guid? ReviewerId { get; private set; }

    protected PerformanceReview() { } // For EF Core

    public PerformanceReview(
        DateTime reviewDate,
        PerformanceRating rating,
        string? reviewComments = null,
        string? goals = null)
    {
        ReviewDate = reviewDate;
        Rating = rating;
        ReviewComments = reviewComments;
        Goals = goals;

        // Default review period to previous year
        ReviewPeriodEnd = reviewDate;
        ReviewPeriodStart = reviewDate.AddYears(-1);
    }

    public void SetReviewPeriod(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");

        ReviewPeriodStart = startDate;
        ReviewPeriodEnd = endDate;
        MarkAsModified();
    }

    public void UpdateReview(
        PerformanceRating rating,
        string? reviewComments = null,
        string? achievements = null,
        string? areasForImprovement = null)
    {
        Rating = rating;
        ReviewComments = reviewComments;
        Achievements = achievements;
        AreasForImprovement = areasForImprovement;
        MarkAsModified();
    }

    public void AddReviewerComments(string reviewerComments, Guid reviewerId)
    {
        ReviewerComments = reviewerComments;
        ReviewerId = reviewerId;
        MarkAsModified();
    }
}
