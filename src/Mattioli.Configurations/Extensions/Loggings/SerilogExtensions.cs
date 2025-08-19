using Mattioli.Configurations.Extensions.Loggings;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.OpenTelemetry;

namespace Mattioli.Configurations.Extensions.Loggings
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseSerilog(this IHostBuilder builder, string collectorUrl, string serviceName)
        {
            return builder.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Information)
                    .MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Warning)
                    .WriteTo.Console(new JsonFormatter(renderMessage: true))
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = collectorUrl;
                        options.Protocol = OtlpProtocol.Grpc;
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            { "service.name", serviceName },
                            { "environment", context.HostingEnvironment.EnvironmentName }
                        };
                    });
            });
        }
    }
}
