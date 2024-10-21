using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.EntityConfigurations;

public class LanguageEntityConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("language");
        builder.Property(e => e.Code).HasColumnName("code");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.HasKey(e => e.Code).HasName("PRIMARY");
    }
}