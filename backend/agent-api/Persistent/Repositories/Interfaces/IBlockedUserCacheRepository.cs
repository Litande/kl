namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface IBlockedUserCacheRepository
{
    Task<bool> IsUserBlocked(string userId);
}