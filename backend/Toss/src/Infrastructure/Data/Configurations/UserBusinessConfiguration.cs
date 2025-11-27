using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Businesses;

namespace Toss.Infrastructure.Data.Configurations;

public class UserBusinessConfiguration : IEntityTypeConfiguration<UserBusiness>
{
    public void Configure(EntityTypeBuilder<UserBusiness> builder)
    {
        builder.Property(ub => ub.Role)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(ub => new { ub.UserId, ub.BusinessId })
            .IsUnique();

        builder.HasIndex(ub => ub.BusinessId);

        builder.HasIndex(ub => ub.UserId);

        builder.HasOne(ub => ub.Business)
            .WithMany(b => b.Members)
            .HasForeignKey(ub => ub.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ub => ub.User)
            .WithMany(u => u.Businesses)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ub => ub.UserId)
            .HasDatabaseName("IX_UserBusinesses_User_Default")
            .IsUnique()
            .HasFilter("\"IsDefault\" = TRUE");
    }
}

