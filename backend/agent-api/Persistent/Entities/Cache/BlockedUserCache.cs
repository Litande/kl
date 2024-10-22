using Redis.OM.Modeling;

namespace KL.Agent.API.Persistent.Entities.Cache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "BlockedUserCache" })]
public class BlockedUserCache
{
    [RedisIdField]
    [Indexed]
    public long UserId { get; init; }
}