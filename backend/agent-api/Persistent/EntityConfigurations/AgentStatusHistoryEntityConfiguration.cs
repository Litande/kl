using KL.Agent.API.Application.Enums;
using KL.Agent.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Agent.API.Persistent.EntityConfigurations;

public class AgentStatusHistoryEntityConfiguration : IEntityTypeConfiguration<AgentStatusHistory>
{
    public void Configure(EntityTypeBuilder<AgentStatusHistory> builder)
    {
        builder.ToTable("agent_status_history");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.AgentId).HasColumnName("agent_id");
        builder.Property(e => e.OldStatus).HasColumnName("old_status").HasConversion(
            r => r.ToString().ToLowerInvariant(),
            r => Enum.Parse<AgentStatusTypes>(r, true));
        builder.Property(e => e.NewStatus).HasColumnName("new_status").HasConversion(
            r => r.ToString().ToLowerInvariant(),
            r => Enum.Parse<AgentStatusTypes>(r, true));
        builder.Property(e => e.Initiator).HasColumnName("initiator");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
