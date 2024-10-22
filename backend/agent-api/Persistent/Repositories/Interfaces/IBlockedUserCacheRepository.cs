namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface IBlockedUserCacheRepository
{
    Task<bool> IsUserBlocked(string userId);
}