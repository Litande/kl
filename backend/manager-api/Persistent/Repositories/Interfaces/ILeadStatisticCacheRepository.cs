using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ILeadStatisticCacheRepository
{
    Task<StatisticCache?> GetLeadStatisticByClient(long clientId);
}
