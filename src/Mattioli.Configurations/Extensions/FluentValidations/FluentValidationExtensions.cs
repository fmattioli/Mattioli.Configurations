using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Mattioli.Configurations.Extensions.FluentValidations
{
    public static class FluentValidationExtensions
    {
        public static IServiceCollection ConfigureValidationErrorResponses(this IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values
                        .Where(v => v.Errors.Count > 0)
                        .SelectMany(v => v.Errors)
                        .Select(v => v.ErrorMessage)
                        .ToList();

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "One or more validation errors occurred.",
                        Detail = string.Join(", ", errors)
                    };

                    return new BadRequestObjectResult(problemDetails);
                };
            });

            return serviceCollection;
        }

    }
}
