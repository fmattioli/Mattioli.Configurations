using Coderaw.Settings.Extensions.Loggings;

using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace Coderaw.Settings.Extensions.Loggings
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseSerilog(this IHostBuilder builder, string colectorUrl, string serviceName)
        {
            return builder.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Information)
                    .MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Warning)
                    .WriteTo.Console(
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level} | Trace: {TraceId} | RequestPath: {RequestPath} | Env: {Environment} | {SourceContext} | {Message} | {Exception}{NewLine}")
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = colectorUrl;
                        options.Protocol = OtlpProtocol.Grpc;
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            { "service.name", serviceName }
                        };
                    });
            });
        }
    }
}
