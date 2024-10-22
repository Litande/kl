using KL.MySql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.MySql.EntityConfigs;

public class FeedConfigurations : IEntityTypeConfiguration<Feed>
{
    public void Configure(EntityTypeBuilder<Feed> builder)
    {   
        builder.ToTable("feed");
        
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.Name).HasColumnName("name");
        builder.Property(i => i.Endpoint).IsRequired(false).HasColumnName("endpoint");

        builder.HasKey(i => i.Id);
    }
}