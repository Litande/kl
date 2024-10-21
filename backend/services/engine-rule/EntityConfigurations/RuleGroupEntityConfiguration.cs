﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Infrastructure.EntityConfigurations;

public class RuleGroupEntityConfiguration : IEntityTypeConfiguration<RuleGroup>
{
    public void Configure(EntityTypeBuilder<RuleGroup> builder)
    {
        builder.ToTable("rule_group");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<RuleGroupStatusTypes>(r, true));
        builder.Property(e => e.GroupType).HasColumnName("group_type")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<RuleGroupTypes>(r, true));
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
