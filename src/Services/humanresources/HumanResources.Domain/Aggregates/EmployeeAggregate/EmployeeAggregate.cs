using TossErp.HumanResources.Domain.SeedWork;
using TossErp.HumanResources.Domain.ValueObjects;
using TossErp.HumanResources.Domain.Enums;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate.Entities;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate.Events;

namespace TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;

/// <summary>
/// Employee Aggregate Root
/// Manages all employee-related business logic and lifecycle
/// </summary>
public class EmployeeAggregate : Entity, IAggregateRoot
{
    // Core Identity Properties
    public EmployeeNumber EmployeeNumber { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public EmailAddress? PersonalEmail { get; private set; }
    public EmailAddress? CompanyEmail { get; private set; }
    public PhoneNumber? PersonalPhone { get; private set; }
    public PhoneNumber? CompanyPhone { get; private set; }
    
    // Personal Information
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public MaritalStatus MaritalStatus { get; private set; }
    public string? Nationality { get; private set; }
    public string? NationalId { get; private set; }
    public string? PassportNumber { get; private set; }
    public Address? HomeAddress { get; private set; }
    public Address? OfficeAddress { get; private set; }
    
    // Employment Information
    public string JobTitle { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public string? Division { get; private set; }
    public string? Team { get; private set; }
    public EmploymentType EmploymentType { get; private set; }
    public EmploymentStatus Status { get; private set; }
    public DateTime JoiningDate { get; private set; }
    public DateTime? ConfirmationDate { get; private set; }
    public DateTime? ResignationDate { get; private set; }
    public DateTime? RelievingDate { get; private set; }
    public string Company { get; private set; } = string.Empty;
    public string? Branch { get; private set; }
    
    // Reporting Structure
    public Guid? ReportsToEmployeeId { get; private set; }
    public string? ReportsToEmployeeNumber { get; private set; }
    public int Level { get; private set; }
    
    // Compensation
    public Salary? BaseSalary { get; private set; }
    public PayrollFrequency PayrollFrequency { get; private set; }
    public string? PayrollNumber { get; private set; }
    public string? BankAccount { get; private set; }
    public string? BankName { get; private set; }
    
    // Work Configuration
    public WorkHours StandardWorkHours { get; private set; } = new(8); // Default 8 hours
    public string? WorkLocation { get; private set; }
    public bool IsRemoteWorker { get; private set; }
    public string? ShiftType { get; private set; }
    
    // Leave Balances
    public decimal AnnualLeaveBalance { get; private set; }
    public decimal SickLeaveBalance { get; private set; }
    public decimal CompensatoryLeaveBalance { get; private set; }
    
    // Child Collections
    private readonly List<EmployeeContact> _emergencyContacts = new();
    private readonly List<EmployeeQualification> _qualifications = new();
    private readonly List<EmployeeExperience> _workExperience = new();
    private readonly List<EmployeeSkill> _skills = new();
    private readonly List<EmployeeDocument> _documents = new();
    private readonly List<SalaryHistory> _salaryHistory = new();
    private readonly List<PerformanceReview> _performanceReviews = new();
    
    // Navigation Properties
    public IReadOnlyCollection<EmployeeContact> EmergencyContacts => _emergencyContacts.AsReadOnly();
    public IReadOnlyCollection<EmployeeQualification> Qualifications => _qualifications.AsReadOnly();
    public IReadOnlyCollection<EmployeeExperience> WorkExperience => _workExperience.AsReadOnly();
    public IReadOnlyCollection<EmployeeSkill> Skills => _skills.AsReadOnly();
    public IReadOnlyCollection<EmployeeDocument> Documents => _documents.AsReadOnly();
    public IReadOnlyCollection<SalaryHistory> SalaryHistory => _salaryHistory.AsReadOnly();
    public IReadOnlyCollection<PerformanceReview> PerformanceReviews => _performanceReviews.AsReadOnly();
    
    // Computed Properties
    public string FullName => string.IsNullOrWhiteSpace(MiddleName) 
        ? $"{FirstName} {LastName}" 
        : $"{FirstName} {MiddleName} {LastName}";
    
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - 
                     (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    
    public bool IsActive => Status == EmploymentStatus.Active;
    public bool IsOnProbation => ConfirmationDate == null && JoiningDate.AddMonths(6) > DateTime.UtcNow;
    public int ServiceYears => DateTime.UtcNow.Year - JoiningDate.Year;
    
    protected EmployeeAggregate() { } // For EF Core

    public EmployeeAggregate(
        EmployeeNumber employeeNumber,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        Gender gender,
        string jobTitle,
        string department,
        EmploymentType employmentType,
        DateTime joiningDate,
        string company)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new ArgumentException("Job title cannot be empty", nameof(jobTitle));
        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("Department cannot be empty", nameof(department));
        if (string.IsNullOrWhiteSpace(company))
            throw new ArgumentException("Company cannot be empty", nameof(company));
        if (dateOfBirth > DateTime.UtcNow.AddYears(-18))
            throw new ArgumentException("Employee must be at least 18 years old", nameof(dateOfBirth));
        if (joiningDate > DateTime.UtcNow.AddDays(30))
            throw new ArgumentException("Joining date cannot be more than 30 days in the future", nameof(joiningDate));

        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        JobTitle = jobTitle;
        Department = department;
        EmploymentType = employmentType;
        JoiningDate = joiningDate;
        Company = company;
        Status = EmploymentStatus.Active;
        MaritalStatus = MaritalStatus.Single;
        PayrollFrequency = PayrollFrequency.Monthly;
        Level = 1;
        AnnualLeaveBalance = 21; // Default 21 days
        SickLeaveBalance = 10;   // Default 10 days
        CompensatoryLeaveBalance = 0;

        AddDomainEvent(new EmployeeCreatedEvent(this));
    }

    // Personal Information Management
    public void UpdatePersonalInfo(
        string firstName,
        string lastName,
        string? middleName = null,
        MaritalStatus? maritalStatus = null,
        string? nationality = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        
        if (maritalStatus.HasValue)
            MaritalStatus = maritalStatus.Value;
            
        if (!string.IsNullOrWhiteSpace(nationality))
            Nationality = nationality;

        MarkAsModified();
        AddDomainEvent(new EmployeePersonalInfoUpdatedEvent(this));
    }

    public void UpdateContactInfo(
        EmailAddress? personalEmail = null,
        EmailAddress? companyEmail = null,
        PhoneNumber? personalPhone = null,
        PhoneNumber? companyPhone = null)
    {
        if (personalEmail != null)
            PersonalEmail = personalEmail;
        if (companyEmail != null)
            CompanyEmail = companyEmail;
        if (personalPhone != null)
            PersonalPhone = personalPhone;
        if (companyPhone != null)
            CompanyPhone = companyPhone;

        MarkAsModified();
        AddDomainEvent(new EmployeeContactInfoUpdatedEvent(this));
    }

    public void UpdateAddress(Address? homeAddress = null, Address? officeAddress = null)
    {
        if (homeAddress != null)
            HomeAddress = homeAddress;
        if (officeAddress != null)
            OfficeAddress = officeAddress;

        MarkAsModified();
        AddDomainEvent(new EmployeeAddressUpdatedEvent(this));
    }

    // Employment Management
    public void UpdateJobInfo(
        string? jobTitle = null,
        string? department = null,
        string? division = null,
        string? team = null,
        Guid? reportsToEmployeeId = null)
    {
        if (!string.IsNullOrWhiteSpace(jobTitle))
            JobTitle = jobTitle;
        if (!string.IsNullOrWhiteSpace(department))
            Department = department;
        if (!string.IsNullOrWhiteSpace(division))
            Division = division;
        if (!string.IsNullOrWhiteSpace(team))
            Team = team;
        if (reportsToEmployeeId.HasValue)
            ReportsToEmployeeId = reportsToEmployeeId.Value;

        MarkAsModified();
        AddDomainEvent(new EmployeeJobInfoUpdatedEvent(this));
    }

    public void ConfirmEmployment(DateTime confirmationDate)
    {
        if (confirmationDate < JoiningDate)
            throw new ArgumentException("Confirmation date cannot be before joining date", nameof(confirmationDate));

        if (ConfirmationDate.HasValue)
            throw new InvalidOperationException("Employee is already confirmed");

        ConfirmationDate = confirmationDate;
        MarkAsModified();

        AddDomainEvent(new EmployeeConfirmedEvent(this));
    }

    public void UpdateSalary(Salary newSalary, string? reason = null)
    {
        if (BaseSalary != null)
        {
            // Record salary history
            var salaryHistory = new SalaryHistory(BaseSalary, newSalary, DateTime.UtcNow, reason);
            _salaryHistory.Add(salaryHistory);
        }

        BaseSalary = newSalary;
        MarkAsModified();

        AddDomainEvent(new EmployeeSalaryUpdatedEvent(this, newSalary));
    }

    // Status Management
    public void Suspend(string reason, DateTime? suspensionEndDate = null)
    {
        if (Status != EmploymentStatus.Active)
            throw new InvalidOperationException("Only active employees can be suspended");

        Status = EmploymentStatus.Suspended;
        MarkAsModified();

        AddDomainEvent(new EmployeeSuspendedEvent(this, reason, suspensionEndDate));
    }

    public void Reinstate()
    {
        if (Status != EmploymentStatus.Suspended)
            throw new InvalidOperationException("Only suspended employees can be reinstated");

        Status = EmploymentStatus.Active;
        MarkAsModified();

        AddDomainEvent(new EmployeeReinstatedEvent(this));
    }

    public void Resign(DateTime resignationDate, DateTime relievingDate, string? reason = null)
    {
        if (Status != EmploymentStatus.Active)
            throw new InvalidOperationException("Only active employees can resign");

        if (resignationDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Resignation date cannot be in the past", nameof(resignationDate));

        if (relievingDate < resignationDate)
            throw new ArgumentException("Relieving date cannot be before resignation date", nameof(relievingDate));

        ResignationDate = resignationDate;
        RelievingDate = relievingDate;
        Status = EmploymentStatus.Left;
        MarkAsModified();

        AddDomainEvent(new EmployeeResignedEvent(this, reason));
    }

    public void Terminate(DateTime terminationDate, string reason)
    {
        if (Status == EmploymentStatus.Terminated)
            throw new InvalidOperationException("Employee is already terminated");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Termination reason is required", nameof(reason));

        RelievingDate = terminationDate;
        Status = EmploymentStatus.Terminated;
        MarkAsModified();

        AddDomainEvent(new EmployeeTerminatedEvent(this, reason));
    }

    // Leave Balance Management
    public void UpdateLeaveBalance(LeaveType leaveType, decimal balance)
    {
        if (balance < 0)
            throw new ArgumentException("Leave balance cannot be negative", nameof(balance));

        switch (leaveType)
        {
            case LeaveType.Annual:
                AnnualLeaveBalance = balance;
                break;
            case LeaveType.Sick:
                SickLeaveBalance = balance;
                break;
            case LeaveType.CompensatoryOff:
                CompensatoryLeaveBalance = balance;
                break;
            default:
                throw new ArgumentException($"Leave type {leaveType} is not supported for balance updates", nameof(leaveType));
        }

        MarkAsModified();
        AddDomainEvent(new EmployeeLeaveBalanceUpdatedEvent(this, leaveType, balance));
    }

    public void DeductLeave(LeaveType leaveType, decimal days)
    {
        if (days <= 0)
            throw new ArgumentException("Leave deduction must be positive", nameof(days));

        switch (leaveType)
        {
            case LeaveType.Annual:
                if (AnnualLeaveBalance < days)
                    throw new InvalidOperationException("Insufficient annual leave balance");
                AnnualLeaveBalance -= days;
                break;
            case LeaveType.Sick:
                if (SickLeaveBalance < days)
                    throw new InvalidOperationException("Insufficient sick leave balance");
                SickLeaveBalance -= days;
                break;
            case LeaveType.CompensatoryOff:
                if (CompensatoryLeaveBalance < days)
                    throw new InvalidOperationException("Insufficient compensatory leave balance");
                CompensatoryLeaveBalance -= days;
                break;
            default:
                // For other leave types, we don't check balance
                break;
        }

        MarkAsModified();
        AddDomainEvent(new EmployeeLeaveDeductedEvent(this, leaveType, days));
    }

    // Child Entity Management
    public void AddEmergencyContact(
        string contactName,
        string relationship,
        PhoneNumber phoneNumber,
        EmailAddress? email = null)
    {
        var contact = new EmployeeContact(contactName, relationship, phoneNumber, email);
        _emergencyContacts.Add(contact);
        MarkAsModified();

        AddDomainEvent(new EmployeeEmergencyContactAddedEvent(this, contact));
    }

    public void AddQualification(
        string qualificationName,
        string institution,
        int yearOfCompletion,
        string? grade = null)
    {
        var qualification = new EmployeeQualification(qualificationName, institution, yearOfCompletion, grade);
        _qualifications.Add(qualification);
        MarkAsModified();

        AddDomainEvent(new EmployeeQualificationAddedEvent(this, qualification));
    }

    public void AddWorkExperience(
        string companyName,
        string jobTitle,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null)
    {
        var experience = new EmployeeExperience(companyName, jobTitle, startDate, endDate, description);
        _workExperience.Add(experience);
        MarkAsModified();

        AddDomainEvent(new EmployeeExperienceAddedEvent(this, experience));
    }

    public void AddSkill(string skillName, string? proficiencyLevel = null)
    {
        if (_skills.Any(s => s.SkillName.Equals(skillName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Skill '{skillName}' already exists for this employee");

        var skill = new EmployeeSkill(skillName, proficiencyLevel);
        _skills.Add(skill);
        MarkAsModified();

        AddDomainEvent(new EmployeeSkillAddedEvent(this, skill));
    }

    public void AddPerformanceReview(
        DateTime reviewDate,
        PerformanceRating rating,
        string? reviewComments = null,
        string? goals = null)
    {
        var review = new PerformanceReview(reviewDate, rating, reviewComments, goals);
        _performanceReviews.Add(review);
        MarkAsModified();

        AddDomainEvent(new EmployeePerformanceReviewAddedEvent(this, review));
    }

    // Business Rules
    public bool IsEligibleForLeave(LeaveType leaveType, decimal days)
    {
        return leaveType switch
        {
            LeaveType.Annual => AnnualLeaveBalance >= days,
            LeaveType.Sick => SickLeaveBalance >= days,
            LeaveType.CompensatoryOff => CompensatoryLeaveBalance >= days,
            _ => true // Other leave types don't have balance restrictions
        };
    }

    public bool IsEligibleForPromotion()
    {
        return IsActive && 
               ConfirmationDate.HasValue && 
               ServiceYears >= 1 &&
               _performanceReviews.Any(r => r.Rating >= PerformanceRating.MeetsExpectations);
    }

    public bool RequiresConfirmation()
    {
        return !ConfirmationDate.HasValue && 
               JoiningDate.AddMonths(6) <= DateTime.UtcNow;
    }

    public decimal GetLeaveBalance(LeaveType leaveType)
    {
        return leaveType switch
        {
            LeaveType.Annual => AnnualLeaveBalance,
            LeaveType.Sick => SickLeaveBalance,
            LeaveType.CompensatoryOff => CompensatoryLeaveBalance,
            _ => 0
        };
    }
}
