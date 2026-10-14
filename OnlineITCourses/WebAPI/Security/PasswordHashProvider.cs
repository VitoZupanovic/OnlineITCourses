using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace WebAPI.Security
{
    public class PasswordHashProvider
    {
        public static string GetSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(salt);
        }

        public static string GetHash(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            var hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32);

            return Convert.ToBase64String(hash);
        }
    }
}
