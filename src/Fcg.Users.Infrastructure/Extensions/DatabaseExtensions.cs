using Dapper;
using Fcg.Core.WebApi.Database;
using Fcg.Users.Infrastructure.Persistence;
using Fcg.Users.Infrastructure.Queries.DapperHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Users.Infrastructure.Extensions
{
    internal static class DatabaseExtensions
    {
        public static IServiceCollection AddDataBasextension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<DatabaseConnectionSettings>()
                .Bind(configuration.GetSection(DatabaseConnectionSettings.DatabaseSettingsSection))
                .ValidateDataAnnotations().ValidateOnStart();

            var dbConfig = configuration.GetSection(DatabaseConnectionSettings.DatabaseSettingsSection).Get<DatabaseConnectionSettings>();
            ArgumentNullException.ThrowIfNull(dbConfig, nameof(DatabaseConnectionSettings));


            SqlMapper.AddTypeHandler(new NameTypeHandler());
            SqlMapper.AddTypeHandler(new EmailTypeHandler());
            SqlMapper.AddTypeHandler(new PasswordTypeHandler());

            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(dbConfig.ToConnectionString(), sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });
            return services;
        }
    }
}
