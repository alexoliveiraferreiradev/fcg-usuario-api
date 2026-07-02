using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.ReativarConta
{
    public class ReactivateAccountCommandHandler : IRequestHandler<ReactivateAccountCommand>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReactivateAccountCommandHandler> _logger;

        public ReactivateAccountCommandHandler(IUserRepository UserRepository, IUnitOfWork unitOfWork, ILogger<ReactivateAccountCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(ReactivateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de reativação de conta. UserId: {UserId}", request.UserId);

            var User = await _UserRepository.GetByIdAsync(request.UserId);
            if (User == null)
            {                
                _logger.LogWarning("[UserAPI] Falha na reativação. Usuário não encontrado no banco de dados. UserId: {UserId}", request.UserId);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }
            User.Reactivate();

            _UserRepository.Update(User);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Conta reativada com sucesso. UserId: {UserId}", request.UserId);
        }
    }
}
