using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class TeamEntityConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("group");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.HasMany(e => e.Agents).WithMany(e => e.Teams).UsingEntity<UserTeam>();
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
