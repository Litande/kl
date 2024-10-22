using KL.MySql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.MySql.EntityConfigs;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {   
        builder.ToTable("category");
        
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.Name).HasColumnName("name");
        builder.Property(i => i.ThresholdPercent).HasColumnName("threshold");
        builder.Property(i => i.BufferSize).HasColumnName("buffer_size");

        builder.HasKey(i => i.Id);
    }
}