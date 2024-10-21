using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities.Projections;

public class AgentProjection
{
    public long AgentId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Score { get; set; }
    public AgentStatusTypes? Status { get; set; }
    public IEnumerable<long> TeamIds { get; set; } = Enumerable.Empty<long>();
    public IEnumerable<long> LeadQueueIds { get; set; } = Enumerable.Empty<long>();
    public IEnumerable<TagProjection> Tags { get; set; } = Enumerable.Empty<TagProjection>();
}