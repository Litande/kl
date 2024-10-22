using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class SettingsEntityConfiguration : IEntityTypeConfiguration<SettingsEntity>
{
    public void Configure(EntityTypeBuilder<SettingsEntity> builder)
    {
        builder.ToTable("settings");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Value).HasColumnName("value");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Type).HasColumnName("type").HasConversion(
            r => r.ToString().ToLowerInvariant(),
            r => Enum.Parse<SettingTypes>(r, true));
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}