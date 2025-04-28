using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Coderaw.Settings.Extensions.Cors
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCors(
            this IServiceCollection services,
            string policyName,
            Func<List<string>> getAllowedIps)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    builder.SetIsOriginAllowed(origin =>
                    {
                        var host = new Uri(origin).Host;
                        var ipAddresses = Dns.GetHostAddresses(host);
                        var allowedIps = getAllowedIps();

                        return ipAddresses.Any(ip => allowedIps.Contains(ip.ToString()));
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

