using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Identity.Domain.AggregatesModel.UserAggregate;

namespace TossErp.Identity.Infrastructure.EntityConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(ur => ur.Id);
            builder.Property(ur => ur.Id).ValueGeneratedOnAdd();

            builder.Property(ur => ur.RoleName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ur => ur.AssignedAt)
                .IsRequired();

            builder.HasIndex(ur => new { ur.Id, ur.RoleName });
        }
    }
} 
