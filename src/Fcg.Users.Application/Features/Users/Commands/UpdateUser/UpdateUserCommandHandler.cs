using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.AutenticarUser;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.AtualizarUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        public UpdateUserCommandHandler(IUserRepository UserRepository, IPasswordHasher passwordHasher, 
            IUnitOfWork unitOfWork,ILogger<UpdateUserCommandHandler> logger)
        {
            _UserRepository = UserRepository; 
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de atualização de usuário. UserId: {UserId}", request.UserId);

            var User = await _UserRepository.GetByIdAsync(request.UserId);
            if (User == null) 
            {
                _logger.LogWarning("[UserAPI] Falha na atualização. Usuário não encontrado no banco de dados. UserId: {UserId}", request.UserId);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (await _UserRepository.CheckNameInUseAsync(request.UserId, request.Name))
            {                
                _logger.LogWarning("[UserAPI] Falha na atualização. O Name de usuário '{Name}' já está em uso por outra conta. UserId: {UserId}", request.Name, request.UserId);
                throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            }

            if (request.password != request.ConfirmacaoSenha) throw new DomainException(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);

            var hashSenha = _passwordHasher.HashPassword(request.password);

            var novaSenhaCriptografa = new Password(request.password,hashSenha);

            var novoUserVO = new Name(request.Name);

            User.Update(novoUserVO, novaSenhaCriptografa);

            _UserRepository.Update(User);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuário atualizado com sucesso. UserId: {UserId}", request.UserId);

            return new UserResponse
            {
                Name = User.Name.Valor,
                Email = User.Email.Valor,
                PerfilUser = User.Role
            };
        }
    }
}
