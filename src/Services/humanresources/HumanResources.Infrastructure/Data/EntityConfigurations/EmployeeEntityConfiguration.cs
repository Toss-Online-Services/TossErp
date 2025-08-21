using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate.Entities;
using TossErp.HumanResources.Domain.ValueObjects;
using TossErp.HumanResources.Domain.Enums;

namespace TossErp.HumanResources.Infrastructure.Data.EntityConfigurations;

public class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(e => e.Id);

        builder.Ignore(e => e.DomainEvents);

        // Employee Number - Value Object
        builder.OwnsOne(e => e.EmployeeNumber, en =>
        {
            en.Property(e => e.Value)
                .HasColumnName("EmployeeNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.HasIndex(e => e.EmployeeNumber)
            .IsUnique()
            .HasDatabaseName("IX_Employee_EmployeeNumber");

        // Personal Information
        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.DateOfBirth)
            .IsRequired();

        builder.Property(e => e.Gender)
            .HasConversion<int>()
            .IsRequired();

        // Contact Information - Value Objects
        builder.OwnsOne(e => e.PhoneNumber, pn =>
        {
            pn.Property(e => e.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(e => e.Email, em =>
        {
            em.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder.OwnsOne(e => e.Address, addr =>
        {
            addr.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .HasMaxLength(255);
            addr.Property(a => a.City)
                .HasColumnName("AddressCity")
                .HasMaxLength(100);
            addr.Property(a => a.State)
                .HasColumnName("AddressState")
                .HasMaxLength(100);
            addr.Property(a => a.PostalCode)
                .HasColumnName("AddressPostalCode")
                .HasMaxLength(20);
            addr.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .HasMaxLength(100);
        });

        // Employment Information
        builder.Property(e => e.Department)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.JobTitle)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.HireDate)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.ManagerId);

        // Salary - Value Object
        builder.OwnsOne(e => e.Salary, s =>
        {
            s.Property(s => s.Amount)
                .HasColumnName("SalaryAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            s.Property(s => s.Currency)
                .HasColumnName("SalaryCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Work Hours - Value Object
        builder.OwnsOne(e => e.WorkHours, wh =>
        {
            wh.Property(w => w.HoursPerDay)
                .HasColumnName("HoursPerDay")
                .IsRequired();
            wh.Property(w => w.DaysPerWeek)
                .HasColumnName("DaysPerWeek")
                .IsRequired();
        });

        // Leave Balances
        builder.Property(e => e.AnnualLeaveBalance)
            .IsRequired();

        builder.Property(e => e.SickLeaveBalance)
            .IsRequired();

        // Audit Fields
        builder.Property(e => e.CreatedDate)
            .IsRequired();

        builder.Property(e => e.LastModifiedDate);

        // Child entities - One-to-Many relationships
        builder.HasMany(e => e.Contacts)
            .WithOne()
            .HasForeignKey("EmployeeId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Qualifications)
            .WithOne()
            .HasForeignKey("EmployeeId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Experiences)
            .WithOne()
            .HasForeignKey("EmployeeId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Skills)
            .WithOne()
            .HasForeignKey("EmployeeId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Documents)
            .WithOne()
            .HasForeignKey("EmployeeId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.SalaryHistories)
            .WithOne()
            .HasForeignKey("EmployeeId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.PerformanceReviews)
            .WithOne()
            .HasForeignKey("EmployeeId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmployeeContactEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeContact>
{
    public void Configure(EntityTypeBuilder<EmployeeContact> builder)
    {
        builder.ToTable("EmployeeContacts", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(ec => ec.Id);

        builder.Property<Guid>("EmployeeId").IsRequired();

        builder.Property(ec => ec.ContactName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ec => ec.Relationship)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(ec => ec.PhoneNumber, pn =>
        {
            pn.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(ec => ec.Email, em =>
        {
            em.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255);
        });

        builder.Property(ec => ec.IsPrimary)
            .IsRequired();
    }
}

public class EmployeeQualificationEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeQualification>
{
    public void Configure(EntityTypeBuilder<EmployeeQualification> builder)
    {
        builder.ToTable("EmployeeQualifications", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(eq => eq.Id);

        builder.Property<Guid>("EmployeeId").IsRequired();

        builder.Property(eq => eq.QualificationName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(eq => eq.Institution)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(eq => eq.YearOfCompletion)
            .IsRequired();

        builder.Property(eq => eq.Grade)
            .HasMaxLength(50);

        builder.Property(eq => eq.FieldOfStudy)
            .HasMaxLength(200);

        builder.Property(eq => eq.IsVerified)
            .IsRequired();
    }
}

public class EmployeeExperienceEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeExperience>
{
    public void Configure(EntityTypeBuilder<EmployeeExperience> builder)
    {
        builder.ToTable("EmployeeExperiences", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(ee => ee.Id);

        builder.Property<Guid>("EmployeeId").IsRequired();

        builder.Property(ee => ee.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ee => ee.JobTitle)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ee => ee.StartDate)
            .IsRequired();

        builder.Property(ee => ee.EndDate);

        builder.Property(ee => ee.Description)
            .HasMaxLength(1000);

        builder.Property(ee => ee.Achievements)
            .HasMaxLength(1000);

        builder.Property(ee => ee.IsCurrentJob)
            .IsRequired();

        builder.Ignore(ee => ee.Duration);
        builder.Ignore(ee => ee.DurationInYears);
    }
}

public class EmployeeSkillEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeSkill>
{
    public void Configure(EntityTypeBuilder<EmployeeSkill> builder)
    {
        builder.ToTable("EmployeeSkills", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(es => es.Id);

        builder.Property<Guid>("EmployeeId").IsRequired();

        builder.Property(es => es.SkillName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(es => es.ProficiencyLevel)
            .HasMaxLength(50);

        builder.Property(es => es.CertificationDate);

        builder.Property(es => es.ExpiryDate);

        builder.Property(es => es.IsVerified)
            .IsRequired();

        builder.Ignore(es => es.IsExpired);
    }
}

public class EmployeeDocumentEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeDocument>
{
    public void Configure(EntityTypeBuilder<EmployeeDocument> builder)
    {
        builder.ToTable("EmployeeDocuments", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(ed => ed.Id);

        builder.Property<Guid>("EmployeeId").IsRequired();

        builder.Property(ed => ed.DocumentType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ed => ed.DocumentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ed => ed.DocumentNumber)
            .HasMaxLength(100);

        builder.Property(ed => ed.FilePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ed => ed.UploadDate)
            .IsRequired();

        builder.Property(ed => ed.ExpiryDate);

        builder.Property(ed => ed.IsVerified)
            .IsRequired();

        builder.Ignore(ed => ed.IsExpired);
    }
}

public class SalaryHistoryEntityTypeConfiguration : IEntityTypeConfiguration<SalaryHistory>
{
    public void Configure(EntityTypeBuilder<SalaryHistory> builder)
    {
        builder.ToTable("SalaryHistories", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(sh => sh.Id);

        builder.Property<Guid>("EmployeeId").IsRequired();

        builder.OwnsOne(sh => sh.PreviousSalary, ps =>
        {
            ps.Property(s => s.Amount)
                .HasColumnName("PreviousSalaryAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            ps.Property(s => s.Currency)
                .HasColumnName("PreviousSalaryCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(sh => sh.NewSalary, ns =>
        {
            ns.Property(s => s.Amount)
                .HasColumnName("NewSalaryAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            ns.Property(s => s.Currency)
                .HasColumnName("NewSalaryCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(sh => sh.EffectiveDate)
            .IsRequired();

        builder.Property(sh => sh.Reason)
            .HasMaxLength(500);

        builder.Property(sh => sh.PercentageIncrease)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Ignore(sh => sh.IsIncrease);
        builder.Ignore(sh => sh.IsDecrease);
        builder.Ignore(sh => sh.AmountChange);
    }
}

public class PerformanceReviewEntityTypeConfiguration : IEntityTypeConfiguration<PerformanceReview>
{
    public void Configure(EntityTypeBuilder<PerformanceReview> builder)
    {
        builder.ToTable("PerformanceReviews", HumanResourcesContext.DEFAULT_SCHEMA);

        builder.HasKey(pr => pr.Id);

        builder.Property<Guid>("EmployeeId").IsRequired();

        builder.Property(pr => pr.ReviewDate)
            .IsRequired();

        builder.Property(pr => pr.ReviewPeriodStart)
            .IsRequired();

        builder.Property(pr => pr.ReviewPeriodEnd)
            .IsRequired();

        builder.Property(pr => pr.Rating)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(pr => pr.ReviewComments)
            .HasMaxLength(2000);

        builder.Property(pr => pr.Goals)
            .HasMaxLength(2000);

        builder.Property(pr => pr.Achievements)
            .HasMaxLength(2000);

        builder.Property(pr => pr.AreasForImprovement)
            .HasMaxLength(2000);

        builder.Property(pr => pr.ReviewerComments)
            .HasMaxLength(2000);

        builder.Property(pr => pr.ReviewerId);
    }
}
