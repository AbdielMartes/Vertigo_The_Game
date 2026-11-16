namespace Vertigo.Server.CrossCutting.Security.Interfaces;

public interface ITokenGenerator
{
    string GenerateSessionToken();

    string GenerateVerificationCode();

    string HashSessionToken(string sessionToken);
}
