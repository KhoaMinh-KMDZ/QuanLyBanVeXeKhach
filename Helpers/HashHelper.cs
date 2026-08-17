using System.Security.Cryptography;
using System.Text;

namespace QuanLyBanVeXeKhach.Helpers
{
    public static class HashHelper
    {
        public static string SHA256Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static bool Verify(string plainText, string hash)
            => SHA256Hash(plainText).Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}
