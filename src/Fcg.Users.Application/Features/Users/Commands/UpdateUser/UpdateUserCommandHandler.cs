using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        public UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, 
            IUnitOfWork unitOfWork,ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository; 
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de atualização de usuário. UserId: {UserId}", request.UserId);
            var nomeValueObject = new Name(request.Name);
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) 
            {
                _logger.LogWarning("[UserAPI] Falha na atualização. Usuário não encontrado no banco de dados. UserId: {UserId}", request.UserId);
                throw new DomainException(DomainMessages.UserNotFound);
            }

            if (await _userRepository.CheckNameInUseAsync(request.UserId, request.Name))
            {                
                _logger.LogWarning("[UserAPI] Falha na atualização. O nome de usuário '{Name}' já está em uso por outra conta. UserId: {UserId}", request.Name, request.UserId);
                throw new DomainException(DomainMessages.UserNameAlreadyRegistered);
            }

            var senha = new Password(request.Password);
                       
            var hashSenha = _passwordHasher.HashPassword(request.Password);

            var novaSenhaCriptografa = new Password(hashSenha);

            var novoUserVO = new Name(request.Name);

            user.Update(novoUserVO, novaSenhaCriptografa);

            _userRepository.Update(user);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuário atualizado com sucesso. UserId: {UserId}", request.UserId);

            return new UserResponse
            {
                Name = user.Name.Valor,
                Email = user.Email.Valor,
                PerfilUser = user.Role
            };
        }

    }
}
