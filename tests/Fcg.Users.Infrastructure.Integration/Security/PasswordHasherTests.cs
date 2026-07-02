using Fcg.Users.Infrastructure.Security;
using FluentAssertions;
using Xunit;

namespace Fcg.Users.Infrastructure.Integration.Security
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_DeveGerarHashDiferenteDaSenhaOriginal()
        {
            // Arrange
            var senhaOriginal = "SenhaSuperForte123!";

            // Act
            var hashGerado = _passwordHasher.HashPassword(senhaOriginal);

            // Assert
            hashGerado.Should().NotBeNullOrWhiteSpace();
            hashGerado.Should().NotBe(senhaOriginal);
        }

        [Fact]
        public void VerifyPassword_DeveRetornarVerdadeiro_QuandoSenhaEstiverCorreta()
        {
            // Arrange
            var senhaOriginal = "SenhaSuperForte123!";
            var hashGerado = _passwordHasher.HashPassword(senhaOriginal);

            // Act
            var isValido = _passwordHasher.VerifyPassword(senhaOriginal, hashGerado);

            // Assert
            isValido.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_DeveRetornarFalso_QuandoSenhaEstiverIncorreta()
        {
            // Arrange
            var senhaOriginal = "SenhaSuperForte123!";
            var senhaIncorreta = "SenhaErrada123!";
            var hashGerado = _passwordHasher.HashPassword(senhaOriginal);

            // Act
            var isValido = _passwordHasher.VerifyPassword(senhaIncorreta, hashGerado);

            // Assert
            isValido.Should().BeFalse();
        }
    }
}
