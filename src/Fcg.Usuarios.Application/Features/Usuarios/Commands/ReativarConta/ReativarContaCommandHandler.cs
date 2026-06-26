using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.ReativarConta
{
    public class ReativarContaCommandHandler : IRequestHandler<ReativarContaCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarContaCommandHandler> _logger;

        public ReativarContaCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork, ILogger<ReativarContaCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(ReativarContaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando processo de reativação de conta. UsuarioId: {UsuarioId}", request.UsuarioId);

            var usuario = await _usuarioRepository.ObterPorId(request.UsuarioId);
            if (usuario == null)
            {                
                _logger.LogWarning("Falha na reativação. Usuário não encontrado no banco de dados. UsuarioId: {UsuarioId}", request.UsuarioId);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }
            usuario.Reativar();

            _usuarioRepository.Atualizar(usuario);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Conta reativada com sucesso. UsuarioId: {UsuarioId}", request.UsuarioId);
        }
    }
}
