using Fcg.Core.Abstractions.Resources;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario;
using FluentAssertions;
using Xunit;

namespace Fcg.Usuarios.Application.Tests.Features.Usuarios.Commands.CadastrarUsuario
{
    public class CadastrarUsuarioCommandValidatorTests
    {
        private readonly CadastrarUsuarioCommandValidator _validator;

        public CadastrarUsuarioCommandValidatorTests()
        {
            _validator = new CadastrarUsuarioCommandValidator();
        }

        [Fact]
        public void Validate_DeveRetornarValido_QuandoComandoEstiverCorreto()
        {
            // Arrange
            var command = new CadastrarUsuarioCommand("Usuario Teste", "teste@teste.com", "SenhaForte123", "SenhaForte123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")] // Menor que 3
        [InlineData("Este nome de usuario e muito longo e deve falhar na validacao porque tem mais de 50 caracteres!!!")] // Maior que 50
        public void Validate_DeveFalhar_QuandoNomeForInvalido(string nomeInvalido)
        {
            // Arrange
            var command = new CadastrarUsuarioCommand(nomeInvalido, "teste@teste.com", "SenhaForte123", "SenhaForte123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Nome");
        }

        [Theory]
        [InlineData("")]
        [InlineData("emailinvalido")]
        [InlineData("a@b.c")] // Menor que 7 caracteres
        public void Validate_DeveFalhar_QuandoEmailForInvalido(string emailInvalido)
        {
            // Arrange
            var command = new CadastrarUsuarioCommand("Usuario Teste", emailInvalido, "SenhaForte123", "SenhaForte123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1234567")] // Menor que 8
        public void Validate_DeveFalhar_QuandoSenhaForInvalida(string senhaInvalida)
        {
            // Arrange
            var command = new CadastrarUsuarioCommand("Usuario Teste", "teste@teste.com", senhaInvalida, senhaInvalida);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Senha");
        }

        [Fact]
        public void Validate_DeveFalhar_QuandoConfirmacaoSenhaNaoForIgualASenha()
        {
            // Arrange
            var command = new CadastrarUsuarioCommand("Usuario Teste", "teste@teste.com", "SenhaForte123", "SenhaDiferente123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ConfirmacaoSenha");
        }
    }
}
