using Redis.OM.Modeling;

namespace Plat4Me.DialLeadCaller.Application.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "WaitingAgent" })]
public class WaitingAgent
{
    public WaitingAgent(
        long clientId,
        long agentId)
    {
        ClientId = clientId;
        AgentId = agentId;
    }

    [Indexed]
    public long ClientId { get; set; }

    [RedisIdField, Indexed]
    public long AgentId { get; set; }
}
