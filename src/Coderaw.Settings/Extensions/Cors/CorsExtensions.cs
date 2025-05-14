using Coderaw.Settings.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Coderaw.Settings.Extensions.Cors
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCors(
            this IServiceCollection services,
            string policyName,
            Func<CorsSettings> getSettings, IWebHostEnvironment env)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    if (env.IsDevelopment())
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
                            var host = uri.Host.ToLowerInvariant();
                            var settings = getSettings();

                            var allowedHosts = settings.AllowedHosts?.Select(h => h.ToLowerInvariant()) ?? [];
                            var allowedIps = settings.AllowedIPs ?? [];

                            if (allowedHosts.Contains(host))
                                return true;

                            var resolvedIps = Dns.GetHostAddresses(host);
                            if (resolvedIps.Any(ip => allowedIps.Contains(ip.ToString())))
                                return true;

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

