using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.PromoteUserToAdmin
{
    public class PromoteUserToAdminCommandHandler : IRequestHandler<PromoteUserToAdminCommand, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PromoteUserToAdminCommandHandler> _logger;

        public PromoteUserToAdminCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<PromoteUserToAdminCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserResponse> Handle(PromoteUserToAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de promoção de usuário para Admin. UserId: {UserId}", request.Id);

            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                _logger.LogWarning("[UserAPI] Falha na promoção. Usuário não encontrado. UserId: {UserId}", request.Id);
                throw new DomainException(DomainMessages.UserNotFound);
            }

            if (request.Id == request.IdOperador)
            {
                _logger.LogWarning("[UserAPI] Tentativa de autopromoção bloqueada. AdminId: {AdminId}", request.IdOperador);
                throw new DomainException("Um Admin não pode promover a si próprio.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("[UserAPI] Falha na promoção. Usuário está inativo. UserId: {UserId}", request.Id);
                throw new DomainException(DomainMessages.UserMustBeActive);
            }

            if (user.Role.Equals(UserRole.Admin))
            {
                _logger.LogWarning("[UserAPI] Falha na promoção. O usuário já é um Admin. UserId: {UserId}", request.Id);
                throw new DomainException(DomainMessages.UserProfileDemoteInvalid);
            }

            user.PromoteRole();

            _userRepository.Update(user);

            await _unitOfWork.CommitAsync();

            _logger.LogWarning("[UserAPI] [AUDITORIA] Usuário {TargetUserId} promovido a Admin pelo operador {AdminId}.", request.Id, request.IdOperador);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email.Valor,
                Name = user.Name.Valor,
                PerfilUser = user.Role
            };
        }
    }
}
