
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using MinimalAPIMovies.DTOs;

namespace MinimalAPIMovies.Filters
{
    public class GeneralFilters<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext
               .RequestServices.GetService<IValidator<T>>();

            if (validator is null)
            {
                return await next(context);
            }
            var obj = context.Arguments.OfType<T>().FirstOrDefault();
            if (obj is null)
            {
                return Results.Problem("The object could not be found");
            }

            var validationResult = await validator.ValidateAsync(obj);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            return await next(context);
        }
    }
}
