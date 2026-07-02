using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, LoginResponse>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AuthenticateUserCommandHandler> _logger;

        public AuthenticateUserCommandHandler(IUserRepository UserRepository, IPasswordHasher passwordHasher,
            ITokenService tokenService, IPublishEndpoint publishEndpoint, ILogger<AuthenticateUserCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<LoginResponse> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando tentativa de autenticação para o e-mail {Email}.", request.Email);

            var User = await _UserRepository.GetByEmailAsync(request.Email);
            if (User == null)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: E-mail {Email} não encontrado no banco de dados.", request.Email);

                throw new DomainException(DomainMessages.InvalidCredentials);
            }

            if (!User.IsActive)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: A conta vinculada ao e-mail {Email} (ID: {UserId}) encontra-se inativa.", request.Email, User.Id);
                throw new DomainException(DomainMessages.UserMustBeActive);
            }

            bool senhaValida = _passwordHasher.VerifyPassword(request.Password, User.Password.Hash);
            if (!senhaValida)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: Password inválida fornecida para o e-mail {Email} (ID: {UserId}).", request.Email, User.Id);
                throw new DomainException(DomainMessages.InvalidCredentials);
            }

            var UserResponse = new UserResponse
            {
                Id = User.Id,
                Name = User.Name.Valor,
                Email = User.Email.Valor,
                PerfilUser = User.Role
            };

            var tokenResult = await _tokenService.GenerateToken(UserResponse);

            _logger.LogInformation("[UserAPI] Login realizado com sucesso. UserId: {UserId}, Email: {Email}", User.Id, User.Email.Valor);

            return new LoginResponse
            {
                AcessToken = tokenResult.AccessToken,
                ExpiresIn = tokenResult.ExpiresIn,
                Id = User.Id.ToString(),
                Email = User.Email.Valor,
                PerfilUser = User.Role,
                Claims = tokenResult.Claims
            };
        }

    }
}
