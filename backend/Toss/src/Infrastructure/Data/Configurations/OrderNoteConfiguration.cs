using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

public class OrderNoteConfiguration : IEntityTypeConfiguration<OrderNote>
{
    public void Configure(EntityTypeBuilder<OrderNote> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Note)
            .IsRequired();

        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderNotes)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

