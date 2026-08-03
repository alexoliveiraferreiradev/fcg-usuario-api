using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;

namespace Fcg.Users.Domain.Tests.ValueObjects
{
    public class NameTests
    {
        [Fact]
        public void Constructor_ShouldCreateName_WhenValueIsValid()
        {
            var name = new Name("User Teste");

            name.Value.Should().Be("User Teste");
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenValueIsEmpty()
        {
            var act = () => new Name(string.Empty);

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNameRequired);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenLengthIsBelowMinimum()
        {
            var act = () => new Name("ab");

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNameLengthInvalid);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenLengthIsAboveMaximum()
        {
            var act = () => new Name(new string('a', 51));

            var exception = act.Should().Throw<DomainException>().Which;
            exception.Message.Should().Be(DomainMessages.UserNameLengthInvalid);
        }

        [Fact]
        public void Equals_ShouldBeConsideredEqual_WhenValuesAreTheSame()
        {
            var name1 = new Name("User Teste");
            var name2 = new Name("User Teste");

            name1.Equals(name2).Should().BeTrue();
        }

        [Fact]
        public void Equals_ShouldNotBeConsideredEqual_WhenValuesAreDifferent()
        {
            var name1 = new Name("User Teste");
            var name2 = new Name("Outro User");

            name1.Equals(name2).Should().BeFalse();
        }
    }
}
