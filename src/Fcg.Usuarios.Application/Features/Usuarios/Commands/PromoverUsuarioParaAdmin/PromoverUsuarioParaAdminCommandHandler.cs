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
        private readonly IUnitOfWork _unitOfWork;

        public PromoverUsuarioParaAdminCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
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

            usuario.PromoverPerfil();

            _usuarioRepository.Atualizar(usuario);

            await _unitOfWork.CommitAsync();

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
