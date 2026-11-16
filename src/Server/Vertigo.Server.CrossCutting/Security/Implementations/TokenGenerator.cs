using Vertigo.Server.CrossCutting.Security.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Vertigo.Server.CrossCutting.Security.Implementations;

public class TokenGenerator : ITokenGenerator
{
    private const int SessionTokenByteLength = 32;
    private const int MinimumVerificationCode = 100000;
    private const int MaximumVerificationCodeExclusive = 1000000;

    public string GenerateSessionToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(SessionTokenByteLength);
        string sessionToken = Convert.ToHexString(randomBytes);

        return sessionToken;
    }

    public string GenerateVerificationCode()
    {
        int code = RandomNumberGenerator.GetInt32(
            MinimumVerificationCode,
            MaximumVerificationCodeExclusive);
        string verificationCode = code.ToString(System.Globalization.CultureInfo.InvariantCulture);

        return verificationCode;
    }

    public string HashSessionToken(string sessionToken)
    {
        byte[] tokenBytes = Encoding.UTF8.GetBytes(sessionToken);
        byte[] hashBytes = SHA256.HashData(tokenBytes);
        string tokenHash = Convert.ToHexString(hashBytes);

        return tokenHash;
    }
}
