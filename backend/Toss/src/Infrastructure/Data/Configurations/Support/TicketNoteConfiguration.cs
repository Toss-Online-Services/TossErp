using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Support;

namespace Toss.Infrastructure.Data.Configurations.Support;

public class TicketNoteConfiguration : IEntityTypeConfiguration<TicketNote>
{
    public void Configure(EntityTypeBuilder<TicketNote> builder)
    {
        builder.Property(n => n.Note)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(n => n.CreatedById)
            .HasMaxLength(450)
            .IsRequired();

        // Relationships
        builder.HasOne(n => n.Business)
            .WithMany()
            .HasForeignKey(n => n.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Ticket)
            .WithMany(t => t.Notes)
            .HasForeignKey(n => n.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(n => n.BusinessId);
        builder.HasIndex(n => n.TicketId);
        builder.HasIndex(n => n.CreatedById);
    }
}

