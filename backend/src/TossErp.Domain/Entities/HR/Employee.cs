using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.HR;

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Intern,
    Casual
}

public enum EmploymentStatus
{
    Active,
    OnLeave,
    Suspended,
    Terminated,
    Retired
}

public class Employee : BaseEntity
{
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();
    
    public string EmployeeNumber { get; set; } = string.Empty;
    public string? IdNumber { get; set; } // National ID
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; } = "South Africa";
    
    // Contact Information
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AlternatePhone { get; set; }
    
    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "South Africa";
    
    // Employment Details
    public EmploymentType Type { get; set; } = EmploymentType.FullTime;
    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    
    public string? JobTitle { get; set; }
    public string? JobDescription { get; set; }
    
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? TerminationReason { get; set; }
    
    // Compensation
    public decimal Salary { get; set; }
    public string? SalaryFrequency { get; set; } // Monthly, Bi-Weekly, etc.
    public string? PaymentMethod { get; set; } // Bank Transfer, Cash, etc.
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    
    // Leave Balances
    public decimal AnnualLeaveDays { get; set; }
    public decimal SickLeaveDays { get; set; }
    public decimal UsedAnnualLeave { get; set; }
    public decimal UsedSickLeave { get; set; }
    public decimal RemainingAnnualLeave => AnnualLeaveDays - UsedAnnualLeave;
    public decimal RemainingSickLeave => SickLeaveDays - UsedSickLeave;
    
    // Emergency Contact
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    
    // Manager
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    
    // Documents and Photos
    public string? ProfilePhotoUrl { get; set; }
    public List<string> Documents { get; set; } = new(); // ID, Certificates, etc.
    
    // Navigation properties
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    
    // Business logic
    public void Terminate(string reason, DateTime terminationDate)
    {
        if (Status == EmploymentStatus.Terminated)
            throw new InvalidOperationException("Employee is already terminated");
        
        Status = EmploymentStatus.Terminated;
        TerminationDate = terminationDate;
        TerminationReason = reason;
        
        AddDomainEvent(new EmployeeTerminatedEvent(Id, FullName, reason));
    }
    
    public void RequestLeave(decimal days, string leaveType)
    {
        if (leaveType == "Annual")
        {
            if (days > RemainingAnnualLeave)
                throw new InvalidOperationException($"Insufficient annual leave balance. Available: {RemainingAnnualLeave}");
        }
        else if (leaveType == "Sick")
        {
            if (days > RemainingSickLeave)
                throw new InvalidOperationException($"Insufficient sick leave balance. Available: {RemainingSickLeave}");
        }
        
        AddDomainEvent(new LeaveRequestedEvent(Id, FullName, days, leaveType));
    }
}

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;
    
    public string LeaveType { get; set; } = string.Empty; // Annual, Sick, Maternity, etc.
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Days { get; set; }
    
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? Reason { get; set; }
    public string? RejectionReason { get; set; }
    
    public int? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
}

public class AttendanceRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;
    
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    
    public decimal? HoursWorked { get; set; }
    public decimal? OvertimeHours { get; set; }
    
    public string Status { get; set; } = string.Empty; // Present, Absent, Late, Half-Day
    public string? Notes { get; set; }
}

// Domain Events
public class EmployeeTerminatedEvent : DomainEvent
{
    public int EmployeeId { get; }
    public string EmployeeName { get; }
    public string Reason { get; }
    
    public EmployeeTerminatedEvent(int employeeId, string employeeName, string reason)
    {
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        Reason = reason;
    }
}

public class LeaveRequestedEvent : DomainEvent
{
    public int EmployeeId { get; }
    public string EmployeeName { get; }
    public decimal Days { get; }
    public string LeaveType { get; }
    
    public LeaveRequestedEvent(int employeeId, string employeeName, decimal days, string leaveType)
    {
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        Days = days;
        LeaveType = leaveType;
    }
}

