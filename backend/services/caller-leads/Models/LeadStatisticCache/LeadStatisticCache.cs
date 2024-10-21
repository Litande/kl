using Redis.OM.Modeling;

namespace Plat4Me.DialLeadCaller.Application.Models.LeadStatisticCache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "LeadStatistic" })]
public class LeadStatisticCache
{
    [RedisIdField] [Indexed] 
    public long ClientId { get; set; }
    public List<StatisticItemCache> Statistics { get; set; }
}