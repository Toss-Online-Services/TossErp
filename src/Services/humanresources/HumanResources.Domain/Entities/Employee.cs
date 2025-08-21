using HumanResources.Domain.Common;
using HumanResources.Domain.Enums;
using HumanResources.Domain.ValueObjects;

namespace HumanResources.Domain.Entities;

/// <summary>
/// Employee aggregate root representing an employee in the organization
/// Based on ERPNext Employee structure with DDD principles
/// </summary>
public class Employee : AggregateRoot<Guid>
{
    // Personal Information
    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string MiddleName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string NationalId { get; private set; } = string.Empty;
    public string PassportNumber { get; private set; } = string.Empty;
    
    // Employment Information
    public DateTime DateOfJoining { get; private set; }
    public DateTime? DateOfLeaving { get; private set; }
    public EmploymentStatus Status { get; private set; }
    public EmploymentType EmploymentType { get; private set; }
    public string Department { get; private set; } = string.Empty;
    public string Designation { get; private set; } = string.Empty;
    public string? ReportsTo { get; private set; } // Employee Number
    public Money Salary { get; private set; } = Money.Zero();
    public string Company { get; private set; } = string.Empty;
    public string Branch { get; private set; } = string.Empty;
    
    // Contact Information
    public Address CurrentAddress { get; private set; } = null!;
    public Address? PermanentAddress { get; private set; }
    public string EmergencyContactName { get; private set; } = string.Empty;
    public string EmergencyContactPhone { get; private set; } = string.Empty;
    public string EmergencyContactRelation { get; private set; } = string.Empty;
    
    // Leave Management
    private readonly List<LeaveApplication> _leaveApplications = new();
    public IReadOnlyCollection<LeaveApplication> LeaveApplications => _leaveApplications.AsReadOnly();
    
    // Attendance
    private readonly List<AttendanceRecord> _attendanceRecords = new();
    public IReadOnlyCollection<AttendanceRecord> AttendanceRecords => _attendanceRecords.AsReadOnly();

    protected Employee() : base() { } // For EF Core

