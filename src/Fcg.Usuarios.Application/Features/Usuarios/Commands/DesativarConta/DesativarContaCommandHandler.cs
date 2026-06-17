using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.DesativarConta
{
    public class DesativarContaCommandHandler : IRequestHandler<DesativarContaCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public DesativarContaCommandHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository; 
        }
        public async Task Handle(DesativarContaCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorId(request.Id);

            if (usuario == null)
            {                
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (usuario.Perfil == TipoUsuario.Administrador)
            {             
                var existeOutroAdmin = await _usuarioRepository.VerificaMaisDeUmAdminCadastrado();
                if (!existeOutroAdmin)
                {             
                    throw new DomainException(MensagensDominio.OperacaoDesativarAdminInvalida);
                }
            }
            usuario.DesativarConta();

            _usuarioRepository.Atualizar(usuario);

            await _usuarioRepository.SaveChanges();
        }
    }
}
