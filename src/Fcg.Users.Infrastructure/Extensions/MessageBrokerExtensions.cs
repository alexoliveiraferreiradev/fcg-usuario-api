using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Core.WebApi.MessageBroker;
using Fcg.Users.Infrastructure.MessageBroker;
using Fcg.Users.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fcg.Users.Infrastructure.Extensions
{
    internal static class MessageBrokerExtensions
    {
        public static IServiceCollection AddMassTransitExtension(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();

            services.AddOptions<AzureServiceBusSettings>().BindConfiguration(AzureServiceBusSettings.SectionName)
           .ValidateDataAnnotations().ValidateOnStart();
                      
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<UserDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    var azureServiceBusConfig = context.GetRequiredService<IOptions<AzureServiceBusSettings>>().Value;
                    cfg.Host(azureServiceBusConfig.ConnectionString);
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    cfg.Message<IUserCreatedIntegrationEvent>(m => m.SetEntityName(ServiceBusTopics.UserCreated));
                    cfg.Message<IRepublishUserIntegrationEvent>(m => m.SetEntityName(ServiceBusTopics.UserRepublished));
                    cfg.Message<IUserDeactivatedIntegrationEvent>(m => m.SetEntityName(ServiceBusTopics.UserDeactivated));
                    cfg.Message<IUserReactivatedIntegrationEvent>(m => m.SetEntityName(ServiceBusTopics.UserReactivated));

                    cfg.ConfigureEndpoints(context);
                });

            });
            return services;
        }
    }
}
