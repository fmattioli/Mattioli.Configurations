using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Coderaw.Settings.Extensions.Cors
{
    public static class ConfigureCorsExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    builder.SetIsOriginAllowed(origin =>
                    {
                        var host = new Uri(origin).Host;
                        var ipAddresses = Dns.GetHostAddresses(host);
                        var allowedIps = GetAllowedIps();

                        return ipAddresses.Any(ip => allowedIps.Contains(ip.ToString()));
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            return services;
        }

        private static List<string> GetAllowedIps()
        {
            return ["85.209.93.218"];
        }
    }
}

