using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Coderaw.Settings.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DurationMetricsAttribute : Attribute, IActionFilter
    {
        private readonly Histogram<double> _requestCounter;
        private Stopwatch _stopwatch = new();

        public DurationMetricsAttribute(string metricName, string applicationName)
        {
            Meter _meter = new(applicationName);
            _requestCounter = _meter.CreateHistogram<double>(
                    "RequestDuration_" + metricName,
                    description: "Request duration average",
                    unit: "ms"
                );
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            _requestCounter.Record(_stopwatch.ElapsedMilliseconds, tag: KeyValuePair.Create<string, object?>("Host", context.HttpContext.Request.Path));
        }
    }
}
