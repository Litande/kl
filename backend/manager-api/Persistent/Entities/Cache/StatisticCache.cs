using Redis.OM.Modeling;

namespace KL.Manager.API.Persistent.Entities.Cache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "LeadStatistic" })]
public class StatisticCache
{
    [RedisIdField]
    [Indexed]
    public long ClientId { get; set; }
    public List<LeadStatisticCache> Statistics { get; set; }
}
