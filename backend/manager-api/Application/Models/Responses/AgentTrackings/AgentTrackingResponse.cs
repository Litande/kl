using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Application.Models.Responses.AgentTrackings;

public class AgentTrackingResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public AgentStatusTypes State { get; set; } = AgentStatusTypes.Offline;
    public IEnumerable<string> Groups { get; set; }
    public IEnumerable<long> TeamIds { get; set; }
    public string? Source { get; set; }
    public DateTimeOffset? RegistrationTime { get; set; }
    public string? LeadStatus { get; set; }
    public DateTimeOffset? CallOriginatedAt { get; set; }
    public DateTimeOffset? LeadAnsweredAt { get; set; }
    public DateTimeOffset? AgentAnsweredAt { get; set; }
    public DateTimeOffset? CallFinishedAt { get; set; }
    public long? LeadId { get; set; }
    public string? LeadName { get; set; }
    public string? AffiliateId { get; set; }
    public long? Weight { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? ManagerRtcUrl { get; set; }
    public string? LeadGroup { get; set; }
    public CallType? CallType { get; set; }
    public IEnumerable<TagProjection> Tags { get; set; } = Enumerable.Empty<TagProjection>();
}
