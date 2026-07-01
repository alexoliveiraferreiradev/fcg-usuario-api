using Bogus;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Application.Features.Usuarios.Responses.Token;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Domain.ValueObjects;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Usuarios.Application.Tests.Features.Usuarios.Commands.AutenticarUsuario
{
    public class AutenticarUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<AutenticarUsuarioCommandHandler>> _loggerMock;
        private readonly AutenticarUsuarioCommandHandler _handler;

        public AutenticarUsuarioCommandHandlerTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _loggerMock = new Mock<ILogger<AutenticarUsuarioCommandHandler>>();

            _handler = new AutenticarUsuarioCommandHandler(
                _usuarioRepositoryMock.Object,
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
            var command = new AutenticarUsuarioCommand("teste@teste.com", "SenhaForte123");
            
            var usuario = new Usuario(
                new Nome("Usuario Teste"), 
                new Email("teste@teste.com"), 
                new Senha("SenhaForte123", "hashed_password")
            );

            _usuarioRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync(usuario);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword(command.Senha, usuario.Senha.Hash))
                .Returns(true);

            _tokenServiceMock
                .Setup(service => service.GerarToken(It.IsAny<UsuarioResponse>()))
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
            result.PerfilUsuario.Should().Be(usuario.Perfil);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoUsuarioNaoEncontrado()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand("teste@teste.com", "Senha123");

            _usuarioRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync((Usuario?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoUsuarioEstiverInativo()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand("teste@teste.com", "Senha123");
            
            var usuario = new Usuario(
                new Nome("Usuario Teste"), 
                new Email("teste@teste.com"), 
                new Senha("SenhaForte123", "hashed_password")
            );
            usuario.DesativarConta();

            _usuarioRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync(usuario);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoSenhaForInvalida()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand("teste@teste.com", "SenhaErrada");
            
            var usuario = new Usuario(
                new Nome("Usuario Teste"), 
                new Email("teste@teste.com"), 
                new Senha("SenhaForte123", "hashed_password")
            );

            _usuarioRepositoryMock
                .Setup(repo => repo.ObterPorEmail(command.Email))
                .ReturnsAsync(usuario);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword(command.Senha, usuario.Senha.Hash))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
