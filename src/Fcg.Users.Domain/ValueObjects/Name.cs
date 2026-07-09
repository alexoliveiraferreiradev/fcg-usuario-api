using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Name : ValueObject<Name>
    {
        public string Value { get; }
        public Name(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, DomainMessages.NameNotReal);
            AssertionConcern.AssertArgumentEmpty(valor, DomainMessages.UserNameRequired);
            AssertionConcern.AssertArgumentLength(valor, 3, 50, DomainMessages.UserNameLengthInvalid);
            Value = valor;
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
