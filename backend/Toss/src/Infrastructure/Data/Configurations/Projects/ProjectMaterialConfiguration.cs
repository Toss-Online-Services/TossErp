using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Projects;

namespace Toss.Infrastructure.Data.Configurations.Projects;

public class ProjectMaterialConfiguration : IEntityTypeConfiguration<ProjectMaterial>
{
    public void Configure(EntityTypeBuilder<ProjectMaterial> builder)
    {
        builder.Property(m => m.UnitCost)
            .HasPrecision(18, 2);

        builder.Property(m => m.TotalCost)
            .HasPrecision(18, 2);

        builder.Property(m => m.Notes)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(m => m.Business)
            .WithMany()
            .HasForeignKey(m => m.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Project)
            .WithMany(p => p.Materials)
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Shop)
            .WithMany()
            .HasForeignKey(m => m.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(m => m.BusinessId);
        builder.HasIndex(m => m.ProjectId);
        builder.HasIndex(m => m.ProductId);
        builder.HasIndex(m => m.ShopId);
        builder.HasIndex(m => m.StockMovementId);
    }
}

