using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.RebaixarUsuarioParaJogador
{
    public class RebaixarUsuarioParaJogadorCommandHandler : IRequestHandler<RebaixarUsuarioParaJogadorCommand, UsuarioResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RebaixarUsuarioParaJogadorCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork    )
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<UsuarioResponse> Handle(RebaixarUsuarioParaJogadorCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == request.IdOperador)
            {                
                throw new DomainException(MensagensDominio.OperacaoRebaixarInvalida);
            }

            var usuario = await _usuarioRepository.ObterPorId(request.Id);

            if (usuario == null)
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);

            if (!usuario.Ativo)
                throw new DomainException(MensagensDominio.UsuarioInativo);

            if (usuario.Perfil.Equals(TipoUsuario.Jogador))
            {                
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            usuario.RebaixarPerfil();

            _usuarioRepository.Atualizar(usuario);

            await _unitOfWork.CommitAsync();

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.NomeUsuario.Valor,
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };
        }
    }
}
