using Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

public interface ILeadStatisticCacheRepository
{
    Task<StatisticCache?> GetLeadStatisticByClient(long clientId);
}