using Bogus;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.CadastrarUser;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.CadastrarUser
{
    public class CadastrarUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _UserRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<CadastrarUserCommandHandler>> _loggerMock;
        private readonly CadastrarUserCommandHandler _handler;

        public CadastrarUserCommandHandlerTests()
        {
            _UserRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _loggerMock = new Mock<ILogger<CadastrarUserCommandHandler>>();

            _handler = new CadastrarUserCommandHandler(
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
            var command = new CadastrarUserCommand(
                Nome: faker.Person.UserName,
                Email: faker.Person.Email,
                Senha: "Password123!",
                ConfirmacaoSenha: "Password123!"
            );

            _UserRepositoryMock
                .Setup(repo => repo.VerificaIndisponibilidade(command.Email, command.Nome))
                .ReturnsAsync((EmailUsado: false, NomeUsado: false));

            _passwordHasherMock
                .Setup(hasher => hasher.HashPassword(command.Senha))
                .Returns("hashed_password");

            // Act
            var resultId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultId.Should().NotBeEmpty();

            _UserRepositoryMock.Verify(repo => repo.Adicionar(It.Is<User>(u => 
                u.EmailUser.Valor == command.Email && 
                u.NomeUser.Valor == command.Nome)), Times.Once);

            _publishEndpointMock.Verify(bus => bus.Publish(It.Is<UserCreatedEvent>(e => 
                e.Email == command.Email && 
                e.Name == command.Nome), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoEmailJaEstiverEmUso()
        {
            // Arrange
            var command = new CadastrarUserCommand("Teste", "teste@teste.com", "Senha123", "Senha123");

            _UserRepositoryMock
                .Setup(repo => repo.VerificaIndisponibilidade(command.Email, command.Nome))
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
            var command = new CadastrarUserCommand("Teste", "teste@teste.com", "Senha123", "Senha123");

            _UserRepositoryMock
                .Setup(repo => repo.VerificaIndisponibilidade(command.Email, command.Nome))
                .ReturnsAsync((EmailUsado: false, NomeUsado: true));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
