using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models.Entities;

public class CallDetailRecord
{
    public long Id { get; set; }
    public string? SessionId { get; set; }
    public long? ClientId { get; set; }
    public string? Brand { get; set; }
    public long? LeadId { get; set; }
    public string? LeadName { get; set; }
    public string LeadPhone { get; set; } = "";
    public string? LeadCountry { get; set; }
    public CallType CallType { get; set; }
    public CallFinishReasons? CallHangupStatus { get; set; }
    public long? LeadQueueId { get; set; }
    public string? LeadQueueName { get; set; }
    public long? LastUserId { get; set; }
    public string? LastUserName { get; set; }
    public DateTimeOffset OriginatedAt { get; set; }
    public DateTimeOffset? CallHangupAt { get; set; }
    public DateTimeOffset? LeadAnswerAt { get; set; }
    public DateTimeOffset? UserAnswerAt { get; set; }
    public LeadStatusTypes? LeadStatusAfter { get; set; }
    public LeadStatusTypes? LeadStatusBefore { get; set; }
    public string? CallerId { get; set; }
    public string? RecordLeadFile { get; set; }
    public string? RecordUserFiles { get; set; }
    public string? RecordManagerFiles { get; set; }
    public string? RecordMixedFile { get; set; }
    public bool IsReplacedUser { get; set; }
    public string? MetaData { get; set; }
    public long? CallDuration { get; set; }

    public virtual Lead? Lead { get; set; }
    //public virtual LeadQueue? LeadQueue { get; set; }
    //public virtual User? User { get; set; }
}
