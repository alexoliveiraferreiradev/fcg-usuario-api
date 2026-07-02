using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.AutenticarUser
{
    public class AutenticarUserCommandHandler : IRequestHandler<AutenticarUserCommand, LoginResponse>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AutenticarUserCommandHandler> _logger;

        public AutenticarUserCommandHandler(IUserRepository UserRepository, IPasswordHasher passwordHasher,
            ITokenService tokenService, IPublishEndpoint publishEndpoint, ILogger<AutenticarUserCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<LoginResponse> Handle(AutenticarUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando tentativa de autenticação para o e-mail {Email}.", request.Email);

            var User = await _UserRepository.ObterPorEmail(request.Email);
            if (User == null)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: E-mail {Email} não encontrado no banco de dados.", request.Email);

                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            if (!User.Ativo)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: A conta vinculada ao e-mail {Email} (ID: {UserId}) encontra-se inativa.", request.Email, User.Id);
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }

            bool senhaValida = _passwordHasher.VerifyPassword(request.Senha, User.Senha.Hash);
            if (!senhaValida)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: Senha inválida fornecida para o e-mail {Email} (ID: {UserId}).", request.Email, User.Id);
                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            var UserResponse = new UserResponse
            {
                Id = User.Id,
                Nome = User.NomeUser.Valor,
                Email = User.EmailUser.Valor,
                PerfilUser = User.Perfil
            };

            var tokenResult = await _tokenService.GerarToken(UserResponse);

            _logger.LogInformation("[UserAPI] Login realizado com sucesso. UserId: {UserId}, Email: {Email}", User.Id, User.EmailUser.Valor);

            return new LoginResponse
            {
                AcessToken = tokenResult.AccessToken,
                ExpiresIn = tokenResult.ExpiresIn,
                Id = User.Id.ToString(),
                Email = User.EmailUser.Valor,
                PerfilUser = User.Perfil,
                Claims = tokenResult.Claims
            };
        }

    }
}
