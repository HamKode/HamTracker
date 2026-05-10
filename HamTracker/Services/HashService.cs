using System.IO;
using System.Security.Cryptography;

namespace HamTracker.Services
{
    public class HashService
    {
        public static string ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return System.BitConverter.ToString(hashBytes)
                                 .Replace("-", "").ToLower();
                }
            }
        }

        public static bool VerifyFileHash(string filePath, string storedHash)
        {
            string currentHash = ComputeFileHash(filePath);
            return currentHash.Equals(storedHash,
                   System.StringComparison.OrdinalIgnoreCase);
        }
    }
}