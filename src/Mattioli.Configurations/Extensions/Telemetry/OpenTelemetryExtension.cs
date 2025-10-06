using Mattioli.Configurations.Extensions.Telemetry;
using Mattioli.Configurations.Models;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sentry.OpenTelemetry;

namespace Mattioli.Configurations.Extensions.Telemetry
{
    public static class OpenTelemetryExtension
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, MltSettings mltSettings)
        {
            var serviceName = mltSettings.ApplicationName;
            var otelExporterEndpoint = mltSettings.OpenTelemetryColectorUrl;

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(otelExporterEndpoint!))
                .WithTracing(builder =>
                {
                    builder
                        .AddSource(mltSettings.ApplicationName)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSentry();
                })
                .WithMetrics(builder =>
                {
                    builder
                        .AddMeter(mltSettings.ApplicationName)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddProcessInstrumentation();
                });

            return services;
        }
    }
}
