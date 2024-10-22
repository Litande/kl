using KL.Agent.API.Application.Enums;
using Redis.OM.Modeling;

namespace KL.Agent.API.Persistent.Entities.Cache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "CallInfoCache" })]
public class CallInfoCache
{
    public CallInfoCache(
        string sessionId,
        long agentId,
        long clientId)
    {
        AgentId = agentId;
        ClientId = clientId;
        SessionId = sessionId;
    }

    [RedisIdField, Indexed]
    public string SessionId { get; init; }
    public CallStatusType CallStatus { get; set; }
    [Indexed]
    public long AgentId { get; set; }
    [Indexed]
    public long ClientId { get; init; }
    [Indexed]
    public long? LeadId { get; set; }
    public string? LeadPhone { get; set; }
    public long? LeadScore { get; set; }
    public long? QueueId { get; set; }
    public CallType CallType { get; set; }
    public string? BridgeId { get; set; }

    public long? CallOriginatedAt { get; set; }
    public long? LeadAnsweredAt { get; set; }
    public long? AgentAnsweredAt { get; set; }
    public long? CallFinishedAt { get; set; }
    public CallFinishReasons? CallFinishReason { get; set; }
    public string? ManagerRtcUrl { get; set; }
    public string? AgentRtcUrl { get; set; }
    public long? CallAgainCount { get; set; }
    public bool AgentDropped { get; set; } = false;
}
