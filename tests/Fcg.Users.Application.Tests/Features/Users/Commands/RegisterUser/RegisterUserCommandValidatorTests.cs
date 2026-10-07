using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using FluentAssertions;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidatorTests
    {
        private readonly RegisterUserCommandValidator _validator;

        public RegisterUserCommandValidatorTests()
        {
            _validator = new RegisterUserCommandValidator();
        }

        [Fact]
        public void Validate_ShouldReturnValid_WhenCommandIsCorrect()
        {
            // Arrange
            var command = new RegisterUserCommand("User Teste", "teste@teste.com", "SenhaForte@123", "SenhaForte@123");

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
        public void Validate_ShouldFail_WhenNameIsInvalid(string nomeInvalido)
        {
            // Arrange
            var command = new RegisterUserCommand(nomeInvalido, "teste@teste.com", "SenhaForte@123", "SenhaForte@123");

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
        public void Validate_ShouldFail_WhenEmailIsInvalid(string EmailInvalid)
        {
            // Arrange
            var command = new RegisterUserCommand("User Teste", EmailInvalid, "SenhaForte@123", "SenhaForte@123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1234567")] // Menor que 8
        public void Validate_ShouldFail_WhenPasswordIsInvalid(string senhaInvalida)
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
        public void Validate_ShouldFail_WhenConfirmPasswordDoesNotMatchPassword()
        {
            // Arrange
            var command = new RegisterUserCommand("User Teste", "teste@teste.com", "SenhaForte@123", "SenhaDiferente123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ConfirmPassword");
        }
    }
}
