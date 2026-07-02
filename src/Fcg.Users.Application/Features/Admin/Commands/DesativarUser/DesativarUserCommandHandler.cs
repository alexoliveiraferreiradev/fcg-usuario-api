using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.DesativarUser
{
    public class DesativarUserCommandHandler : IRequestHandler<DesativarUserCommand>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarUserCommandHandler> _logger;

        public DesativarUserCommandHandler(IUserRepository UserRepository, IUnitOfWork unitOfWork, ILogger<DesativarUserCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(DesativarUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de desativação de usuário por um operador. UserId: {UserId}, OperadorId: {OperadorId}", request.Id, request.IdOperador);

            var UserDesativar = await _UserRepository.GetByIdAsync(request.Id);

            if(request.Id == request.IdOperador)
            {
                _logger.LogWarning("[UserAPI] Tentativa de auto-inativação bloqueada para o operador {IdOperador}.", request.IdOperador);
                throw new DomainException(MensagensDominio.OperacaoDesativarInvalida);
            }

            if (UserDesativar == null)
            {
                _logger.LogWarning("[UserAPI] Falha na desativação. Usuário alvo não encontrado. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            UserDesativar.Deactivate(request.MotivoDelecao);

            await _unitOfWork.CommitAsync();

            _logger.LogWarning("[UserAPI] Usuário desativado com sucesso pelo operador. UserId: {UserId}, OperadorId: {OperadorId}, reason: {reason}", request.Id, request.IdOperador, request.MotivoDelecao);
        }
    }
}
