using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.EntityConfigurations;

public class LeadDataSourceMapEntityConfiguration : IEntityTypeConfiguration<LeadDataSourceMap>
{
    public void Configure(EntityTypeBuilder<LeadDataSourceMap> builder)
    {
        builder.ToTable("lead_data_source_map");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.DataSourceId).HasColumnName("data_source_id");
        builder.Property(e => e.DestinationProperty).HasColumnName("destination_property")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadDataSource>(r, true));
        builder.Property(e => e.SourceProperty).HasColumnName("source_property");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}