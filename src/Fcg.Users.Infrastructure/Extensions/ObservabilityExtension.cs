using Fcg.Users.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Users.Infrastructure.Extensions
{
    internal static class ObservabilityExtension
    {
        public static IServiceCollection AddHealthCheckExtension(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks()
              .AddDbContextCheck<UserDbContext>(
              name: "database-healthcheck",
              tags: new[] { "ready" });

            return services;
        }
    }
}
