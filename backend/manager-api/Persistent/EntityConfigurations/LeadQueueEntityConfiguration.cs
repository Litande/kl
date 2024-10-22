using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class LeadQueueEntityConfiguration : IEntityTypeConfiguration<LeadQueue>
{
    public void Configure(EntityTypeBuilder<LeadQueue> builder)
    {
        builder.ToTable("lead_queue");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadQueueStatusTypes>(r, true));
        builder.Property(e => e.Default).HasColumnName("default");
        builder.Property(e => e.Priority).HasColumnName("priority");
        builder.Property(e => e.Ratio).HasColumnName("ratio");
        builder.Property(e => e.RatioUpdatedAt).HasColumnName("ratio_updated_at");
        builder.Property(e => e.QueueType).HasColumnName("type")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadQueueTypes>(r, true));
        builder.HasMany(e => e.Agents).WithMany(e => e.LeadQueues).UsingEntity<UserLeadQueue>();
        builder.Property(e => e.DisplayOrder).HasColumnName("display_order");
        builder.Property(e => e.DropRateUpperThreshold).HasColumnName("drop_rate_upper_threshold");
        builder.Property(e => e.DropRateLowerThreshold).HasColumnName("drop_rate_lower_threshold");
        builder.Property(e => e.DropRatePeriod).HasColumnName("drop_rate_period");
        builder.Property(e => e.RatioStep).HasColumnName("ratio_step");
        builder.Property(e => e.MaxRatio).HasColumnName("max_ratio");
        builder.Property(e => e.MinRatio).HasColumnName("min_ratio");
        builder.Property(e => e.RatioFreezeTime).HasColumnName("ratio_freeze_time");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}