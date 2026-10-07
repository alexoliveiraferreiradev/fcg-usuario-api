using Fcg.Users.Domain.Common.Interfaces;

namespace Fcg.Users.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) { return BCrypt.Net.BCrypt.HashPassword(password); }

        public bool VerifyPassword(string password, string hashedPassoword) { return BCrypt.Net.BCrypt.Verify(password, hashedPassoword); }
    }
}
