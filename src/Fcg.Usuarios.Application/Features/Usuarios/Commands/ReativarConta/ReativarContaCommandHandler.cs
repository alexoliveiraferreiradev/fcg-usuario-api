using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.ReativarConta
{
    public class ReativarContaCommandHandler : IRequestHandler<ReativarContaCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public ReativarContaCommandHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public async Task Handle(ReativarContaCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorId(request.UsuarioId);
            if (usuario == null)
            {                
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }
            usuario.Reativar();

            _usuarioRepository.Atualizar(usuario);

            await _usuarioRepository.SaveChanges();
        }
    }
}
