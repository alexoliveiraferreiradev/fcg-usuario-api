using Fcg.Core.Abstractions.Application;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Users.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);

            });
            services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
