using KL.Agent.API.Application.Enums;
using KL.Agent.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Agent.API.Persistent.EntityConfigurations;

public class LeadHistoryEntityConfiguration : IEntityTypeConfiguration<LeadHistory>
{
    public void Configure(EntityTypeBuilder<LeadHistory> builder)
    {
        builder.ToTable("lead_history");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.LeadId).HasColumnName("lead_id");
        builder.Property(e => e.ActionType).HasColumnName("action_type")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadHistoryActionType>(r, true));
        builder.Property(e => e.Changes).HasColumnName("changes");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
