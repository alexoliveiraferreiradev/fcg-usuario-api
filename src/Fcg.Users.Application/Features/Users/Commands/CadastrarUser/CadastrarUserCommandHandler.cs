using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Users.Application.Features.Users.Commands.CadastrarUser
{
    public class CadastrarUserCommandHandler : IRequestHandler<CadastrarUserCommand, Guid>
    {
        private readonly IUserRepository _UserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CadastrarUserCommandHandler> _logger;
        public CadastrarUserCommandHandler(IUserRepository UserRepository,
            IPasswordHasher passwordHasher, ITokenService tokenService,IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint, ILogger<CadastrarUserCommandHandler> logger)
        {
            _UserRepository = UserRepository;
            _passwordHasher = passwordHasher;   
            _tokenService = tokenService;   
            _unitOfWork= unitOfWork;    
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<Guid> Handle(CadastrarUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando tentativa de cadastro de novo usuário. Email: {Email}", request.Email);
            var nomeValueObject = new Nome(request.Nome);
            var emailValueObject = new Email(request.Email);
            var indisponivel = await _UserRepository.VerificaIndisponibilidade(request.Email, request.Nome);
            
            if (indisponivel.EmailUsado) throw new DomainException(MensagensDominio.EmailJaCadastrado);
            if (indisponivel.NomeUsado) throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            
            var hashSenha = _passwordHasher.HashPassword(request.Senha);

            var senhaCriptografada = new Senha(request.Senha,hashSenha);

            var User = new User(nomeValueObject,emailValueObject,senhaCriptografada);

            _UserRepository.Adicionar(User);

            await _publishEndpoint.Publish(new UserCreatedEvent(User.Id, 
                User.NomeUser.Valor, User.EmailUser.Valor));

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuário {UserId} cadastrado com sucesso no banco de dados com o perfil base.", User.Id);

            _logger.LogInformation("[UserAPI] Processo de cadastro finalizado. Usuário {UserId} pronto para login.", User.Id);

            return User.Id;
        }
    }
}
