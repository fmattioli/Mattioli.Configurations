using Mattioli.Configurations.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Sentry.OpenTelemetry;

namespace Mattioli.Configurations.Extensions.Sentry
{
    public static class SentryExtension
    {
        public static WebApplicationBuilder UseMltSentry(this WebApplicationBuilder builder, MltSettings mltSettings)
        {
            if (string.IsNullOrWhiteSpace(mltSettings.Dsn))
            {
                return builder;
            }
            else
            {
                builder.WebHost.UseSentry(options =>
                {
                    options.Dsn = mltSettings.Dsn;
                    options.TracesSampleRate = mltSettings.TracesSampleRate > 0 ? mltSettings.TracesSampleRate : 1.0;
                    options.AttachStacktrace = mltSettings.AttachStacktrace;

                    if (!string.IsNullOrWhiteSpace(mltSettings.DiagnosticLevel)
                        && Enum.TryParse<SentryLevel>(mltSettings.DiagnosticLevel, true, out var level))
                    {
                        options.DiagnosticLevel = level;
                        options.Debug = level == SentryLevel.Debug;
                    }

                    options.UseOpenTelemetry();
                });
            }

            return builder;
        }
    }
}
