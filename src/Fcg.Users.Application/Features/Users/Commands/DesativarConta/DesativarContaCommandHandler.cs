using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.DesativarConta
{
    public class DesativarContaCommandHandler : IRequestHandler<DesativarContaCommand>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarContaCommandHandler> _logger;

        public DesativarContaCommandHandler(IUserRepository UserRepository, IUnitOfWork unitOfWork, ILogger<DesativarContaCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Handle(DesativarContaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de desativação de conta. UserId: {UserId}", request.Id);

            var User = await _UserRepository.ObterPorId(request.Id);

            if (User == null)
            {                
                _logger.LogWarning("[UserAPI] Falha na desativação. Usuário não encontrado no banco de dados. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (User.Perfil == TipoUser.Administrador)
            {             
                var existeOutroAdmin = await _UserRepository.VerificaMaisDeUmAdminCadastrado();
                if (!existeOutroAdmin)
                {             
                    _logger.LogWarning("[UserAPI] Falha na desativação. Não é possível desativar o único administrador cadastrado. UserId: {UserId}", request.Id);
                    throw new DomainException(MensagensDominio.OperacaoDesativarAdminInvalida);
                }
            }
            User.DesativarConta();

            _UserRepository.Atualizar(User);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Conta do usuário desativada com sucesso. UserId: {UserId}", request.Id);
        }
    }
}
