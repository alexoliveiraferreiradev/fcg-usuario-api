using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.DesativarUsuario
{
    public class DesativarUsuarioCommandHandler : IRequestHandler<DesativarUsuarioCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public DesativarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository; 
        }
        public async Task Handle(DesativarUsuarioCommand request, CancellationToken cancellationToken)
        {
            if (request.IdOperador == request.Id)
            {
                throw new DomainException(MensagensDominio.OperacaoDesativarInvalida);
            }

            var adminOperador = await _usuarioRepository.ObterPorId(request.IdOperador);

            if (adminOperador == null)
            {
                throw new DomainException(MensagensDominio.AdminNaoEncontrado);
            }

            var usuarioDesativar = await _usuarioRepository.ObterPorId(request.Id);

            if (usuarioDesativar == null)
            {
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            usuarioDesativar.Desativar(request.MotivoDelecao);

            await _usuarioRepository.SaveChanges();
        }
    }
}
