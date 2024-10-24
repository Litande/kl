﻿using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Engine.Rule.EntityConfigurations;

public class LeadQueueEntityConfiguration : IEntityTypeConfiguration<LeadQueue>
{
    public void Configure(EntityTypeBuilder<LeadQueue> builder)
    {
        builder.ToTable("lead_queue");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadQueueStatusTypes>(r, true));
        builder.Property(e => e.Type).HasColumnName("type")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadQueueTypes>(r, true));
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}