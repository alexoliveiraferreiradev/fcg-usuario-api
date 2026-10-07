using Fcg.Core.WebApi.Security;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Fcg.Users.Infrastructure.Integration.Security
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
                ExpirationHours = 2,
                Issuer = "FcgApp",
                Audience = "FcgAppClients"
            };

            var optionsMock = Options.Create(_jwtSettings);
            _tokenService = new TokenService(optionsMock);
        }

        [Fact]
        public async Task ObtemClaims_DeveIncluirClaimDeAdmin_QuandoUserForAdministrador()
        {
            // Arrange
            var User = new UserResponse
            {
                Id = Guid.NewGuid(),
                Name = "Admin Teste",
                Email = "admin@teste.com",
                PerfilUser = UserRole.Admin
            };

            // Act
            var claims = await _tokenService.GetClaims(User);

            // Assert
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "AdminRole");
        }

        [Fact]
        public async Task ObtemClaims_DeveIncluirClaimDeJogador_QuandoUserForJogador()
        {
            // Arrange
            var User = new UserResponse
            {
                Id = Guid.NewGuid(),
                Name = "Player Teste",
                Email = "Player@teste.com",
                PerfilUser = UserRole.Player
            };

            // Act
            var claims = await _tokenService.GetClaims(User);

            // Assert
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "PlayerRole");
        }

        [Fact]
        public async Task ObtemClaims_DeveConterClaimsBasicas()
        {
            // Arrange
            var UserId = Guid.NewGuid();
            var User = new UserResponse
            {
                Id = UserId,
                Name = "User Teste",
                Email = "User@teste.com",
                PerfilUser = UserRole.Player
            };

            // Act
            var claims = await _tokenService.GetClaims(User);

            // Assert
            claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == User.Name);
            claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == User.Id.ToString());
            claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == User.Email);
        }

        [Fact]
        public async Task GerarToken_DeveRetornarTokenValidoComDadosCorretos()
        {
            // Arrange
            var User = new UserResponse
            {
                Id = Guid.NewGuid(),
                Name = "User JWT",
                Email = "jwt@teste.com",
                PerfilUser = UserRole.Admin
            };

            // Act
            var result = await _tokenService.GenerateToken(User);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.ExpiresIn.Should().Be(_jwtSettings.ExpirationHours * 3600);
            
            // Verifica se os claims mapeados voltaram no objeto ClaimResponse corretamente
            result.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "AdminRole");
            result.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == User.Email);
        }
    }
}
