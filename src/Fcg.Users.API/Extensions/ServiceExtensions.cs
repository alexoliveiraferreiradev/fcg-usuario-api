using Dapper;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.WebApi.Security;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Infrastructure.Queries.DapperHandlers;
using Fcg.Users.Infrastructure.Queries;
using Fcg.Users.Infrastructure.Persistence;
using Fcg.Users.Infrastructure.Repository;
using Fcg.Users.Infrastructure.Security;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Data;
using System.Text;

namespace Fcg.User.API.Extensions
{
    public static class ServiceExtensions
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder
                .HealthCheckExtension()
                .AddSerilogExtension()
                .AddSwaggerService()
                .AddDbContextExtension()
                .AddMassTransitExtension()
                .AddCQRSExtension()
                .AddJwtExtension();

            

            SqlMapper.AddTypeHandler(new NameTypeHandler());
            SqlMapper.AddTypeHandler(new EmailTypeHandler());
            SqlMapper.AddTypeHandler(new PasswordTypeHandler());

            builder.DependencyInjection();

            return builder;
        }
        private static WebApplicationBuilder AddSerilogExtension(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            return builder;
        }

        private static WebApplicationBuilder HealthCheckExtension(this WebApplicationBuilder builder)
        {
            var sqlConnection = builder.Configuration.GetConnectionString("UserConnection");

            builder.Services.AddHealthChecks()
                .AddSqlServer(sqlConnection!);

            return builder;
        }

        private static WebApplicationBuilder AddDbContextExtension(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("UserConnection");

            builder.Services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            return builder;
        }

        private static WebApplicationBuilder AddMassTransitExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<UserDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
                    cfg.ConfigureEndpoints(context);
                });
            });
            return builder;
        }

        private static WebApplicationBuilder AddCQRSExtension(this WebApplicationBuilder builder) {
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);

            });


            builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);

            return builder;
        }

        private static WebApplicationBuilder AddJwtExtension(this WebApplicationBuilder builder) {
            var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            builder.Services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("AdminRole"));

                options.AddPolicy("GeneralAccess", policy => policy.RequireRole("AdminRole", "PlayerRole"));
            });

            return builder;
        }

        private static WebApplicationBuilder DependencyInjection(this WebApplicationBuilder builder) {
            builder.Services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<UserDbContext>().Database.GetDbConnection());
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ISeedAdminAccount,AdminAccountSeeder>(); 
            builder.Services.AddScoped<IAdminQueryRepository, AdminQueryRepository>();
            return builder;
        }
    }
}
