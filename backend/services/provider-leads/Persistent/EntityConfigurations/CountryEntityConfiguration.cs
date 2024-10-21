using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.EntityConfigurations;

public class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("country");
        builder.Property(e => e.Code).HasColumnName("code");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.CountryCode3).HasColumnName("country_code_3");
        builder.Property(e => e.DialCode).HasColumnName("dial_code");
        builder.HasKey(e => e.Code).HasName("PRIMARY");
    }
}