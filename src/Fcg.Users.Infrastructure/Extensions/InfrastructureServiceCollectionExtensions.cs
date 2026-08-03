using Dapper;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Infrastructure.Persistence;
using Fcg.Users.Infrastructure.Queries;
using Fcg.Users.Infrastructure.Queries.DapperHandlers;
using Fcg.Users.Infrastructure.Repository;
using Fcg.Users.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Fcg.Users.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMassTransitExtension(configuration);    
            services.AddDbContextExtension(configuration);
            services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<UserDbContext>().Database.GetDbConnection());
            SqlMapper.AddTypeHandler(new NameTypeHandler());
            SqlMapper.AddTypeHandler(new EmailTypeHandler());
            SqlMapper.AddTypeHandler(new PasswordTypeHandler());
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminQueryRepository, AdminQueryRepository>();
            return services;
        }
    }
}
