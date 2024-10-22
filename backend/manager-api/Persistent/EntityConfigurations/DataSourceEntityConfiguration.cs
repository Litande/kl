using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class DataSourceEntityConfiguration : IEntityTypeConfiguration<DataSource>
{
    public void Configure(EntityTypeBuilder<DataSource> builder)
    {
        builder.ToTable("data_source");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.DataSourceType).HasColumnName("source_type")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<DataSourceTypes>(r, true));
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.ApiKey).HasColumnName("api_key");
        builder.Property(e => e.Endpoint).HasColumnName("endpoint");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<DataSourceStatusTypes>(r, true));
        builder.Property(e => e.IframeTemplate).HasColumnName("iframe_template");
        builder.Property(e => e.MinUpdateDate).HasColumnName("min_update_date");
        builder.Property(e => e.Metadata).HasColumnName("metadata");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
