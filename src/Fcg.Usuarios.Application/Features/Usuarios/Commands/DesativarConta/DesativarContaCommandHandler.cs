using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.DesativarConta
{
    public class DesativarContaCommandHandler : IRequestHandler<DesativarContaCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarContaCommandHandler> _logger;

        public DesativarContaCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork, ILogger<DesativarContaCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(DesativarContaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UsuarioAPI] Iniciando processo de desativação de conta. UsuarioId: {UsuarioId}", request.Id);

            var usuario = await _usuarioRepository.ObterPorId(request.Id);

            if (usuario == null)
            {                
                _logger.LogWarning("[UsuarioAPI] Falha na desativação. Usuário não encontrado no banco de dados. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (usuario.Perfil == TipoUsuario.Administrador)
            {             
                var existeOutroAdmin = await _usuarioRepository.VerificaMaisDeUmAdminCadastrado();
                if (!existeOutroAdmin)
                {             
                    _logger.LogWarning("[UsuarioAPI] Falha na desativação. Não é possível desativar o único administrador cadastrado. UsuarioId: {UsuarioId}", request.Id);
                    throw new DomainException(MensagensDominio.OperacaoDesativarAdminInvalida);
                }
            }
            usuario.DesativarConta();

            _usuarioRepository.Atualizar(usuario);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UsuarioAPI] Conta do usuário desativada com sucesso. UsuarioId: {UsuarioId}", request.Id);
        }
    }
}
