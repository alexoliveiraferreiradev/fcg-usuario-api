using Serilog;

namespace Fcg.User.API.Extensions
{
    public static class ObservabilityExtension
    {
        public static WebApplicationBuilder AddSerilogExtension(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            return builder;
        }
    }
}
