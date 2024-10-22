using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.EntityConfigurations;

public class CallDetailRecordEntityConfiguration : IEntityTypeConfiguration<CallDetailRecord>
{
    public void Configure(EntityTypeBuilder<CallDetailRecord> builder)
    {
        builder.ToTable("call_detail_record");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.SessionId).HasColumnName("session_id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.Brand).HasColumnName("brand_name");
        builder.Property(e => e.LeadId).HasColumnName("lead_id");
        builder.Property(e => e.LeadName).HasColumnName("lead_name");
        builder.Property(e => e.LeadPhone).HasColumnName("lead_phone");
        builder.Property(e => e.LeadCountry).HasColumnName("lead_country");
        builder.Property(e => e.CallType).HasColumnName("call_type")
           .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<CallType>(r, true));
        builder.Property(e => e.CallHangupStatus).HasColumnName("call_hangup_status")
           .HasConversion(
                r => r == null ? null : r.Value.ToString().ToLowerInvariant(),
                r => r == null ? null : Enum.Parse<CallFinishReasons>(r, true));
        builder.Property(e => e.LeadQueueId).HasColumnName("queue_id");
        builder.Property(e => e.LeadQueueName).HasColumnName("queue_name");
        builder.Property(e => e.LastUserId).HasColumnName("user_id");
        builder.Property(e => e.LastUserName).HasColumnName("user_name");
        builder.Property(e => e.OriginatedAt).HasColumnName("originated_at");
        builder.Property(e => e.CallHangupAt).HasColumnName("call_hangup_at");
        builder.Property(e => e.LeadAnswerAt).HasColumnName("lead_answer_at");
        builder.Property(e => e.UserAnswerAt).HasColumnName("user_answer_at");
        builder.Property(e => e.LeadStatusAfter).HasColumnName("lead_status_after")
            .HasConversion(
                r => r == null ? null : r.Value.ToString().ToLowerInvariant(),
                r => r == null ? null : Enum.Parse<LeadStatusTypes>(r, true));
        builder.Property(e => e.LeadStatusBefore).HasColumnName("lead_status_before")
            .HasConversion(
                r => r == null ? null : r.Value.ToString().ToLowerInvariant(),
                r => r == null ? null : Enum.Parse<LeadStatusTypes>(r, true));
        builder.Property(e => e.CallerId).HasColumnName("caller_id");
        builder.Property(e => e.RecordLeadFile).HasColumnName("record_lead_file");
        builder.Property(e => e.RecordUserFiles).HasColumnName("record_user_files");
        builder.Property(e => e.RecordManagerFiles).HasColumnName("record_manager_files");
        builder.Property(e => e.RecordMixedFile).HasColumnName("record_mixed_file");
        builder.Property(e => e.IsReplacedUser).HasColumnName("is_replaced_user");
        builder.Property(e => e.MetaData).HasColumnName("metadata");
        builder.Property(e => e.CallDuration).HasColumnName("call_duration");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
