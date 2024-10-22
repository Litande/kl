using Microsoft.Extensions.Caching.Memory;

namespace KL.Statistics.Application.Cache;

public class MemoryCacheService<T> : ICacheService<T>
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get(string key)
    {
        _ = _memoryCache.TryGetValue(key, out T? entry);
        return entry;
    }

    public void Remove(string key)
        => _memoryCache.Remove(key);

    public void Set(string key, T value, MemoryCacheEntryOptions? options = null)
    => _memoryCache.Set(key, value, options);
}