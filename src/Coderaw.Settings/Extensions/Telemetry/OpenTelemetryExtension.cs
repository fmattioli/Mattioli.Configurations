using Coderaw.Settings.Extensions.Telemetry;
using Coderaw.Settings.Models;

using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Coderaw.Settings.Extensions.Telemetry
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
                    .AddAspNetCoreInstrumentation();
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
