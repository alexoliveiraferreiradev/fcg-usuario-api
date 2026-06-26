using Fcg.Core.Abstractions.Common;
using Fcg.Usuarios.Domain.Constants;

namespace Fcg.Usuarios.Domain.ValueObjects
{
    public class Email : ValueObject<Email>
    {
        public string Valor { get; }

        public Email(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, MensagensDominio.EmailNaoReal);
            AssertionConcern.AssertArgumentEmpty(valor, MensagensDominio.UsuarioEmailObrigatorio);
            AssertionConcern.AssertArgumentEmailFormat(valor, MensagensDominio.EmailInvalido);
            AssertionConcern.AssertArgumentLength(valor, 7, 100, MensagensDominio.EmailTamanhoInvalido);
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
