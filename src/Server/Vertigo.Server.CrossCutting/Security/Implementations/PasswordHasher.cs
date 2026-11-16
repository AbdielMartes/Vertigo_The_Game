using Vertigo.Server.CrossCutting.Security.Interfaces;
using System.Security.Cryptography;

namespace Vertigo.Server.CrossCutting.Security.Implementations;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltByteLength = 16;
    private const int HashByteLength = 32;
    private const int Iterations = 600000;

    public string GenerateSalt()
    {
        byte[] randomSalt = RandomNumberGenerator.GetBytes(SaltByteLength);
        string passwordSalt = Convert.ToBase64String(randomSalt);

        return passwordSalt;
    }

    public string HashPassword(string password, string passwordSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(passwordSalt);
        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            HashByteLength);
        string passwordHash = Convert.ToHexString(hashBytes);

        return passwordHash;
    }

    public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
    {
        string computedHash = HashPassword(password, passwordSalt);
        byte[] computedBytes = Convert.FromHexString(computedHash);
        byte[] storedBytes = Convert.FromHexString(passwordHash);
        bool isValid = CryptographicOperations.FixedTimeEquals(computedBytes, storedBytes);

        return isValid;
    }
}
