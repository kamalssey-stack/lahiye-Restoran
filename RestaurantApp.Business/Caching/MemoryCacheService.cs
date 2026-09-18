using Microsoft.Extensions.Caching.Memory;

namespace RestaurantApp.Business.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? DefaultExpiration
        };
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
        {
            return cachedValue;
        }

        var newValue = await factory();
        if (newValue != null)
        {
            await SetAsync(key, newValue, expiration);
        }
        return newValue!;
    }
}
