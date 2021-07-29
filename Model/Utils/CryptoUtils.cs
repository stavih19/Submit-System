using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace Submit_System
{
    public static class CryptoUtils
    {
        public static int HashLength { get; }
        public static int DefaultSaltLength { get; }
        public static int DefaultIterations { get; }
        private const string SEP = "$";
        private const string FORMAT = "{0}" + SEP + "{1}" + SEP +"{2}";

        static CryptoUtils()
        {
            var conf = MyConfig.Configuration.GetSection("Hash");
            DefaultIterations = conf.GetValue<int>("Iterations");
            DefaultSaltLength = conf.GetValue<int>("Salt length");
            HashLength = conf.GetValue<int>("Hash length");
        }
        /// <summary>
        /// Hashes the given password with PBKDR2
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>
        /// Format: {Password hash}${Salt}${Number of iterations in thousands}
        /// </returns>
        private static string Hash(string password, byte[] salt, int iter)
        {
            byte[] hash;
            using (var hasher = new Rfc2898DeriveBytes(password, salt, iter))
            {
                hash = hasher.GetBytes(HashLength);
            }
            return String.Format(FORMAT, Convert.ToBase64String(hash), Convert.ToBase64String(salt), iter.ToString());
        }
        public static string Sha256Hash(string str)
        {
            byte[] result_bytes;
            using (var hasher = SHA256.Create())
            {
                result_bytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(str));
            }
            return Convert.ToBase64String(result_bytes);
        }
        /// <summary>
        /// Overload. Gets Salt length.
        /// </summary>
        /// <param name="passsword"></param>
        /// <param name="saltLength"></param>
        /// <param name="iter">Iterations. Must be at least 1000.</param>
        /// <returns></returns>
        public static string KDFHash(string passsword, int saltLength, int iter)
        {
            return Hash(passsword, GetRandomBytes(saltLength), iter);
        }
        public static string KDFHash(string passsword)
        {
            return Hash(passsword, GetRandomBytes(DefaultSaltLength), DefaultIterations);
        }
        public static string GetRandomBase64String(int bytes_len)
        {
            var bytes = GetRandomBytes(bytes_len);
            return Convert.ToBase64String(bytes);
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
        public static bool Verify(string password, string storedHash)
        {
            string[] components = storedHash.Split(SEP);
            if(components.Length != 3) { return false; }
            string inputHash = Hash(password, Convert.FromBase64String(components[1]), Int32.Parse(components[2]));
            return inputHash == storedHash;
        }
        /// <summary>
        /// Generate random weak random string
        /// </summary>
        public static string GeneratePassword()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var builder = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                builder.Append(chars[random.Next(chars.Length)]);
            }
            return builder.ToString();
            
        }
        /// <summary>
        /// Test the hash function.
        /// </summary>
        public static void Test()
        {
            const int num = 10;
            int count = 0;
            int count2 = 0;
            var stop = new Stopwatch();
            stop.Start();
            var r = new Random();
            for(int i = 0; i < num; i++)
            {
                string pass = GeneratePassword();
                string fakePass = GeneratePassword();
                var salt = GetRandomBytes(8);
                var iter = 10000;
                var hash = Hash(pass, salt, iter);
                if(Verify(pass, hash))
                {
                    count++;
                }
                if(!Verify(fakePass, hash)) {
                    count2++;
                }
            }
            stop.Stop();
            Debug.WriteLine($"Matches: {count}/{num} | Mismatches: {count2}/{num} | Elapsed: {stop.Elapsed}");
        }
    }
}