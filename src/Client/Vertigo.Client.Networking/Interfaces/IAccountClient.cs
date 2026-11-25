using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IAccountClient
{
    ClientResult<bool> RegisterAccount(AccountRegistration registration);

    ClientResult<bool> ChangePassword(PasswordChange change, string sessionToken);

    ClientResult<bool> UpdateProfile(ProfileUpdate profile, string sessionToken);
}
