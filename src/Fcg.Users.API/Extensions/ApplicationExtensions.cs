using Fcg.Core.WebApi.Middleware;
using Fcg.User.API.Endpoint.Admin;
using Fcg.User.API.Endpoint.User;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Entitites;
using Serilog;

namespace Fcg.User.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static async Task<WebApplication> AddAplicationExtension(this WebApplication app)
        {
            await app.SeedAdminAccount();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();
            app.ConfigureEndpoints();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseRouting();
            app.UseSwaggerExtension();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;

        }

        private async static Task<WebApplication> SeedAdminAccount(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ISeedAdminAccount>();
                await seeder.SeedAsync();
                return app;
            }
        }

        private static WebApplication ConfigureEndpoints(this WebApplication app)
        {
            #region Conta - Player/Admin
            app.MapAcessUserEndpoint();
            app.MapManageAccountEndpoints();
            #endregion

            #region Admin - Gerenciamento de Usuários
            app.MapManageUserEndpoints();
            #endregion

            return app;
        }
    }
}
