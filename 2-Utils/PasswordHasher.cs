using System.Security.Cryptography;
using System.Text;

namespace Talent;

public static class PasswordHasher
{
    // Hashes the given plain text password using a salt and SHA-512 algorithm
    public static string HashPassword(string plainText)
    {
        // Define a static salt value
        string salt = "Hj5BV#9$kL7@zW2qpXrTmN8*yE6!cF3dA1sG4^vU";

        // Convert the salt to a byte array
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

        // Create an instance of Rfc2898DeriveBytes with the plain text password, salt, iteration count, and hash algorithm
        Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(plainText, saltBytes, 18, HashAlgorithmName.SHA512);

        // Generate the hash bytes
        byte[] hashBytes = rfc.GetBytes(64);

        // Convert the hash bytes to a Base64 string
        string hashPassword = Convert.ToBase64String(hashBytes);

        return hashPassword;
    }
}
