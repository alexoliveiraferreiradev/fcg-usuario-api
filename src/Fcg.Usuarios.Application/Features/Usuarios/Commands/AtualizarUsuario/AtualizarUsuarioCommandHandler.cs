using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario
{
    public class AtualizarUsuarioCommandHandler : IRequestHandler<AtualizarUsuarioCommand, UsuarioResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AtualizarUsuarioCommandHandler> _logger;
        public AtualizarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher, 
            IUnitOfWork unitOfWork,ILogger<AtualizarUsuarioCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository; 
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<UsuarioResponse> Handle(AtualizarUsuarioCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando processo de atualização de usuário. UsuarioId: {UsuarioId}", request.UsuarioId);

            var usuario = await _usuarioRepository.ObterPorId(request.UsuarioId);
            if (usuario == null) 
            {
                _logger.LogWarning("Falha na atualização. Usuário não encontrado no banco de dados. UsuarioId: {UsuarioId}", request.UsuarioId);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (await _usuarioRepository.VerificaNomeCadastradoParaAlteracao(request.UsuarioId, request.NomeUsuario))
            {                
                _logger.LogWarning("Falha na atualização. O nome de usuário '{NomeUsuario}' já está em uso por outra conta. UsuarioId: {UsuarioId}", request.NomeUsuario, request.UsuarioId);
                throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            }

            var hashSenha = _passwordHasher.HashPassword(request.SenhaUsuario);

            var novaSenhaCriptografa = new Senha(request.SenhaUsuario,hashSenha);

            var novoUsuarioVO = new Nome(request.NomeUsuario);

            usuario.Atualizar(novoUsuarioVO, novaSenhaCriptografa);

            _usuarioRepository.Atualizar(usuario);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Usuário atualizado com sucesso. UsuarioId: {UsuarioId}", request.UsuarioId);

            return new UsuarioResponse
            {
                Nome = usuario.NomeUsuario.Valor,
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };
        }
    }
}
