using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario
{
    public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, LoginResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        public AutenticarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;   
            _tokenService = tokenService;   
        }
        public async Task<LoginResponse> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorEmail(request.Email);
            if (usuario == null)
            {
                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            if (!usuario.Ativo)
            {               
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }

            bool senhaValida = _passwordHasher.VerifyPassword(request.Senha, usuario.Senha.Hash);
            if (!senhaValida)
            {                
                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            var usuarioResponse = new UsuarioResponse
            {
                Nome = usuario.NomeUsuario.Valor,
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };

            var tokenResult = await _tokenService.GerarToken(usuarioResponse);

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
