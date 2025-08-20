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
                        .WriteTo.Console(outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level} | Trace: {TraceId} | RequestPath: {RequestPath} | Env: {Environment} | {SourceContext} | {Message} | {Exception}{NewLine}");
                }
                else
                {
                    loggerConfiguration
                        .MinimumLevel.Information()                        
                        .WriteTo.Console(outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level} | Trace: {TraceId} | RequestPath: {RequestPath} | Env: {Environment} | {SourceContext} | {Message} | {Exception}{NewLine}")
                        .WriteTo.OpenTelemetry(options =>
                        {
                            options.Endpoint = collectorUrl;
                            options.Protocol = OtlpProtocol.Grpc;
                            options.ResourceAttributes = new Dictionary<string, object>
                            {
                                { "service.name", serviceName }
                            };
                        });
                }
            });
        }
    }
}
