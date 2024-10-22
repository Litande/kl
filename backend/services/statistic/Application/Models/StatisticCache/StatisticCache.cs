using Redis.OM.Modeling;

namespace KL.Statistics.Application.Models.StatisticCache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "LeadStatistic" })]
public class StatisticCache
{
    [RedisIdField] [Indexed]
    public long ClientId { get; set; }
    public List<LeadStatisticCache> Statistics { get; set; }
}
