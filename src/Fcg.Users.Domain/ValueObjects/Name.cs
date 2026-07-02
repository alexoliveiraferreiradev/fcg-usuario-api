using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Name : ValueObject<Name>
    {
        public string Valor { get; }
        public Name(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, MensagensDominio.NomeNaoReal);
            AssertionConcern.AssertArgumentEmpty(valor, MensagensDominio.UsuarioNomeObrigatorio);
            AssertionConcern.AssertArgumentLength(valor, 3, 50, MensagensDominio.UsuarioTamanhoNomeInvalido);
            Valor = valor;
        }

        protected override bool EqualsCore(Name other)
        {
            return Valor == other.Valor;
        }

        protected override int GetHashCodeCore()
        {
            return Valor.GetHashCode();
        }
    }
}
