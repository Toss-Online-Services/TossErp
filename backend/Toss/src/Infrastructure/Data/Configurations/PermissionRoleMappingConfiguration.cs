using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Security;

namespace Toss.Infrastructure.Data.Configurations;

public class PermissionRoleMappingConfiguration : IEntityTypeConfiguration<PermissionRoleMapping>
{
    public void Configure(EntityTypeBuilder<PermissionRoleMapping> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RoleName)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(x => x.PermissionRecord)
            .WithMany(x => x.PermissionRoleMappings)
            .HasForeignKey(x => x.PermissionRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

