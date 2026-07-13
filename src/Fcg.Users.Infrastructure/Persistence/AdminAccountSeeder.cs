using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.ValueObjects;
using MassTransit;

namespace Fcg.Users.Infrastructure.Persistence
{
    public class AdminAccountSeeder : ISeedAdminAccount
    {
        private readonly UserDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public AdminAccountSeeder(UserDbContext dbContext,
            IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task SeedAsync()
        {
            if (!_dbContext.Users.Any(u => u.Name.Value == "Admin Sistema"))
            {
                var nameVo = new Name("Admin Sistema");
                var emailVo = new Email("admin@fiapcloudgames.com.br");
                var password = new Password("$2a$11$Soy4TsNUDtuazT6CJulPleFnp82cF5BkICiOmF9sk19x0X6pMAic.");
                var adminUser = new User(nameVo, emailVo, password);
                adminUser.PromoteRole();
                _dbContext.Users.Add(adminUser);
                await _dbContext.SaveChangesAsync();

                var userCreatedEvent = new UserCreatedEvent(adminUser.Id, adminUser.Name.Value, adminUser.Email.Value);
                await _publishEndpoint.Publish(userCreatedEvent);

            }

        }
    }
}
