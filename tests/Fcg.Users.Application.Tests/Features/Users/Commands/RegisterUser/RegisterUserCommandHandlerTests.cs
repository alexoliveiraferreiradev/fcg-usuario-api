using Bogus;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.CadastrarUser
{
    public class CadastrarUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _UserRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
        private readonly RegisterUserCommandHandler _handler;

        public CadastrarUserCommandHandlerTests()
        {
            _UserRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();

            _handler = new RegisterUserCommandHandler(
                _UserRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _unitOfWorkMock.Object,
                _publishEndpointMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveCriarUserEPublicarEvento_QuandoDadosForemValidosESemDuplicidade()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var command = new RegisterUserCommand(
                Name: faker.Person.UserName,
                Email: faker.Person.Email,
                Password: "Password123!",
                ConfirmPassword: "Password123!"
            );

            _UserRepositoryMock
                .Setup(repo => repo.CheckAvailabilityAsync(command.Email, command.Name))
                .ReturnsAsync((EmailUsado: false, NomeUsado: false));

            _passwordHasherMock
                .Setup(hasher => hasher.HashPassword(command.Password))
                .Returns("hashed_password");

            // Act
            var resultId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultId.Should().NotBeEmpty();

            _UserRepositoryMock.Verify(repo => repo.Add(It.Is<User>(u => 
                u.Email.Valor == command.Email && 
                u.Name.Valor == command.Name)), Times.Once);

            _publishEndpointMock.Verify(bus => bus.Publish(It.Is<UserCreatedEvent>(e => 
                e.Email == command.Email && 
                e.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoEmailJaEstiverEmUso()
        {
            // Arrange
            var command = new RegisterUserCommand("Teste", "teste@teste.com", "Senha123", "Senha123");

            _UserRepositoryMock
                .Setup(repo => repo.CheckAvailabilityAsync(command.Email, command.Name))
                .ReturnsAsync((EmailUsado: true, NomeUsado: false));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoNomeJaEstiverEmUso()
        {
            // Arrange
            var command = new RegisterUserCommand("Teste", "teste@teste.com", "Senha123", "Senha123");

            _UserRepositoryMock
                .Setup(repo => repo.CheckAvailabilityAsync(command.Email, command.Name))
                .ReturnsAsync((EmailUsado: false, NomeUsado: true));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
