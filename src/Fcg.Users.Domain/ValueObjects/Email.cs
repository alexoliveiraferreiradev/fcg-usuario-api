using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Email : ValueObject<Email>
    {
        public string Value { get; }

        public Email(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, DomainMessages.EmailNotReal);
            AssertionConcern.AssertArgumentEmpty(valor, DomainMessages.UserEmailRequired);
            AssertionConcern.AssertArgumentEmailFormat(valor, DomainMessages.EmailInvalid);
            AssertionConcern.AssertArgumentLength(valor, 7, 100, DomainMessages.EmailLengthInvalid);
            Value = valor;
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
