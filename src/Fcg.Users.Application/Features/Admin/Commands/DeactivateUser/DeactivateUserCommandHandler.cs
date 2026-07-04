using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.DeactiveUser
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivateUserCommandHandler> _logger;

        public DeactivateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DeactivateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de desativação de usuário por um operador. UserId: {UserId}, OperadorId: {OperadorId}", request.Id, request.IdOperador);

            var user = await _userRepository.GetByIdAsync(request.Id);

            if(request.Id == request.IdOperador)
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

            await _unitOfWork.CommitAsync();

            _logger.LogWarning("[UserAPI] Usuário desativado com sucesso pelo operador. UserId: {UserId}, OperadorId: {OperadorId}, motivo: {reason}", request.Id, request.IdOperador, request.ReasonDeactivation);
        }
    }
}
