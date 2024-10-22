using KL.Provider.Leads.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Provider.Leads.Persistent.EntityConfigurations;

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