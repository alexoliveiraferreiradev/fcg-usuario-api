using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.RepublishUsers
{
    public class RepublishUsersCommandHandler : IRequestHandler<RepublishUsersCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RepublishUsersCommandHandler> _logger;
        private readonly IIntegrationEventPublisher _integrationEventPublisher;

        public RepublishUsersCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
            ILogger<RepublishUsersCommandHandler> logger, IIntegrationEventPublisher integrationEventPublisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _integrationEventPublisher = integrationEventPublisher;
        }

        public async Task<int> Handle(RepublishUsersCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando republicação dos usuários.");

            var users = (await _userRepository.GetAllAsync()).ToList();

            if (users.Count == 0)
            {
                _logger.LogInformation("[UserAPI] Nenhum usuário encontrado para republicar.");
                return 0;
            }

            foreach (var user in users)
            {
                await _integrationEventPublisher.PublishAsync<IRepublishUserIntegrationEvent>(new
                {
                    EventId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id,
                    Name = user.Name.Value,
                    Email = user.Email.Value,
                }, cancellationToken);
            }

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuários republicados com sucesso. Total: {Total}", users.Count);
            return users.Count;
        }
    }
}
