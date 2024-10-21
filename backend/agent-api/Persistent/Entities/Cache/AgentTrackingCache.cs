using Plat4Me.DialAgentApi.Application.Enums;
using Redis.OM.Modeling;

namespace Plat4Me.DialAgentApi.Persistent.Entities.Cache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "AgentStateCache" })]
public class AgentStateCache
{
    public AgentStateCache(
        long agentId,
        long clientId,
        AgentInternalStatusTypes agentStatus)
    {
        AgentId = agentId;
        AgentStatus = agentStatus;
        ClientId = clientId;
    }

    [RedisIdField]
    [Indexed]
    public long AgentId { get; init; }
    [Indexed]
    public long ClientId { get; init; }
    public AgentInternalStatusTypes AgentStatus { get; set; }
    public AgentStatusTypes AgentDisplayStatus { get; set; } = AgentStatusTypes.Offline;
    public string? CallSession { get; set; }
    public long AssignedCallCount { get; set; }
    public string? ManagerRtcUrl { get; set; }
}
