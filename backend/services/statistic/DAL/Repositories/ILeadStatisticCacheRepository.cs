using KL.Statistics.Application.Models.StatisticCache;

namespace KL.Statistics.DAL.Repositories;

public interface ILeadStatisticCacheRepository
{
    Task<StatisticCache?> GetLeadStatisticByClient(long clientId);
}