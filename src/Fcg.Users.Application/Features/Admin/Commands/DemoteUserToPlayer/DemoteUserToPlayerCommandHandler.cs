using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.DemoteUserToPlayer
{
    public class DemoteUserToPlayerCommandHandler : IRequestHandler<DemoteUserToPlayerCommand, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DemoteUserToPlayerCommandHandler> _logger;

        public DemoteUserToPlayerCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DemoteUserToPlayerCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<UserResponse> Handle(DemoteUserToPlayerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de rebaixamento de usuário para Player. UserId: {UserId}, OperadorId: {OperadorId}", request.Id, request.IdOperador);

            if (request.Id == request.IdOperador)
            {                
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. Um Admin não pode rebaixar a própria conta. OperadorId: {OperadorId}", request.IdOperador);
                throw new DomainException(DomainMessages.InvalidDemoteOperation);
            }

            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. Usuário alvo não encontrado. UserId: {UserId}", request.Id);
                throw new DomainException(DomainMessages.UserNotFound);
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. Usuário está inativo. UserId: {UserId}", request.Id);
                throw new DomainException(DomainMessages.UserMustBeActive);
            }

            if (user.Role.Equals(UserRole.Player))
            {                
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. O usuário alvo já é um Player. UserId: {UserId}", request.Id);
                throw new DomainException(DomainMessages.UserProfileDemoteInvalid);
            }

            user.DemoteRole();

            _userRepository.Update(user);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuário rebaixado para Player com sucesso. UserId: {UserId}", request.Id);

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name.Valor,
                Email = user.Email.Valor,
                PerfilUser = user.Role
            };
        }
    }
}
