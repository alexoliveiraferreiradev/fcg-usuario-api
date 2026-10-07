using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Name : ValueObject<Name>
    {
        public string Value { get; }
        public Name(string value)
        {
            AssertionConcern.AssertArgumentRealValues(value, DomainMessages.NameNotReal);
            AssertionConcern.AssertArgumentEmpty(value, DomainMessages.UserNameRequired);
            AssertionConcern.AssertArgumentLength(value, 3, 50, DomainMessages.UserNameLengthInvalid);
            Value = value;
        }

        protected override bool EqualsCore(Name other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
