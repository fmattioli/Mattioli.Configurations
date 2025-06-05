using Microsoft.Extensions.Caching.Distributed;

namespace Coderaw.Settings.Repositories
{
    public interface ICacheRepository
    {
        Task<T?> GetCacheValueAsync<T>(string key, CancellationToken cancellationToken);
        Task SetCacheValueAsync<T>(string key, T value, CancellationToken cancellationToken);
    }
}
