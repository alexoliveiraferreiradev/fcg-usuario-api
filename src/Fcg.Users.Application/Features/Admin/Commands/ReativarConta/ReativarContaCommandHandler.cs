using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.ReativarConta
{
    public class ReativarContaCommandHandler : IRequestHandler<ReativarContaCommand>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarContaCommandHandler> _logger;

        public ReativarContaCommandHandler(IUserRepository UserRepository, IUnitOfWork unitOfWork, ILogger<ReativarContaCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(ReativarContaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de reativação de conta. UserId: {UserId}", request.UserId);

            var User = await _UserRepository.ObterPorId(request.UserId);
            if (User == null)
            {                
                _logger.LogWarning("[UserAPI] Falha na reativação. Usuário não encontrado no banco de dados. UserId: {UserId}", request.UserId);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }
            User.Reativar();

            _UserRepository.Atualizar(User);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Conta reativada com sucesso. UserId: {UserId}", request.UserId);
        }
    }
}
