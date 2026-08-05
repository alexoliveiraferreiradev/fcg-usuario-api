using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.DeactivateAccount
{
    public class DeactiveUserCommandHandler : IRequestHandler<DeactiveAccountCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactiveUserCommandHandler> _logger;
        private readonly IIntegrationEventPublisher _integrationEventPublisher;

        public DeactiveUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
            ILogger<DeactiveUserCommandHandler> logger, IIntegrationEventPublisher integrationEventPublisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _integrationEventPublisher = integrationEventPublisher;
        }
        public async Task Handle(DeactiveAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de desativação de conta. UserId: {UserId}", request.Id);

            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
            {                
                _logger.LogWarning("[UserAPI] Falha na desativação. Usuário não encontrado no banco de dados. UserId: {UserId}", request.Id);
                throw new DomainException(DomainMessages.UserNotFound);
            }

            if (user.Role == UserRole.Admin)
            {             
                var existeOutroAdmin = await _userRepository.HasMultipleAdminsAsync();
                if (!existeOutroAdmin)
                {             
                    _logger.LogWarning("[UserAPI] Falha na desativação. Não é possível desativar o único Admin cadastrado. UserId: {UserId}", request.Id);
                    throw new DomainException(DomainMessages.InvalidDeactivateAdminOperation);
                }
            }
            user.DeactivateAccount();

            _userRepository.Update(user);

            await _integrationEventPublisher.PublishAsync<IUserDeactivatedIntegrationEvent>(new
            {
                UserId = user.Id
            }, cancellationToken);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Conta do usuário desativada com sucesso. UserId: {UserId}", request.Id);
        }
    }
}
