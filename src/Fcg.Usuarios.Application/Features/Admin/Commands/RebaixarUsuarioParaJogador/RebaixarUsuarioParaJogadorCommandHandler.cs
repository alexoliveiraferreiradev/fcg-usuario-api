using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Core.Abstractions.Resources;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Admin.Commands.RebaixarUsuarioParaJogador
{
    public class RebaixarUsuarioParaJogadorCommandHandler : IRequestHandler<RebaixarUsuarioParaJogadorCommand, UsuarioResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RebaixarUsuarioParaJogadorCommandHandler> _logger;

        public RebaixarUsuarioParaJogadorCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork, ILogger<RebaixarUsuarioParaJogadorCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<UsuarioResponse> Handle(RebaixarUsuarioParaJogadorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando processo de rebaixamento de usuário para jogador. UsuarioId: {UsuarioId}, OperadorId: {OperadorId}", request.Id, request.IdOperador);

            if (request.Id == request.IdOperador)
            {                
                _logger.LogWarning("Falha no rebaixamento. Um administrador não pode rebaixar a própria conta. OperadorId: {OperadorId}", request.IdOperador);
                throw new DomainException(MensagensDominio.OperacaoRebaixarInvalida);
            }

            var usuario = await _usuarioRepository.ObterPorId(request.Id);

            if (usuario == null)
            {
                _logger.LogWarning("Falha no rebaixamento. Usuário alvo não encontrado. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (!usuario.Ativo)
            {
                _logger.LogWarning("Falha no rebaixamento. Usuário está inativo. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }

            if (usuario.Perfil.Equals(TipoUsuario.Jogador))
            {                
                _logger.LogWarning("Falha no rebaixamento. O usuário alvo já é um jogador. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            usuario.RebaixarPerfil();

            _usuarioRepository.Atualizar(usuario);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Usuário rebaixado para jogador com sucesso. UsuarioId: {UsuarioId}", request.Id);

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.NomeUsuario.Valor,
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };
        }
    }
}
