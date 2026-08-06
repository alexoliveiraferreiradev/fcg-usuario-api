using Fcg.Core.Abstractions.Common;
using Fcg.Users.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fcg.Users.Infrastructure.Extensions
{
    internal static class ObservabilityExtension
    {
        public static IServiceCollection AddHealthCheckExtension(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks()
              .AddCheck("live", () => HealthCheckResult.Healthy(), tags: new[] { HealthCheckTags.Live})
              .AddDbContextCheck<UserDbContext>(
              name: "database-healthcheck",
              tags: new[] { HealthCheckTags.Ready });

            return services;
        }
    }
}
