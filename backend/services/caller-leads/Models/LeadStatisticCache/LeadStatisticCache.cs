using Redis.OM.Modeling;

namespace KL.Caller.Leads.Models.LeadStatisticCache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "LeadStatistic" })]
public class LeadStatisticCache
{
    [RedisIdField] [Indexed] 
    public long ClientId { get; set; }
    public List<StatisticItemCache> Statistics { get; set; }
}