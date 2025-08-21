using Microsoft.EntityFrameworkCore;
using Financial.Domain.Entities;

namespace Financial.Infrastructure.Data;

/// <summary>
/// Entity Framework context for Financial Services module
/// </summary>
public class FinancialDbContext : DbContext
{
    public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options)
    {
    }

    // DbSets for Financial Services entities
    public DbSet<MicroLoan> MicroLoans => Set<MicroLoan>();
    public DbSet<LoanPayment> LoanPayments => Set<LoanPayment>();
    public DbSet<LoanReview> LoanReviews => Set<LoanReview>();
    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
    public DbSet<InsuranceClaim> InsuranceClaims => Set<InsuranceClaim>();
    public DbSet<PolicyPayment> PolicyPayments => Set<PolicyPayment>();
    public DbSet<LinkedBankAccount> LinkedBankAccounts => Set<LinkedBankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema
        modelBuilder.HasDefaultSchema("financial");

        // Configure entities
        ConfigureMicroLoan(modelBuilder);
        ConfigureLoanPayment(modelBuilder);
        ConfigureLoanReview(modelBuilder);
        ConfigureInsurancePolicy(modelBuilder);
        ConfigureInsuranceClaim(modelBuilder);
        ConfigurePolicyPayment(modelBuilder);
        ConfigureLinkedBankAccount(modelBuilder);
        ConfigureBankTransaction(modelBuilder);

        // Add global query filters for multi-tenancy
        modelBuilder.Entity<MicroLoan>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
        modelBuilder.Entity<LoanPayment>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
        modelBuilder.Entity<LoanReview>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
        modelBuilder.Entity<InsurancePolicy>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
        modelBuilder.Entity<InsuranceClaim>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
        modelBuilder.Entity<PolicyPayment>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
        modelBuilder.Entity<LinkedBankAccount>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
        modelBuilder.Entity<BankTransaction>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetCurrentTenantId());
    }

    private void ConfigureMicroLoan(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<MicroLoan>();
        
        entity.ToTable("MicroLoans");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.BorrowerName).IsRequired().HasMaxLength(200);
        entity.Property(e => e.BusinessName).HasMaxLength(200);
        entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2);
        entity.Property(e => e.InterestRate).HasPrecision(5, 4);
        entity.Property(e => e.MonthlyPayment).HasPrecision(18, 2);
        entity.Property(e => e.OutstandingBalance).HasPrecision(18, 2);
        entity.Property(e => e.Status).HasConversion<string>();
        
        entity.HasMany(e => e.Payments)
            .WithOne(p => p.Loan)
            .HasForeignKey(p => p.LoanId)
            .OnDelete(DeleteBehavior.Cascade);
            
        entity.HasMany(e => e.Reviews)
            .WithOne(r => r.Loan)
            .HasForeignKey(r => r.LoanId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureLoanPayment(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<LoanPayment>();
        
        entity.ToTable("LoanPayments");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Amount).HasPrecision(18, 2);
        entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2);
        entity.Property(e => e.InterestAmount).HasPrecision(18, 2);
        entity.Property(e => e.PenaltyAmount).HasPrecision(18, 2);
        entity.Property(e => e.RemainingBalance).HasPrecision(18, 2);
    }

    private void ConfigureLoanReview(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<LoanReview>();
        
        entity.ToTable("LoanReviews");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.ReviewerName).IsRequired().HasMaxLength(200);
        entity.Property(e => e.RecommendedAmount).HasPrecision(18, 2);
        entity.Property(e => e.RecommendedRate).HasPrecision(5, 4);
    }

    private void ConfigureInsurancePolicy(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<InsurancePolicy>();
        
        entity.ToTable("InsurancePolicies");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.PolicyNumber).IsRequired().HasMaxLength(100);
        entity.Property(e => e.PolicyName).IsRequired().HasMaxLength(200);
        entity.Property(e => e.CoverageAmount).HasPrecision(18, 2);
        entity.Property(e => e.PremiumAmount).HasPrecision(18, 2);
        entity.Property(e => e.Deductible).HasPrecision(18, 2);
        entity.Property(e => e.Type).HasConversion<string>();
        entity.Property(e => e.Status).HasConversion<string>();
        
        entity.HasMany(e => e.Claims)
            .WithOne(c => c.Policy)
            .HasForeignKey(c => c.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        entity.HasMany(e => e.Payments)
            .WithOne(p => p.Policy)
            .HasForeignKey(p => p.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureInsuranceClaim(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<InsuranceClaim>();
        
        entity.ToTable("InsuranceClaims");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.ClaimNumber).IsRequired().HasMaxLength(100);
        entity.Property(e => e.ClaimedAmount).HasPrecision(18, 2);
        entity.Property(e => e.ApprovedAmount).HasPrecision(18, 2);
        entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
        entity.Property(e => e.Deductible).HasPrecision(18, 2);
        entity.Property(e => e.Status).HasConversion<string>();
    }

    private void ConfigurePolicyPayment(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<PolicyPayment>();
        
        entity.ToTable("PolicyPayments");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Amount).HasPrecision(18, 2);
        entity.Property(e => e.LateFee).HasPrecision(18, 2);
    }

    private void ConfigureLinkedBankAccount(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<LinkedBankAccount>();
        
        entity.ToTable("LinkedBankAccounts");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
        entity.Property(e => e.AccountName).IsRequired().HasMaxLength(200);
        entity.Property(e => e.BankName).IsRequired().HasMaxLength(200);
        entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
        entity.Property(e => e.AvailableBalance).HasPrecision(18, 2);
        
        entity.HasMany(e => e.Transactions)
            .WithOne(t => t.LinkedBankAccount)
            .HasForeignKey(t => t.LinkedBankAccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureBankTransaction(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<BankTransaction>();
        
        entity.ToTable("BankTransactions");
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Amount).HasPrecision(18, 2);
        entity.Property(e => e.RunningBalance).HasPrecision(18, 2);
        entity.Property(e => e.Description).HasMaxLength(500);
    }

    private string GetCurrentTenantId()
    {
        // This would be implemented to get the current tenant ID from context
        // For now, return a placeholder
        return "default-tenant";
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Property("CreatedAt").CurrentValue == null)
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                if (entry.Property("CreatedBy").CurrentValue == null)
                    entry.Property("CreatedBy").CurrentValue = "system"; // Get from context
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                entry.Property("UpdatedBy").CurrentValue = "system"; // Get from context
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
