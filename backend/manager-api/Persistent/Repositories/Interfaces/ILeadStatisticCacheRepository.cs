using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ILeadStatisticCacheRepository
{
    Task<StatisticCache?> GetLeadStatisticByClient(long clientId);
}
