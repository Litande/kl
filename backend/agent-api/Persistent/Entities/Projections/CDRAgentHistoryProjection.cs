using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Persistent.Entities.Projections;

public class CDRAgentHistoryProjection
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public long? CallDuration { get; set; }
    public DateTimeOffset? Date { get; set; }
    public LeadStatusTypes? LeadStatusAfter { get; set; }
}
