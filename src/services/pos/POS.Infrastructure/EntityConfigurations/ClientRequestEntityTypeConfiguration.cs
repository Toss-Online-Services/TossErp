namespace TossErp.POS.Infrastructure.EntityConfigurations;

class ClientRequestEntityTypeConfiguration
    : IEntityTypeConfiguration<ClientRequest>
{
    public void Configure(EntityTypeBuilder<ClientRequest> requestConfiguration)
    {
        requestConfiguration.ToTable("requests");

        requestConfiguration.Property(cr => cr.Id)
            .HasMaxLength(36)
            .IsRequired();

        requestConfiguration.Property(cr => cr.Name)
            .HasMaxLength(200)
            .IsRequired();

        requestConfiguration.Property(cr => cr.Time)
            .IsRequired();
    }
}
