using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Provider.Leads.Persistent.EntityConfigurations;

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