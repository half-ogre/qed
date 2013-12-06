using System;
using System.Security.Cryptography;

namespace qed
{
    public static partial class Functions
    {
        public static bool CompareToPasswordHash(this string password, string passwordHash)
        {
            var firstSeparator = passwordHash.IndexOf('.');
            if (firstSeparator < 0)
                return false;

            var secondSeparator = passwordHash.IndexOf('.', firstSeparator + 1);
            if (secondSeparator < 0)
                return false;

            var salt = passwordHash.Substring(0, firstSeparator);
            var iterationsText = passwordHash.Substring(firstSeparator + 1, secondSeparator - (firstSeparator + 1));
            var hash = passwordHash.Substring(secondSeparator + 1);

            int iterations;
            if (!int.TryParse(iterationsText, out iterations))
            {
                return false;
            }

            string hashToCompare;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations))
            {
                var key = pbkdf2.GetBytes(64);
                hashToCompare = Convert.ToBase64String(key);
            }

            var result = 0;
            for (var i = 0; i < hash.Length; i++)
                result |= hash[i] ^ hashToCompare[i];

            return 0 == result;
        }
    }
}