using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.EntityConfigurations;

public class UserFilterPreferencesEntityConfiguration : IEntityTypeConfiguration<UserFilterPreferences>
{
    public void Configure(EntityTypeBuilder<UserFilterPreferences> builder)
    {
        builder.ToTable("user_filter_preferences");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.FilterName).HasColumnName("filter_name");
        builder.Property(e => e.Filter).HasColumnName("filter");
        builder.Property(e => e.CreatedById).HasColumnName("created_by");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}