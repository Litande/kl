using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Engine.Rule.EntityConfigurations;

public class LeadEntityConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("lead");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<LeadStatusTypes>(r, true));
        builder.Property(e => e.SystemStatus).HasColumnName("system_status")
            .HasConversion(
                r => r == null ? null : r.Value.ToString().ToLowerInvariant(),
                r => r == null ? null : Enum.Parse<LeadSystemStatusTypes>(r, true));
        builder.Property(e => e.DataSourceId).HasColumnName("data_source_id");
        builder.Property(e => e.DuplicateOfId).HasColumnName("duplicate_of_id");
        builder.Property(e => e.Phone).HasColumnName("phone");
        builder.Property(e => e.LastUpdateTime).HasColumnName("last_update_time");
        builder.Property(e => e.ExternalId).HasColumnName("external_id");
        builder.Property(e => e.FirstName).HasColumnName("first_name");
        builder.Property(e => e.LastName).HasColumnName("last_name");
        builder.Property(e => e.LanguageCode).HasColumnName("language_code");
        builder.Property(e => e.CountryCode).HasColumnName("country_code");
        builder.Property(e => e.LastTimeOnline).HasColumnName("last_time_online");
        builder.Property(e => e.RegistrationTime).HasColumnName("registration_time");
        builder.Property(e => e.LastCallAgentId).HasColumnName("last_call_user_id");
        builder.Property(e => e.AssignedAgentId).HasColumnName("assigned_agent_id");
        builder.Property(e => e.FirstDepositTime).HasColumnName("first_deposit_time");
        builder.Property(e => e.RemindOn).HasColumnName("remind_on");
        builder.Property(e => e.FreezeTo).HasColumnName("freeze_to");
        builder.Property(e => e.DeletedOn).HasColumnName("deleted_on");
        builder.Property(e => e.Timezone).HasColumnName("timezone");
        builder.Property(e => e.IsTest).HasColumnName("is_test");
        builder.HasMany(x => x.LeadBlacklist).WithOne(x => x.Lead);
        builder.Property(e => e.FirstTimeQueuedOn).HasColumnName("first_time_queued_on");
        builder.Property(e => e.MetaData).HasColumnName("metadata");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
