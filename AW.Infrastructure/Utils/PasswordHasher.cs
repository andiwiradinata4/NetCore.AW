using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AW.Infrastructure.Utils
{
    public class PasswordHasher
    {
        const int iterations = 10000, copyLengthSalt = 16, copyLengthHash = 20, saltBytes = 16;

        public static string HashPassword(string password)
        {
            byte[] salt;
#pragma warning disable SYSLIB0023 // Type or member is obsolete
            using (var rng = new RNGCryptoServiceProvider())
            {
                salt = new byte[saltBytes];
                rng.GetBytes(salt);
            }
#pragma warning restore SYSLIB0023 // Type or member is obsolete

#pragma warning disable SYSLIB0041 // Type or member is obsolete
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
#pragma warning restore SYSLIB0041 // Type or member is obsolete
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, copyLengthSalt);
            Array.Copy(hash, 0, hashBytes, copyLengthSalt, copyLengthHash);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedPasswordHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedPasswordHash);
            byte[] salt = new byte[saltBytes];
            Array.Copy(hashBytes, 0, salt, 0, copyLengthSalt);

#pragma warning disable SYSLIB0041 // Type or member is obsolete
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
#pragma warning restore SYSLIB0041 // Type or member is obsolete
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < copyLengthHash; i++)
            {
                if (hashBytes[i + copyLengthSalt] != hash[i]) return false;
            }
            return true;
        }
    }
}
