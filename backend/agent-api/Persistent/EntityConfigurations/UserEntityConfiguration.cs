using KL.Agent.API.Application.Enums;
using KL.Agent.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Agent.API.Persistent.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.RoleType).HasColumnName("role")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<RoleTypes>(r, true));
        builder.Property(e => e.FirstName).HasColumnName("first_name");
        builder.Property(e => e.LastName).HasColumnName("last_name");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.Property(e => e.OfflineSince).HasColumnName("offline_since");
        builder.HasKey(e => e.UserId).HasName("PRIMARY");
    }
}
