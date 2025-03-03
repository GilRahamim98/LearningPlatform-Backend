using System.Security.Cryptography;
using System.Text;

namespace Talent;

public static class PasswordHasher
{
    public static string HashPassword(string plainText)
    {
        string salt = "Hj5BV#9$kL7@zW2qpXrTmN8*yE6!cF3dA1sG4^vU";
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(plainText, saltBytes, 18, HashAlgorithmName.SHA512);
        byte[] hashBytes = rfc.GetBytes(64);
        string hashPassword = Convert.ToBase64String(hashBytes);
        return hashPassword;
    }
}
