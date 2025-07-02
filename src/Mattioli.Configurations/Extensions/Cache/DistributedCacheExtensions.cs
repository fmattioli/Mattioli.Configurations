using Coderaw.Settings.Models;
using Coderaw.Settings.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace Coderaw.Settings.Extensions.Cache
{
    public static class DistributedCacheExtensions
    {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services, CacheSettings cacheSettings)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheSettings.ConnectionString;
                options.InstanceName = cacheSettings.InstanceName;
            });

            services.AddScoped<ICacheRepository, CacheRepository>();

            return services;
        }
    }
}
