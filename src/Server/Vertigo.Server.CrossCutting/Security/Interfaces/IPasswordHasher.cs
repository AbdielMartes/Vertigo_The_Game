namespace Vertigo.Server.CrossCutting.Security.Interfaces;

public interface IPasswordHasher
{
    string GenerateSalt();

    string HashPassword(string password, string passwordSalt);

    bool VerifyPassword(string password, string passwordHash, string passwordSalt);
}
