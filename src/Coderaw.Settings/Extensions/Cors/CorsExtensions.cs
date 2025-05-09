using Coderaw.Settings.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Coderaw.Settings.Extensions.Cors
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCors(
            this IServiceCollection services,
            string policyName,
            Func<CorsSettings> getSettings)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    builder.SetIsOriginAllowed(origin =>
                    {
                        var uri = new Uri(origin);
                        var host = uri.Host.ToLowerInvariant();
                        var settings = getSettings();

                        var allowedHosts = settings.AllowedHosts?.Select(h => h.ToLowerInvariant()) ?? [];
                        var allowedIps = settings.AllowedIPs ?? [];

                        if (allowedHosts.Contains(host))
                            return true;

                        var resolvedIps = Dns.GetHostAddresses(host);
                        return resolvedIps.Any(ip => allowedIps.Contains(ip.ToString()));
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            return services;
        }
    }
}

