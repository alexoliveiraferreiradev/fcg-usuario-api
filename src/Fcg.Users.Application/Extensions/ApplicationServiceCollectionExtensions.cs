using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using FluentValidation;
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
            return services;
        }
    }
}
