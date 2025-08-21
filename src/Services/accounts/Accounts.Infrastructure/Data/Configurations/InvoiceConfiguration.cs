namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Invoice entity configuration
/// </summary>
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.CustomerId)
            .IsRequired();

        // Invoice Details
        builder.Property(e => e.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.InvoiceDate)
            .IsRequired();

        builder.Property(e => e.DueDate)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.InvoiceType)
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
        builder.ComplexProperty(e => e.SubTotal, st =>
        {
            st.Property(s => s.Amount)
                .HasColumnName("SubTotal")
                .HasPrecision(18, 2)
                .IsRequired();

            st.Property(s => s.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        builder.ComplexProperty(e => e.TaxTotal, tt =>
        {
            tt.Property(t => t.Amount)
                .HasColumnName("TaxTotal")
                .HasPrecision(18, 2)
                .IsRequired();

            tt.Property(t => t.Currency)
                .HasColumnName("TaxCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        builder.ComplexProperty(e => e.Total, to =>
        {
            to.Property(t => t.Amount)
                .HasColumnName("Total")
                .HasPrecision(18, 2)
                .IsRequired();

            to.Property(t => t.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        builder.ComplexProperty(e => e.AmountPaid, ap =>
        {
            ap.Property(a => a.Amount)
                .HasColumnName("AmountPaid")
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0);

            ap.Property(a => a.Currency)
                .HasColumnName("PaidCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Settings
        builder.Property(e => e.PaymentTerms)
            .HasMaxLength(100);

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
        builder.HasOne(e => e.Customer)
            .WithMany(c => c.Invoices)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Lines)
            .WithOne(l => l.Invoice)
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Payments)
            .WithOne(p => p.Invoice)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.InvoiceNumber })
            .IsUnique()
            .HasDatabaseName("IX_Invoices_TenantId_InvoiceNumber_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.CustomerId, e.InvoiceDate })
            .HasDatabaseName("IX_Invoices_TenantId_CustomerId_InvoiceDate")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Status, e.DueDate })
            .HasDatabaseName("IX_Invoices_TenantId_Status_DueDate")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Invoice Line entity configuration
/// </summary>
public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.InvoiceId)
            .IsRequired();

        builder.Property(e => e.ProductId);

        // Line Details
        builder.Property(e => e.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.ProductCode)
            .HasMaxLength(50);

        builder.Property(e => e.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        // Financial Amounts
        builder.ComplexProperty(e => e.UnitPrice, up =>
        {
            up.Property(u => u.Amount)
                .HasColumnName("UnitPrice")
                .HasPrecision(18, 4)
                .IsRequired();

            up.Property(u => u.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        builder.ComplexProperty(e => e.LineTotal, lt =>
        {
            lt.Property(l => l.Amount)
                .HasColumnName("LineTotal")
                .HasPrecision(18, 2)
                .IsRequired();

            lt.Property(l => l.Currency)
                .HasColumnName("LineTotalCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Tax Information
        builder.ComplexProperty(e => e.TaxRate, tr =>
        {
            tr.Property(t => t.Rate)
                .HasColumnName("TaxRate")
                .HasPrecision(5, 4)
                .IsRequired()
                .HasDefaultValue(0);

            tr.Property(t => t.Name)
                .HasColumnName("TaxName")
                .HasMaxLength(100);

            tr.Property(t => t.Type)
                .HasColumnName("TaxType")
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        builder.ComplexProperty(e => e.TaxAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TaxAmount")
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0);

            ta.Property(t => t.Currency)
                .HasColumnName("TaxCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Relationships
        builder.HasOne(e => e.Invoice)
            .WithMany(i => i.Lines)
            .HasForeignKey(e => e.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.InvoiceId })
            .HasDatabaseName("IX_InvoiceLines_TenantId_InvoiceId")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.ProductId })
            .HasDatabaseName("IX_InvoiceLines_TenantId_ProductId")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Invoice Payment entity configuration
/// </summary>
public class InvoicePaymentConfiguration : IEntityTypeConfiguration<InvoicePayment>
{
    public void Configure(EntityTypeBuilder<InvoicePayment> builder)
    {
        builder.ToTable("InvoicePayments", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.InvoiceId)
            .IsRequired();

        builder.Property(e => e.PaymentId)
            .IsRequired();

        // Payment Details
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

        builder.Property(e => e.PaymentDate)
            .IsRequired();

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Invoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(e => e.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Payment)
            .WithMany()
            .HasForeignKey(e => e.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.InvoiceId })
            .HasDatabaseName("IX_InvoicePayments_TenantId_InvoiceId")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.PaymentId })
            .HasDatabaseName("IX_InvoicePayments_TenantId_PaymentId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.PaymentDate })
            .HasDatabaseName("IX_InvoicePayments_TenantId_PaymentDate")
            .HasFillFactor(90);
    }
}
