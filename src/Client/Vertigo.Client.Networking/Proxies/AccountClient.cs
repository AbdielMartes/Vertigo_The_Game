using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class AccountClient : IAccountClient
{
    private readonly IServiceCaller _serviceCaller;

    public AccountClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<bool> RegisterAccount(AccountRegistration request)
    {
        return _serviceCaller.Call<IAccountService, AccountRegistration, bool>(
            ServiceEndpoints.AccountServiceAddress,
            RegisterAccountOperation,
            request);
    }

    public ClientResult<bool> ChangePassword(PasswordChange change, string sessionToken)
    {
        return _serviceCaller.Call<IAccountService, (PasswordChange, string), bool>(
            ServiceEndpoints.AccountServiceAddress,
            (channel, args) => channel.ChangePassword(args.Item1, args.Item2),
            (change, sessionToken));
    }

    public ClientResult<bool> UpdateProfile(ProfileUpdate profile, string sessionToken)
    {
        return _serviceCaller.Call<IAccountService, (ProfileUpdate Profile, string Token), bool>(
            ServiceEndpoints.AccountServiceAddress,
            (channel, args) => channel.UpdateProfile(args.Profile, args.Token),
            (profile, sessionToken));
    }

    private static bool RegisterAccountOperation(IAccountService channel, AccountRegistration request)
    {
        return channel.RegisterAccount(request);
    }
}
