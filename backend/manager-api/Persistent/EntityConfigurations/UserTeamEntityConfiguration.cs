using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class UserTeamEntityConfiguration : IEntityTypeConfiguration<UserTeam>
{
    public void Configure(EntityTypeBuilder<UserTeam> builder)
    {
        builder.ToTable("user_group");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.TeamId).HasColumnName("group_id");
        builder.HasKey(e => new { e.UserId, e.TeamId }).HasName("PRIMARY");
    }
}
