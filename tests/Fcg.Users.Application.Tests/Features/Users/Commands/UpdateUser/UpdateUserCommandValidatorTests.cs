using Fcg.Users.Application.Features.Users.Commands.UpdateUser;
using FluentAssertions;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidatorTests
    {
        private readonly UpdateUserCommandValidator _validator = new();

        [Fact]
        public void Validate_ShouldReturnValid_WhenCommandIsCorrect()
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), "User Teste", "SenhaForte@123", "SenhaForte@123");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("Este Name de User e muito longo e deve falhar na validacao porque tem mais de 50 caracteres!!!")]
        public void Validate_ShouldFail_WhenNameIsInvalid(string nomeInvalido)
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), nomeInvalido, "SenhaForte@123", "SenhaForte@123");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1234567")]
        [InlineData("semmaiuscula@123")]
        [InlineData("SEMMINUSCULA@123")]
        [InlineData("SemNumero@aaaa")]
        [InlineData("SemCaractereEspecial123")]
        public void Validate_ShouldFail_WhenPasswordIsInvalid(string senhaInvalida)
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), "User Teste", senhaInvalida, senhaInvalida);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Fact]
        public void Validate_ShouldFail_WhenConfirmPasswordDoesNotMatchPassword()
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), "User Teste", "SenhaForte@123", "SenhaDiferente@456");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ConfirmPassword");
        }
    }
}
