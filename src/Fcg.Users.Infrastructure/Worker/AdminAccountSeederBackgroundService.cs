using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.ValueObjects;
using Fcg.Users.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fcg.Users.Infrastructure.Worker
{
    public class AdminAccountSeederBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public AdminAccountSeederBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var adminEmail = "admin@fiapcloudgames.com.br";
                        
            var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.Value == adminEmail, stoppingToken);

            if (adminUser == null)
            {
                var nameVo = new Name("Admin Sistema");
                var emailVo = new Email(adminEmail);
                var password = new Password("$2a$11$Soy4TsNUDtuazT6CJulPleFnp82cF5BkICiOmF9sk19x0X6pMAic.");

                adminUser = new User(nameVo, emailVo, password);
                adminUser.PromoteRole();

                dbContext.Users.Add(adminUser);
                await dbContext.SaveChangesAsync(stoppingToken); 
            }
                       
            var userCreatedEvent = new UserCreatedEvent(adminUser.Id, adminUser.Name.Value, adminUser.Email.Value);
            await publishEndpoint.Publish(userCreatedEvent, stoppingToken);
        }
    }
}
