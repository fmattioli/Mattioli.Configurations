namespace Mattioli.Configurations.Repositories
{
    public interface ICacheRepository
    {
        Task<T?> GetCacheValueAsync<T>(string key, CancellationToken cancellationToken);
        Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken);
    }
}
