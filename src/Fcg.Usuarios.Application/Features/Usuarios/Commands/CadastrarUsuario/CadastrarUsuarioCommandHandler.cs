using Fcg.MessageContracts;
using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Common;
using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Domain.ValueObjects;
using MassTransit;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario
{
    public class CadastrarUsuarioCommandHandler : IRequestHandler<CadastrarUsuarioCommand, LoginResponse>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        public CadastrarUsuarioCommandHandler(IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher, ITokenService tokenService,IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;   
            _tokenService = tokenService;   
            _unitOfWork= unitOfWork;    
            _publishEndpoint = publishEndpoint;
        }
        public async Task<LoginResponse> Handle(CadastrarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var nomeValueObject = new Nome(request.Nome);
            var emailValueObject = new Email(request.Email);
            //if (await _usuarioRepository.VerificaEmailCadastrado(request.Email))
            //{
            //    throw new DomainException(MensagensDominio.EmailJaCadastrado);
            //}

            //if (await _usuarioRepository.VerificaNomeCadastrado(request.Nome))
            //{
            //    throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            //}

            if (request.Senha != request.ConfirmacaoSenha)
                throw new DomainException(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);

            AssertionConcern.AssertArgumentLength(request.Senha, 8, 60, MensagensDominio.SenhaTamanhoInvalido);
            AssertionConcern.AssertArgumentPasswordStrenght(request.Senha, MensagensDominio.UsuarioSenhaFraca);

            var hashSenha = _passwordHasher.HashPassword(request.Senha);

            var senhaCriptografada = new Senha(hashSenha);

            var usuario = new Usuario(nomeValueObject,emailValueObject,senhaCriptografada); 

            //_usuarioRepository.Adicionar(usuario);

            await _publishEndpoint.Publish(new UserCreatedEvent(usuario.Id, 
                usuario.NomeUsuario.Valor, usuario.EmailUsuario.Valor));

            //await _unitOfWork.CommitAsync();

            var usuarioResponse = new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.EmailUsuario.Valor,
                Nome = usuario.NomeUsuario.Valor,
                PerfilUsuario = usuario.Perfil
            };           

            var tokenResult = await _tokenService.GerarToken(usuarioResponse);

            return new LoginResponse
            {
                AcessToken = tokenResult.AccessToken,
                ExpiresIn = tokenResult.ExpiresIn,
                Id = usuario.Id.ToString(),
                Email = usuario.EmailUsuario.Valor,
                PerfilUsuario = usuario.Perfil,
                Claims = tokenResult.Claims
            };
        }
    }
}
