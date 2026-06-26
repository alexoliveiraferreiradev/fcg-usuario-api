using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
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
        public Senha(string senha,string hash)
        {          
            AssertionConcern.AssertArgumentLength(senha, 8, 60, MensagensDominio.SenhaTamanhoInvalido);          
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
