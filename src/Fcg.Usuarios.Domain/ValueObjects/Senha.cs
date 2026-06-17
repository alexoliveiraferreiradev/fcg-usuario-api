using Fcg.Usuarios.Domain.Common;
using Fcg.Usuarios.Domain.Constants;

namespace Fcg.Usuarios.Domain.ValueObjects
{
    public class Senha : ValueObject<Senha>
    {
        public string Hash { get; }

        public Senha(string hash)
        {
            AssertionConcern.AssertArgumentEmpty(hash, MensagensDominio.UsuarioSenhaObrigatoria);
            Hash = hash;
        }

        protected override bool EqualsCore(Senha other)
        {
            return Hash == other.Hash;
        }

        protected override int GetHashCodeCore()
        {
            return Hash.GetHashCode();
        }
    }
}
