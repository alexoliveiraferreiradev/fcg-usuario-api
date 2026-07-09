using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Email : ValueObject<Email>
    {
        public string Value { get; }

        public Email(string value)
        {
            AssertionConcern.AssertArgumentRealValues(value, DomainMessages.EmailNotReal);
            AssertionConcern.AssertArgumentEmpty(value, DomainMessages.UserEmailRequired);
            AssertionConcern.AssertArgumentEmailFormat(value, DomainMessages.EmailInvalid);
            AssertionConcern.AssertArgumentLength(value, 7, 100, DomainMessages.EmailLengthInvalid);
            Value = value;
        }

        protected override bool EqualsCore(Email other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
