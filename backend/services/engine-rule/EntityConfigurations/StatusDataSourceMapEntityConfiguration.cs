using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.EntityConfigurations;

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