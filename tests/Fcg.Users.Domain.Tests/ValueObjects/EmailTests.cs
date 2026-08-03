using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;

namespace Fcg.Users.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Constructor_ShouldCreateEmail_WhenValueIsValid()
        {
            var email = new Email("teste@teste.com");

            email.Value.Should().Be("teste@teste.com");
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenValueIsEmpty()
        {
            var act = () => new Email(string.Empty);

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserEmailRequired);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenFormatIsInvalid()
        {
            var act = () => new Email("email-invalido");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.EmailInvalid);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenLengthIsBelowMinimum()
        {
            var act = () => new Email("a@b.c");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.EmailLengthInvalid);
        }

        [Fact]
        public void Equals_ShouldBeConsideredEqual_WhenValuesAreTheSame()
        {
            var email1 = new Email("teste@teste.com");
            var email2 = new Email("teste@teste.com");

            email1.Equals(email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_ShouldNotBeConsideredEqual_WhenValuesAreDifferent()
        {
            var email1 = new Email("teste@teste.com");
            var email2 = new Email("outro@teste.com");

            email1.Equals(email2).Should().BeFalse();
        }
    }
}
