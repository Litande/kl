﻿using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Caller.Leads.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.RoleType).HasColumnName("role")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<RoleTypes>(r, true));
        builder.Property(e => e.FirstName).HasColumnName("first_name");
        builder.Property(e => e.LastName).HasColumnName("last_name");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<UserStatusTypes>(r, true));
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.Property(e => e.Timezone).HasColumnName("timezone");
    }
}