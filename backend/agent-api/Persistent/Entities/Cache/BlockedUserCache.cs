using Redis.OM.Modeling;

namespace Plat4Me.DialAgentApi.Persistent.Entities.Cache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "BlockedUserCache" })]
public class BlockedUserCache
{
    [RedisIdField]
    [Indexed]
    public long UserId { get; init; }
}