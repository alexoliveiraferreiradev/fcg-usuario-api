using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Domain.ValueObjects
{
    public class Password : ValueObject<Password>
    {
        public string Hash { get; }

        public Password(string hash)
        {
            AssertionConcern.AssertArgumentEmpty(hash, DomainMessages.UserPasswordRequired);
            Hash = hash;
        }
        public Password(string Password,string hash)
        {          
            AssertionConcern.AssertArgumentLength(Password, 8, 60, DomainMessages.PasswordLengthInvalid);          
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
