using System.Security.Cryptography;

namespace Api.AuthLib.Services
{
    public class SecretKeyGenerator
    {
        public static string GenerateSecretKey(int length = 32)
        {
            byte[] keyBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            return Convert.ToBase64String(keyBytes);
        }
    }
}