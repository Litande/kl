namespace KL.Manager.API.Persistent.Entities.Projections;

public class AgentInfoProjection
{
    public long AgentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BrandName { get; set; }
    public int Score { get; set; }
    public IEnumerable<string> LeadQueues { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<long> TeamIds { get; set; }
    public IEnumerable<TagProjection> Tags { get; set; } = Enumerable.Empty<TagProjection>();
}