using KL.Agent.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Agent.API.Persistent.EntityConfigurations;

public class DataSourceEntityConfiguration : IEntityTypeConfiguration<DataSource>
{
    public void Configure(EntityTypeBuilder<DataSource> builder)
    {
        builder.ToTable("data_source");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.IframeTemplate).HasColumnName("iframe_template");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
