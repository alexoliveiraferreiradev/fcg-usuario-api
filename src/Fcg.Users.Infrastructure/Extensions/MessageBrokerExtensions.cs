using Fcg.Users.Infrastructure.MessageBroker;
using Fcg.Users.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fcg.Users.Infrastructure.Extensions
{
    internal static class MessageBrokerExtensions
    {
        public static IServiceCollection AddMassTransitExtension(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));
            services.AddMassTransit(x =>
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
            return services;
        }
    }
}
