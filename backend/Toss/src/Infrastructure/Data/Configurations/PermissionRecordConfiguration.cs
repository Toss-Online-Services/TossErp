using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Security;

namespace Toss.Infrastructure.Data.Configurations;

public class PermissionRecordConfiguration : IEntityTypeConfiguration<PermissionRecord>
{
    public void Configure(EntityTypeBuilder<PermissionRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.SystemName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasMany(x => x.PermissionRoleMappings)
            .WithOne(x => x.PermissionRecord)
            .HasForeignKey(x => x.PermissionRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

