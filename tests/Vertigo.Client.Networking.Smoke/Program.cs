using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

ServiceCollection registrations = new();
registrations.AddVertigoClientNetworking();
using ServiceProvider services = registrations.BuildServiceProvider();
ISessionCoordinator authentication = services.GetRequiredService<ISessionCoordinator>();
PlayerSession session = services.GetRequiredService<PlayerSession>();
INavigator navigation = services.GetRequiredService<INavigator>();

string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "guest";
ClientResult<UserSession> login;
if (mode == "login" && args.Length == 3)
{
    login = authentication.Login(args[1], args[2]);
}
else if (mode == "guest")
{
    login = authentication.CreateGuestSession();
}
else
{
    Console.Error.WriteLine("Usage: dotnet run -- guest OR dotnet run -- login USERNAME PASSWORD");
    return 2;
}

if (!login.IsSuccess || login.Value is null)
{
    Console.Error.WriteLine($"Login failed: {login.Failure}, {login.ErrorCode}");
    return 1;
}

SessionSnapshot? active = session.GetActiveSession();
if (active is null)
{
    Console.Error.WriteLine("The session was not stored locally.");
    return 1;
}

Console.WriteLine($"Authenticated: {active.Username} (Guest={active.IsGuest})");
Console.WriteLine("Token received: YES (value not printed)");
navigation.NavigateTo(NavigationPage.Home);
Console.WriteLine($"Navigation: {navigation.CurrentPage}");

using ILobbyClient lobby = services.GetRequiredService<ILobbyClient>();
lobby.PlayerJoined += (_, member) => Console.WriteLine($"Joined: {member.Username}");
lobby.MessageReceived += (_, message) => Console.WriteLine($"Chat: {message.SenderUsername}: {message.Text}");

int exitCode = 0;
try
{
    string lobbyCode = lobby.CreateLobby(true, 2, active.Token);
    Console.WriteLine($"Lobby created: {lobbyCode}");
    navigation.NavigateTo(NavigationPage.Lobby);
    IChatClient chat = services.GetRequiredService<IChatClient>();
    ClientResult<bool> sent = chat.SendMessage(lobbyCode, "Hello from Vertigo smoke test", active.Token);
    Console.WriteLine($"Chat sent: {sent.IsSuccess && sent.Value} ({sent.Failure})");
    bool left = lobby.LeaveLobby(lobbyCode, active.Token);
    Console.WriteLine($"Lobby left: {left}");
    if (!sent.IsSuccess || !sent.Value || !left)
    {
        exitCode = 1;
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Lobby transport/business failure: {exception.GetType().Name}: {exception.Message}");
    exitCode = 1;
}
finally
{
    ClientResult<bool> logout = authentication.Logout();
    bool isLoggedOut = logout.IsSuccess && logout.Value && !session.IsAuthenticated;
    Console.WriteLine($"Logout: {isLoggedOut} ({logout.Failure})");
    if (!isLoggedOut)
    {
        exitCode = 1;
    }
}

return exitCode;
