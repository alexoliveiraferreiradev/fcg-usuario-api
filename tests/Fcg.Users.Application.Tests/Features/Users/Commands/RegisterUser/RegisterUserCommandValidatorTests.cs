using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using FluentAssertions;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.CadastrarUser
{
    public class CadastrarUserCommandValidatorTests
    {
        private readonly CadastrarUserCommandValidator _validator;

        public CadastrarUserCommandValidatorTests()
        {
            _validator = new CadastrarUserCommandValidator();
        }

        [Fact]
        public void Validate_DeveRetornarValido_QuandoComandoEstiverCorreto()
        {
            // Arrange
            var command = new RegisterUserCommand("User Teste", "teste@teste.com", "SenhaForte123", "SenhaForte123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")] // Menor que 3
        [InlineData("Este Name de User e muito longo e deve falhar na validacao porque tem mais de 50 caracteres!!!")] // Maior que 50
        public void Validate_DeveFalhar_QuandoNomeForInvalido(string nomeInvalido)
        {
            // Arrange
            var command = new RegisterUserCommand(nomeInvalido, "teste@teste.com", "SenhaForte123", "SenhaForte123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Theory]
        [InlineData("")]
        [InlineData("EmailInvalid")]
        [InlineData("a@b.c")] // Menor que 7 caracteres
        public void Validate_DeveFalhar_QuandoEmailForInvalido(string EmailInvalid)
        {
            // Arrange
            var command = new RegisterUserCommand("User Teste", EmailInvalid, "SenhaForte123", "SenhaForte123");

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
            var command = new RegisterUserCommand("User Teste", "teste@teste.com", senhaInvalida, senhaInvalida);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Fact]
        public void Validate_DeveFalhar_QuandoConfirmacaoSenhaNaoForIgualASenha()
        {
            // Arrange
            var command = new RegisterUserCommand("User Teste", "teste@teste.com", "SenhaForte123", "SenhaDiferente123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ConfirmPassword");
        }
    }
}
