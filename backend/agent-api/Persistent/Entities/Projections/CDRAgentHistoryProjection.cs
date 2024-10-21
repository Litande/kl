using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Persistent.Entities.Projections;

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
