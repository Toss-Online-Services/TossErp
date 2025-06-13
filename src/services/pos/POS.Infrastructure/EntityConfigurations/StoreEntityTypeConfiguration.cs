namespace TossErp.POS.Infrastructure.EntityConfigurations;

class StoreEntityTypeConfiguration
    : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> storeConfiguration)
    {
        storeConfiguration.ToTable("stores", "POS");

        storeConfiguration.Ignore(s => s.DomainEvents);

        storeConfiguration.Property(s => s.Id)
            .HasMaxLength(36)
            .IsRequired();

        storeConfiguration.Property(s => s.Code)
            .HasMaxLength(50)
            .IsRequired();

        storeConfiguration.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        storeConfiguration.Property(s => s.Email)
            .HasMaxLength(200)
            .IsRequired();

        storeConfiguration.Property(s => s.Phone)
            .HasMaxLength(20);

        storeConfiguration.Property(s => s.Address)
            .HasMaxLength(500);

        storeConfiguration.Property(s => s.Region)
            .HasMaxLength(100);

        storeConfiguration.Property(s => s.OwnerId)
            .HasMaxLength(36)
            .IsRequired();

        storeConfiguration.Property(s => s.IsActive)
            .IsRequired();

        storeConfiguration.HasIndex(s => s.Code)
            .IsUnique();

        storeConfiguration.HasIndex(s => s.Email)
            .IsUnique();

        storeConfiguration.HasIndex(s => s.OwnerId);
    }
} 
