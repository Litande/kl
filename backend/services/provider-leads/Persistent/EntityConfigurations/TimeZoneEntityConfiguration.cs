using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeZone = KL.Provider.Leads.Persistent.Entities.TimeZone;

namespace KL.Provider.Leads.Persistent.EntityConfigurations;

public class TimeZoneEntityConfiguration : IEntityTypeConfiguration<Entities.TimeZone>
{
    public void Configure(EntityTypeBuilder<TimeZone> builder)
    {
        builder.ToTable("timezone");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CityName).HasColumnName("city_name");
        builder.Property(e => e.CityNameEn).HasColumnName("city_name_en");
        builder.Property(e => e.CountryName).HasColumnName("country_name");
        builder.Property(e => e.CountryCode).HasColumnName("country_code");
        builder.Property(e => e.Timezone).HasColumnName("timezone");
        builder.Property(e => e.Longitude).HasColumnName("longitude");
        builder.Property(e => e.Latitude).HasColumnName("latitude");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}