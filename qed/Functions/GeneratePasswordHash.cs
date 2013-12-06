using System;
using System.Security.Cryptography;

namespace qed
{
    public static partial class Functions
    {
        public static string GeneratePasswordHash(
            string password,
            byte[] saltBytes = null,
            int iterations = 2048)
        {
            if (saltBytes == null)
            {
                saltBytes = new byte[16];

                var rng = RandomNumberGenerator.Create();
                rng.GetBytes(saltBytes);
            }

            string hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations))
            {
                var key = pbkdf2.GetBytes(64);
                hash = Convert.ToBase64String(key);
            }

            return string.Concat(Convert.ToBase64String(saltBytes), ".", iterations, ".", hash);
        }
    }
}