using Plat4Me.DialRuleEngine.Application.Enums;
using Redis.OM.Modeling;

namespace Plat4Me.DialRuleEngine.Application.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "QueueLead" })]
public class QueueLeadCache
{
    [Indexed]
    public long ClientId { get; set; }

    [Indexed]
    public long QueueId { get; set; }

    [RedisIdField, Indexed]
    public long LeadId { get; set; }
    public long Score { get; set; } = 0;
    public LeadStatusTypes Status { get; set; }
    public DateTimeOffset? RemindOn { get; set; }

    [Indexed]
    public LeadSystemStatusTypes? SystemStatus { get; set; }
}
