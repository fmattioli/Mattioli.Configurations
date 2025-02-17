using Coderaw.Settings.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Coderaw.Settings.Extensions.Controllers
{
    public static class ControllerExtensions
    {
        public static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add<ValidationsFilterAttribute>();
        }

        public static void ConfigureNewtonsoftJson(MvcNewtonsoftJsonOptions options)
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        }

        public static void ConfigureApiBehaviorOptions(ApiBehaviorOptions options)
        {
            options.SuppressModelStateInvalidFilter = true;
            options.SuppressInferBindingSourcesForParameters = true;
        }
    }
}
