namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Chart of Accounts and Account Type entity configurations
/// </summary>
public class ChartOfAccountsConfiguration : IEntityTypeConfiguration<ChartOfAccount>
{
    public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        builder.ToTable("ChartOfAccounts", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Account Details
        builder.Property(e => e.AccountCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.AccountName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.AccountTypeId)
            .IsRequired();

        builder.Property(e => e.ParentAccountId);

        // Financial Properties
        builder.Property(e => e.NormalBalance)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.IsSystemAccount)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.AllowDirectPosting)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.AccountType)
            .WithMany(at => at.Accounts)
            .HasForeignKey(e => e.AccountTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ParentAccount)
            .WithMany(pa => pa.SubAccounts)
            .HasForeignKey(e => e.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.AccountCode })
            .IsUnique()
            .HasDatabaseName("IX_ChartOfAccounts_TenantId_AccountCode_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.AccountTypeId })
            .HasDatabaseName("IX_ChartOfAccounts_TenantId_AccountTypeId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.IsActive })
            .HasDatabaseName("IX_ChartOfAccounts_TenantId_IsActive")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.ParentAccountId })
            .HasDatabaseName("IX_ChartOfAccounts_TenantId_ParentAccountId")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Account Type entity configuration
/// </summary>
public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.ToTable("AccountTypes", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant (allows global account types)
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Type Details
        builder.Property(e => e.TypeCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.TypeName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.NormalBalance)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.TypeCode })
            .IsUnique()
            .HasDatabaseName("IX_AccountTypes_TenantId_TypeCode_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.Category })
            .HasDatabaseName("IX_AccountTypes_TenantId_Category")
            .HasFillFactor(90);
    }
}
