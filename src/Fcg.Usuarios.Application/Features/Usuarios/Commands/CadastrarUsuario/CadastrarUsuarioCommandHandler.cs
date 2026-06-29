using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Domain.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario
{
    public class CadastrarUsuarioCommandHandler : IRequestHandler<CadastrarUsuarioCommand, Guid>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CadastrarUsuarioCommandHandler> _logger;
        public CadastrarUsuarioCommandHandler(IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher, ITokenService tokenService,IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint, ILogger<CadastrarUsuarioCommandHandler> logger)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;   
            _tokenService = tokenService;   
            _unitOfWork= unitOfWork;    
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<Guid> Handle(CadastrarUsuarioCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando tentativa de cadastro de novo usuário. Email: {Email}", request.Email);
            var nomeValueObject = new Nome(request.Nome);
            var emailValueObject = new Email(request.Email);
            var indisponivel = await _usuarioRepository.VerificaIndisponibilidade(request.Email, request.Nome);
            
            if (indisponivel.EmailUsado) throw new DomainException(MensagensDominio.EmailJaCadastrado);
            if (indisponivel.NomeUsado) throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            
            var hashSenha = _passwordHasher.HashPassword(request.Senha);

            var senhaCriptografada = new Senha(request.Senha,hashSenha);

            var usuario = new Usuario(nomeValueObject,emailValueObject,senhaCriptografada);

            _usuarioRepository.Adicionar(usuario);

            await _publishEndpoint.Publish(new UserCreatedEvent(usuario.Id, 
                usuario.NomeUsuario.Valor, usuario.EmailUsuario.Valor));

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Usuário {UsuarioId} cadastrado com sucesso no banco de dados com o perfil base.", usuario.Id);

            _logger.LogInformation("Processo de cadastro finalizado. Usuário {UsuarioId} pronto para login.", usuario.Id);

            return usuario.Id;
        }
    }
}
