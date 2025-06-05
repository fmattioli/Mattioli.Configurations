using Microsoft.Extensions.DependencyInjection;

namespace Coderaw.Settings.Extensions.Cache
{
    public static class DistributedCacheExtensions
    {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services, string connectionString, string instanceName)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = instanceName;
            });

            return services;
        }
    }
}
