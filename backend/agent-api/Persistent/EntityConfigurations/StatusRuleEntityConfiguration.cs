using KL.Agent.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Agent.API.Persistent.EntityConfigurations;

public class StatusRuleEntityConfiguration : IEntityTypeConfiguration<StatusRule>
{
    public void Configure(EntityTypeBuilder<StatusRule> builder)
    {
        builder.ToTable("lead_status_rules");
        builder.Property(e => e.Status).HasColumnName("status");
        builder.Property(e => e.AllowTransitStatus).HasColumnName("allow_transit_status");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.HasKey(e => new { e.Status, e.AllowTransitStatus, e.ClientId }).HasName("PRIMARY");
    }
}
