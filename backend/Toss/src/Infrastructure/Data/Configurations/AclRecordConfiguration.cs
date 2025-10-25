using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Security;

namespace Toss.Infrastructure.Data.Configurations;

public class AclRecordConfiguration : IEntityTypeConfiguration<AclRecord>
{
    public void Configure(EntityTypeBuilder<AclRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.RoleName)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => new { x.EntityId, x.EntityName });
    }
}

