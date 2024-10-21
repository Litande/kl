using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialAgentApi.Persistent.Entities;

namespace Plat4Me.DialAgentApi.Persistent.EntityConfigurations;

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
