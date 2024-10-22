using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.EntityConfigurations;

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
