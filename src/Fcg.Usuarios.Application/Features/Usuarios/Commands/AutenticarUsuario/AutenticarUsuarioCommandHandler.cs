using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario
{
    public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, LoginResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AutenticarUsuarioCommandHandler> _logger;

        public AutenticarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher,
            ITokenService tokenService, IPublishEndpoint publishEndpoint, ILogger<AutenticarUsuarioCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<LoginResponse> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando tentativa de autenticação para o e-mail {Email}.", request.Email);

            var usuario = await _usuarioRepository.ObterPorEmail(request.Email);
            if (usuario == null)
            {
                _logger.LogWarning("Falha de autenticação: E-mail {Email} não encontrado no banco de dados.", request.Email);

                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            if (!usuario.Ativo)
            {
                _logger.LogWarning("Falha de autenticação: A conta vinculada ao e-mail {Email} (ID: {UsuarioId}) encontra-se inativa.", request.Email, usuario.Id);
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }

            bool senhaValida = _passwordHasher.VerifyPassword(request.Senha, usuario.Senha.Hash);
            if (!senhaValida)
            {
                _logger.LogWarning("Falha de autenticação: Senha inválida fornecida para o e-mail {Email} (ID: {UsuarioId}).", request.Email, usuario.Id);
                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            var usuarioResponse = new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.NomeUsuario.Valor,
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };

            var tokenResult = await _tokenService.GerarToken(usuarioResponse);

            _logger.LogInformation("Login realizado com sucesso. UsuarioId: {UserId}, Email: {Email}", usuario.Id, usuario.EmailUsuario.Valor);

            return new LoginResponse
            {
                AcessToken = tokenResult.AccessToken,
                ExpiresIn = tokenResult.ExpiresIn,
                Id = usuario.Id.ToString(),
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil,
                Claims = tokenResult.Claims
            };
        }

    }
}
