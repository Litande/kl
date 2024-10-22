using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class RuleEntityConfiguration : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> builder)
    {
        builder.ToTable("rule");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.RuleGroupId).HasColumnName("rule_group_id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<StatusTypes>(r, true));
        builder.Property(e => e.Rules).HasColumnName("rules");
        builder.Property(x => x.Ordinal).HasColumnName("ordinal");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}