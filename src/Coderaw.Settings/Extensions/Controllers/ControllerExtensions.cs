using Coderaw.Settings.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Coderaw.Settings.Extensions.Controllers
{
    public static class ControllerExtensions
    {
        public static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add<ValidationsFilterAttribute>();
        }

        public static void ConfigureJsonOptions(JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public static void ConfigureApiBehaviorOptions(ApiBehaviorOptions options)
        {
            options.SuppressModelStateInvalidFilter = true;
            options.SuppressInferBindingSourcesForParameters = true;
        }
    }
}
