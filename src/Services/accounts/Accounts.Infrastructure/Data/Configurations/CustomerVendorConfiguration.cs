namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Customer entity configuration
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Customer Details
        builder.Property(e => e.CustomerNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.TradingName)
            .HasMaxLength(200);

        builder.Property(e => e.TaxNumber)
            .HasMaxLength(50);

        builder.Property(e => e.RegistrationNumber)
            .HasMaxLength(50);

        // Contact Information
        builder.Property(e => e.PrimaryContactName)
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .HasMaxLength(256);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Website)
            .HasMaxLength(200);

        // Address Information
        builder.Property(e => e.BillingAddress)
            .HasMaxLength(1000);

        builder.Property(e => e.ShippingAddress)
            .HasMaxLength(1000);

        // Financial Settings
        builder.Property(e => e.CreditLimit)
            .HasPrecision(18, 2);

        builder.Property(e => e.PaymentTerms)
            .HasMaxLength(100);

        builder.Property(e => e.DefaultTaxRateId);

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Status and Settings
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CustomerType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.PriceLevel)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasMany(e => e.Contacts)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Invoices)
            .WithOne(i => i.Customer)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.CustomerNumber })
            .IsUnique()
            .HasDatabaseName("IX_Customers_TenantId_CustomerNumber_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.CompanyName })
            .HasDatabaseName("IX_Customers_TenantId_CompanyName")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Email })
            .HasDatabaseName("IX_Customers_TenantId_Email")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.IsActive })
            .HasDatabaseName("IX_Customers_TenantId_IsActive")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Vendor entity configuration
/// </summary>
public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendors", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Vendor Details
        builder.Property(e => e.VendorNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.TradingName)
            .HasMaxLength(200);

        builder.Property(e => e.TaxNumber)
            .HasMaxLength(50);

        builder.Property(e => e.RegistrationNumber)
            .HasMaxLength(50);

        // Contact Information
        builder.Property(e => e.PrimaryContactName)
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .HasMaxLength(256);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Website)
            .HasMaxLength(200);

        // Address Information
        builder.Property(e => e.Address)
            .HasMaxLength(1000);

        // Financial Settings
        builder.Property(e => e.PaymentTerms)
            .HasMaxLength(100);

        builder.Property(e => e.DefaultTaxRateId);

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Status and Settings
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.VendorType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasMany(e => e.Contacts)
            .WithOne(c => c.Vendor)
            .HasForeignKey(c => c.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Bills)
            .WithOne(b => b.Vendor)
            .HasForeignKey(b => b.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.VendorNumber })
            .IsUnique()
            .HasDatabaseName("IX_Vendors_TenantId_VendorNumber_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.CompanyName })
            .HasDatabaseName("IX_Vendors_TenantId_CompanyName")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Email })
            .HasDatabaseName("IX_Vendors_TenantId_Email")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.IsActive })
            .HasDatabaseName("IX_Vendors_TenantId_IsActive")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Contact entity configuration (shared between customers and vendors)
/// </summary>
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.CustomerId);
        builder.Property(e => e.VendorId);

        // Contact Details
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.JobTitle)
            .HasMaxLength(100);

        builder.Property(e => e.Department)
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .HasMaxLength(256);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Mobile)
            .HasMaxLength(20);

        builder.Property(e => e.Fax)
            .HasMaxLength(20);

        // Settings
        builder.Property(e => e.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

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
            .WithMany(c => c.Contacts)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Vendor)
            .WithMany(v => v.Contacts)
            .HasForeignKey(e => e.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Constraints - contact must belong to either customer or vendor
        builder.ToTable(t => t.HasCheckConstraint("CK_Contacts_CustomerOrVendor", 
            "([CustomerId] IS NOT NULL AND [VendorId] IS NULL) OR ([CustomerId] IS NULL AND [VendorId] IS NOT NULL)"));

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.CustomerId })
            .HasDatabaseName("IX_Contacts_TenantId_CustomerId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.VendorId })
            .HasDatabaseName("IX_Contacts_TenantId_VendorId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Email })
            .HasDatabaseName("IX_Contacts_TenantId_Email")
            .HasFillFactor(90);
    }
}
