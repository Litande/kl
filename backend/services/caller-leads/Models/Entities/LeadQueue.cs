using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Entities;

public class LeadQueue
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public LeadQueueStatusTypes Status { get; set; } = LeadQueueStatusTypes.Enable;
    public bool Default { get; set; }
    public long Priority { get; set; }
    public double Ratio { get; set; }
    public DateTimeOffset? RatioUpdatedAt { get; set; }
    public double DropRateUpperThreshold { get; set; }
    public double DropRateLowerThreshold { get; set; }
    public int DropRatePeriod { get; set; }
    public double RatioStep { get; set; }
    public int RatioFreezeTime { get; set; }
    public LeadQueueTypes QueueType { get; set; }
    public double? MaxRatio { get; set; }
    public double? MinRatio { get; set; }

    public virtual ICollection<UserLeadQueue> AgentLeadQueues { get; set; } = new HashSet<UserLeadQueue>();
}
