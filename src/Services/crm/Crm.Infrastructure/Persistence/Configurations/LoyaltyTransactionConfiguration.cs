using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        // Primary key
        builder.HasKey(lt => lt.Id);
        builder.Property(lt => lt.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Foreign key
        builder.Property(lt => lt.CustomerId)
            .IsRequired();

        // Properties
        builder.Property(lt => lt.Points)
            .IsRequired();

        builder.Property(lt => lt.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(lt => lt.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(lt => lt.CreatedAt)
            .IsRequired();

        builder.Property(lt => lt.RelatedOrderId);

        builder.Property(lt => lt.ExpiryDate);

        // Indexes (simplified for InMemory)
        builder.HasIndex(lt => lt.CustomerId);
        builder.HasIndex(lt => lt.Type);
        builder.HasIndex(lt => lt.CreatedAt);
        builder.HasIndex(lt => lt.ExpiryDate);
        builder.HasIndex(lt => lt.RelatedOrderId);

        // Computed properties (read-only)
        builder.Ignore(lt => lt.IsExpired);
        builder.Ignore(lt => lt.IsEarned);
        builder.Ignore(lt => lt.IsRedeemed);
    }
}
