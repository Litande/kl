using KL.Engine.Rule.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Engine.Rule.EntityConfigurations;

public class RuleEntityConfiguration : IEntityTypeConfiguration<Models.Entities.Rule>
{
    public void Configure(EntityTypeBuilder<Models.Entities.Rule> builder)
    {
        builder.ToTable("rule");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.RuleGroupId).HasColumnName("rule_group_id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<RuleStatusTypes>(r, true));
        builder.Property(e => e.Rules).HasColumnName("rules");
        builder.Property(e => e.QueueId).HasColumnName("queue_id");
        builder.Property(e => e.Ordinal).HasColumnName("ordinal");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
