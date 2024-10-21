using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Infrastructure.EntityConfigurations;

public class UserLeadQueueEntityConfiguration : IEntityTypeConfiguration<UserLeadQueue>
{
    public void Configure(EntityTypeBuilder<UserLeadQueue> builder)
    {
        builder.ToTable("user_lead_queue");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.LeadQueueId).HasColumnName("lead_queue_id");
        builder.HasKey(e => new { e.UserId, e.LeadQueueId });
    }
}