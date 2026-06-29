using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Admin.Commands.DesativarUsuario
{
    public class DesativarUsuarioCommandHandler : IRequestHandler<DesativarUsuarioCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarUsuarioCommandHandler> _logger;

        public DesativarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork, ILogger<DesativarUsuarioCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(DesativarUsuarioCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando processo de desativação de usuário por um operador. UsuarioId: {UsuarioId}, OperadorId: {OperadorId}", request.Id, request.IdOperador);

            var usuarioDesativar = await _usuarioRepository.ObterPorId(request.Id);

            if(request.Id == request.IdOperador)
            {
                _logger.LogWarning("Tentativa de auto-inativação bloqueada para o operador {IdOperador}.", request.IdOperador);
                throw new DomainException(MensagensDominio.OperacaoDesativarInvalida);
            }

            if (usuarioDesativar == null)
            {
                _logger.LogWarning("Falha na desativação. Usuário alvo não encontrado. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            usuarioDesativar.Desativar(request.MotivoDelecao);

            await _unitOfWork.CommitAsync();

            _logger.LogWarning("Usuário desativado com sucesso pelo operador. UsuarioId: {UsuarioId}, OperadorId: {OperadorId}, Motivo: {Motivo}", request.Id, request.IdOperador, request.MotivoDelecao);
        }
    }
}
