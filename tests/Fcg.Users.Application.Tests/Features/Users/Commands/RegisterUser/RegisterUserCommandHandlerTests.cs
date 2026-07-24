using Bogus;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.CadastrarUser
{
    public class CadastrarUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
        private readonly RegisterUserCommandHandler _handler;

        public CadastrarUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();

            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _unitOfWorkMock.Object,
                _publishEndpointMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveCriarUserEPublicarEvento_QuandoDadosForemValidosESemDuplicidade()
        {
            // Arrange
            var harness = new InMemoryTestHarness();
            var faker = new Faker("pt_BR");
            var command = new RegisterUserCommand(
                Name: faker.Person.UserName,
                Email: faker.Person.Email,
                Password: "Password123!",
                ConfirmPassword: "Password123!"
            );

            await harness.Start();
            try
            {

                _userRepositoryMock
                    .Setup(repo => repo.CheckAvailabilityAsync(command.Email, command.Name))
                    .ReturnsAsync((EmailUsado: false, NomeUsado: false));

                _passwordHasherMock
                    .Setup(hasher => hasher.HashPassword(command.Password))
                    .Returns("HashedPassword@123");

                // Act
                var resultId = await _handler.Handle(command, CancellationToken.None);

                // Assert
                resultId.Should().NotBeEmpty();

                Assert.True(await harness.Published.Any<IUserCreatedIntegrationEvent>());

                var publishedMessage = harness.Published.Select<IUserCreatedIntegrationEvent>()
                        .First().Context.Message;

                publishedMessage.Email.Should().Be(command.Email);
                publishedMessage.Name.Should().Be(command.Name);

                _userRepositoryMock.Verify(repo => repo.Add(It.Is<User>(u =>
                    u.Email.Value == command.Email &&
                    u.Name.Value == command.Name)), Times.Once);

                _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
            }
            catch
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoEmailJaEstiverEmUso()
        {
            // Arrange
            var command = new RegisterUserCommand("Teste", "teste@teste.com", "SenhaForte@123", "SenhaForte@123");

            _userRepositoryMock
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
            var command = new RegisterUserCommand("Teste", "teste@teste.com", "SenhaForte@123", "SenhaForte@123");

            _userRepositoryMock
                .Setup(repo => repo.CheckAvailabilityAsync(command.Email, command.Name))
                .ReturnsAsync((EmailUsado: false, NomeUsado: true));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
