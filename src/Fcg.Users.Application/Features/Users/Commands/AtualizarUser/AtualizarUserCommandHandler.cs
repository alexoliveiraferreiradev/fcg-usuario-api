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
    public class AtualizarUserCommandHandler : IRequestHandler<AtualizarUserCommand, UserResponse>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AtualizarUserCommandHandler> _logger;
        public AtualizarUserCommandHandler(IUserRepository UserRepository, IPasswordHasher passwordHasher, 
            IUnitOfWork unitOfWork,ILogger<AtualizarUserCommandHandler> logger)
        {
            _UserRepository = UserRepository; 
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<UserResponse> Handle(AtualizarUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de atualização de usuário. UserId: {UserId}", request.UserId);

            var User = await _UserRepository.ObterPorId(request.UserId);
            if (User == null) 
            {
                _logger.LogWarning("[UserAPI] Falha na atualização. Usuário não encontrado no banco de dados. UserId: {UserId}", request.UserId);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (await _UserRepository.VerificaNomeCadastradoParaAlteracao(request.UserId, request.NomeUser))
            {                
                _logger.LogWarning("[UserAPI] Falha na atualização. O nome de usuário '{NomeUser}' já está em uso por outra conta. UserId: {UserId}", request.NomeUser, request.UserId);
                throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            }

            if (request.SenhaUser != request.ConfirmacaoSenha) throw new DomainException(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);

            var hashSenha = _passwordHasher.HashPassword(request.SenhaUser);

            var novaSenhaCriptografa = new Senha(request.SenhaUser,hashSenha);

            var novoUserVO = new Nome(request.NomeUser);

            User.Atualizar(novoUserVO, novaSenhaCriptografa);

            _UserRepository.Atualizar(User);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuário atualizado com sucesso. UserId: {UserId}", request.UserId);

            return new UserResponse
            {
                Nome = User.NomeUser.Valor,
                Email = User.EmailUser.Valor,
                PerfilUser = User.Perfil
            };
        }
    }
}
