using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Persistent.Entities;

public class AgentStatusHistory
{
    public long Id { get; set;}
    public User Agent { get; set; } = null!;
    public long AgentId { get; set; }
    public AgentStatusTypes OldStatus { get; set; }
    public AgentStatusTypes NewStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Initiator { get; set; } = null!;
}
