using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.PromoverUserParaAdmin
{
    public class PromoverUserParaAdminCommandHandler : IRequestHandler<PromoverUserParaAdminCommand, UserResponse>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PromoverUserParaAdminCommandHandler> _logger;

        public PromoverUserParaAdminCommandHandler(IUserRepository UserRepository, IUnitOfWork unitOfWork, ILogger<PromoverUserParaAdminCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserResponse> Handle(PromoverUserParaAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de promoção de usuário para Admin. UserId: {UserId}", request.Id);

            var User = await _UserRepository.GetByIdAsync(request.Id);
            if (User == null)
            {
                _logger.LogWarning("[UserAPI] Falha na promoção. Usuário não encontrado. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (request.Id == request.IdOperador)
            {
                _logger.LogWarning("[UserAPI] Tentativa de autopromoção bloqueada. AdminId: {AdminId}", request.IdOperador);
                throw new DomainException("Um Admin não pode promover a si próprio.");
            }

            if (!User.IsActive)
            {
                _logger.LogWarning("[UserAPI] Falha na promoção. Usuário está inativo. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }

            if (User.Role.Equals(UserRole.Admin))
            {
                _logger.LogWarning("[UserAPI] Falha na promoção. O usuário já é um Admin. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            User.PromoteRole();

            _UserRepository.Update(User);

            await _unitOfWork.CommitAsync();

            _logger.LogWarning("[UserAPI] [AUDITORIA] Usuário {TargetUserId} promovido a Admin pelo operador {AdminId}.", request.Id, request.IdOperador);

            return new UserResponse
            {
                Id = User.Id,
                Email = User.Email.Valor,
                Name = User.Name.Valor,
                PerfilUser = User.Role
            };
        }
    }
}
