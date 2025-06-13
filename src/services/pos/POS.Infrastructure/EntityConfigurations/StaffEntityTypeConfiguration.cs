namespace TossErp.POS.Infrastructure.EntityConfigurations;

class StaffEntityTypeConfiguration
    : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> staffConfiguration)
    {
        staffConfiguration.ToTable("staff", "POS");

        staffConfiguration.Ignore(s => s.DomainEvents);

        staffConfiguration.Property(s => s.Id)
            .HasMaxLength(36)
            .IsRequired();

        staffConfiguration.Property(s => s.Code)
            .HasMaxLength(50)
            .IsRequired();

        staffConfiguration.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        staffConfiguration.Property(s => s.Email)
            .HasMaxLength(200)
            .IsRequired();

        staffConfiguration.Property(s => s.Phone)
            .HasMaxLength(20);

        staffConfiguration.Property(s => s.StoreId)
            .HasMaxLength(36)
            .IsRequired();

        staffConfiguration.Property(s => s.Role)
            .HasMaxLength(50)
            .IsRequired();

        staffConfiguration.HasIndex(s => new { s.StoreId, s.Code })
            .IsUnique();

        staffConfiguration.HasIndex(s => s.Email)
            .IsUnique();

        staffConfiguration.HasIndex(s => s.Phone)
            .IsUnique();

        staffConfiguration.HasIndex(s => s.StoreId);
    }
} 
