using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.Metrics;

namespace Coderaw.Settings.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CounterMetricsAttribute : Attribute, IActionFilter
    {
        private readonly Counter<long> _requestCounter;

        public CounterMetricsAttribute(string metricName, string applicationName)
        {
            var meter = new Meter(applicationName);

            _requestCounter = meter.CreateCounter<long>(
                        name: "RequestCounter_" + metricName,
                        unit: "Requests",
                        description: "Count total of requests"
                    );
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerName = context.Controller.GetType().Name;
            var actionName = context.ActionDescriptor.DisplayName;

            _requestCounter.Add(1, new("Controller", controllerName), new("Action", actionName));
        }
    }
}
