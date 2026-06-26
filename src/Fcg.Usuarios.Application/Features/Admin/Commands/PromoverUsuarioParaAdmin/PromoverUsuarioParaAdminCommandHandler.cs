using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Admin.Commands.PromoverUsuarioParaAdmin
{
    public class PromoverUsuarioParaAdminCommandHandler : IRequestHandler<PromoverUsuarioParaAdminCommand, UsuarioResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PromoverUsuarioParaAdminCommandHandler> _logger;

        public PromoverUsuarioParaAdminCommandHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork, ILogger<PromoverUsuarioParaAdminCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UsuarioResponse> Handle(PromoverUsuarioParaAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando processo de promoção de usuário para administrador. UsuarioId: {UsuarioId}", request.Id);

            var usuario = await _usuarioRepository.ObterPorId(request.Id);
            if (usuario == null)
            {
                _logger.LogWarning("Falha na promoção. Usuário não encontrado. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (request.Id == request.IdOperador)
            {
                _logger.LogWarning("Tentativa de autopromoção bloqueada. AdminId: {AdminId}", request.IdOperador);
                throw new DomainException("Um administrador não pode promover a si próprio.");
            }

            if (!usuario.Ativo)
            {
                _logger.LogWarning("Falha na promoção. Usuário está inativo. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }

            if (usuario.Perfil.Equals(TipoUsuario.Administrador))
            {
                _logger.LogWarning("Falha na promoção. O usuário já é um administrador. UsuarioId: {UsuarioId}", request.Id);
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            usuario.PromoverPerfil();

            _usuarioRepository.Atualizar(usuario);

            await _unitOfWork.CommitAsync();

            _logger.LogWarning("[AUDITORIA] Usuário {TargetUserId} promovido a Administrador pelo operador {AdminId}.", request.Id, request.IdOperador);

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.EmailUsuario.Valor,
                Nome = usuario.NomeUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };
        }
    }
}
