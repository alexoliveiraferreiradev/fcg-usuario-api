using Fcg.Core.WebApi.Security;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Usuarios.Infrastructure.Integration.Security
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public TokenServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                Secret = "esta_eh_uma_chave_secreta_super_segura_de_32_caracteres_minimos!",
                ExpiracaoHoras = 2,
                Emissor = "FcgApp",
                ValidoEm = "FcgAppClients"
            };

            var optionsMock = Options.Create(_jwtSettings);
            _tokenService = new TokenService(optionsMock);
        }

        [Fact]
        public async Task ObtemClaims_DeveIncluirClaimDeAdmin_QuandoUsuarioForAdministrador()
        {
            // Arrange
            var usuario = new UsuarioResponse
            {
                Id = Guid.NewGuid(),
                Nome = "Admin Teste",
                Email = "admin@teste.com",
                PerfilUsuario = TipoUsuario.Administrador
            };

            // Act
            var claims = await _tokenService.ObtemClaims(usuario);

            // Assert
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "AdminRole");
        }

        [Fact]
        public async Task ObtemClaims_DeveIncluirClaimDeJogador_QuandoUsuarioForJogador()
        {
            // Arrange
            var usuario = new UsuarioResponse
            {
                Id = Guid.NewGuid(),
                Nome = "Jogador Teste",
                Email = "jogador@teste.com",
                PerfilUsuario = TipoUsuario.Jogador
            };

            // Act
            var claims = await _tokenService.ObtemClaims(usuario);

            // Assert
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "JogadorRole");
        }

        [Fact]
        public async Task ObtemClaims_DeveConterClaimsBasicas()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var usuario = new UsuarioResponse
            {
                Id = usuarioId,
                Nome = "Usuario Teste",
                Email = "usuario@teste.com",
                PerfilUsuario = TipoUsuario.Jogador
            };

            // Act
            var claims = await _tokenService.ObtemClaims(usuario);

            // Assert
            claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == usuario.Nome);
            claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == usuario.Id.ToString());
            claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == usuario.Email);
        }

        [Fact]
        public async Task GerarToken_DeveRetornarTokenValidoComDadosCorretos()
        {
            // Arrange
            var usuario = new UsuarioResponse
            {
                Id = Guid.NewGuid(),
                Nome = "Usuario JWT",
                Email = "jwt@teste.com",
                PerfilUsuario = TipoUsuario.Administrador
            };

            // Act
            var result = await _tokenService.GerarToken(usuario);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.ExpiresIn.Should().Be(_jwtSettings.ExpiracaoHoras * 3600);
            
            // Verifica se os claims mapeados voltaram no objeto ClaimResponse corretamente
            result.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "AdminRole");
            result.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == usuario.Email);
        }
    }
}
