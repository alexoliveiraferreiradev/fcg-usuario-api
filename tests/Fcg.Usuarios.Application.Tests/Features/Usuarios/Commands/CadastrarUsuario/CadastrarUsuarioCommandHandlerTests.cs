using Bogus;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Usuarios.Application.Tests.Features.Usuarios.Commands.CadastrarUsuario
{
    public class CadastrarUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<CadastrarUsuarioCommandHandler>> _loggerMock;
        private readonly CadastrarUsuarioCommandHandler _handler;

        public CadastrarUsuarioCommandHandlerTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _loggerMock = new Mock<ILogger<CadastrarUsuarioCommandHandler>>();

            _handler = new CadastrarUsuarioCommandHandler(
                _usuarioRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _unitOfWorkMock.Object,
                _publishEndpointMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveCriarUsuarioEPublicarEvento_QuandoDadosForemValidosESemDuplicidade()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var command = new CadastrarUsuarioCommand(
                Nome: faker.Person.UserName,
                Email: faker.Person.Email,
                Senha: "Password123!",
                ConfirmacaoSenha: "Password123!"
            );

            _usuarioRepositoryMock
                .Setup(repo => repo.VerificaIndisponibilidade(command.Email, command.Nome))
                .ReturnsAsync((EmailUsado: false, NomeUsado: false));

            _passwordHasherMock
                .Setup(hasher => hasher.HashPassword(command.Senha))
                .Returns("hashed_password");

            // Act
            var resultId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultId.Should().NotBeEmpty();

            _usuarioRepositoryMock.Verify(repo => repo.Adicionar(It.Is<Usuario>(u => 
                u.EmailUsuario.Valor == command.Email && 
                u.NomeUsuario.Valor == command.Nome)), Times.Once);

            _publishEndpointMock.Verify(bus => bus.Publish(It.Is<UserCreatedEvent>(e => 
                e.Email == command.Email && 
                e.Name == command.Nome), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoEmailJaEstiverEmUso()
        {
            // Arrange
            var command = new CadastrarUsuarioCommand("Teste", "teste@teste.com", "Senha123", "Senha123");

            _usuarioRepositoryMock
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
            var command = new CadastrarUsuarioCommand("Teste", "teste@teste.com", "Senha123", "Senha123");

            _usuarioRepositoryMock
                .Setup(repo => repo.VerificaIndisponibilidade(command.Email, command.Nome))
                .ReturnsAsync((EmailUsado: false, NomeUsado: true));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
