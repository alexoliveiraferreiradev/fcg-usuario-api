using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.PromoverUsuarioParaAdmin
{
    public class PromoverUsuarioParaAdminCommandHandler : IRequestHandler<PromoverUsuarioParaAdminCommand, UsuarioResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public PromoverUsuarioParaAdminCommandHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioResponse> Handle(PromoverUsuarioParaAdminCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorId(request.Id);
            if (usuario == null)
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);

            if (!usuario.Ativo)
                throw new DomainException(MensagensDominio.UsuarioInativo);

            if (usuario.Perfil.Equals(TipoUsuario.Administrador))
            {
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            usuario.PromoverPerfil(usuario);

            _usuarioRepository.Atualizar(usuario);

            await _usuarioRepository.SaveChanges();

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.EmailUsuario.Valor,
                Nome = usuario.NomeUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };
        }
    }
}
