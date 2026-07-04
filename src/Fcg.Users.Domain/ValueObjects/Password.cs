using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Password : ValueObject<Password>
    {
        public string Hash { get; }

        public Password(string hash)
        {            
            AssertionConcern.AssertArgumentPasswordStrenght(hash, DomainMessages.UserNewPasswordWeak); 
            AssertionConcern.AssertArgumentEmpty(hash, DomainMessages.UserPasswordRequired);
            Hash = hash;
        }

        protected override bool EqualsCore(Password other)
        {
            return Hash == other.Hash;
        }

        protected override int GetHashCodeCore()
        {
            return Hash.GetHashCode();
        }
    }
}
