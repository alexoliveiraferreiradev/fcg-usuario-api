using Dapper;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.WebApi.Security;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Infrastructure.MessageBroker;
using Fcg.Users.Infrastructure.Persistence;
using Fcg.Users.Infrastructure.Queries;
using Fcg.Users.Infrastructure.Queries.DapperHandlers;
using Fcg.Users.Infrastructure.Repository;
using Fcg.Users.Infrastructure.Security;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
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
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<UserDbContext>(
                name: "database-healthcheck",
                tags: new[] {"ready"});

            return builder;
        }

        private static WebApplicationBuilder AddDbContextExtension(this WebApplicationBuilder builder)
        {
            var dbConfig = builder.Configuration.GetSection(DatabaseSettings.DatabaseSettingsSection).Get<DatabaseSettings>();
            ArgumentNullException.ThrowIfNull(dbConfig, nameof(DatabaseSettings));

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $"{dbConfig.Host},{dbConfig.Port}",
                InitialCatalog = dbConfig.DatabaseName,
                UserID = dbConfig.Username,
                Password = dbConfig.Password,
                TrustServerCertificate = true,
                Encrypt = false
            };

            builder.Services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(connectionStringBuilder.ConnectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });
            return builder;
        }

        private static WebApplicationBuilder AddMassTransitExtension(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));
            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<UserDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, "/", h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });
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
