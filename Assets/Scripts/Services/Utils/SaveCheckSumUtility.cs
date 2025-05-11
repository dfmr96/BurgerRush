using System.Security.Cryptography;
using System.Text;

namespace Services.Utils
{
    public static class SaveCheckSumUtility
    {
        public static string GenerateChecksum(string json)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(json);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return System.BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}