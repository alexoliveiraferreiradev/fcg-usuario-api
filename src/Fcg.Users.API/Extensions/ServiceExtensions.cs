using Fcg.Users.Application.Extensions;
using Fcg.Users.Infrastructure.Extensions;

namespace Fcg.User.API.Extensions
{
    public static class ServiceExtensions
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.AddAuthorizationExtension()
                .AddSerilogExtension()
                .AddSwaggerExtension();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddAplicationServices();                     
            return builder;
        }
      
    }
}
