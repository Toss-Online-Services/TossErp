using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Localization;

namespace Toss.Infrastructure.Data.Configurations;

public class LocaleStringResourceConfiguration : IEntityTypeConfiguration<LocaleStringResource>
{
    public void Configure(EntityTypeBuilder<LocaleStringResource> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ResourceValue)
            .IsRequired();

        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.LanguageId, x.ResourceName })
            .IsUnique();
    }
}

