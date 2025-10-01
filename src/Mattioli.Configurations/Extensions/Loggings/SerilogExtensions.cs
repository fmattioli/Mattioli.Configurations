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
            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (httpContext, elapsed, ex) =>
                {
                    var statusCode = httpContext.Response.StatusCode;

                    if (statusCode >= 500)
                    {
                        return Serilog.Events.LogEventLevel.Error;
                    }
                    else if (statusCode >= 400)
                    {
                        return Serilog.Events.LogEventLevel.Warning;
                    }
                    else
                    {
                        return Serilog.Events.LogEventLevel.Information;
                    }
                };

                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} -> {StatusCode} in {Elapsed:0}ms";
            });


            return app;
        }

        public static IHostBuilder UseSerilog(this IHostBuilder builder, string collectorUrl, string serviceName, string seqUrl)
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
                        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                        .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning)
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
                        })
                        .WriteTo.Seq(seqUrl);
                }
            });
        }
    }
}
