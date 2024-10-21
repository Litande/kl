using Plat4Me.DialLeadCaller.Application.Models.LeadStatisticCache;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface ILeadStatisticCacheRepository
{
    Task UpdateStatistics(long clientId, List<StatisticItemCache> leadsStatisticCaches);
    Task<LeadStatisticCache?> GetLeadStatisticByClient(long clientId);
}