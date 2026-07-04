using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.DeactivateAccount
{
    public class DesativarContaCommandHandler : IRequestHandler<DesativarContaCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarContaCommandHandler> _logger;

        public DesativarContaCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DesativarContaCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(DesativarContaCommand request, CancellationToken cancellationToken)
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
                    _logger.LogWarning("[UserAPI] Falha na desativação. Não é possível Deactivate o único Admin cadastrado. UserId: {UserId}", request.Id);
                    throw new DomainException(DomainMessages.InvalidDeactivateAdminOperation);
                }
            }
            user.DeactivateAccount();

            _userRepository.Update(user);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Conta do usuário desativada com sucesso. UserId: {UserId}", request.Id);
        }
    }
}
