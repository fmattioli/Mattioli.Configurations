using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Mattioli.Configurations.Repositories;

public class CacheRepository(IDistributedCache _cache) : ICacheRepository
{
    public async Task<T?> GetCacheValueAsync<T>(string key, CancellationToken cancellationToken)
    {
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedData))
        {
            return default;
        }
        else
        {
            var value = JsonSerializer.Deserialize<T>(cachedData);
            return value;
        }
    }

    public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var serializedValue = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key, serializedValue, cacheOptions, cancellationToken);
    }
}
