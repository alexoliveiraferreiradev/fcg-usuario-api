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
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticateUserCommandHandler> _logger;

        public AuthenticateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher,
            ITokenService tokenService,  ILogger<AuthenticateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }
        public async Task<LoginResponse> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando tentativa de autenticação para o e-mail {Email}.", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: E-mail {Email} não encontrado no banco de dados.", request.Email);

                throw new DomainException(DomainMessages.InvalidCredentials);
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: A conta vinculada ao e-mail {Email} (ID: {UserId}) encontra-se inativa.", request.Email, user.Id);
                throw new DomainException(DomainMessages.UserMustBeActive);
            }

            bool senhaValida = _passwordHasher.VerifyPassword(request.Password, user.Password.Hash);
            if (!senhaValida)
            {
                _logger.LogWarning("[UserAPI] Falha de autenticação: Senha inválida fornecida para o e-mail {Email} (ID: {UserId}).", request.Email, user.Id);
                throw new DomainException(DomainMessages.InvalidCredentials);
            }

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Name = user.Name.Valor,
                Email = user.Email.Valor,
                PerfilUser = user.Role
            };

            var tokenResult = await _tokenService.GenerateToken(userResponse);

            _logger.LogInformation("[UserAPI] Login realizado com sucesso. UserId: {UserId}, Email: {Email}", user.Id, user.Email.Valor);

            return new LoginResponse
            {
                AcessToken = tokenResult.AccessToken,
                ExpiresIn = tokenResult.ExpiresIn,
                Id = user.Id.ToString(),
                Email = user.Email.Valor,
                PerfilUser = user.Role,
                Claims = tokenResult.Claims
            };
        }

    }
}
