using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IBlockedUserCacheRepository
{
    Task<BlockedUserCache> Add(long userId);
}