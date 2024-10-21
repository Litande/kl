using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.EntityConfigurations;

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