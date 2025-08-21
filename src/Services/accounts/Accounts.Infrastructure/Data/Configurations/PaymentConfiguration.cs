namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Payment entity configuration
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.PaymentAccountId)
            .IsRequired();

        // Payment Details
        builder.Property(e => e.PaymentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.PaymentDate)
            .IsRequired();

        builder.Property(e => e.PaymentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Reference)
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        // Financial Amounts
        builder.ComplexProperty(e => e.Amount, am =>
        {
            am.Property(a => a.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            am.Property(a => a.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Bank Details
        builder.Property(e => e.CheckNumber)
            .HasMaxLength(50);

        builder.Property(e => e.BankReference)
            .HasMaxLength(100);

        builder.Property(e => e.BankTransactionId)
            .HasMaxLength(100);

        // Settings
        builder.Property(e => e.AutoGenerateJournalEntry)
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
        builder.HasOne(e => e.PaymentAccount)
            .WithMany()
            .HasForeignKey(e => e.PaymentAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Lines)
            .WithOne(l => l.Payment)
            .HasForeignKey(l => l.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.PaymentNumber })
            .IsUnique()
            .HasDatabaseName("IX_Payments_TenantId_PaymentNumber_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.PaymentDate, e.PaymentType })
            .HasDatabaseName("IX_Payments_TenantId_PaymentDate_PaymentType")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Status, e.PaymentMethod })
            .HasDatabaseName("IX_Payments_TenantId_Status_PaymentMethod")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.BankTransactionId })
            .HasDatabaseName("IX_Payments_TenantId_BankTransactionId")
            .HasFillFactor(95);
    }
}

/// <summary>
/// Payment Line entity configuration
/// </summary>
public class PaymentLineConfiguration : IEntityTypeConfiguration<PaymentLine>
{
    public void Configure(EntityTypeBuilder<PaymentLine> builder)
    {
        builder.ToTable("PaymentLines", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.PaymentId)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .IsRequired();

        // Line Details
        builder.Property(e => e.LineType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Reference)
            .HasMaxLength(100);

        // Financial Amounts
        builder.ComplexProperty(e => e.Amount, am =>
        {
            am.Property(a => a.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            am.Property(a => a.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");

            am.Property(a => a.IsCredit)
                .HasColumnName("IsCredit")
                .IsRequired();
        });

        // Relationships
        builder.HasOne(e => e.Payment)
            .WithMany(p => p.Lines)
            .HasForeignKey(e => e.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.PaymentId })
            .HasDatabaseName("IX_PaymentLines_TenantId_PaymentId")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.AccountId })
            .HasDatabaseName("IX_PaymentLines_TenantId_AccountId")
            .HasFillFactor(90);
    }
}
