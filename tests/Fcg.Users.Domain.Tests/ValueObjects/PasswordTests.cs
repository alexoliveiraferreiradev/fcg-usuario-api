using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;

namespace Fcg.Users.Domain.Tests.ValueObjects
{
    public class PasswordTests
    {
        [Fact]
        public void Constructor_ShouldCreatePassword_WhenHashIsStrong()
        {
            var password = new Password("SenhaForte@123");

            password.Hash.Should().Be("SenhaForte@123");
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenHashIsEmpty()
        {
            var act = () => new Password(string.Empty);

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNewPasswordWeak);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenHashHasNoUppercase()
        {
            var act = () => new Password("senhaforte@123");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNewPasswordWeak);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenHashHasNoLowercase()
        {
            var act = () => new Password("SENHAFORTE@123");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNewPasswordWeak);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenHashHasNoDigit()
        {
            var act = () => new Password("SenhaForte@Test");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNewPasswordWeak);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenHashHasNoSpecialCharacter()
        {
            var act = () => new Password("SenhaForte123");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNewPasswordWeak);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenHashIsTooShort()
        {
            var act = () => new Password("Ab1!Ab1");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNewPasswordWeak);
        }

        [Fact]
        public void Equals_ShouldBeConsideredEqual_WhenHashesAreTheSame()
        {
            var password1 = new Password("SenhaForte@123");
            var password2 = new Password("SenhaForte@123");

            password1.Equals(password2).Should().BeTrue();
        }

        [Fact]
        public void Equals_ShouldNotBeConsideredEqual_WhenHashesAreDifferent()
        {
            var password1 = new Password("SenhaForte@123");
            var password2 = new Password("OutraSenha@456");

            password1.Equals(password2).Should().BeFalse();
        }
    }
}
