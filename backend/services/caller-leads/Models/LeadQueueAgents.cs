using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models;

public class LeadQueueAgents
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public long Priority { get; set; }
    public double Ratio { get; set; }
    public LeadQueueTypes QueueType { get; set; }
    public IEnumerable<long> AssignedAgentIds { get; set; } = new List<long>();
}
