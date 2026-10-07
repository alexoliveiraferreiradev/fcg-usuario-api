using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Users.Application.Extensions;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.DeactivateUser
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivateUserCommandHandler> _logger;
        private readonly IIntegrationEventPublisher _integrationEventPublisher;

        public DeactivateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
            ILogger<DeactivateUserCommandHandler> logger, IIntegrationEventPublisher integrationEventPublisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _integrationEventPublisher = integrationEventPublisher;
        }
        public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                _logger.LogInformation("[UserAPI] Iniciando processo de desativação de usuário por um operador. UserId: {UserId}, OperadorId: {OperadorId}", request.Id, request.IdOperador);

                var user = await _userRepository.GetByIdAsync(request.Id);

                if (request.Id == request.IdOperador)
                {
                    _logger.LogWarning("[UserAPI] Tentativa de auto-inativação bloqueada para o operador {IdOperador}.", request.IdOperador);
                    throw new DomainException(DomainMessages.InvalidDeactivateOperation);
                }

                if (user == null)
                {
                    _logger.LogWarning("[UserAPI] Falha na desativação. Usuário alvo não encontrado. UserId: {UserId}", request.Id);
                    throw new DomainException(DomainMessages.UserNotFound);
                }

                user.Deactivate(request.ReasonDeactivation);

                await _integrationEventPublisher.PublishAsync<IUserDeactivatedIntegrationEvent>(new
                {
                    EventId = Guid.NewGuid(),
                    CreatedAt = user.CreatedAt,
                    UserId = user.Id
                }, cancellationToken);

                await _unitOfWork.CommitAsync();

                AuthMetrics.AdminActions.WithLabels("deactivate", "success").Inc();

                _logger.LogInformation("[UserAPI] Usuário desativado com sucesso pelo operador. UserId: {UserId}, OperadorId: {OperadorId}, motivo: {reason}", request.Id, request.IdOperador, request.ReasonDeactivation);
            }
            catch (DomainException)
            {
                AuthMetrics.AdminActions.WithLabels("deactivate", "failure").Inc();
                throw;
            }
        }
    }
}
