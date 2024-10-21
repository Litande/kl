using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models.Entities;

namespace Plat4Me.Dial.Statistic.Api.DAL.EntityConfigurations;

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
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<UserStatusTypes>(r, true));
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.Property(e => e.Timezone).HasColumnName("timezone");
        builder.HasKey(e => e.UserId).HasName("PRIMARY");
    }
}
