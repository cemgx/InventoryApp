using Microsoft.AspNet.Identity;
using System.Security.Cryptography;

namespace InventoryApp.Application.Hash
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

        private const string AppKey = "AppKey";
        public static string GenerateSalt()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(SaltSize));
        }
        public (string Hash, string Salt) HashPassword(string password)
        {
            byte[] userSalt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password + AppKey, userSalt, Iterations, HashAlgorithm, HashSize);

            return (Convert.ToHexString(hash), Convert.ToHexString(userSalt));
        }


        public bool VerifyPassword(string hashedPassword, string salt, string providedPassword)
        {
            byte[] Salt = Convert.FromHexString(salt);
            byte[] Hash = Rfc2898DeriveBytes.Pbkdf2(providedPassword + AppKey, Salt, Iterations, HashAlgorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(hashedPassword), Hash);
        }
    }
}
