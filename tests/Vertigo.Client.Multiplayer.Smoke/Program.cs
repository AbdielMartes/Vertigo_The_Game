using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

using ServiceProvider first = CreateClient();
using ServiceProvider second = CreateClient();
using ILobbyClient hostLobby = first.GetRequiredService<ILobbyClient>();
using ILobbyClient guestLobby = second.GetRequiredService<ILobbyClient>();
using ManualResetEventSlim joinedEvent = new(false);
using ManualResetEventSlim chatEvent = new(false);
using ManualResetEventSlim replyEvent = new(false);
using ManualResetEventSlim leftEvent = new(false);

string? code = null;
string? hostToken = null;
string? guestToken = null;
Guid guestId = Guid.Empty;

int exitCode = 0;
try
{
    hostToken = CreateGuestToken(first);
    guestToken = CreateGuestToken(second);
    guestId = second.GetRequiredService<Vertigo.Client.Core.Sessions.PlayerSession>()
        .GetActiveSession()!.PlayerId;

    hostLobby.PlayerJoined += (_, player) =>
    {
        if (player.PlayerId == guestId)
        {
            joinedEvent.Set();
        }
    };
    hostLobby.PlayerLeft += (_, playerId) =>
    {
        if (playerId == guestId)
        {
            leftEvent.Set();
        }
    };
    hostLobby.MessageReceived += (_, message) =>
    {
        if (message.SenderId == guestId && message.Text == "Vertigo multiplayer smoke")
        {
            chatEvent.Set();
        }
    };

    Guid hostId = first.GetRequiredService<Vertigo.Client.Core.Sessions.PlayerSession>()
        .GetActiveSession()!.PlayerId;
    guestLobby.MessageReceived += (_, message) =>
    {
        if (message.SenderId == hostId && message.Text == "Vertigo multiplayer reply")
        {
            replyEvent.Set();
        }
    };

    code = hostLobby.CreateLobby(true, 2, hostToken);
    LobbyAdmission admission = guestLobby.JoinLobby(code, guestToken);
    Require(admission.LobbyCode == code, "Joined lobby code mismatch");
    Require(joinedEvent.Wait(TimeSpan.FromSeconds(10)), "Host did not receive PlayerJoined");
    Console.WriteLine("Two clients: JOIN OK");

    IChatClient hostChat = first.GetRequiredService<IChatClient>();
    IChatClient guestChat = second.GetRequiredService<IChatClient>();
    ClientResult<bool>[] sent = await Task.WhenAll(
        Task.Run(() => guestChat.SendMessage(code, "Vertigo multiplayer smoke", guestToken)),
        Task.Run(() => hostChat.SendMessage(code, "Vertigo multiplayer reply", hostToken)));
    Require(sent.All(result => result.IsSuccess && result.Value), "A concurrent chat request failed");
    Require(chatEvent.Wait(TimeSpan.FromSeconds(10)), "Host did not receive chat callback");
    Require(replyEvent.Wait(TimeSpan.FromSeconds(10)), "Guest did not receive chat callback");
    Console.WriteLine("Two clients: CONCURRENT CHAT CALLBACKS OK");

    Require(guestLobby.LeaveLobby(code, guestToken), "Guest could not leave");
    Require(leftEvent.Wait(TimeSpan.FromSeconds(10)), "Host did not receive PlayerLeft");
    Console.WriteLine("Two clients: LEAVE CALLBACK OK");

    Require(hostLobby.LeaveLobby(code, hostToken), "Host could not leave");
    Console.WriteLine("PASS: Multiplayer smoke");
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {exception.Message}");
    exitCode = 1;
}
finally
{
    guestLobby.Disconnect();
    hostLobby.Disconnect();
    LogoutQuietly(first);
    LogoutQuietly(second);
}

return exitCode;

static ServiceProvider CreateClient()
{
    ServiceCollection registrations = new();
    registrations.AddVertigoClientNetworking();
    return registrations.BuildServiceProvider();
}

static string CreateGuestToken(ServiceProvider provider)
{
    var result = provider.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Could not create guest session");
    }
    return result.Value.Token;
}

static void LogoutQuietly(ServiceProvider provider)
{
    try
    {
        provider.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (Exception)
    {
        // Best-effort cleanup after a failed smoke test.
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
