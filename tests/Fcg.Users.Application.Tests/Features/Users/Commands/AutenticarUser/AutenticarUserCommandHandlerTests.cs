using Bogus;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.AutenticarUser;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Application.Features.Users.Responses.Token;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.AutenticarUser
{
    public class AutenticarUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _UserRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<AutenticarUserCommandHandler>> _loggerMock;
        private readonly AutenticarUserCommandHandler _handler;

        public AutenticarUserCommandHandlerTests()
        {
            _UserRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _loggerMock = new Mock<ILogger<AutenticarUserCommandHandler>>();

            _handler = new AutenticarUserCommandHandler(
                _UserRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _publishEndpointMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveRetornarToken_QuandoCredenciaisForemValidas()
        {
            // Arrange
            var command = new AutenticarUserCommand("teste@teste.com", "SenhaForte123");
            
            var User = new User(
                new Nome("User Teste"), 
                new Email("teste@teste.com"), 
                new Senha("SenhaForte123", "hashed_password")
            );

            _UserRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync(User);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword(command.Senha, User.Senha.Hash))
                .Returns(true);

            _tokenServiceMock
                .Setup(service => service.GerarToken(It.IsAny<UserResponse>()))
                .ReturnsAsync(new TokenResult 
                { 
                    AccessToken = "token_jwt_gerado", 
                    ExpiresIn = 3600, 
                    Claims = new List<ClaimResponse>() 
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AcessToken.Should().Be("token_jwt_gerado");
            result.Email.Should().Be(command.Email);
            result.PerfilUser.Should().Be(User.Perfil);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoUserNaoEncontrado()
        {
            // Arrange
            var command = new AutenticarUserCommand("teste@teste.com", "Senha123");

            _UserRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoUserEstiverInativo()
        {
            // Arrange
            var command = new AutenticarUserCommand("teste@teste.com", "Senha123");
            
            var User = new User(
                new Nome("User Teste"), 
                new Email("teste@teste.com"), 
                new Senha("SenhaForte123", "hashed_password")
            );
            User.DesativarConta();

            _UserRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync(User);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoSenhaForInvalida()
        {
            // Arrange
            var command = new AutenticarUserCommand("teste@teste.com", "SenhaErrada");
            
            var User = new User(
                new Nome("User Teste"), 
                new Email("teste@teste.com"), 
                new Senha("SenhaForte123", "hashed_password")
            );

            _UserRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync(User);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword(command.Senha, User.Senha.Hash))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
