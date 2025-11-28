using Mattioli.Configurations.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Mattioli.Configurations.Extensions.Middlewares;

public static class MiddlewaresExtensions
{
    public static IApplicationBuilder UseCorrelationIdEnricherMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdEnricherMiddleware>();

        return app;
    }
}
