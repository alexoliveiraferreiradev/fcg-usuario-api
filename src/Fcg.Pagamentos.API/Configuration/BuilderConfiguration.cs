using Fcg.Usuarios.Infrastructure.Persistance;
using Fcg.Usuarios.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace Fcg.Pagamentos.API.Configuration
{
    public static class BuilderConfiguration
    {
        private static string connectionString;
        public static WebApplicationBuilder AddApiConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();
            AddDbContextConfig(builder);
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            AddJwtBearerConfiguration(builder);
            AddAuthorizationConfiguration(builder);
            builder.Services.AddEndpointsApiExplorer();
            return builder;
        }

        private static void AddDbContextConfig(WebApplicationBuilder builder)
        {
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }
       

        private static void AddJwtBearerConfiguration(WebApplicationBuilder builder)
        {
            var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettingsSection);

            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            //builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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
                    ValidIssuer = jwtSettings.Emissor,
                    ValidAudience = jwtSettings.ValidoEm
                };
            });
        }

        private static void AddAuthorizationConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AcessoGeral", policy =>
                    policy.RequireRole("AdminRole", "JogadorRole"));
            });
        }
    }
}
