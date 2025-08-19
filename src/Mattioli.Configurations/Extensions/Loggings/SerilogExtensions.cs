using Mattioli.Configurations.Extensions.Loggings;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
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
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

                if (context.HostingEnvironment.IsDevelopment())
                {
                    loggerConfiguration
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("System", LogEventLevel.Information)
                        .WriteTo.Console(outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level} | Trace: {TraceId} | RequestPath: {RequestPath} | Env: {Environment} | {SourceContext} | {Message} | {Exception}{NewLine}");
                }
                else
                {
                    loggerConfiguration
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Warning)
                        .WriteTo.Console(outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level} | {Message} | {Exception}{NewLine}")
                        .WriteTo.OpenTelemetry(options =>
                        {
                            options.Endpoint = collectorUrl;
                            options.Protocol = OtlpProtocol.Grpc;
                            options.ResourceAttributes = new Dictionary<string, object>
                            {
                                { "serviceName", serviceName }
                            };
                        });
                }
            });
        }
    }
}
