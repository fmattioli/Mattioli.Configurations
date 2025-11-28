using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
namespace Mattioli.Configurations.Middlewares;

internal class CorrelationIdEnricherMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = GetCorrelationId(context);

        context.Request.Headers[CorrelationIdHeaderName] = correlationId;

        await _next.Invoke(context);
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationIdHeaderName, out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