    public Employee(Guid id, string employeeNumber, string firstName, string lastName,
        string email, DateTime dateOfBirth, Gender gender, DateTime dateOfJoining,
        EmploymentType employmentType, string department, string designation,
        string company, string tenantId) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException("Employee number cannot be empty", nameof(employeeNumber));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        EmployeeNumber = employeeNumber.Trim().ToUpper();
        FirstName = firstName.Trim();
        MiddleName = string.Empty;
        LastName = lastName.Trim();
        Email = email.Trim().ToLower();
        Phone = string.Empty;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        NationalId = string.Empty;
        PassportNumber = string.Empty;
        DateOfJoining = dateOfJoining;
        Status = EmploymentStatus.Active;
        EmploymentType = employmentType;
        Department = department.Trim();
        Designation = designation.Trim();
        Company = company.Trim();
        Branch = string.Empty;
    }

    public static Employee Create(string employeeNumber, string firstName, string lastName,
        string email, DateTime dateOfBirth, Gender gender, DateTime dateOfJoining,
        EmploymentType employmentType, string department, string designation,
        string company, string tenantId)
    {
        return new Employee(Guid.NewGuid(), employeeNumber, firstName, lastName, email,
            dateOfBirth, gender, dateOfJoining, employmentType, department, designation,
            company, tenantId);
    }

    public void UpdatePersonalInfo(string firstName, string middleName, string lastName,
        string phone, string nationalId, string passportNumber, string updatedBy)
    {
        FirstName = firstName.Trim();
        MiddleName = middleName.Trim();
        LastName = lastName.Trim();
        Phone = phone.Trim();
        NationalId = nationalId.Trim();
        PassportNumber = passportNumber.Trim();
        MarkAsUpdated(updatedBy);
    }

    public void UpdateEmploymentInfo(string department, string designation, string? reportsTo,
        Money salary, string branch, string updatedBy)
    {
        Department = department.Trim();
        Designation = designation.Trim();
        ReportsTo = reportsTo?.Trim();
        Salary = salary;
        Branch = branch.Trim();
        MarkAsUpdated(updatedBy);
    }

    public void SetCurrentAddress(Address address, string updatedBy)
    {
        CurrentAddress = address ?? throw new ArgumentNullException(nameof(address));
        MarkAsUpdated(updatedBy);
    }

    public void SetPermanentAddress(Address address, string updatedBy)
    {
        PermanentAddress = address;
        MarkAsUpdated(updatedBy);
    }

    public void SetEmergencyContact(string name, string phone, string relation, string updatedBy)
    {
        EmergencyContactName = name.Trim();
        EmergencyContactPhone = phone.Trim();
        EmergencyContactRelation = relation.Trim();
        MarkAsUpdated(updatedBy);
    }

    public void ApplyForLeave(DateTime fromDate, DateTime toDate, LeaveType leaveType,
        string reason, string appliedBy)
    {
        if (Status != EmploymentStatus.Active)
            throw new InvalidOperationException("Only active employees can apply for leave");

        if (fromDate > toDate)
            throw new ArgumentException("From date cannot be after to date");

        var application = new LeaveApplication(fromDate, toDate, leaveType, reason, appliedBy);
        _leaveApplications.Add(application);
        MarkAsUpdated(appliedBy);
    }

    public void RecordAttendance(DateTime date, DateTime checkIn, DateTime? checkOut,
        AttendanceStatus status, string recordedBy)
    {
        if (Status != EmploymentStatus.Active)
            throw new InvalidOperationException("Cannot record attendance for inactive employees");

        // Check if attendance already exists for this date
        var existingRecord = _attendanceRecords.FirstOrDefault(a => a.Date.Date == date.Date);
        if (existingRecord != null)
            throw new InvalidOperationException($"Attendance already recorded for {date:yyyy-MM-dd}");

        var attendanceRecord = new AttendanceRecord(date, checkIn, checkOut, status);
        _attendanceRecords.Add(attendanceRecord);
        MarkAsUpdated(recordedBy);
    }

    public void ChangeStatus(EmploymentStatus newStatus, string changedBy, string? reason = null)
    {
        if (newStatus == EmploymentStatus.Left && Status != EmploymentStatus.Active)
            throw new InvalidOperationException("Only active employees can be marked as left");

        Status = newStatus;
        
        if (newStatus == EmploymentStatus.Left)
        {
            DateOfLeaving = DateTime.UtcNow;
        }

        MarkAsUpdated(changedBy);
    }

    public void Terminate(DateTime terminationDate, string terminatedBy, string reason)
    {
        if (Status != EmploymentStatus.Active)
            throw new InvalidOperationException("Only active employees can be terminated");

        Status = EmploymentStatus.Left;
        DateOfLeaving = terminationDate;
        MarkAsUpdated(terminatedBy);
    }

    // Calculated properties
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();
    
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year -
        (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    
    public TimeSpan ServiceDuration => (DateOfLeaving ?? DateTime.UtcNow) - DateOfJoining;
    
    public bool IsActive => Status == EmploymentStatus.Active;
    
    public string DisplayName => $"{EmployeeNumber} - {FullName}";
}

/// <summary>
/// Leave Application value object
/// </summary>
public record LeaveApplication
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public LeaveType LeaveType { get; init; }
    public string Reason { get; init; } = string.Empty;
    public LeaveStatus Status { get; init; }
    public string AppliedBy { get; init; } = string.Empty;
    public DateTime AppliedAt { get; init; }
    public string? ApprovedBy { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public string? RejectionReason { get; init; }
    public int Days => (ToDate - FromDate).Days + 1;

    public LeaveApplication(DateTime fromDate, DateTime toDate, LeaveType leaveType,
        string reason, string appliedBy)
    {
        FromDate = fromDate;
        ToDate = toDate;
        LeaveType = leaveType;
        Reason = reason.Trim();
        Status = LeaveStatus.Pending;
        AppliedBy = appliedBy.Trim();
        AppliedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Attendance Record value object
/// </summary>
public record AttendanceRecord
{
    public DateTime Date { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime? CheckOut { get; init; }
    public AttendanceStatus Status { get; init; }
    public TimeSpan? WorkingHours => CheckOut.HasValue ? CheckOut - CheckIn : null;

    public AttendanceRecord(DateTime date, DateTime checkIn, DateTime? checkOut, AttendanceStatus status)
    {
        Date = date.Date;
        CheckIn = checkIn;
        CheckOut = checkOut;
        Status = status;
    }
}
