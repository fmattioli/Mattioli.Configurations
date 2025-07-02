using Mattioli.Configurations.Extensions.Cors;
using Mattioli.Configurations.Models;

using Microsoft.Extensions.DependencyInjection;

using System.Net;

namespace Mattioli.Configurations.Extensions.Cors
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCors(
            this IServiceCollection services,
            string policyName,
            Func<CorsSettings> getSettings, string environment)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    if (environment.Contains("dev", StringComparison.OrdinalIgnoreCase))
                    {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
                    }
                    else
                    {
                        builder.SetIsOriginAllowed(origin =>
                        {
                            var uri = new Uri(origin);
                            var host = uri.Host.Trim();
                    
                            var settings = getSettings();
                            var allowedHosts = settings.AllowedHosts ?? [];
                            var allowedIps = settings.AllowedIPs ?? [];
                    
                            if (allowedHosts.Any(h => string.Equals(h.Trim(), host, StringComparison.OrdinalIgnoreCase)))
                            {
                                return true;
                            }
                            
                            var resolvedIps = Dns.GetHostAddresses(host);
                            if (resolvedIps.Any(ip => allowedIps.Contains(ip.ToString())))
                            {    
                                return true;
                            }
                    
                            return false;
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    }

                });
            });

            return services;
        }
    }
}

