using Mattioli.Configurations.Extensions.Loggings;
using Mattioli.Configurations.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenTelemetry;

namespace Mattioli.Configurations.Extensions.Loggings
{
    public static class SerilogExtensions
    {
        public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestContextLoggingMiddleware>();
            app.UseSerilogRequestLogging();

            return app;
        }

        public static IHostBuilder UseSerilog(this IHostBuilder builder, string collectorUrl, string serviceName)
        {
            return builder.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .Enrich.WithExceptionDetails();

                if (context.HostingEnvironment.IsDevelopment())
                {
                    loggerConfiguration
                        .MinimumLevel.Debug()
                        .WriteTo.Console(outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss} | Level {Level} | CorrelationId: {CorrelationId} | RequestPath: {RequestPath} | Env: {Environment} | {SourceContext} | {Message} | {Exception}{NewLine}");
                }
                else
                {
                    loggerConfiguration
                        .MinimumLevel.Information()
                        .WriteTo.Console(outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level} | CorrelationId: {CorrelationId} Trace: {TraceId} | RequestPath: {RequestPath} | Env: {Environment} | {SourceContext} | {Message} | {Exception}{NewLine}")
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
