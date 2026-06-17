using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Common;
using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Domain.ValueObjects;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario
{
    public class AtualizarUsuarioCommandHandler : IRequestHandler<AtualizarUsuarioCommand, UsuarioResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        public AtualizarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository; 
            _passwordHasher = passwordHasher;
        }
        public async Task<UsuarioResponse> Handle(AtualizarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorId(request.UsuarioId);
            if (usuario == null) throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);

            if (await _usuarioRepository.VerificaNomeCadastradoParaAlteracao(request.UsuarioId, request.NomeUsuario))
            {                
                throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            }

            if (request.SenhaUsuario != request.ConfirmacaoSenha)
                throw new DomainException(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);

            AssertionConcern.AssertArgumentLength(request.SenhaUsuario, 8, 60, MensagensDominio.SenhaTamanhoInvalido);
            AssertionConcern.AssertArgumentPasswordStrenght(request.SenhaUsuario, MensagensDominio.UsuarioSenhaFraca);

            var hashSenha = _passwordHasher.HashPassword(request.SenhaUsuario);

            var novaSenhaCriptografa = new Senha(hashSenha);

            var novoUsuarioVO = new Nome(request.NomeUsuario);

            usuario.Atualizar(novoUsuarioVO, novaSenhaCriptografa);

            _usuarioRepository.Atualizar(usuario);

            await _usuarioRepository.SaveChanges();

            return new UsuarioResponse
            {
                Nome = usuario.NomeUsuario.Valor,
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };
        }
    }
}
