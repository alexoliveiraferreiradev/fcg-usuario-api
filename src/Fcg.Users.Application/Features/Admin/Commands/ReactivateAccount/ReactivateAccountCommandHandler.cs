using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.ReactiveAccount
{
    public class ReactivateAccountCommandHandler : IRequestHandler<ReactivateAccountCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReactivateAccountCommandHandler> _logger;

        public ReactivateAccountCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<ReactivateAccountCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(ReactivateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de reativação de conta. UserId: {UserId}", request.UserId);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {                
                _logger.LogWarning("[UserAPI] Falha na reativação. Usuário não encontrado no banco de dados. UserId: {UserId}", request.UserId);
                throw new DomainException(DomainMessages.UserNotFound);
            }
            user.Reactivate();

            _userRepository.Update(user);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Conta reativada com sucesso. UserId: {UserId}", request.UserId);
        }
    }
}
