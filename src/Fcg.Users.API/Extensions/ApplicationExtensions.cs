using Fcg.Core.WebApi.Middleware;
using Fcg.User.API.Endpoint.Admin;
using Fcg.User.API.Endpoint.User;

namespace Fcg.User.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication AddAplicationExtension(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.ConfigureEndpoints();
            
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseSwaggerExtension();  
            app.UseHttpsRedirection();

            app.UseAuthorization();

            return app;

        }

        public static WebApplication ConfigureEndpoints(this WebApplication app)
        {
            #region Conta - Player/Admin
            app.MapAcessUserEndpoint();
            app.MapManageAccountEndpoints();
            #endregion

            #region Admin - Gerenciamento de Usuários
            app.MapGerenciaUserEndpoints();
            #endregion
            return app;
        }
    }
}
