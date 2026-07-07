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

namespace Fcg.Users.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        public RegisterUserCommandHandler(IUserRepository userRepository,
            IPasswordHasher passwordHasher, IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint, ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;   
            _unitOfWork= unitOfWork;    
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UserAPI] Iniciando tentativa de cadastro de novo usuário. Email: {Email}", request.Email);
            var nomeValueObject = new Name(request.Name);
            var emailValueObject = new Email(request.Email);
            var unavailable = await _userRepository.CheckAvailabilityAsync(request.Email, request.Name);
            
            if (unavailable.EmailUsado) throw new DomainException(DomainMessages.EmailAlreadyRegistered);
            if (unavailable.NomeUsado) throw new DomainException(DomainMessages.UserNameAlreadyRegistered);

            var senha = new Password(request.Password);

            var hashSenha = _passwordHasher.HashPassword(senha.Hash);

            var senhaCriptografada = new Password(hashSenha);

            var user = new User(nomeValueObject,emailValueObject,senhaCriptografada);

            _userRepository.Add(user);

            await _publishEndpoint.Publish(new UserCreatedEvent(user.Id, 
                user.Name.Valor, user.Email.Valor));

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[UserAPI] Usuário {UserId} cadastrado com sucesso no banco de dados com o Role base.", user.Id);

            _logger.LogInformation("[UserAPI] Processo de cadastro finalizado. Usuário {UserId} pronto para login.", user.Id);

            return user.Id;
        }
    }
}
