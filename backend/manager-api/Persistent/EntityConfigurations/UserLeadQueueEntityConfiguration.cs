using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class UserLeadQueueEntityConfiguration : IEntityTypeConfiguration<UserLeadQueue>
{
    public void Configure(EntityTypeBuilder<UserLeadQueue> builder)
    {
        builder.ToTable("user_lead_queue");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.LeadQueueId).HasColumnName("lead_queue_id");
        builder.HasKey(e => new { e.UserId, e.LeadQueueId }).HasName("PRIMARY");
    }
}
