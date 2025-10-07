using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.HR;
using System.Text.Json;

namespace TossErp.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.MiddleName)
            .HasMaxLength(100);
        
        builder.Property(e => e.EmployeeNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(e => e.EmployeeNumber)
            .IsUnique();
        
        builder.Property(e => e.IdNumber)
            .HasMaxLength(50);
        
        builder.Property(e => e.Gender)
            .HasMaxLength(20);
        
        builder.Property(e => e.Nationality)
            .HasMaxLength(100);
        
        builder.Property(e => e.Email)
            .HasMaxLength(100);
        
        builder.HasIndex(e => e.Email)
            .HasFilter("[Email] IS NOT NULL");
        
        builder.Property(e => e.Phone)
            .HasMaxLength(20);
        
        builder.Property(e => e.AlternatePhone)
            .HasMaxLength(20);
        
        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(e => e.DepartmentName)
            .HasMaxLength(100);
        
        builder.Property(e => e.JobTitle)
            .HasMaxLength(100);
        
        builder.Property(e => e.JobDescription)
            .HasMaxLength(1000);
        
        builder.Property(e => e.TerminationReason)
            .HasMaxLength(500);
        
        builder.Property(e => e.Salary)
            .HasPrecision(18, 2);
        
        builder.Property(e => e.SalaryFrequency)
            .HasMaxLength(50);
        
        builder.Property(e => e.PaymentMethod)
            .HasMaxLength(50);
        
        builder.Property(e => e.BankName)
            .HasMaxLength(200);
        
        builder.Property(e => e.BankAccountNumber)
            .HasMaxLength(50);
        
        builder.Property(e => e.AnnualLeaveDays)
            .HasPrecision(10, 2);
        
        builder.Property(e => e.SickLeaveDays)
            .HasPrecision(10, 2);
        
        builder.Property(e => e.UsedAnnualLeave)
            .HasPrecision(10, 2);
        
        builder.Property(e => e.UsedSickLeave)
            .HasPrecision(10, 2);
        
        builder.Property(e => e.EmergencyContactName)
            .HasMaxLength(200);
        
        builder.Property(e => e.EmergencyContactPhone)
            .HasMaxLength(20);
        
        builder.Property(e => e.EmergencyContactRelationship)
            .HasMaxLength(50);
        
        builder.Property(e => e.ManagerName)
            .HasMaxLength(200);
        
        builder.Property(e => e.ProfilePhotoUrl)
            .HasMaxLength(500);
        
        // Store Documents as JSON
        builder.Property(e => e.Documents)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>()
            )
            .HasColumnType("nvarchar(max)");
        
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.DepartmentId);
        
        // Computed properties
        builder.Ignore(e => e.FullName);
        builder.Ignore(e => e.RemainingAnnualLeave);
        builder.Ignore(e => e.RemainingSickLeave);
        
        // Relationships
        builder.HasMany(e => e.LeaveRequests)
            .WithOne(l => l.Employee)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.AttendanceRecords)
            .WithOne(a => a.Employee)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.LeaveType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(l => l.Days)
            .HasPrecision(10, 2);
        
        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(l => l.Reason)
            .HasMaxLength(1000);
        
        builder.Property(l => l.RejectionReason)
            .HasMaxLength(500);
        
        builder.Property(l => l.ApprovedByName)
            .HasMaxLength(200);
        
        builder.HasIndex(l => l.EmployeeId);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.StartDate);
    }
}

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.HoursWorked)
            .HasPrecision(10, 2);
        
        builder.Property(a => a.OvertimeHours)
            .HasPrecision(10, 2);
        
        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(a => a.Notes)
            .HasMaxLength(500);
        
        builder.HasIndex(a => a.EmployeeId);
        builder.HasIndex(a => a.Date);
        builder.HasIndex(a => new { a.EmployeeId, a.Date })
            .IsUnique();
    }
}

