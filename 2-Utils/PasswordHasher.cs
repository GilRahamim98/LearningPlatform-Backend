using System.Security.Cryptography;
using System.Text;

namespace Talent;

public static class PasswordHasher
{
    public static string HashPassword(string plainText)
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(plainText, saltBytes, 10000, HashAlgorithmName.SHA512);
        byte[] hashBytes = rfc.GetBytes(64);

        byte[] combinedBytes = new byte[saltBytes.Length + hashBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(hashBytes, 0, combinedBytes, saltBytes.Length, hashBytes.Length);

        string hashPassword = Convert.ToBase64String(hashBytes);
        return hashPassword;
    }
}
