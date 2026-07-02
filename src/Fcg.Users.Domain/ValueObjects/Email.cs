using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Email : ValueObject<Email>
    {
        public string Valor { get; }

        public Email(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, DomainMessages.EmailNotReal);
            AssertionConcern.AssertArgumentEmpty(valor, DomainMessages.UserEmailRequired);
            AssertionConcern.AssertArgumentEmailFormat(valor, DomainMessages.EmailInvalid);
            AssertionConcern.AssertArgumentLength(valor, 7, 100, DomainMessages.EmailLengthInvalid);
            Valor = valor;
        }

        protected override bool EqualsCore(Email other)
        {
            return Valor == other.Valor;
        }

        protected override int GetHashCodeCore()
        {
            return Valor.GetHashCode();
        }
    }
}
