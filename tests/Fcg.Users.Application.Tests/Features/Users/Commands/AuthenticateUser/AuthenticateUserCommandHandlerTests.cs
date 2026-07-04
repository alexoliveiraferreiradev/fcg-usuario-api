using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.AuthenticateUser;
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

namespace Fcg.Users.Application.Tests.Features.Users.Commands.AutenticarUser
{
    public class AutenticarUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<AuthenticateUserCommandHandler>> _loggerMock;
        private readonly AuthenticateUserCommandHandler _handler;

        public AutenticarUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            _handler = new AuthenticateUserCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveRetornarToken_QuandoCredenciaisForemValidas()
        {
            // Arrange
            var command = new AuthenticateUserCommand("teste@teste.com", "SenhaForte123@");
            
            var User = new User(
                new Name("User Teste"), 
                new Email("teste@teste.com"), 
                new Password("SenhaForte123@")
            );

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email))
                .ReturnsAsync(User);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword(command.Password, User.Password.Hash))
                .Returns(true);

            _tokenServiceMock
                .Setup(service => service.GenerateToken(It.IsAny<UserResponse>()))
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
            result.PerfilUser.Should().Be(User.Role);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoUserNaoEncontrado()
        {
            // Arrange
            var command = new AuthenticateUserCommand("teste@teste.com", "Senha123");

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email))
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
            var command = new AuthenticateUserCommand("teste@teste.com", "SenhaForte123@");
            
            var User = new User(
                new Name("User Teste"), 
                new Email("teste@teste.com"), 
                new Password("SenhaForte123@")
            );
            User.DeactivateAccount();

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email))
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
            var command = new AuthenticateUserCommand("teste@teste.com", "SenhaErrada");
            
            var User = new User(
                new Name("User Teste"), 
                new Email("teste@teste.com"), 
                new Password("hashed_password")
            );

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email))
                .ReturnsAsync(User);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword(command.Password, User.Password.Hash))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
