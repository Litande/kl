using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.EntityConfigurations;

public class StatusDataSourceMapEntityConfiguration : IEntityTypeConfiguration<StatusDataSourceMap>
{
    public void Configure(EntityTypeBuilder<StatusDataSourceMap> builder)
    {
        builder.ToTable("status_data_source_map");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.DataSourceId).HasColumnName("data_source_id");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadStatusTypes>(r, true));
        builder.Property(e => e.ExternalStatusId).HasColumnName("external_status_id");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}