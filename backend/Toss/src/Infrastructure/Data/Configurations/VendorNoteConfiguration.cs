using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Vendors;

namespace Toss.Infrastructure.Data.Configurations;

public class VendorNoteConfiguration : IEntityTypeConfiguration<VendorNote>
{
    public void Configure(EntityTypeBuilder<VendorNote> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Note)
            .IsRequired();

        builder.HasOne(x => x.Vendor)
            .WithMany(x => x.VendorNotes)
            .HasForeignKey(x => x.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

