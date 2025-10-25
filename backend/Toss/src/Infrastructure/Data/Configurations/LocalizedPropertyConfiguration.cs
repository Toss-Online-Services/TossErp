using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Localization;

namespace Toss.Infrastructure.Data.Configurations;

public class LocalizedPropertyConfiguration : IEntityTypeConfiguration<LocalizedProperty>
{
    public void Configure(EntityTypeBuilder<LocalizedProperty> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LocaleKeyGroup)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.LocaleKey)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.LocaleValue)
            .IsRequired();

        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.EntityId, x.LanguageId, x.LocaleKeyGroup, x.LocaleKey });
    }
}

