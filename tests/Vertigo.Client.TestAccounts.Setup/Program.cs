using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

const string HostUsernameKey = "VERTIGO_TEST_HOST_USER";
const string HostPasswordKey = "VERTIGO_TEST_HOST_PASSWORD";
const string InviteeUsernameKey = "VERTIGO_TEST_INVITEE_USER";
const string InviteePasswordKey = "VERTIGO_TEST_INVITEE_PASSWORD";

string? hostName = Environment.GetEnvironmentVariable(HostUsernameKey);
string? hostPassword = Environment.GetEnvironmentVariable(HostPasswordKey);
string? inviteeName = Environment.GetEnvironmentVariable(InviteeUsernameKey);
string? inviteePassword = Environment.GetEnvironmentVariable(InviteePasswordKey);

if (string.IsNullOrWhiteSpace(hostName) || string.IsNullOrWhiteSpace(hostPassword)
    || string.IsNullOrWhiteSpace(inviteeName) || string.IsNullOrWhiteSpace(inviteePassword))
{
    Console.Error.WriteLine("CONFIGURATION REQUIRED: set the four VERTIGO_TEST_* variables.");
    return 2;
}

if (string.Equals(hostName, inviteeName, StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine("FAIL: Test accounts must have different usernames.");
    return 2;
}

ServiceCollection registrations = new();
registrations.AddVertigoClientNetworking();
using ServiceProvider provider = registrations.BuildServiceProvider();
IAccountClient accounts = provider.GetRequiredService<IAccountClient>();
IAuthClient authentication = provider.GetRequiredService<IAuthClient>();

bool hostReady = PrepareAccount(accounts, authentication, hostName, hostPassword, "host");
bool inviteeReady = PrepareAccount(accounts, authentication, inviteeName, inviteePassword, "invitee");

if (!hostReady || !inviteeReady)
{
    Console.Error.WriteLine("FAIL: Registered test accounts are not ready.");
    return 1;
}

Console.WriteLine("PASS: Both registered test accounts are ready.");
return 0;

static bool PrepareAccount(IAccountClient accounts, IAuthClient authentication,
    string username, string password, string label)
{
    // The test identity never needs incoming email; use a unique, reserved example address.
    string email = $"vertigo-test-{Guid.NewGuid():N}@example.com";
    AccountRegistration registration = new()
    {
        Username = username,
        Email = email,
        Password = password
    };

    ClientResult<bool> register = accounts.RegisterAccount(registration);
    if (!register.IsSuccess && register.ErrorCode != ServiceErrorCodes.UsernameAlreadyRegistered)
    {
        Console.Error.WriteLine($"FAIL: {label} registration: {register.Failure} ({register.ErrorCode}).");
        return false;
    }

    ClientResult<UserSession> login = authentication.Login(new LoginCredentials
    {
        Username = username,
        Password = password
    });

    if (!login.IsSuccess || login.Value is null || login.Value.IsGuest)
    {
        Console.Error.WriteLine($"FAIL: {label} authentication: {login.Failure} ({login.ErrorCode}).");
        return false;
    }

    ClientResult<bool> logout = authentication.Logout(login.Value.Token);
    if (!logout.IsSuccess || !logout.Value)
    {
        Console.Error.WriteLine($"FAIL: {label} logout: {logout.Failure} ({logout.ErrorCode}).");
        return false;
    }

    Console.WriteLine($"PASS: {label} registered account verified.");
    return true;
}
