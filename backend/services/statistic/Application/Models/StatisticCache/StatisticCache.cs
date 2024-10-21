using Redis.OM.Modeling;

namespace Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "LeadStatistic" })]
public class StatisticCache
{
    [RedisIdField] [Indexed]
    public long ClientId { get; set; }
    public List<LeadStatisticCache> Statistics { get; set; }
}
