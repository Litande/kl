using Microsoft.Extensions.Caching.Memory;

namespace Plat4Me.Dial.Statistic.Api.Application.Cache;

public interface ICacheService<T>
{
    T? Get(string key);
    void Set(string key, T value, MemoryCacheEntryOptions? options = null);
    void Remove(string key);
}