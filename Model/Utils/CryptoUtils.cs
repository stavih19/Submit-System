using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Linq;
namespace Submit_System
{
    public class CryptoUtils
    {
        private const int HASH_LEN = 20;
        private const int SALT_START = HASH_LEN + sizeof(int);
        private const int SALT_LEN = 8;
        private const int ITER = 1000;
        /// <summary>
        /// Hashes the given password with PBKDR2
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>
        /// [20 bytes representing hash of password][4 bytes representing number of iterations][The salt (8 bytes by default)]
        /// </returns>
        public static byte[] Hash(string password, byte[] salt=null, int iter=ITER)
        {
            if(salt == null) {
                salt = GetRandomBytes(SALT_LEN);
            }
            var result = new byte[SALT_START + salt.Length];
            using (var hasher = new Rfc2898DeriveBytes(password, salt, iter))
            {
                hasher.GetBytes(HASH_LEN).CopyTo(result, 0);
                hasher.Salt.CopyTo(result, SALT_START);
            }
            BitConverter.GetBytes(iter).CopyTo(result, HASH_LEN);
            return result;
        }
        public static byte[] GetRandomBytes(int len)
        {
            byte[] rand = new byte[len];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(rand);
            }
            return rand;
        }
        /// <summary>
        /// Comapres the hash of an input password with the hash in the database
        /// </summary>
        /// <param name="password">The input password</param>
        /// <param name="storedHash">The database hash</param>
        /// <returns>True if they equal, false otherwise</returns>
        public static bool Compare(string password, byte[] storedHash)
        {
            int iter = BitConverter.ToInt32(storedHash[HASH_LEN..SALT_START]);
            byte[] inputHash = Hash(password, storedHash[SALT_START..storedHash.Length], iter);
            return Enumerable.SequenceEqual(inputHash, storedHash);
        }
        /// <summary>
        /// Generate random password for testing purposes
        /// </summary>
        public static string GeneratePassword()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string str = "";
            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                str += chars[random.Next(chars.Length)];
            }
            return str;
            
        }
        /// <summary>
        /// Test the hash function.
        /// </summary>
        public static void Test()
        {
            const int num = 1;
            int count = 0;
            int count2 = 0;
            var stop = new Stopwatch();
            stop.Start();
            var r = new Random();
            for(int i = 0; i < num; i++)
            {
                string pass = GeneratePassword();
                string fakePass = GeneratePassword();
                var salt = GetRandomBytes(r.Next(8, 16));
                var iter = 500000; //r.Next(500, 1500);
                var hash = Hash(pass, salt, iter);
                // if(Compare(pass, hash))
                // {
                //     count++;
                // }
                // if(!Compare(fakePass, hash)) {
                //     count2++;
                // }
            }
            stop.Stop();
            Trace.WriteLine($"Matches: {count}/{num} | Mismatches: {count2}/{num} | Elapsed: {stop.Elapsed}");
        }
    }
}