using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Localization;

namespace Toss.Infrastructure.Data.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LanguageCulture)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.UniqueSeoCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.FlagImageFileName)
            .HasMaxLength(50);
    }
}

