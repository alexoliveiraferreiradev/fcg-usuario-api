using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Admin.Commands.RebaixarUserParaJogador
{
    public class RebaixarUserParaJogadorCommandHandler : IRequestHandler<RebaixarUserParaJogadorCommand, UserResponse>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RebaixarUserParaJogadorCommandHandler> _logger;

        public RebaixarUserParaJogadorCommandHandler(IUserRepository UserRepository, IUnitOfWork unitOfWork, ILogger<RebaixarUserParaJogadorCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<UserResponse> Handle(RebaixarUserParaJogadorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando processo de rebaixamento de usuário para jogador. UserId: {UserId}, OperadorId: {OperadorId}", request.Id, request.IdOperador);

            if (request.Id == request.IdOperador)
            {                
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. Um administrador não pode rebaixar a própria conta. OperadorId: {OperadorId}", request.IdOperador);
                throw new DomainException(MensagensDominio.OperacaoRebaixarInvalida);
            }

            var User = await _UserRepository.ObterPorId(request.Id);

            if (User == null)
            {
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. Usuário alvo não encontrado. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (!User.Ativo)
            {
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. Usuário está inativo. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }

            if (User.Perfil.Equals(TipoUser.Jogador))
            {                
                _logger.LogWarning("[UserAPI] Falha no rebaixamento. O usuário alvo já é um jogador. UserId: {UserId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            User.RebaixarPerfil();

            _UserRepository.Atualizar(User);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuário rebaixado para jogador com sucesso. UserId: {UserId}", request.Id);

            return new UserResponse
            {
                Id = User.Id,
                Nome = User.NomeUser.Valor,
                Email = User.EmailUser.Valor,
                PerfilUser = User.Perfil
            };
        }
    }
}
