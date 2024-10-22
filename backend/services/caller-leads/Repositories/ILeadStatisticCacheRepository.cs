using KL.Caller.Leads.Models.LeadStatisticCache;

namespace KL.Caller.Leads.Repositories;

public interface ILeadStatisticCacheRepository
{
    Task UpdateStatistics(long clientId, List<StatisticItemCache> leadsStatisticCaches);
    Task<LeadStatisticCache?> GetLeadStatisticByClient(long clientId);
}