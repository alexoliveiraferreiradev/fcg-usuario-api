
using FluentValidation;

namespace Fcg.Usuario.API.Endpoint
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        private readonly IValidator<T> _validator;
        public ValidationFilter(IValidator<T> validator) => _validator = validator;
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var argument = context.Arguments.OfType<T>().FirstOrDefault();
            if (argument is null) return Results.BadRequest("Corpo da requisição inválido.");
            var result = await _validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
            if (!result.IsValid) return Results.ValidationProblem(result.ToDictionary());
            return await next(context);
        }
    }
}
