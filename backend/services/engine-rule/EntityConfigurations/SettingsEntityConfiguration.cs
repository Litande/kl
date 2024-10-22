using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.EntityConfigurations;

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