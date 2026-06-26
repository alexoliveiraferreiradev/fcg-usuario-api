using Fcg.Core.Abstractions.Common;
using Fcg.Usuarios.Domain.Common;
using Fcg.Usuarios.Domain.Constants;

namespace Fcg.Usuarios.Domain.ValueObjects
{
    public class Nome : ValueObject<Nome>
    {
        public string Valor { get; }
        public Nome(string valor)
        {
            AssertionConcern.AssertArgumentRealValues(valor, MensagensDominio.NomeNaoReal);
            AssertionConcern.AssertArgumentEmpty(valor, MensagensDominio.UsuarioNomeObrigatorio);
            AssertionConcern.AssertArgumentLength(valor, 3, 50, MensagensDominio.UsuarioTamanhoNomeInvalido);
            Valor = valor;
        }

        protected override bool EqualsCore(Nome other)
        {
            return Valor == other.Valor;
        }

        protected override int GetHashCodeCore()
        {
            return Valor.GetHashCode();
        }
    }
}
