using TossErp.HumanResources.Domain.SeedWork;
using TossErp.HumanResources.Domain.ValueObjects;
using TossErp.HumanResources.Domain.Enums;

namespace TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate.Events;

/// <summary>
/// Employee Created Domain Event
/// </summary>
public record EmployeeCreatedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    EmployeeNumber EmployeeNumber,
    string FirstName,
    string LastName,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Status Changed Domain Event
/// </summary>
public record EmployeeStatusChangedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    EmployeeStatus PreviousStatus,
    EmployeeStatus NewStatus,
    string? Reason,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Department Changed Domain Event
/// </summary>
public record EmployeeDepartmentChangedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    string? PreviousDepartment,
    string NewDepartment,
    DateTime EffectiveDate,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Job Title Changed Domain Event
/// </summary>
public record EmployeeJobTitleChangedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    string? PreviousJobTitle,
    string NewJobTitle,
    DateTime EffectiveDate,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Salary Changed Domain Event
/// </summary>
public record EmployeeSalaryChangedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Salary PreviousSalary,
    Salary NewSalary,
    DateTime EffectiveDate,
    string? Reason,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Manager Changed Domain Event
/// </summary>
public record EmployeeManagerChangedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Guid? PreviousManagerId,
    Guid? NewManagerId,
    DateTime EffectiveDate,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Work Hours Changed Domain Event
/// </summary>
public record EmployeeWorkHoursChangedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    WorkHours PreviousWorkHours,
    WorkHours NewWorkHours,
    DateTime EffectiveDate,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Performance Review Completed Domain Event
/// </summary>
public record EmployeePerformanceReviewCompletedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Guid ReviewId,
    PerformanceRating Rating,
    DateTime ReviewDate,
    Guid? ReviewerId,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Leave Balance Updated Domain Event
/// </summary>
public record EmployeeLeaveBalanceUpdatedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    int PreviousBalance,
    int NewBalance,
    string LeaveType,
    string? Reason,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Contact Information Updated Domain Event
/// </summary>
public record EmployeeContactUpdatedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    PhoneNumber? PhoneNumber,
    EmailAddress? Email,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Emergency Contact Added Domain Event
/// </summary>
public record EmployeeEmergencyContactAddedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Guid ContactId,
    string ContactName,
    string Relationship,
    PhoneNumber PhoneNumber,
    bool IsPrimary,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Qualification Added Domain Event
/// </summary>
public record EmployeeQualificationAddedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Guid QualificationId,
    string QualificationName,
    string Institution,
    int YearOfCompletion,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Skill Added Domain Event
/// </summary>
public record EmployeeSkillAddedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Guid SkillId,
    string SkillName,
    string? ProficiencyLevel,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Document Added Domain Event
/// </summary>
public record EmployeeDocumentAddedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Guid DocumentId,
    string DocumentType,
    string DocumentName,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Document Verified Domain Event
/// </summary>
public record EmployeeDocumentVerifiedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    Guid DocumentId,
    string DocumentType,
    DateTime VerifiedOn,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Training Completed Domain Event
/// </summary>
public record EmployeeTrainingCompletedDomainEvent(
    Guid Id,
    Guid EmployeeId,
    string TrainingName,
    DateTime CompletionDate,
    string? CertificateNumber,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Birthday Domain Event (for automated notifications)
/// </summary>
public record EmployeeBirthdayDomainEvent(
    Guid Id,
    Guid EmployeeId,
    string FirstName,
    string LastName,
    DateTime BirthDate,
    int Age,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Employee Work Anniversary Domain Event (for automated notifications)
/// </summary>
public record EmployeeWorkAnniversaryDomainEvent(
    Guid Id,
    Guid EmployeeId,
    string FirstName,
    string LastName,
    DateTime HireDate,
    int YearsOfService,
    DateTime OccurredOn) : IDomainEvent;
