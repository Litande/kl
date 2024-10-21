using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Infrastructure.EntityConfigurations;

public class StatusDataSourceMapEntityConfiguration : IEntityTypeConfiguration<StatusDataSourceMap>
{
    public void Configure(EntityTypeBuilder<StatusDataSourceMap> builder)
    {
        builder.ToTable("status_data_source_map");
        builder.Property(e => e.DataSourceId).HasColumnName("data_source_id");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadStatusTypes>(r, true));
        builder.Property(e => e.ExternalStatusId).HasColumnName("external_status_id");
        builder.HasKey(e => e.DataSourceId).HasName("PRIMARY");
    }
}