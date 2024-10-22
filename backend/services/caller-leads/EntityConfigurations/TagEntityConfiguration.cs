using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.EntityConfigurations;

public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tag");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.Value).HasColumnName("value");
        builder.Property(e => e.LifetimeSeconds).HasColumnName("lifetime_seconds");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<TagStatusTypes>(r, true));
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}