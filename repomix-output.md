This file is a merged representation of the entire codebase, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of the entire repository's contents.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
src/
  Client/
    Vertigo.Client.Core/
      Navigation/
        INavigator.cs
        NavigationPage.cs
        Navigator.cs
      Sessions/
        PlayerSession.cs
        SessionSnapshot.cs
      Vertigo.Client.Core.csproj
    Vertigo.Client.Networking/
      Callbacks/
        FriendshipCallbackHandler.cs
        LobbyCallbackHandler.cs
        PendingLobbyEvent.cs
      Interfaces/
        IAccountClient.cs
        IAuthClient.cs
        IBlockedPlayerClient.cs
        IChatClient.cs
        IFriendshipClient.cs
        IFriendshipNotificationClient.cs
        ILobbyClient.cs
        ILobbyInvitationClient.cs
        IServiceCaller.cs
      Proxies/
        AccountClient.cs
        AuthClient.cs
        BlockedPlayerClient.cs
        ChatClient.cs
        FriendshipClient.cs
        FriendshipNotificationClient.cs
        LobbyClient.cs
        LobbyInvitationClient.cs
        ServiceCaller.cs
      Results/
        ClientResult.cs
        FailureKind.cs
      Sessions/
        Interfaces/
          ISessionCoordinator.cs
        SessionCoordinator.cs
      DependencyInjection.cs
      Vertigo.Client.Networking.csproj
    Vertigo.Client.UI/
      Assets/
        avalonia-logo.ico
      Resources/
        Localization/
          Strings.en.resx
          Strings.resx
      Utilities/
        Localization/
          Interfaces/
            IClientErrorText.cs
            ITextLocalizer.cs
          Services/
            ClientErrorText.cs
            TextLocalizer.cs
        Sessions/
          Interfaces/
            ISessionGuard.cs
          Services/
            SessionGuard.cs
      ViewModels/
        Dependencies/
          AccessContext.cs
          HomeContext.cs
          LobbyContext.cs
          MainContext.cs
          UiNavigationContext.cs
          UiServices.cs
        Lifecycle/
          INavigationAware.cs
        AccessViewModel.cs
        HomeViewModel.cs
        LobbyViewModel.cs
        MainViewModel.cs
        ViewModelBase.cs
      Views/
        MainWindow.axaml
        MainWindow.axaml.cs
      App.axaml
      App.axaml.cs
      app.manifest
      Program.cs
      Vertigo.Client.UI.csproj
      ViewLocator.cs
  Server/
    Vertigo.Server.CrossCutting/
      Security/
        Implementations/
          PasswordHasher.cs
          TokenGenerator.cs
        Interfaces/
          IPasswordHasher.cs
          ITokenGenerator.cs
      Vertigo.Server.CrossCutting.csproj
    Vertigo.Server.DataAccess/
      Contexts/
        VertigoDatabaseContext.cs
      Entities/
        Enums/
          FriendshipStatus.cs
        Game/
          MatchHistory.cs
        Identity/
          BlockedPlayer.cs
          Friendship.cs
          Player.cs
          PlayerStats.cs
          SessionToken.cs
      Repositories/
        Implementations/
          BlockedPlayerRepository.cs
          FriendshipRepository.cs
          PlayerRepository.cs
          SessionTokenRepository.cs
        Interfaces/
          IBlockedPlayerRepository.cs
          IFriendshipRepository.cs
          IPlayerRepository.cs
          ISessionTokenRepository.cs
      DependencyInjection.cs
      Vertigo.Server.DataAccess.csproj
    Vertigo.Server.Game/
      Vertigo.Server.Game.csproj
    Vertigo.Server.Host/
      Program.cs
      Vertigo.Server.Host.csproj
    Vertigo.Server.Matchmaking/
      Chat/
        Services/
          ChatService.cs
      Invitations/
        Dependencies/
          LobbyInvitationServiceDependencies.cs
        Services/
          LobbyInvitationService.cs
      Lobbies/
        Services/
          LobbyService.cs
        LobbyConnection.cs
        LobbySettings.cs
        LobbyState.cs
        LobbyStatus.cs
      Vertigo.Server.Matchmaking.csproj
    Vertigo.Server.PlayerManagement/
      Authentication/
        Dependencies/
          AuthenticationDependencies.cs
        Interfaces/
          IGuestSessionRegistry.cs
          ISessionIdentityResolver.cs
        Models/
          AuthenticatedSession.cs
          SessionIdentity.cs
        Services/
          AuthenticationService.cs
          AuthService.cs
          GuestSessionRegistry.cs
          SessionIdentityResolver.cs
      Blocking/
        Services/
          BlockedPlayerService.cs
      Friendships/
        Dependencies/
          FriendshipServiceDependencies.cs
        Models/
          FriendshipSubscription.cs
        Services/
          FriendshipNotificationService.cs
          FriendshipNotifier.cs
          FriendshipService.cs
      Registration/
        Dependencies/
          AccountServiceDependencies.cs
        Services/
          AccountService.cs
      Vertigo.Server.PlayerManagement.csproj
  Shared/
    Vertigo.Contracts/
      Callbacks/
        IFriendshipCallback.cs
        ILobbyCallback.cs
      Endpoints/
        ServiceEndpoints.cs
      Faults/
        ServiceErrorCodes.cs
        ServiceMessageKeys.cs
      Interfaces/
        IAccountService.cs
        IAuthService.cs
        IBlockedPlayerService.cs
        IChatService.cs
        IFriendshipNotificationService.cs
        IFriendshipService.cs
        ILobbyInvitationService.cs
        ILobbyService.cs
      Internal/
        IFriendshipNotifier.cs
        ILobbyDirectory.cs
        LobbyOverview.cs
      Models/
        AccountRegistration.cs
        BlockedPlayerSummary.cs
        FriendEntry.cs
        FriendshipDecision.cs
        FriendshipInvitation.cs
        FriendshipInvitationEntry.cs
        InvitableFriendsQuery.cs
        LobbyAdmission.cs
        LobbyChatMessage.cs
        LobbyInvitationDecline.cs
        LobbyInvitationEntry.cs
        LobbyInvitations.cs
        LobbyMember.cs
        LobbySummary.cs
        LoginCredentials.cs
        PasswordChange.cs
        PlayerPair.cs
        PlayerRelation.cs
        PlayerSearch.cs
        PlayerSearchEntry.cs
        ServiceFault.cs
        SessionLogout.cs
        UserSession.cs
      Vertigo.Contracts.csproj
tests/
  Vertigo.Client.Networking.Smoke/
    Program.cs
    Vertigo.Client.Networking.Smoke.csproj
  Vertigo.Server.Game.Tests/
    UnitTest1.cs
    Vertigo.Server.Game.Tests.csproj
.gitignore
Vertigo.Client.slnx
Vertigo.Server.slnx
```

# Files

## File: src/Client/Vertigo.Client.Core/Navigation/INavigator.cs
```csharp
namespace Vertigo.Client.Core.Navigation;

public interface INavigator
{
    NavigationPage CurrentPage { get; }
    event EventHandler<NavigationPage>? PageChanged;
    void NavigateTo(NavigationPage page);
}
```

## File: src/Client/Vertigo.Client.Core/Navigation/NavigationPage.cs
```csharp
namespace Vertigo.Client.Core.Navigation;

public enum NavigationPage
{
    Access,
    Home,
    Lobby
}
```

## File: src/Client/Vertigo.Client.Core/Navigation/Navigator.cs
```csharp
namespace Vertigo.Client.Core.Navigation;

public sealed class Navigator : INavigator
{
    private readonly object _syncRoot = new();
    private NavigationPage _currentPage = NavigationPage.Access;

    public event EventHandler<NavigationPage>? PageChanged;

    public NavigationPage CurrentPage
    {
        get
        {
            lock (_syncRoot)
            {
                return _currentPage;
            }
        }
    }

    public void NavigateTo(NavigationPage page)
    {
        bool changed;
        lock (_syncRoot)
        {
            changed = _currentPage != page;
            _currentPage = page;
        }

        if (changed)
        {
            PageChanged?.Invoke(this, page);
        }
    }
}
```

## File: src/Client/Vertigo.Client.Core/Sessions/PlayerSession.cs
```csharp
namespace Vertigo.Client.Core.Sessions;

public sealed class PlayerSession
{
    private readonly object _syncRoot = new();
    private readonly TimeProvider _timeProvider;
    private SessionSnapshot? _session;

    public PlayerSession(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public bool IsAuthenticated => GetActiveSession() is not null;

    public SessionSnapshot? GetActiveSession()
    {
        lock (_syncRoot)
        {
            if (_session is not null && _session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
            {
                _session = null;
            }

            return _session;
        }
    }

    public void SetSession(SessionSnapshot session)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (session.PlayerId == Guid.Empty || string.IsNullOrWhiteSpace(session.Username)
            || string.IsNullOrWhiteSpace(session.Token) || session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            throw new ArgumentException("Session data is incomplete or expired.", nameof(session));
        }

        lock (_syncRoot)
        {
            _session = session;
        }
    }

    public void Clear(SessionSnapshot session)
    {
        lock (_syncRoot)
        {
            if (ReferenceEquals(_session, session))
            {
                _session = null;
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Core/Sessions/SessionSnapshot.cs
```csharp
namespace Vertigo.Client.Core.Sessions;

public sealed record SessionSnapshot(
    Guid PlayerId,
    string Username,
    string Token,
    DateTime ExpiresAt,
    bool IsGuest)
{
    public override string ToString()
    {
        return nameof(SessionSnapshot);
    }
}
```

## File: src/Client/Vertigo.Client.Core/Vertigo.Client.Core.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

## File: src/Client/Vertigo.Client.Networking/Callbacks/FriendshipCallbackHandler.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
public sealed class FriendshipCallbackHandler : IFriendshipCallback
{
    public event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived;

    public event EventHandler<FriendEntry>? FriendshipEstablished;

    public event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived;

    public event EventHandler<string>? LobbyInvitationWithdrawn;

    void IFriendshipCallback.OnFriendRequestReceived(FriendshipInvitationEntry request)
    {
        FriendRequestReceived?.Invoke(this, request);
    }

    void IFriendshipCallback.OnFriendshipEstablished(FriendEntry friend)
    {
        FriendshipEstablished?.Invoke(this, friend);
    }

    void IFriendshipCallback.OnLobbyInvitationReceived(LobbyInvitationEntry invitation)
    {
        LobbyInvitationReceived?.Invoke(this, invitation);
    }

    void IFriendshipCallback.OnLobbyInvitationWithdrawn(string lobbyCode)
    {
        LobbyInvitationWithdrawn?.Invoke(this, lobbyCode);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Callbacks/LobbyCallbackHandler.cs
```csharp
#nullable enable

using System;
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
public sealed class LobbyCallbackHandler : ILobbyCallback
{
    public event EventHandler<LobbyMember>? PlayerJoined;

    public event EventHandler<Guid>? PlayerLeft;

    public event EventHandler<LobbyChatMessage>? MessageReceived;

    void ILobbyCallback.PlayerJoined(LobbyMember player)
    {
        PlayerJoined?.Invoke(this, player);
    }

    void ILobbyCallback.PlayerLeft(Guid playerId)
    {
        PlayerLeft?.Invoke(this, playerId);
    }

    void ILobbyCallback.MessageReceived(LobbyChatMessage message)
    {
        MessageReceived?.Invoke(this, message);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Callbacks/PendingLobbyEvent.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

public sealed record PendingLobbyEvent(LobbyMember? Joined, Guid LeftPlayerId, LobbyChatMessage? Chat);
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IAccountClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IAccountClient
{
    ClientResult<bool> RegisterAccount(AccountRegistration registration);

    ClientResult<bool> ChangePassword(PasswordChange change, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IAuthClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IAuthClient
{
    ClientResult<UserSession> Login(LoginCredentials request);

    ClientResult<UserSession> ResumeSession(string sessionToken);

    ClientResult<UserSession> CreateGuestSession();

    ClientResult<bool> Logout(string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IBlockedPlayerClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IBlockedPlayerClient
{
    ClientResult<List<BlockedPlayerSummary>> GetBlockedPlayers(string sessionToken);

    ClientResult<bool> BlockPlayer(Guid otherPlayerId, string sessionToken);

    ClientResult<bool> UnblockPlayer(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IChatClient.cs
```csharp
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.Networking.Interfaces;

public interface IChatClient
{
    ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IFriendshipClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IFriendshipClient
{
    ClientResult<List<FriendEntry>> GetFriends(string sessionToken);

    ClientResult<List<PlayerSearchEntry>> SearchPlayers(string searchTerm, string sessionToken);

    ClientResult<PlayerRelation> SendFriendRequest(Guid targetPlayerId, string sessionToken);

    ClientResult<List<FriendshipInvitationEntry>> GetPendingFriendRequests(string sessionToken);

    ClientResult<bool> RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken);

    ClientResult<bool> RemoveFriendship(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IFriendshipNotificationClient.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IFriendshipNotificationClient : IDisposable
{
    event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived;

    event EventHandler<FriendEntry>? FriendshipEstablished;

    event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived;

    event EventHandler<string>? LobbyInvitationWithdrawn;

    void Subscribe(string sessionToken);

    void Unsubscribe();
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/ILobbyClient.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyClient : IDisposable
{
    event EventHandler<LobbyMember>? PlayerJoined;

    event EventHandler<Guid>? PlayerLeft;

    event EventHandler<LobbyChatMessage>? MessageReceived;

    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    bool LeaveLobby(string lobbyCode, string sessionToken);

    List<LobbySummary> GetPublicLobbies();

    void ReplayPendingEvents();
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/ILobbyInvitationClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyInvitationClient
{
    ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken);

    ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken);

    ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken);

    ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IServiceCaller.cs
```csharp
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.Networking.Interfaces;

public interface IServiceCaller
{
    ClientResult<TResult> Call<TChannel, TArgument, TResult>(
        string address,
        Func<TChannel, TArgument, TResult> operation,
        TArgument argument)
        where TChannel : class;
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/AccountClient.cs
```csharp
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

    private static bool RegisterAccountOperation(IAccountService channel, AccountRegistration request)
    {
        return channel.RegisterAccount(request);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/AuthClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class AuthClient : IAuthClient
{
    private readonly IServiceCaller _serviceCaller;

    public AuthClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<UserSession> Login(LoginCredentials request)
    {
        return _serviceCaller.Call<IAuthService, LoginCredentials, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            LoginOperation,
            request);
    }

    public ClientResult<UserSession> ResumeSession(string sessionToken)
    {
        return _serviceCaller.Call<IAuthService, string, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            ResumeSessionOperation,
            sessionToken);
    }

    public ClientResult<UserSession> CreateGuestSession()
    {
        return _serviceCaller.Call<IAuthService, object?, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            CreateGuestSessionOperation,
            null);
    }

    public ClientResult<bool> Logout(string sessionToken)
    {
        SessionLogout request = new()
        {
            SessionToken = sessionToken
        };

        return _serviceCaller.Call<IAuthService, SessionLogout, bool>(
            ServiceEndpoints.AuthServiceAddress,
            LogoutOperation,
            request);
    }

    private static UserSession LoginOperation(IAuthService channel, LoginCredentials request)
    {
        return channel.Login(request);
    }

    private static UserSession ResumeSessionOperation(IAuthService channel, string sessionToken)
    {
        return channel.ResumeSession(sessionToken);
    }

    private static UserSession CreateGuestSessionOperation(IAuthService channel, object? unused)
    {
        return channel.CreateGuestSession();
    }

    private static bool LogoutOperation(IAuthService channel, SessionLogout request)
    {
        return channel.Logout(request);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/BlockedPlayerClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class BlockedPlayerClient : IBlockedPlayerClient
{
    private readonly IServiceCaller _serviceCaller;

    public BlockedPlayerClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<BlockedPlayerSummary>> GetBlockedPlayers(string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, string, List<BlockedPlayerSummary>>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, token) => channel.GetBlockedPlayers(token),
            sessionToken);
    }

    public ClientResult<bool> BlockPlayer(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, (Guid, string), bool>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, args) => channel.BlockPlayer(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }

    public ClientResult<bool> UnblockPlayer(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, (Guid, string), bool>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, args) => channel.UnblockPlayer(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/ChatClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class ChatClient : IChatClient
{
    private readonly IServiceCaller _serviceCaller;

    public ChatClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken)
    {
        return _serviceCaller.Call<IChatService, (string, string, string), bool>(
            ServiceEndpoints.ChatServiceAddress,
            (channel, args) => channel.SendMessage(args.Item1, args.Item2, args.Item3),
            (lobbyCode, text, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/FriendshipClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class FriendshipClient : IFriendshipClient
{
    private readonly IServiceCaller _serviceCaller;

    public FriendshipClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<FriendEntry>> GetFriends(string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, string, List<FriendEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, token) => channel.GetFriends(token),
            sessionToken);
    }

    public ClientResult<List<PlayerSearchEntry>> SearchPlayers(string searchTerm, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (string, string), List<PlayerSearchEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.SearchPlayers(args.Item1, args.Item2),
            (searchTerm, sessionToken));
    }

    public ClientResult<PlayerRelation> SendFriendRequest(Guid targetPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, string), PlayerRelation>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.SendFriendRequest(args.Item1, args.Item2),
            (targetPlayerId, sessionToken));
    }

    public ClientResult<List<FriendshipInvitationEntry>> GetPendingFriendRequests(string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, string, List<FriendshipInvitationEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, token) => channel.GetPendingFriendRequests(token),
            sessionToken);
    }

    public ClientResult<bool> RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, bool, string), bool>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.RespondToFriendship(args.Item1, args.Item2, args.Item3),
            (senderPlayerId, accept, sessionToken));
    }

    public ClientResult<bool> RemoveFriendship(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, string), bool>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.RemoveFriendship(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/FriendshipNotificationClient.cs
```csharp
using System.ServiceModel;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class FriendshipNotificationClient : IFriendshipNotificationClient
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    private readonly FriendshipCallbackHandler _callbackHandler;
    private readonly DuplexChannelFactory<IFriendshipNotificationService> _channelFactory;
    private readonly object _syncRoot = new();
    private IFriendshipNotificationService? _channel;
    private bool _isDisposed;

    public FriendshipNotificationClient()
    {
        _callbackHandler = new FriendshipCallbackHandler();

        NetTcpBinding binding = new(SecurityMode.None)
        {
            OpenTimeout = OperationTimeout,
            SendTimeout = OperationTimeout
        };
        InstanceContext callbackContext = new(_callbackHandler);
        _channelFactory = new DuplexChannelFactory<IFriendshipNotificationService>(
            callbackContext,
            binding,
            new EndpointAddress(ServiceEndpoints.FriendshipNotificationServiceAddress));
    }

    public event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived
    {
        add => _callbackHandler.FriendRequestReceived += value;
        remove => _callbackHandler.FriendRequestReceived -= value;
    }

    public event EventHandler<FriendEntry>? FriendshipEstablished
    {
        add => _callbackHandler.FriendshipEstablished += value;
        remove => _callbackHandler.FriendshipEstablished -= value;
    }

    public event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived
    {
        add => _callbackHandler.LobbyInvitationReceived += value;
        remove => _callbackHandler.LobbyInvitationReceived -= value;
    }

    public event EventHandler<string>? LobbyInvitationWithdrawn
    {
        add => _callbackHandler.LobbyInvitationWithdrawn += value;
        remove => _callbackHandler.LobbyInvitationWithdrawn -= value;
    }

    public void Subscribe(string sessionToken)
    {
        IFriendshipNotificationService channel = GetOrCreateChannel();

        try
        {
            channel.Subscribe(sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public void Unsubscribe()
    {
        IFriendshipNotificationService channel = GetOrCreateChannel();

        try
        {
            channel.Unsubscribe();
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
        }
    }

    public void Dispose()
    {
        IFriendshipNotificationService? channel;
        bool wasDisposed;

        lock (_syncRoot)
        {
            wasDisposed = _isDisposed;
            channel = _channel;
            _channel = null;
            _isDisposed = true;
        }

        if (!wasDisposed)
        {
            if (channel is not null)
            {
                CloseSafely((ICommunicationObject)channel);
            }

            CloseSafely(_channelFactory);
        }
    }

    private static bool IsTransportFailure(Exception exception)
    {
        bool isCommunicationFailure = exception is CommunicationException and not FaultException;
        bool isTimeout = exception is TimeoutException;

        return isCommunicationFailure || isTimeout;
    }

    private static bool IsUnusable(ICommunicationObject communicationObject)
    {
        bool isUnusable = communicationObject.State is CommunicationState.Faulted
            or CommunicationState.Closing
            or CommunicationState.Closed;

        return isUnusable;
    }

    private static void CloseSafely(ICommunicationObject communicationObject)
    {
        try
        {
            communicationObject.Close();
        }
        catch (CommunicationException)
        {
            communicationObject.Abort();
        }
        catch (TimeoutException)
        {
            communicationObject.Abort();
        }
    }

    private IFriendshipNotificationService GetOrCreateChannel()
    {
        IFriendshipNotificationService channel;

        lock (_syncRoot)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_channel is null || IsUnusable((ICommunicationObject)_channel))
            {
                if (_channel is not null)
                {
                    ((ICommunicationObject)_channel).Abort();
                }

                _channel = _channelFactory.CreateChannel();
            }

            channel = _channel;
        }

        return channel;
    }

    private void AbortChannel(IFriendshipNotificationService channel)
    {
        ((ICommunicationObject)channel).Abort();

        lock (_syncRoot)
        {
            if (ReferenceEquals(_channel, channel))
            {
                _channel = null;
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/LobbyClient.cs
```csharp
using System.ServiceModel;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class LobbyClient : ILobbyClient
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    private readonly LobbyCallbackHandler _callbackHandler;
    private readonly DuplexChannelFactory<ILobbyService> _channelFactory;
    private readonly object _syncRoot = new();
    private readonly object _eventLock = new();
    private readonly Queue<PendingLobbyEvent> _pendingEvents = new();
    private EventHandler<LobbyMember>? _playerJoined;
    private EventHandler<Guid>? _playerLeft;
    private EventHandler<LobbyChatMessage>? _messageReceived;
    private ILobbyService? _channel;
    private bool _isDisposed;

    public LobbyClient()
    {
        _callbackHandler = new LobbyCallbackHandler();
        _callbackHandler.PlayerJoined += OnPlayerJoined;
        _callbackHandler.PlayerLeft += OnPlayerLeft;
        _callbackHandler.MessageReceived += OnChatReceived;

        NetTcpBinding binding = new(SecurityMode.None)
        {
            OpenTimeout = OperationTimeout,
            SendTimeout = OperationTimeout
        };
        InstanceContext callbackContext = new(_callbackHandler);
        _channelFactory = new DuplexChannelFactory<ILobbyService>(
            callbackContext,
            binding,
            new EndpointAddress(ServiceEndpoints.LobbyServiceAddress));
    }

    public event EventHandler<LobbyMember>? PlayerJoined
    {
        add
        {
            lock (_eventLock)
            {
                _playerJoined += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _playerJoined -= value;
            }
        }
    }

    public event EventHandler<Guid>? PlayerLeft
    {
        add
        {
            lock (_eventLock)
            {
                _playerLeft += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _playerLeft -= value;
            }
        }
    }
    
    public event EventHandler<LobbyChatMessage>? MessageReceived
    {
        add
        {
            lock (_eventLock)
            {
                _messageReceived += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _messageReceived -= value;
            }
        }
    }

    public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
    {
        ClearPendingEvents();
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.CreateLobby(isPublic, maxPlayers, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
    {
        ClearPendingEvents();
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.JoinLobby(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public bool LeaveLobby(string lobbyCode, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.LeaveLobby(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public List<LobbySummary> GetPublicLobbies()
    {
        ILobbyService channel = GetOrCreateChannel();
        List<LobbySummary> lobbies;

        try
        {
            lobbies = channel.GetPublicLobbies();
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }

        return lobbies;
    }

    public void ReplayPendingEvents()
    {
        lock (_eventLock)
        {
            while (_pendingEvents.Count > 0)
            {
                PendingLobbyEvent pending = _pendingEvents.Dequeue();

                if (pending.Joined is not null)
                {
                    _playerJoined?.Invoke(this, pending.Joined);
                }
                else if (pending.Chat is not null)
                {
                    _messageReceived?.Invoke(this, pending.Chat);
                }
                else
                {
                    _playerLeft?.Invoke(this, pending.LeftPlayerId);
                }
            }
        }
    }

    public void Dispose()
    {
        ILobbyService? channel;
        bool wasDisposed;

        lock (_syncRoot)
        {
            wasDisposed = _isDisposed;
            channel = _channel;
            _channel = null;
            _isDisposed = true;
        }

        if (!wasDisposed)
        {
            _callbackHandler.PlayerJoined -= OnPlayerJoined;
            _callbackHandler.PlayerLeft -= OnPlayerLeft;
            _callbackHandler.MessageReceived -= OnChatReceived;
            ClearPendingEvents();

            if (channel is not null)
            {
                CloseSafely((ICommunicationObject)channel);
            }

            CloseSafely(_channelFactory);
        }
    }

    private static bool IsTransportFailure(Exception exception)
    {
        bool isCommunicationFailure = exception is CommunicationException and not FaultException;
        bool isTimeout = exception is TimeoutException;

        return isCommunicationFailure || isTimeout;
    }

    private ILobbyService GetOrCreateChannel()
    {
        ILobbyService channel;

        lock (_syncRoot)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_channel is null || IsUnusable((ICommunicationObject)_channel))
            {
                if (_channel is not null)
                {
                    ((ICommunicationObject)_channel).Abort();
                }

                _channel = _channelFactory.CreateChannel();
            }

            channel = _channel;
        }

        return channel;
    }

    private void AbortChannel(ILobbyService channel)
    {
        ((ICommunicationObject)channel).Abort();

        lock (_syncRoot)
        {
            if (ReferenceEquals(_channel, channel))
            {
                _channel = null;
            }
        }
    }

    private static bool IsUnusable(ICommunicationObject communicationObject)
    {
        bool isUnusable = communicationObject.State is CommunicationState.Faulted
            or CommunicationState.Closing
            or CommunicationState.Closed;

        return isUnusable;
    }

    private static void CloseSafely(ICommunicationObject communicationObject)
    {
        try
        {
            communicationObject.Close();
        }
        catch (CommunicationException)
        {
            communicationObject.Abort();
        }
        catch (TimeoutException)
        {
            communicationObject.Abort();
        }
    }

    private void ClearPendingEvents()
    {
        lock (_eventLock)
        {
            _pendingEvents.Clear();
        }
    }

    // Events are raised inside the lock so replayed and live events keep the server's order;
    // subscribers must only marshal to the UI thread (Dispatcher.Post) and return.
    private void OnPlayerJoined(object? sender, LobbyMember player)
    {
        lock (_eventLock)
        {
            if (_playerJoined is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(player, Guid.Empty, null));
            }
            else
            {
                _playerJoined.Invoke(this, player);
            }
        }
    }

    private void OnPlayerLeft(object? sender, Guid playerId)
    {
        lock (_eventLock)
        {
            if (_playerLeft is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, playerId, null));
            }
            else
            {
                _playerLeft.Invoke(this, playerId);
            }
        }
    }
    private void OnChatReceived(object? sender, LobbyChatMessage message)
    {
        lock (_eventLock)
        {
            if (_messageReceived is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, Guid.Empty, message));
            }
            else
            {
                _messageReceived.Invoke(this, message);
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/LobbyInvitationClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class LobbyInvitationClient : ILobbyInvitationClient
{
    private readonly IServiceCaller _serviceCaller;

    public LobbyInvitationClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, string), List<FriendEntry>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.GetInvitableFriends(args.Item1, args.Item2),
            (lobbyCode, sessionToken));
    }

    public ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, List<Guid>, string), List<Guid>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.SendLobbyInvitations(args.Item1, args.Item2, args.Item3),
            (lobbyCode, targetIds, sessionToken));
    }

    public ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, string, List<LobbyInvitationEntry>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, token) => channel.GetPendingLobbyInvitations(token),
            sessionToken);
    }

    public ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, string), bool>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.DeclineLobbyInvitation(args.Item1, args.Item2),
            (lobbyCode, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/ServiceCaller.cs
```csharp
using System.Diagnostics;
using System.ServiceModel;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class ServiceCaller : IServiceCaller
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    public ClientResult<TResult> Call<TChannel, TArgument, TResult>(
        string address,
        Func<TChannel, TArgument, TResult> operation,
        TArgument argument)
        where TChannel : class
    {
        ChannelFactory<TChannel>? channelFactory = null;
        TChannel? channel = null;
        ClientResult<TResult> result;

        try
        {
            channelFactory = new ChannelFactory<TChannel>(CreateBinding(), new EndpointAddress(address));
            channel = channelFactory.CreateChannel();
            TResult value = operation(channel, argument);
            result = ClientResult<TResult>.Success(value);
        }
        catch (FaultException<ServiceFault> exception)
        {
            result = ClientResult<TResult>.BusinessFailure(exception.Detail.ErrorCode, exception.Detail.MessageKey);
        }
        catch (FaultException exception)
        {
            Debug.WriteLine($"ServiceCaller: server fault at {address}: {exception}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Unexpected);
        }
        catch (TimeoutException exception)
        {
            Debug.WriteLine($"ServiceCaller: timeout at {address}: {exception.Message}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Timeout);
        }
        catch (CommunicationException exception)
        {
            Debug.WriteLine($"ServiceCaller: unreachable {address}: {exception.Message}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.ServerUnreachable);
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"ServiceCaller: unexpected failure at {address}: {exception}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Unexpected);
        }
        finally
        {
            CloseCommunicationObject(channel as ICommunicationObject);
            CloseCommunicationObject(channelFactory);
        }

        return result;
    }

    private static NetTcpBinding CreateBinding()
    {
        return new NetTcpBinding(SecurityMode.None)
        {
            OpenTimeout = OperationTimeout,
            SendTimeout = OperationTimeout
        };
    }

    private static void CloseCommunicationObject(ICommunicationObject? communicationObject)
    {
        if (communicationObject is not null)
        {
            try
            {
                if (communicationObject.State == CommunicationState.Faulted)
                {
                    communicationObject.Abort();
                }
                else
                {
                    communicationObject.Close();
                }
            }
            catch (CommunicationException)
            {
                communicationObject.Abort();
            }
            catch (TimeoutException)
            {
                communicationObject.Abort();
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Results/ClientResult.cs
```csharp
using System.Diagnostics;

namespace Vertigo.Client.Networking.Results;

[DebuggerDisplay("{ToString(),nq}")]
public sealed class ClientResult<TValue>
{
    private ClientResult()
    {
    }

    public bool IsSuccess { get; private set; }

    public TValue? Value { get; private set; }

    public FailureKind Failure { get; private set; }
    
    public string ErrorCode { get; private set; } = string.Empty;

    public string MessageKey { get; private set; } = string.Empty;

    public bool IsConnectionFailure => Failure is FailureKind.ServerUnreachable or FailureKind.Timeout;

    public static ClientResult<TValue> Success(TValue value)
    {
        return new ClientResult<TValue> { IsSuccess = true, Value = value, Failure = FailureKind.None };
    }

    public static ClientResult<TValue> BusinessFailure(string errorCode, string messageKey)
    {
        return new ClientResult<TValue>
        {
            Failure = FailureKind.Business,
            ErrorCode = errorCode,
            MessageKey = messageKey
        };
    }

    public static ClientResult<TValue> TechnicalFailure(FailureKind failure)
    {
        return new ClientResult<TValue> { Failure = failure };
    }

    public override string ToString()
    {
        string text = IsSuccess ? "Success" : $"Failure {Failure}";

        if (Failure == FailureKind.Business)
        {
            text += $" {ErrorCode}";
        }

        return text;
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Results/FailureKind.cs
```csharp
namespace Vertigo.Client.Networking.Results;

public enum FailureKind
{
    None,
    Business,
    ServerUnreachable,
    Timeout,
    Unexpected
}
```

## File: src/Client/Vertigo.Client.Networking/Sessions/Interfaces/ISessionCoordinator.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Sessions.Interfaces;

public interface ISessionCoordinator
{
    ClientResult<UserSession> Login(string username, string password);
    ClientResult<UserSession> ResumeSession(string token);
    ClientResult<UserSession> CreateGuestSession();
    ClientResult<bool> Logout();
}
```

## File: src/Client/Vertigo.Client.Networking/Sessions/SessionCoordinator.cs
```csharp
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Sessions;

public sealed class SessionCoordinator : ISessionCoordinator
{
    private readonly IAuthClient _authClient;
    private readonly PlayerSession _playerSession;

    public SessionCoordinator(IAuthClient authClient, PlayerSession playerSession)
    {
        _authClient = authClient;
        _playerSession = playerSession;
    }

    public ClientResult<UserSession> Login(string username, string password)
    {
        ClientResult<UserSession> result = _authClient.Login(new LoginCredentials
        {
            Username = username,
            Password = password
        });
        ApplySession(result);
        return result;
    }

    public ClientResult<UserSession> ResumeSession(string token)
    {
        ClientResult<UserSession> result = _authClient.ResumeSession(token);
        ApplySession(result);
        return result;
    }

    public ClientResult<UserSession> CreateGuestSession()
    {
        ClientResult<UserSession> result = _authClient.CreateGuestSession();
        ApplySession(result);
        return result;
    }

    public ClientResult<bool> Logout()
    {
        SessionSnapshot? session = _playerSession.GetActiveSession();
        ClientResult<bool> result = ClientResult<bool>.Success(true);
        if (session is not null)
        {
            result = _authClient.Logout(session.Token);
            if (result.IsSuccess && result.Value)
            {
                _playerSession.Clear(session);
            }
        }

        return result;
    }

    private void ApplySession(ClientResult<UserSession> result)
    {
        if (result.IsSuccess && result.Value is UserSession session)
        {
            _playerSession.SetSession(new SessionSnapshot(
                session.PlayerId, session.Username, session.Token, session.ExpiresAt, session.IsGuest));
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/DependencyInjection.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Proxies;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.Networking.Sessions;

namespace Vertigo.Client.Networking;

public static class DependencyInjection
{
    public static IServiceCollection AddVertigoClientNetworking(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<PlayerSession>();
        services.AddSingleton<INavigator, Navigator>();
        services.AddSingleton<IServiceCaller, ServiceCaller>();
        services.AddTransient<IAuthClient, AuthClient>();
        services.AddTransient<IAccountClient, AccountClient>();
        services.AddTransient<IBlockedPlayerClient, BlockedPlayerClient>();
        services.AddTransient<IFriendshipClient, FriendshipClient>();
        services.AddTransient<ILobbyInvitationClient, LobbyInvitationClient>();
        services.AddTransient<IChatClient, ChatClient>();
        services.AddSingleton<ISessionCoordinator, SessionCoordinator>();
        services.AddSingleton<ILobbyClient, LobbyClient>();
        services.AddTransient<IFriendshipNotificationClient, FriendshipNotificationClient>();
        return services;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Resources/Localization/Strings.en.resx
```
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Error.Unknown" xml:space="preserve"><value>An error occurred. Please try again.</value></data>
  <data name="Error.Timeout" xml:space="preserve"><value>The server took too long to respond.</value></data>
  <data name="Error.Connection" xml:space="preserve"><value>Could not connect to the server.</value></data>
  <data name="Access.MissingCredentials" xml:space="preserve"><value>Enter a username and password.</value></data>
  <data name="Access.LoginFailed" xml:space="preserve"><value>Sign-in could not be completed.</value></data>
  <data name="Access.GuestFailed" xml:space="preserve"><value>Guest access could not be completed.</value></data>
  <data name="Access.MissingToken" xml:space="preserve"><value>A previous session token is required.</value></data>
  <data name="Access.ResumeFailed" xml:space="preserve"><value>The session could not be restored.</value></data>
  <data name="Home.LogoutFailed" xml:space="preserve"><value>The session could not be closed.</value></data>
  <data name="Lobby.SessionRequired" xml:space="preserve"><value>Your session has expired. Please sign in again.</value></data>
  <data name="Lobby.CodeRequired" xml:space="preserve"><value>Enter a lobby code.</value></data>
  <data name="Lobby.CreateFailed" xml:space="preserve"><value>The lobby could not be created.</value></data>
  <data name="Lobby.JoinFailed" xml:space="preserve"><value>Could not join the lobby.</value></data>
  <data name="Lobby.MessageFailed" xml:space="preserve"><value>The message could not be sent.</value></data>
  <data name="Lobby.LeaveFailed" xml:space="preserve"><value>The lobby could not be left.</value></data>
  <data name="Authentication.Unauthorized" xml:space="preserve"><value>Your session is no longer valid. Sign in again.</value></data>
  <data name="Lobby.ChatNotAllowed" xml:space="preserve"><value>You cannot send messages in this lobby.</value></data>
  <data name="RegisterAccount.UsernameAlreadyRegistered" xml:space="preserve"><value>Username already exists.</value></data>
  <data name="RegisterAccount.EmailAlreadyRegistered" xml:space="preserve"><value>Email already exists.</value></data>
  <data name="RegisterAccount.ValidationSummary" xml:space="preserve"><value>Check the registration details.</value></data>
  <data name="ROOM_NOT_FOUND" xml:space="preserve"><value>Lobby not found.</value></data>
  <data name="ROOM_FULL" xml:space="preserve"><value>The lobby is full.</value></data>
  <data name="MSG-ROOM-009" xml:space="preserve"><value>The match has already started.</value></data>
  <data name="MSG-ROOM-011" xml:space="preserve"><value>Invalid lobby capacity.</value></data>
  <data name="ErrorInvalidData" xml:space="preserve"><value>The supplied data is invalid.</value></data>
  <data name="ErrorUserNotFound" xml:space="preserve"><value>Player not found.</value></data>
  <data name="ErrorInvalidCode" xml:space="preserve"><value>The code is invalid.</value></data>
  <data name="ErrorInvalidCredentials" xml:space="preserve"><value>Incorrect username or password.</value></data>
  <data name="AccessMenu.SessionAlreadyActive" xml:space="preserve"><value>A session is already active.</value></data>
  <data name="Friends.RequestSent" xml:space="preserve"><value>The request has already been sent.</value></data>
  <data name="Friends.AlreadyFriends" xml:space="preserve"><value>You are already friends.</value></data>
  <data name="Friends.RequestBlocked" xml:space="preserve"><value>The request is blocked.</value></data>
  <data name="Notifications.FriendRequestNotFound" xml:space="preserve"><value>Friend request not found.</value></data>
  <data name="InviteFriends.NotAllowed" xml:space="preserve"><value>This invitation is not allowed.</value></data>
</root>
```

## File: src/Client/Vertigo.Client.UI/Resources/Localization/Strings.resx
```
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Error.Unknown" xml:space="preserve"><value>Ocurrió un error. Inténtalo de nuevo.</value></data>
  <data name="Error.Timeout" xml:space="preserve"><value>El servidor tardó demasiado en responder.</value></data>
  <data name="Error.Connection" xml:space="preserve"><value>No se pudo conectar con el servidor.</value></data>
  <data name="Access.MissingCredentials" xml:space="preserve"><value>Ingresa usuario y contraseña.</value></data>
  <data name="Access.LoginFailed" xml:space="preserve"><value>No se pudo iniciar sesión.</value></data>
  <data name="Access.GuestFailed" xml:space="preserve"><value>No se pudo ingresar como invitado.</value></data>
  <data name="Access.MissingToken" xml:space="preserve"><value>Se requiere un token de sesión previo.</value></data>
  <data name="Access.ResumeFailed" xml:space="preserve"><value>No se pudo recuperar la sesión.</value></data>
  <data name="Home.LogoutFailed" xml:space="preserve"><value>No se pudo cerrar sesión.</value></data>
  <data name="Lobby.SessionRequired" xml:space="preserve"><value>Tu sesión caducó. Inicia sesión nuevamente.</value></data>
  <data name="Lobby.CodeRequired" xml:space="preserve"><value>Ingresa el código de sala.</value></data>
  <data name="Lobby.CreateFailed" xml:space="preserve"><value>No se pudo crear la sala.</value></data>
  <data name="Lobby.JoinFailed" xml:space="preserve"><value>No se pudo entrar a la sala.</value></data>
  <data name="Lobby.MessageFailed" xml:space="preserve"><value>No se pudo enviar el mensaje.</value></data>
  <data name="Lobby.LeaveFailed" xml:space="preserve"><value>No se pudo salir de la sala.</value></data>
  <data name="Authentication.Unauthorized" xml:space="preserve"><value>Tu sesión ya no es válida. Inicia sesión nuevamente.</value></data>
  <data name="Lobby.ChatNotAllowed" xml:space="preserve"><value>No puedes enviar mensajes en esta sala.</value></data>
  <data name="RegisterAccount.UsernameAlreadyRegistered" xml:space="preserve"><value>Ese nombre de usuario ya está registrado.</value></data>
  <data name="RegisterAccount.EmailAlreadyRegistered" xml:space="preserve"><value>Ese correo ya está registrado.</value></data>
  <data name="RegisterAccount.ValidationSummary" xml:space="preserve"><value>Revisa los datos de registro.</value></data>
  <data name="ROOM_NOT_FOUND" xml:space="preserve"><value>No se encontró la sala.</value></data>
  <data name="ROOM_FULL" xml:space="preserve"><value>La sala está llena.</value></data>
  <data name="MSG-ROOM-009" xml:space="preserve"><value>La partida de esa sala ya comenzó.</value></data>
  <data name="MSG-ROOM-011" xml:space="preserve"><value>La capacidad de la sala no es válida.</value></data>
  <data name="ErrorInvalidData" xml:space="preserve"><value>Los datos proporcionados no son válidos.</value></data>
  <data name="ErrorUserNotFound" xml:space="preserve"><value>No se encontró el jugador.</value></data>
  <data name="ErrorInvalidCode" xml:space="preserve"><value>El código no es válido.</value></data>
  <data name="ErrorInvalidCredentials" xml:space="preserve"><value>Usuario o contraseña incorrectos.</value></data>
  <data name="AccessMenu.SessionAlreadyActive" xml:space="preserve"><value>Ya existe una sesión activa.</value></data>
  <data name="Friends.RequestSent" xml:space="preserve"><value>La solicitud ya fue enviada.</value></data>
  <data name="Friends.AlreadyFriends" xml:space="preserve"><value>Ya son amigos.</value></data>
  <data name="Friends.RequestBlocked" xml:space="preserve"><value>La solicitud está bloqueada.</value></data>
  <data name="Notifications.FriendRequestNotFound" xml:space="preserve"><value>No se encontró la solicitud de amistad.</value></data>
  <data name="InviteFriends.NotAllowed" xml:space="preserve"><value>No está permitido invitar a este jugador.</value></data>
</root>
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Interfaces/IClientErrorText.cs
```csharp
using System;
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.UI.Utilities.Localization.Interfaces;

public interface IClientErrorText
{
    string FromResult<T>(ClientResult<T> result, string fallbackKey);
    string FromException(Exception exception, string fallbackKey);
    bool IsUnauthorized<T>(ClientResult<T> result);
    bool IsUnauthorized(Exception exception);
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Interfaces/ITextLocalizer.cs
```csharp
namespace Vertigo.Client.UI.Utilities.Localization.Interfaces;

public interface ITextLocalizer
{
    string Get(string key);
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Services/ClientErrorText.cs
```csharp
using System;
using System.ServiceModel;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.Utilities.Localization.Services;

public sealed class ClientErrorText : IClientErrorText
{
    private readonly ITextLocalizer _localizer;

    public ClientErrorText(ITextLocalizer localizer)
    {
        _localizer = localizer;
    }

    public string FromResult<T>(ClientResult<T> result, string fallbackKey)
    {
        if (result.Failure == FailureKind.Business)
        {
            return _localizer.Get(string.IsNullOrWhiteSpace(result.MessageKey) ? fallbackKey : result.MessageKey);
        }

        string key = result.Failure switch
        {
            FailureKind.Timeout => "Error.Timeout",
            FailureKind.ServerUnreachable => "Error.Connection",
            _ => fallbackKey
        };
        return _localizer.Get(key);
    }

    public string FromException(Exception exception, string fallbackKey)
    {
        if (exception is FaultException<ServiceFault> fault)
        {
            return _localizer.Get(string.IsNullOrWhiteSpace(fault.Detail?.MessageKey)
                ? fallbackKey : fault.Detail.MessageKey);
        }

        string key = exception switch
        {
            TimeoutException => "Error.Timeout",
            CommunicationException => "Error.Connection",
            _ => fallbackKey
        };
        return _localizer.Get(key);
    }

    public bool IsUnauthorized<T>(ClientResult<T> result)
    {
        return result.Failure == FailureKind.Business
            && result.ErrorCode == ServiceErrorCodes.Unauthorized;
    }

    public bool IsUnauthorized(Exception exception)
    {
        return exception is FaultException<ServiceFault> fault
            && fault.Detail?.ErrorCode == ServiceErrorCodes.Unauthorized;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Services/TextLocalizer.cs
```csharp
using System.Globalization;
using System.Resources;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.Utilities.Localization.Services;

public sealed class TextLocalizer : ITextLocalizer
{
    private const string ResourceBaseName = "Vertigo.Client.UI.Resources.Localization.Strings";
    private readonly ResourceManager _resources = new(ResourceBaseName, typeof(TextLocalizer).Assembly);

    public string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return _resources.GetString("Error.Unknown", CultureInfo.CurrentUICulture) ?? "Ocurrió un error.";
        }

        return _resources.GetString(key, CultureInfo.CurrentUICulture)
            ?? _resources.GetString("Error.Unknown", CultureInfo.CurrentUICulture)
            ?? "Ocurrió un error.";
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Sessions/Interfaces/ISessionGuard.cs
```csharp
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.Utilities.Sessions.Interfaces;

public interface ISessionGuard
{
    bool HasActiveSession();
    void Invalidate(SessionSnapshot? session = null);
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Sessions/Services/SessionGuard.cs
```csharp
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.Utilities.Sessions.Services;

public sealed class SessionGuard : ISessionGuard
{
    private readonly PlayerSession _session;
    private readonly INavigator _navigation;

    public SessionGuard(PlayerSession session, INavigator navigation)
    {
        _session = session;
        _navigation = navigation;
    }

    public bool HasActiveSession()
    {
        if (_session.GetActiveSession() is not null)
        {
            return true;
        }

        Invalidate();
        return false;
    }

    public void Invalidate(SessionSnapshot? session = null)
    {
        if (session is not null)
        {
            _session.Clear(session);
        }

        if (!_session.IsAuthenticated)
        {
            _navigation.NavigateTo(NavigationPage.Access);
        }
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/AccessContext.cs
```csharp
using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record AccessContext(ISessionCoordinator Sessions, UiNavigationContext Ui);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/HomeContext.cs
```csharp
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record HomeContext(ISessionCoordinator Sessions, PlayerSession PlayerSession,
    UiNavigationContext Ui);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/LobbyContext.cs
```csharp
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record LobbyContext(PlayerSession PlayerSession, UiNavigationContext Ui);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/MainContext.cs
```csharp
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MainContext(HomeViewModel Home, LobbyViewModel Lobby, PlayerSession PlayerSession);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/UiNavigationContext.cs
```csharp
using Vertigo.Client.Core.Navigation;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record UiNavigationContext(INavigator Navigation, UiServices Services);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/UiServices.cs
```csharp
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record UiServices(IClientErrorText Errors, ITextLocalizer Text,
    ISessionGuard SessionGuard);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Lifecycle/INavigationAware.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Lifecycle;

public interface INavigationAware
{
    void OnNavigatedTo();
    void OnNavigatedFrom();
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/AccessViewModel.cs
```csharp
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed class AccessViewModel : ObservableObject, INavigationAware
{
    private readonly ISessionCoordinator _sessions;
    private readonly INavigator _navigation;
    private readonly IClientErrorText _errors;
    private readonly ITextLocalizer _text;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public AccessViewModel(AccessContext context)
    {
        _sessions = context.Sessions;
        _navigation = context.Ui.Navigation;
        _errors = context.Ui.Services.Errors;
        _text = context.Ui.Services.Text;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        GuestCommand = new AsyncRelayCommand(EnterAsGuestAsync);
        ResumeCommand = new AsyncRelayCommand<string>(ResumeSessionAsync);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    // In-memory only. A future secure storage service may supply this value.
    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand GuestCommand { get; }
    public IAsyncRelayCommand ResumeCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = _text.Get("Access.MissingCredentials");
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            // Existing WCF proxies are synchronous; offload their blocking calls from the UI thread.
            string username = Username.Trim();
            string password = Password;
            ClientResult<UserSession> result = await Task.Run(() => _sessions.Login(username, password));
            CompleteAccess(result);
        }
        catch (Exception)
        {
            ErrorMessage = _text.Get("Access.LoginFailed");
        }
        finally
        {
            Password = string.Empty;
            IsBusy = false;
        }
    }

    private async Task EnterAsGuestAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            ClientResult<UserSession> result = await Task.Run(_sessions.CreateGuestSession);
            CompleteAccess(result);
        }
        catch (Exception)
        {
            ErrorMessage = _text.Get("Access.GuestFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ResumeSessionAsync(string? sessionToken)
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            ErrorMessage = _text.Get("Access.MissingToken");
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            ClientResult<UserSession> result = await Task.Run(() => _sessions.ResumeSession(sessionToken));
            CompleteAccess(result);
        }
        catch (Exception)
        {
            ErrorMessage = _text.Get("Access.ResumeFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void OnNavigatedTo()
    {
        ErrorMessage = string.Empty;
        Password = string.Empty;
    }

    public void OnNavigatedFrom()
    {
        Password = string.Empty;
    }

    private void CompleteAccess(ClientResult<UserSession> result)
    {
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessage = _errors.FromResult(result, "Access.LoginFailed");
            return;
        }

        _navigation.NavigateTo(NavigationPage.Home);
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/HomeViewModel.cs
```csharp
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels;

public sealed class HomeViewModel : ObservableObject, INavigationAware
{
    private readonly ISessionCoordinator _sessions;
    private readonly PlayerSession _playerSession;
    private readonly INavigator _navigation;
    private readonly LobbyViewModel _lobby;
    private readonly IClientErrorText _errors;
    private readonly ISessionGuard _coordinator;
    private readonly ITextLocalizer _text;
    private string _lobbyCode = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isPublic = true;
    private int _maxPlayers = 4;
    private bool _isBusy;

    public HomeViewModel(HomeContext context, LobbyViewModel lobby)
    {
        _sessions = context.Sessions;
        _playerSession = context.PlayerSession;
        _navigation = context.Ui.Navigation;
        _lobby = lobby;
        _errors = context.Ui.Services.Errors;
        _coordinator = context.Ui.Services.SessionGuard;
        _text = context.Ui.Services.Text;
        CreateLobbyCommand = new AsyncRelayCommand(CreateLobbyAsync);
        JoinLobbyCommand = new AsyncRelayCommand(JoinLobbyAsync);
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
    }

    public string Username => _playerSession.GetActiveSession()?.Username ?? string.Empty;
    public bool IsGuest => _playerSession.GetActiveSession()?.IsGuest ?? false;

    public string LobbyCode
    {
        get => _lobbyCode;
        set => SetProperty(ref _lobbyCode, value);
    }

    public bool IsPublic
    {
        get => _isPublic;
        set => SetProperty(ref _isPublic, value);
    }

    public int MaxPlayers
    {
        get => _maxPlayers;
        set => SetProperty(ref _maxPlayers, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public IAsyncRelayCommand CreateLobbyCommand { get; }
    public IAsyncRelayCommand JoinLobbyCommand { get; }
    public IAsyncRelayCommand LogoutCommand { get; }

    public void OnNavigatedTo()
    {
        ErrorMessage = string.Empty;
        if (!_coordinator.HasActiveSession()) return;
        RefreshIdentity();
    }

    public void OnNavigatedFrom()
    {
        ErrorMessage = string.Empty;
    }

    public void RefreshIdentity()
    {
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(IsGuest));
    }

    private async Task CreateLobbyAsync()
    {
        if (IsBusy || !_coordinator.HasActiveSession()) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            bool created = await _lobby.CreateAsync(IsPublic, MaxPlayers);
            if (!created) ErrorMessage = _lobby.ErrorMessage;
        }
        finally { IsBusy = false; }
    }

    private async Task JoinLobbyAsync()
    {
        if (IsBusy || !_coordinator.HasActiveSession()) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            bool joined = await _lobby.JoinAsync(LobbyCode);
            if (!joined) ErrorMessage = _lobby.ErrorMessage;
        }
        finally { IsBusy = false; }
    }

    private async Task LogoutAsync()
    {
        if (IsBusy || !string.IsNullOrEmpty(_lobby.LobbyCode)) return;
        if (!_coordinator.HasActiveSession()) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            SessionSnapshot? session = _playerSession.GetActiveSession();
            ClientResult<bool> result = await Task.Run(_sessions.Logout);
            if (_errors.IsUnauthorized(result)) { _coordinator.Invalidate(session); return; }
            if (result.IsSuccess && result.Value) _navigation.NavigateTo(NavigationPage.Access);
            else ErrorMessage = _errors.FromResult(result, "Home.LogoutFailed");
        }
        catch (Exception exception) { ErrorMessage = _errors.FromException(exception, "Home.LogoutFailed"); }
        finally { IsBusy = false; }
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/LobbyViewModel.cs
```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ServiceModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels;

public sealed class LobbyViewModel : ObservableObject, IDisposable, INavigationAware
{
    private readonly ILobbyClient _lobbyClient;
    private readonly IChatClient _chatClient;
    private readonly PlayerSession _playerSession;
    private readonly INavigator _navigation;
    private readonly IClientErrorText _errors;
    private readonly ISessionGuard _coordinator;
    private readonly ITextLocalizer _text;
    private string _lobbyCode = string.Empty;
    private string _messageText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _disposed;

    public LobbyViewModel(ILobbyClient lobbyClient, IChatClient chatClient, LobbyContext context)
    {
        _lobbyClient = lobbyClient;
        _chatClient = chatClient;
        _playerSession = context.PlayerSession;
        _navigation = context.Ui.Navigation;
        _errors = context.Ui.Services.Errors;
        _coordinator = context.Ui.Services.SessionGuard;
        _text = context.Ui.Services.Text;
        _lobbyClient.PlayerJoined += OnPlayerJoined;
        _lobbyClient.PlayerLeft += OnPlayerLeft;
        _lobbyClient.MessageReceived += OnMessageReceived;
        SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);
        LeaveCommand = new AsyncRelayCommand(LeaveAsync);
    }

    public ObservableCollection<LobbyMember> Members { get; } = new();
    public ObservableCollection<LobbyChatMessage> Messages { get; } = new();

    public string LobbyCode
    {
        get => _lobbyCode;
        private set => SetProperty(ref _lobbyCode, value);
    }

    public string MessageText
    {
        get => _messageText;
        set => SetProperty(ref _messageText, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public IAsyncRelayCommand SendMessageCommand { get; }
    public IAsyncRelayCommand LeaveCommand { get; }

    public async Task<bool> CreateAsync(bool isPublic, int maxPlayers)
    {
        if (IsBusy || !string.IsNullOrEmpty(LobbyCode)) return false;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return false; }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            string code = await Task.Run(() => _lobbyClient.CreateLobby(isPublic, maxPlayers, session.Token));
            if (!IsCurrentSession(session)) { _coordinator.Invalidate(); return false; }
            Members.Clear();
            Messages.Clear();
            Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                IsGuest = session.IsGuest });
            LobbyCode = code;
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
            return true;
        }
        catch (Exception exception)
        {
            return HandleServiceFailure(exception, session, "Lobby.CreateFailed");
        }
        finally { IsBusy = false; }
    }

    public async Task<bool> JoinAsync(string code)
    {
        if (IsBusy || !string.IsNullOrEmpty(LobbyCode)) return false;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return false; }
        if (string.IsNullOrWhiteSpace(code)) return Fail("Lobby.CodeRequired");

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            LobbyAdmission admission = await Task.Run(() => _lobbyClient.JoinLobby(code.Trim(), session.Token));
            if (!IsCurrentSession(session)) { _coordinator.Invalidate(); return false; }
            Members.Clear();
            Messages.Clear();
            foreach (LobbyMember member in admission.ExistingMembers) Members.Add(member);
            Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                IsGuest = session.IsGuest });
            LobbyCode = admission.LobbyCode;
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
            return true;
        }
        catch (Exception exception)
        {
            return HandleServiceFailure(exception, session, "Lobby.JoinFailed");
        }
        finally { IsBusy = false; }
    }

    private async Task SendMessageAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(MessageText) || string.IsNullOrEmpty(LobbyCode)) return;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return; }
        string text = MessageText.Trim();
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var result = await Task.Run(() => _chatClient.SendMessage(LobbyCode, text, session.Token));
            if (_errors.IsUnauthorized(result)) { _coordinator.Invalidate(session); return; }
            if (!IsCurrentSession(session)) { _coordinator.Invalidate(); return; }
            if (result.IsSuccess && result.Value) MessageText = string.Empty;
            else ErrorMessage = _errors.FromResult(result, "Lobby.MessageFailed");
        }
        catch (Exception exception) { HandleServiceFailure(exception, session, "Lobby.MessageFailed"); }
        finally { IsBusy = false; }
    }

    private async Task LeaveAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(LobbyCode)) return;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return; }
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            bool left = await Task.Run(() => _lobbyClient.LeaveLobby(LobbyCode, session.Token));
            if (!IsCurrentSession(session)) { _coordinator.Invalidate(); return; }
            if (!left) { Fail("Lobby.LeaveFailed"); return; }
            ResetState();
            _navigation.NavigateTo(NavigationPage.Home);
        }
        catch (Exception exception)
        {
            HandleServiceFailure(exception, session, "Lobby.LeaveFailed");
        }
        finally { IsBusy = false; }
    }

    private void OnPlayerJoined(object? sender, LobbyMember member)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_disposed || string.IsNullOrEmpty(LobbyCode)) return;
            if (Members.All(existing => existing.PlayerId != member.PlayerId)) Members.Add(member);
        });
    }

    private void OnPlayerLeft(object? sender, Guid playerId)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_disposed || string.IsNullOrEmpty(LobbyCode)) return;
            LobbyMember? member = Members.FirstOrDefault(existing => existing.PlayerId == playerId);
            if (member is not null) Members.Remove(member);
        });
    }

    private void OnMessageReceived(object? sender, LobbyChatMessage message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (!_disposed && string.Equals(LobbyCode, message.LobbyCode, StringComparison.OrdinalIgnoreCase))
                Messages.Add(message);
        });
    }

    public void OnNavigatedTo()
    {
        if (!_coordinator.HasActiveSession()) return;
        ErrorMessage = string.Empty;
    }

    public void OnNavigatedFrom()
    {
        // Keep the lobby connection only when moving back from a transient view.
        if (_navigation.CurrentPage == NavigationPage.Access) ResetState();
    }

    public void ResetState()
    {
        LobbyCode = string.Empty;
        MessageText = string.Empty;
        ErrorMessage = string.Empty;
        Members.Clear();
        Messages.Clear();
    }

    private bool IsCurrentSession(SessionSnapshot previous)
    {
        SessionSnapshot? current = _playerSession.GetActiveSession();
        return current is not null && current.PlayerId == previous.PlayerId
            && string.Equals(current.Token, previous.Token, StringComparison.Ordinal);
    }

    private bool HandleServiceFailure(Exception exception, SessionSnapshot session, string fallbackKey)
    {
        if (_errors.IsUnauthorized(exception) || !_coordinator.HasActiveSession())
        {
            _coordinator.Invalidate(session);
            return false;
        }

        ErrorMessage = _errors.FromException(exception, fallbackKey);
        return false;
    }

    private bool Fail(string key)
    {
        ErrorMessage = _text.Get(key);
        return false;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lobbyClient.PlayerJoined -= OnPlayerJoined;
        _lobbyClient.PlayerLeft -= OnPlayerLeft;
        _lobbyClient.MessageReceived -= OnMessageReceived;
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Chat/Services/ChatService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Chat.Services;

public sealed class ChatService : IChatService
{
    private const int MaximumMessageLength = 500;

    private readonly ILobbyDirectory _lobbies;
    private readonly ISessionIdentityResolver _sessions;
    private readonly TimeProvider _timeProvider;

    public ChatService(ILobbyDirectory lobbies, ISessionIdentityResolver sessions, TimeProvider timeProvider)
    {
        _lobbies = lobbies;
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public bool SendMessage(string lobbyCode, string text, string sessionToken)
    {
        SessionIdentity sender = _sessions.Require(sessionToken);
        if (string.IsNullOrWhiteSpace(lobbyCode) || string.IsNullOrWhiteSpace(text)
            || text.Length > MaximumMessageLength)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        LobbyChatMessage message = new LobbyChatMessage
        {
            LobbyCode = lobbyCode.Trim(),
            SenderId = sender.PlayerId,
            SenderUsername = sender.Username,
            Text = text.Trim(),
            SentAt = _timeProvider.GetUtcNow().UtcDateTime
        };

        if (!_lobbies.PublishChatMessage(message))
        {
            throw CreateFault(ServiceErrorCodes.LobbyChatNotAllowed, ServiceMessageKeys.LobbyChatNotAllowed);
        }

        return true;
    }

    private static FaultException<ServiceFault> CreateFault(string code, string key)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = code, MessageKey = key };
        return new FaultException<ServiceFault>(fault, new FaultReason(code));
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Invitations/Dependencies/LobbyInvitationServiceDependencies.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.Matchmaking.Invitations.Dependencies;

public sealed record LobbyInvitationServiceDependencies(
    IFriendshipRepository FriendshipRepository,
    IBlockedPlayerRepository BlockedPlayerRepository,
    ILobbyDirectory LobbyDirectory,
    IFriendshipNotifier Notifier);
```

## File: src/Server/Vertigo.Server.Matchmaking/Invitations/Services/LobbyInvitationService.cs
```csharp
using Vertigo.Server.Matchmaking.Invitations.Dependencies;
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Invitations.Services;

public class LobbyInvitationService : ILobbyInvitationService
{
    private const int MaximumInvitationsPerRequest = 5;

    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly ILobbyDirectory _lobbyDirectory;
    private readonly IFriendshipNotifier _notifier;
    private readonly ISessionIdentityResolver _sessions;

    public LobbyInvitationService(LobbyInvitationServiceDependencies dependencies, ISessionIdentityResolver sessions)
    {
        _friendshipRepository = dependencies.FriendshipRepository;
        _blockedPlayerRepository = dependencies.BlockedPlayerRepository;
        _lobbyDirectory = dependencies.LobbyDirectory;
        _notifier = dependencies.Notifier;
        _sessions = sessions;
    }

    public List<FriendEntry> GetInvitableFriends(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        InvitableFriendsQuery query = new InvitableFriendsQuery
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return GetInvitableFriendsForPlayer(query);
    }

    public List<Guid> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitations invitations = new LobbyInvitations
        {
            SenderId = identity.PlayerId,
            LobbyCode = lobbyCode,
            TargetIds = targetIds
        };

        return SendLobbyInvitationsForPlayer(invitations);
    }

    public List<LobbyInvitationEntry> GetPendingLobbyInvitations(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetPendingInvitationsForPlayer(identity.PlayerId);
    }

    public bool DeclineLobbyInvitation(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitationDecline decline = new LobbyInvitationDecline
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return DeclineInvitationForPlayer(decline);
    }

    private List<FriendEntry> GetInvitableFriendsForPlayer(InvitableFriendsQuery request)
    {
        bool isValid = request is not null && request.PlayerId != Guid.Empty;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        LobbyOverview lobby = FindLobbyOfMember(request!.LobbyCode, request.PlayerId);

        return FindInvitableFriends(request.PlayerId, lobby);
    }

    private List<Guid> SendLobbyInvitationsForPlayer(LobbyInvitations request)
    {
        List<Guid> invitedIds = new List<Guid>();

        EnsureInvitationRequestIsValid(request);

        LobbyOverview lobby = FindLobbyOfMember(request.LobbyCode, request.SenderId);
        LobbyMember sender = lobby.Members.First(member => member.PlayerId == request.SenderId);
        int freeSlots = lobby.MaxPlayers - lobby.Members.Count;

        if (!lobby.IsWaiting)
        {
            throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
        }

        if (sender.IsGuest)
        {
            throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
        }

        if (freeSlots <= 0)
        {
            throw CreateFault(ServiceErrorCodes.LobbyFull, ServiceMessageKeys.LobbyFull);
        }

        List<FriendEntry> invitableFriends = FindInvitableFriends(request.SenderId, lobby)
            .Where(friend => request.TargetIds.Contains(friend.PlayerId))
            .ToList();

        foreach (FriendEntry friend in invitableFriends)
        {
            bool isRegistered = invitedIds.Count < freeSlots
                                && _lobbyDirectory.RegisterInvitation(lobby.LobbyCode, friend.PlayerId, sender.PlayerId);

            if (isRegistered)
            {
                invitedIds.Add(friend.PlayerId);
                _notifier.NotifyLobbyInvitationReceived(friend.PlayerId, CreateEntry(lobby, sender.Username, sender.PlayerId));
            }
        }

        return invitedIds;
    }

    private List<LobbyInvitationEntry> GetPendingInvitationsForPlayer(Guid playerId)
    {
        List<LobbyInvitationEntry> entries = new List<LobbyInvitationEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        foreach (LobbyOverview lobby in _lobbyDirectory.FindLobbiesInviting(playerId))
        {
            Guid senderId = lobby.Invitations[playerId];
            LobbyMember? sender = lobby.Members.FirstOrDefault(member => member.PlayerId == senderId)
                                  ?? lobby.Members.FirstOrDefault(member => member.PlayerId == lobby.HostPlayerId);

            entries.Add(CreateEntry(lobby, sender?.Username ?? string.Empty, senderId));
        }

        return entries;
    }

    private bool DeclineInvitationForPlayer(LobbyInvitationDecline request)
    {
        bool isValid = request is not null && request.PlayerId != Guid.Empty;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        bool isDeclined = _lobbyDirectory.CancelInvitation(request!.LobbyCode, request.PlayerId);

        return isDeclined;
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }

    private static void EnsureInvitationRequestIsValid(LobbyInvitations request)
    {
        bool isValid = request is not null
                       && request.SenderId != Guid.Empty
                       && request.TargetIds is not null
                       && request.TargetIds.Count > 0
                       && request.TargetIds.Count <= MaximumInvitationsPerRequest
                       && !request.TargetIds.Contains(request.SenderId);

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static LobbyInvitationEntry CreateEntry(LobbyOverview lobby, string senderUsername, Guid senderId)
    {
        return new LobbyInvitationEntry
        {
            LobbyCode = lobby.LobbyCode,
            SenderId = senderId,
            SenderUsername = senderUsername,
            CurrentPlayers = lobby.Members.Count,
            MaxPlayers = lobby.MaxPlayers
        };
    }

    private static FriendEntry CreateFriendEntry(Guid playerId, Friendship friendship)
    {
        Player friend = friendship.PlayerRequesterId == playerId
            ? friendship.PlayerAddressee
            : friendship.PlayerRequester;

        return new FriendEntry
        {
            PlayerId = friend.PlayerId,
            Username = friend.Username
        };
    }

    private LobbyOverview FindLobbyOfMember(string lobbyCode, Guid playerId)
    {
        LobbyOverview? lobby = _lobbyDirectory.FindLobby(lobbyCode);

        if (lobby is null)
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }

        if (lobby.Members.All(member => member.PlayerId != playerId))
        {
            throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
        }

        return lobby;
    }

    private List<FriendEntry> FindInvitableFriends(Guid playerId, LobbyOverview lobby)
    {
        List<Friendship> friendships = _friendshipRepository.GetAcceptedFriendships(playerId);
        List<FriendEntry> friends = friendships
            .Select(friendship => CreateFriendEntry(playerId, friendship))
            .ToList();
        List<Guid> friendIds = friends.Select(friend => friend.PlayerId).ToList();
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(playerId, friendIds);

        List<FriendEntry> invitableFriends = friends
            .Where(friend => lobby.Members.All(member => member.PlayerId != friend.PlayerId))
            .Where(friend => !lobby.Invitations.ContainsKey(friend.PlayerId))
            .Where(friend => blocks.All(block => block.PlayerBlockerId != friend.PlayerId && block.PlayerBlockedId != friend.PlayerId))
            .Where(friend => _notifier.IsOnline(friend.PlayerId))
            .Where(friend => !_lobbyDirectory.IsInAnyLobby(friend.PlayerId))
            .OrderBy(friend => friend.Username)
            .ToList();

        return invitableFriends;
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/Services/LobbyService.cs
```csharp
using Vertigo.Contracts.Internal;
using System.Security.Cryptography;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.Matchmaking.Lobbies;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class LobbyService : ILobbyService, ILobbyDirectory
{
    private const string LobbyCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int LobbyCodeLength = 6;
    private const int MinimumMaxPlayers = 2;
    private const int MaximumMaxPlayers = 6;

    private readonly Dictionary<string, LobbyState> _lobbies = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lobbyLock = new();
    private readonly HashSet<string> _watchedSessions = new();
    private readonly TimeProvider _timeProvider;
    private readonly IFriendshipNotifier _notifier;
    private readonly IServiceScopeFactory _scopeFactory;

    public LobbyService(TimeProvider timeProvider, IFriendshipNotifier notifier, IServiceScopeFactory scopeFactory)
    {
        _timeProvider = timeProvider;
        _notifier = notifier;
        _scopeFactory = scopeFactory;
    }

    public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
    {
        if (maxPlayers < MinimumMaxPlayers || maxPlayers > MaximumMaxPlayers)
        {
            throw CreateFault(ServiceErrorCodes.InvalidLobbyCapacity, ServiceMessageKeys.InvalidLobbyCapacity);
        }

        SessionIdentity identity = ResolveIdentity(sessionToken);
        LobbyMember host = CreateMember(identity);
        LobbyConnection hostConnection = new(host, GetCurrentCallback(), WatchCurrentSession());
        DateTimeOffset createdAtUtc = _timeProvider.GetUtcNow();
        string lobbyCode;

        lock (_lobbyLock)
        {
            if (_lobbies.Values.Any(lobby => lobby.Contains(host.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
            }

            lobbyCode = GenerateUniqueLobbyCode();
            LobbySettings settings = new(lobbyCode, host.PlayerId, isPublic, maxPlayers, createdAtUtc);
            LobbyState lobby = new(settings);
            lobby.AddOrReplace(hostConnection);
            _lobbies.Add(lobbyCode, lobby);
        }

        return lobbyCode;
    }

    public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        LobbyMember player = CreateMember(identity);
        LobbyConnection connection = new(player, GetCurrentCallback(), WatchCurrentSession());
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        List<ILobbyCallback> callbacksToNotify = new();
        LobbyAdmission response;

        lock (_lobbyLock)
        {
            if (!_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
            }

            if (lobby.Status != LobbyStatus.Waiting)
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
            }

            if (lobby.Contains(player.PlayerId))
            {
                lobby.CancelInvitation(player.PlayerId);
                lobby.AddOrReplace(connection);
                response = CreateJoinResponse(lobby, player.PlayerId);
            }
            else if (lobby.IsFull)
            {
                throw CreateFault(ServiceErrorCodes.LobbyFull, ServiceMessageKeys.LobbyFull);
            }
            else if (!lobby.IsPublic && !lobby.IsInvited(player.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
            }
            else if (_lobbies.Values.Any(otherLobby => otherLobby.Contains(player.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
            }
            else
            {
                response = CreateJoinResponse(lobby, player.PlayerId);
                callbacksToNotify = lobby.GetCallbacksExcept(player.PlayerId);
                lobby.CancelInvitation(player.PlayerId);
                lobby.AddOrReplace(connection);
            }
        }

        Notify(callbacksToNotify, SendPlayerJoined, player);

        return response;
    }

    // One-way operation: it cannot return a fault, so leaving an unknown lobby is ignored.
    public bool LeaveLobby(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        Guid playerId = identity.PlayerId;
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        List<ILobbyCallback> callbacksToNotify = new();
        List<Guid> withdrawnInvitedIds = new();
        bool wasRemoved = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby) && lobby.Remove(playerId))
            {
                wasRemoved = true;
                if (lobby.IsEmpty)
                {
                    withdrawnInvitedIds = lobby.GetInvitedPlayerIds();
                    _lobbies.Remove(lobby.LobbyCode);
                }
                else
                {
                    callbacksToNotify = lobby.GetCallbacksExcept(playerId);
                }
            }
        }

        Notify(callbacksToNotify, SendPlayerLeft, playerId);
        WithdrawInvitations(normalizedCode, withdrawnInvitedIds);
        return wasRemoved;
    }

    public List<LobbySummary> GetPublicLobbies()
    {
        List<LobbySummary> publicLobbies;

        lock (_lobbyLock)
        {
            publicLobbies = _lobbies.Values
                .Where(lobby => lobby.IsPublic && lobby.Status == LobbyStatus.Waiting && !lobby.IsFull)
                .OrderBy(lobby => lobby.CreatedAtUtc)
                .Select(lobby => lobby.ToSummary())
                .ToList();
        }

        return publicLobbies;
    }

    public LobbyOverview? FindLobby(string lobbyCode)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        LobbyOverview? overview = null;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                overview = lobby.ToOverview();
            }
        }

        return overview;
    }

    public bool IsInAnyLobby(Guid playerId)
    {
        bool isInLobby;

        lock (_lobbyLock)
        {
            isInLobby = _lobbies.Values.Any(lobby => lobby.Contains(playerId));
        }

        return isInLobby;
    }

    public bool RegisterInvitation(string lobbyCode, Guid targetPlayerId, Guid senderPlayerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        bool isRegistered = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby)
                && lobby.Status == LobbyStatus.Waiting
                && !lobby.Contains(targetPlayerId))
            {
                isRegistered = lobby.Invite(targetPlayerId, senderPlayerId);
            }
        }

        return isRegistered;
    }

    public bool CancelInvitation(string lobbyCode, Guid targetPlayerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        bool isCancelled = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                isCancelled = lobby.CancelInvitation(targetPlayerId);
            }
        }

        return isCancelled;
    }

    public List<LobbyOverview> FindLobbiesInviting(Guid playerId)
    {
        List<LobbyOverview> overviews;

        lock (_lobbyLock)
        {
            overviews = _lobbies.Values
                .Where(lobby => lobby.Status == LobbyStatus.Waiting && lobby.IsInvited(playerId))
                .Select(lobby => lobby.ToOverview())
                .ToList();
        }

        return overviews;
    }

    // A closed or faulted channel means the client left or dropped: its players are removed from every lobby.
    public bool PublishChatMessage(LobbyChatMessage message)
    {
        List<ILobbyCallback> callbacks;
        lock (_lobbyLock)
        {
            if (!_lobbies.TryGetValue(message.LobbyCode, out LobbyState? lobby)
                || lobby.Status != LobbyStatus.Waiting
                || !lobby.Contains(message.SenderId))
            {
                return false;
            }

            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        Notify(callbacks, SendChatMessage, message);
        return true;
    }

    private SessionIdentity ResolveIdentity(string sessionToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISessionIdentityResolver resolver = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        return resolver.Require(sessionToken);
    }

    private static LobbyMember CreateMember(SessionIdentity identity)
    {
        return new LobbyMember
        {
            PlayerId = identity.PlayerId,
            Username = identity.Username,
            IsGuest = identity.IsGuest
        };
    }

    private static void SendChatMessage(ILobbyCallback callback, LobbyChatMessage message)
    {
        callback.MessageReceived(message);
    }

    private string WatchCurrentSession()
    {
        IContextChannel channel = (IContextChannel)GetCurrentCallback();
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        bool isNewSession;

        lock (_lobbyLock)
        {
            isNewSession = _watchedSessions.Add(sessionId);
        }

        if (isNewSession)
        {
            channel.Closed += OnChannelClosed;
            channel.Faulted += OnChannelClosed;
        }

        return sessionId;
    }

    private void OnChannelClosed(object? sender, EventArgs eventArgs)
    {
        string sessionId = (sender as IContextChannel)?.SessionId ?? string.Empty;
        List<(List<ILobbyCallback> Callbacks, List<Guid> LeftIds)> notifications = new();
        List<(string LobbyCode, List<Guid> InvitedIds)> withdrawals = new();

        lock (_lobbyLock)
        {
            _watchedSessions.Remove(sessionId);

            foreach (LobbyState lobby in _lobbies.Values.ToList())
            {
                List<Guid> leftIds = lobby.RemoveSession(sessionId);

                if (lobby.IsEmpty)
                {
                    withdrawals.Add((lobby.LobbyCode, lobby.GetInvitedPlayerIds()));
                    _lobbies.Remove(lobby.LobbyCode);
                }
                else if (leftIds.Count > 0)
                {
                    notifications.Add((lobby.GetCallbacksExcept(Guid.Empty), leftIds));
                }
            }
        }

        foreach ((List<ILobbyCallback> callbacks, List<Guid> leftIds) in notifications)
        {
            foreach (Guid leftId in leftIds)
            {
                Notify(callbacks, SendPlayerLeft, leftId);
            }
        }

        foreach ((string lobbyCode, List<Guid> invitedIds) in withdrawals)
        {
            WithdrawInvitations(lobbyCode, invitedIds);
        }
    }

    private void WithdrawInvitations(string lobbyCode, List<Guid> invitedIds)
    {
        foreach (Guid invitedId in invitedIds)
        {
            _notifier.NotifyLobbyInvitationWithdrawn(invitedId, lobbyCode);
        }
    }

    private string GenerateUniqueLobbyCode()
    {
        string lobbyCode;

        do
        {
            lobbyCode = RandomNumberGenerator.GetString(LobbyCodeAlphabet, LobbyCodeLength);
        }
        while (_lobbies.ContainsKey(lobbyCode));

        return lobbyCode;
    }

    private static LobbyAdmission CreateJoinResponse(LobbyState lobby, Guid joiningPlayerId)
    {
        LobbyAdmission response = new()
        {
            LobbyCode = lobby.LobbyCode,
            HostPlayerId = lobby.HostPlayerId,
            IsPublic = lobby.IsPublic,
            MaxPlayers = lobby.MaxPlayers,
            ExistingMembers = lobby.GetMembersExcept(joiningPlayerId)
        };

        return response;
    }

    private static ILobbyCallback GetCurrentCallback()
    {
        ILobbyCallback callback = OperationContext.Current.GetCallbackChannel<ILobbyCallback>();

        return callback;
    }

    private static void SendPlayerJoined(ILobbyCallback callback, LobbyMember player)
    {
        callback.PlayerJoined(player);
    }

    private static void SendPlayerLeft(ILobbyCallback callback, Guid playerId)
    {
        callback.PlayerLeft(playerId);
    }

    private static void Notify<TArgument>(
        IReadOnlyList<ILobbyCallback> callbacks,
        Action<ILobbyCallback, TArgument> notification,
        TArgument argument)
    {
        foreach (ILobbyCallback callback in callbacks)
        {
            try
            {
                notification(callback, argument);
            }
            catch (CommunicationException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
            catch (TimeoutException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
            catch (ObjectDisposedException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
        }
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new()
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbyConnection.cs
```csharp
#nullable enable

using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed class LobbyConnection
{
    public LobbyConnection(LobbyMember member, ILobbyCallback callback, string sessionId)
    {
        Member = member;
        Callback = callback;
        SessionId = sessionId;
    }

    public string SessionId { get; }

    public LobbyMember Member { get; }

    public ILobbyCallback Callback { get; }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbySettings.cs
```csharp
namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed record LobbySettings(
    string LobbyCode,
    Guid HostPlayerId,
    bool IsPublic,
    int MaxPlayers,
    DateTimeOffset CreatedAtUtc);
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbyState.cs
```csharp
#nullable enable

using Vertigo.Contracts.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed class LobbyState
{
    private readonly List<LobbyConnection> _connections = new();
    private readonly Dictionary<Guid, Guid> _invitations = new();

    public LobbyState(LobbySettings settings)
    {
        LobbyCode = settings.LobbyCode;
        HostPlayerId = settings.HostPlayerId;
        IsPublic = settings.IsPublic;
        MaxPlayers = settings.MaxPlayers;
        CreatedAtUtc = settings.CreatedAtUtc;
        Status = LobbyStatus.Waiting;
    }

    public string LobbyCode { get; }

    public Guid HostPlayerId { get; private set; }

    public bool IsPublic { get; }

    public int MaxPlayers { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public LobbyStatus Status { get; private set; }

    public bool IsEmpty => _connections.Count == 0;

    public bool IsFull => _connections.Count >= MaxPlayers;

    public bool Contains(Guid playerId)
    {
        bool contains = _connections.Exists(connection => connection.Member.PlayerId == playerId);

        return contains;
    }

    public void AddOrReplace(LobbyConnection connection)
    {
        int index = _connections.FindIndex(existing => existing.Member.PlayerId == connection.Member.PlayerId);
        if (index >= 0)
        {
            _connections[index] = connection;
        }
        else
        {
            _connections.Add(connection);
        }
    }

    public bool Remove(Guid playerId)
    {
        int removedCount = _connections.RemoveAll(connection => connection.Member.PlayerId == playerId);

        // The role passes to the participant who has been longest in the lobby (CU-014 FA-04).
        if (removedCount > 0 && playerId == HostPlayerId && _connections.Count > 0)
        {
            HostPlayerId = _connections[0].Member.PlayerId;
        }

        return removedCount > 0;
    }

    public List<Guid> RemoveSession(string sessionId)
    {
        List<Guid> removedIds = _connections
            .Where(connection => connection.SessionId == sessionId)
            .Select(connection => connection.Member.PlayerId)
            .ToList();

        foreach (Guid playerId in removedIds)
        {
            Remove(playerId);
        }

        return removedIds;
    }

    public bool Invite(Guid targetPlayerId, Guid senderPlayerId)
    {
        bool isInvited = _invitations.TryAdd(targetPlayerId, senderPlayerId);

        return isInvited;
    }

    public bool CancelInvitation(Guid targetPlayerId)
    {
        bool isCancelled = _invitations.Remove(targetPlayerId);

        return isCancelled;
    }

    public bool IsInvited(Guid playerId)
    {
        bool isInvited = _invitations.ContainsKey(playerId);

        return isInvited;
    }

    public List<Guid> GetInvitedPlayerIds()
    {
        List<Guid> invitedIds = _invitations.Keys.ToList();

        return invitedIds;
    }

    public LobbyOverview ToOverview()
    {
        LobbyOverview overview = new(
            LobbyCode,
            HostPlayerId,
            MaxPlayers,
            Status == LobbyStatus.Waiting,
            GetMembersExcept(Guid.Empty),
            new Dictionary<Guid, Guid>(_invitations));

        return overview;
    }

    public void StartMatch()
    {
        Status = LobbyStatus.InGame;
    }

    public List<LobbyMember> GetMembersExcept(Guid playerId)
    {
        List<LobbyMember> members = _connections
            .Where(connection => connection.Member.PlayerId != playerId)
            .Select(connection => connection.Member)
            .ToList();

        return members;
    }

    public List<ILobbyCallback> GetCallbacksExcept(Guid playerId)
    {
        List<ILobbyCallback> callbacks = _connections
            .Where(connection => connection.Member.PlayerId != playerId)
            .Select(connection => connection.Callback)
            .ToList();

        return callbacks;
    }

    public LobbySummary ToSummary()
    {
        LobbyConnection? hostConnection = _connections.Find(
            connection => connection.Member.PlayerId == HostPlayerId);

        LobbySummary summary = new()
        {
            LobbyCode = LobbyCode,
            HostUsername = hostConnection?.Member.Username ?? string.Empty,
            CurrentPlayers = _connections.Count,
            MaxPlayers = MaxPlayers
        };

        return summary;
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbyStatus.cs
```csharp
namespace Vertigo.Server.Matchmaking.Lobbies;

public enum LobbyStatus
{
    Waiting,
    InGame
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Interfaces/IGuestSessionRegistry.cs
```csharp
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Interfaces;

public interface IGuestSessionRegistry
{
    AuthenticatedSession Create();

    SessionIdentity? Find(string tokenHash);

    AuthenticatedSession? GetSession(string tokenHash);

    bool Revoke(string tokenHash);
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Interfaces/ISessionIdentityResolver.cs
```csharp
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Interfaces;

public interface ISessionIdentityResolver
{
    SessionIdentity? Resolve(string token);

    SessionIdentity Require(string token);

    SessionIdentity RequireRegistered(string token);
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Models/SessionIdentity.cs
```csharp
namespace Vertigo.Server.PlayerManagement.Authentication.Models;

public sealed record SessionIdentity(Guid PlayerId, string Username, bool IsGuest);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/AuthenticationService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public class AuthenticationService : IAuthService
{
    private readonly AuthService _authentication;

    public AuthenticationService(AuthService authentication)
    {
        _authentication = authentication;
    }

    public UserSession Login(LoginCredentials credentials)
    {
        if (credentials is null || string.IsNullOrWhiteSpace(credentials.Username)
            || string.IsNullOrWhiteSpace(credentials.Password))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        AuthenticatedSession? session = _authentication.Login(credentials.Username, credentials.Password);
        if (session is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        return ConvertSession(session);
    }

    public UserSession ResumeSession(string token)
    {
        AuthenticatedSession? session = _authentication.ResumeSession(token);
        if (session is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        return ConvertSession(session);
    }

    public UserSession CreateGuestSession()
    {
        return ConvertSession(_authentication.CreateGuestSession());
    }

    public bool Logout(SessionLogout logout)
    {
        return logout is not null && _authentication.Logout(logout.SessionToken);
    }

    private static UserSession ConvertSession(AuthenticatedSession session)
    {
        return new UserSession
        {
            PlayerId = session.PlayerId,
            Username = session.Username,
            Token = session.Token,
            ExpiresAt = session.ExpiresAt,
            IsGuest = session.IsGuest
        };
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/GuestSessionRegistry.cs
```csharp
using System.Collections.Concurrent;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class GuestSessionRegistry : IGuestSessionRegistry
{
    private const int GuestDurationHours = 12;

    private readonly ConcurrentDictionary<string, AuthenticatedSession> _sessions = new();
    private readonly ITokenGenerator _tokenGenerator;
    private readonly TimeProvider _timeProvider;

    public GuestSessionRegistry(ITokenGenerator tokenGenerator, TimeProvider timeProvider)
    {
        _tokenGenerator = tokenGenerator;
        _timeProvider = timeProvider;
    }

    public AuthenticatedSession Create()
    {
        string token = _tokenGenerator.GenerateSessionToken();
        string guestCode = _tokenGenerator.GenerateVerificationCode();
        AuthenticatedSession session = new AuthenticatedSession
        {
            PlayerId = Guid.NewGuid(),
            Username = $"Guest_{guestCode}",
            Token = token,
            ExpiresAt = _timeProvider.GetUtcNow().UtcDateTime.AddHours(GuestDurationHours),
            IsGuest = true
        };

        string tokenHash = _tokenGenerator.HashSessionToken(token);
        if (!_sessions.TryAdd(tokenHash, session))
        {
            throw new InvalidOperationException("A guest session could not be registered.");
        }

        return session;
    }

    public SessionIdentity? Find(string tokenHash)
    {
        if (!_sessions.TryGetValue(tokenHash, out AuthenticatedSession? session))
        {
            return null;
        }

        if (session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            _sessions.TryRemove(tokenHash, out _);
            return null;
        }

        return new SessionIdentity(session.PlayerId, session.Username, true);
    }

    public AuthenticatedSession? GetSession(string tokenHash)
    {
        if (Find(tokenHash) is null)
        {
            return null;
        }

        return _sessions.TryGetValue(tokenHash, out AuthenticatedSession? session) ? session : null;
    }

    public bool Revoke(string tokenHash)
    {
        return _sessions.TryRemove(tokenHash, out _);
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/SessionIdentityResolver.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class SessionIdentityResolver : ISessionIdentityResolver
{
    private const int SessionTokenLength = 64;

    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IGuestSessionRegistry _guests;

    public SessionIdentityResolver(
        ISessionTokenRepository sessionTokenRepository,
        ITokenGenerator tokenGenerator,
        IGuestSessionRegistry guests)
    {
        _sessionTokenRepository = sessionTokenRepository;
        _tokenGenerator = tokenGenerator;
        _guests = guests;
    }

    public SessionIdentity? Resolve(string token)
    {
        SessionIdentity? identity = null;
        if (!string.IsNullOrWhiteSpace(token) && token.Length == SessionTokenLength)
        {
            string tokenHash = _tokenGenerator.HashSessionToken(token);
            identity = _guests.Find(tokenHash);
            if (identity is null)
            {
                SessionToken? session = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
                if (session is not null)
                {
                    identity = new SessionIdentity(session.PlayerId, session.Player.Username, false);
                }
            }
        }

        return identity;
    }

    public SessionIdentity Require(string token)
    {
        return Resolve(token) ?? throw CreateUnauthorizedFault();
    }

    public SessionIdentity RequireRegistered(string token)
    {
        SessionIdentity identity = Require(token);
        if (identity.IsGuest)
        {
            throw CreateUnauthorizedFault();
        }

        return identity;
    }

    private static FaultException<ServiceFault> CreateUnauthorizedFault()
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = ServiceErrorCodes.Unauthorized,
            MessageKey = ServiceMessageKeys.Unauthorized
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(fault.ErrorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Blocking/Services/BlockedPlayerService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Blocking.Services;

public class BlockedPlayerService : IBlockedPlayerService
{
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly ISessionIdentityResolver _sessions;

    public BlockedPlayerService(IBlockedPlayerRepository blockedPlayerRepository, ISessionIdentityResolver sessions)
    {
        _blockedPlayerRepository = blockedPlayerRepository;
        _sessions = sessions;
    }

    public List<BlockedPlayerSummary> GetBlockedPlayers(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetBlockedPlayersForPlayer(identity.PlayerId);
    }

    public bool BlockPlayer(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return BlockPlayerBetween(new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId });
    }

    public bool UnblockPlayer(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return UnblockPlayerBetween(new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId });
    }

    private List<BlockedPlayerSummary> GetBlockedPlayersForPlayer(Guid playerId)
    {
        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<BlockedPlayer> blockedPlayers = _blockedPlayerRepository.GetBlockedPlayers(playerId);
        return blockedPlayers.Select(blocked => new BlockedPlayerSummary
        {
            PlayerId = blocked.PlayerBlockedId,
            Username = blocked.PlayerBlocked.Username,
            BlockedAt = blocked.CreatedAt
        }).ToList();
    }

    private bool BlockPlayerBetween(PlayerPair pair)
    {
        EnsureValidPair(pair);
        if (_blockedPlayerRepository.IsPlayerBlocked(pair.PlayerId, pair.OtherPlayerId))
        {
            return false;
        }

        return _blockedPlayerRepository.BlockPlayer(pair.PlayerId, pair.OtherPlayerId);
    }

    private bool UnblockPlayerBetween(PlayerPair pair)
    {
        EnsureValidPair(pair);
        return _blockedPlayerRepository.UnblockPlayer(pair.PlayerId, pair.OtherPlayerId);
    }

    private static void EnsureValidPair(PlayerPair pair)
    {
        if (pair is null || pair.PlayerId == Guid.Empty || pair.OtherPlayerId == Guid.Empty
            || pair.PlayerId == pair.OtherPlayerId)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Dependencies/FriendshipServiceDependencies.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Friendships.Dependencies;

public sealed record FriendshipServiceDependencies(
    IPlayerRepository PlayerRepository,
    IFriendshipRepository FriendshipRepository,
    IBlockedPlayerRepository BlockedPlayerRepository,
    IFriendshipNotifier Notifier);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Models/FriendshipSubscription.cs
```csharp
using Vertigo.Contracts.Callbacks;

namespace Vertigo.Server.PlayerManagement.Friendships.Models;

public sealed record FriendshipSubscription(IFriendshipCallback Callback, string SessionId);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Services/FriendshipNotificationService.cs
```csharp
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class FriendshipNotificationService : IFriendshipNotificationService
{
    private readonly IFriendshipNotifier _notifier;
    private readonly IServiceScopeFactory _scopeFactory;

    public FriendshipNotificationService(IFriendshipNotifier notifier, IServiceScopeFactory scopeFactory)
    {
        _notifier = notifier;
        _scopeFactory = scopeFactory;
    }

    public bool Subscribe(string sessionToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISessionIdentityResolver resolver = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        SessionIdentity identity = resolver.RequireRegistered(sessionToken);

        IFriendshipCallback callback = OperationContext.Current.GetCallbackChannel<IFriendshipCallback>();
        IContextChannel channel = (IContextChannel)callback;
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        _notifier.Subscribe(identity.PlayerId, callback, sessionId);

        channel.Closed -= OnChannelClosed;
        channel.Faulted -= OnChannelClosed;
        channel.Closed += OnChannelClosed;
        channel.Faulted += OnChannelClosed;

        return true;
    }

    public void Unsubscribe()
    {
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        _notifier.UnsubscribeSession(sessionId);
    }

    private void OnChannelClosed(object? sender, EventArgs eventArgs)
    {
        string sessionId = (sender as IContextChannel)?.SessionId ?? string.Empty;

        _notifier.UnsubscribeSession(sessionId);
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Services/FriendshipNotifier.cs
```csharp
using Vertigo.Server.PlayerManagement.Friendships.Models;
using Vertigo.Contracts.Internal;
using System.Collections.Concurrent;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public sealed class FriendshipNotifier : IFriendshipNotifier
{
    private readonly ConcurrentDictionary<Guid, FriendshipSubscription> _subscriptions = new();

    public void Subscribe(Guid playerId, IFriendshipCallback callback, string sessionId)
    {
        _subscriptions[playerId] = new FriendshipSubscription(callback, sessionId);
    }

    public void Unsubscribe(Guid playerId, string sessionId)
    {
        if (_subscriptions.TryGetValue(playerId, out FriendshipSubscription? subscription)
            && subscription.SessionId == sessionId)
        {
            ICollection<KeyValuePair<Guid, FriendshipSubscription>> entries = _subscriptions;
            entries.Remove(new KeyValuePair<Guid, FriendshipSubscription>(playerId, subscription));
        }
    }

    public void UnsubscribeSession(string sessionId)
    {
        List<Guid> playerIds = _subscriptions
            .Where(pair => pair.Value.SessionId == sessionId)
            .Select(pair => pair.Key)
            .ToList();

        foreach (Guid playerId in playerIds)
        {
            Unsubscribe(playerId, sessionId);
        }
    }

    public void NotifyFriendRequestReceived(Guid targetPlayerId, FriendshipInvitationEntry request)
    {
        Send(targetPlayerId, SendFriendRequestReceived, request);
    }

    public void NotifyFriendshipEstablished(Guid targetPlayerId, FriendEntry friend)
    {
        Send(targetPlayerId, SendFriendshipEstablished, friend);
    }

    public void NotifyLobbyInvitationReceived(Guid targetPlayerId, LobbyInvitationEntry invitation)
    {
        Send(targetPlayerId, SendLobbyInvitationReceived, invitation);
    }

    public void NotifyLobbyInvitationWithdrawn(Guid targetPlayerId, string lobbyCode)
    {
        Send(targetPlayerId, SendLobbyInvitationWithdrawn, lobbyCode);
    }

    public bool IsOnline(Guid playerId)
    {
        bool isOnline = _subscriptions.ContainsKey(playerId);

        return isOnline;
    }

    private static void SendLobbyInvitationReceived(IFriendshipCallback callback, LobbyInvitationEntry invitation)
    {
        callback.OnLobbyInvitationReceived(invitation);
    }

    private static void SendLobbyInvitationWithdrawn(IFriendshipCallback callback, string lobbyCode)
    {
        callback.OnLobbyInvitationWithdrawn(lobbyCode);
    }

    private static void SendFriendRequestReceived(IFriendshipCallback callback, FriendshipInvitationEntry request)
    {
        callback.OnFriendRequestReceived(request);
    }

    private static void SendFriendshipEstablished(IFriendshipCallback callback, FriendEntry friend)
    {
        callback.OnFriendshipEstablished(friend);
    }

    private void Send<TArgument>(
        Guid targetPlayerId,
        Action<IFriendshipCallback, TArgument> notification,
        TArgument argument)
    {
        if (_subscriptions.TryGetValue(targetPlayerId, out FriendshipSubscription? subscription))
        {
            try
            {
                notification(subscription.Callback, argument);
            }
            catch (CommunicationException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
            catch (TimeoutException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
            catch (ObjectDisposedException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
        }
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Services/FriendshipService.cs
```csharp
using Vertigo.Server.PlayerManagement.Friendships.Dependencies;
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public class FriendshipService : IFriendshipService
{
    private const int MinimumSearchLength = 3;
    private const int MaximumSearchResults = 20;

    private readonly IPlayerRepository _playerRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly IFriendshipNotifier _notifier;
    private readonly ISessionIdentityResolver _sessions;

    public FriendshipService(FriendshipServiceDependencies dependencies, ISessionIdentityResolver sessions)
    {
        _playerRepository = dependencies.PlayerRepository;
        _friendshipRepository = dependencies.FriendshipRepository;
        _blockedPlayerRepository = dependencies.BlockedPlayerRepository;
        _notifier = dependencies.Notifier;
        _sessions = sessions;
    }

    public List<FriendEntry> GetFriends(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetFriendsForPlayer(identity.PlayerId);
    }

    public List<PlayerSearchEntry> SearchPlayers(string searchTerm, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerSearch search = new PlayerSearch { RequesterId = identity.PlayerId, SearchTerm = searchTerm };
        return SearchPlayersForPlayer(search);
    }

    public PlayerRelation SendFriendRequest(Guid targetPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipInvitation invitation = new FriendshipInvitation
        {
            SenderId = identity.PlayerId,
            TargetId = targetPlayerId
        };

        return SendFriendRequestForPlayer(invitation);
    }

    public List<FriendshipInvitationEntry> GetPendingFriendRequests(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetPendingRequestsForPlayer(identity.PlayerId);
    }

    public bool RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipDecision decision = new FriendshipDecision
        {
            SenderId = senderPlayerId,
            ResponderId = identity.PlayerId,
            Accept = accept
        };

        return DecideFriendshipForPlayer(decision);
    }

    public bool RemoveFriendship(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerPair pair = new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId };
        return RemoveFriendshipForPlayer(pair);
    }

    private List<FriendEntry> GetFriendsForPlayer(Guid playerId)
    {
        List<FriendEntry> entries = new List<FriendEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<Friendship> friendships = _friendshipRepository.GetAcceptedFriendships(playerId);

        foreach (Friendship friendship in friendships)
        {
            entries.Add(CreateFriendEntry(playerId, friendship));
        }

        return entries.OrderBy(entry => entry.Username).ToList();
    }

    private List<PlayerSearchEntry> SearchPlayersForPlayer(PlayerSearch request)
    {
        List<PlayerSearchEntry> entries = new List<PlayerSearchEntry>();

        EnsureSearchRequestIsValid(request);

        string searchTerm = request.SearchTerm.Trim();
        List<Player> players = _playerRepository.SearchPlayersByUsername(
            searchTerm,
            request.RequesterId,
            MaximumSearchResults);
        List<Guid> playerIds = players.Select(player => player.PlayerId).ToList();
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(request.RequesterId, playerIds);
        List<Friendship> friendships = _friendshipRepository.GetFriendshipsBetween(request.RequesterId, playerIds);

        foreach (Player player in players)
        {
            bool hasBlockedRequester = blocks.Any(block => block.PlayerBlockerId == player.PlayerId);

            if (!hasBlockedRequester)
            {
                PlayerRelation relation = ResolveRelation(request.RequesterId, player.PlayerId, blocks, friendships);
                entries.Add(CreateSearchEntry(player, relation));
            }
        }

        return entries;
    }

    private PlayerRelation SendFriendRequestForPlayer(FriendshipInvitation request)
    {
        PlayerRelation relation = PlayerRelation.None;

        EnsureFriendRequestIsValid(request);

        Player sender = FindRegisteredPlayer(request.SenderId);
        Player target = FindRegisteredPlayer(request.TargetId);
        EnsurePlayersAreNotBlocked(request);

        Friendship? existingFriendship = _friendshipRepository.GetFriendshipBetween(request.SenderId, request.TargetId);

        if (existingFriendship is null)
        {
            relation = CreatePendingFriendship(sender, target);
        }
        else if (existingFriendship.Status == FriendshipStatus.Accepted)
        {
            throw CreateFault(ServiceErrorCodes.AlreadyFriends, ServiceMessageKeys.AlreadyFriends);
        }
        else if (existingFriendship.PlayerRequesterId == request.SenderId)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestAlreadySent, ServiceMessageKeys.FriendRequestAlreadySent);
        }
        else
        {
            relation = AcceptCrossedRequest(existingFriendship, sender, target);
        }

        return relation;
    }

    private List<FriendshipInvitationEntry> GetPendingRequestsForPlayer(Guid playerId)
    {
        List<FriendshipInvitationEntry> entries = new List<FriendshipInvitationEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<Friendship> pendingFriendships = _friendshipRepository.GetPendingRequestsReceivedBy(playerId);

        foreach (Friendship friendship in pendingFriendships)
        {
            entries.Add(CreateRequestEntry(friendship));
        }

        return entries;
    }

    private bool DecideFriendshipForPlayer(FriendshipDecision request)
    {
        bool isResponded = false;

        EnsureResponseIsValid(request);

        Friendship? friendship = _friendshipRepository.GetFriendshipBetween(request.SenderId, request.ResponderId);
        bool isPendingForResponder = friendship is not null
                                     && friendship.Status == FriendshipStatus.Pending
                                     && friendship.PlayerRequesterId == request.SenderId;

        if (!isPendingForResponder)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestNotFound, ServiceMessageKeys.FriendRequestNotFound);
        }

        if (request.Accept)
        {
            isResponded = AcceptFriendRequest(request);
        }
        else
        {
            isResponded = _friendshipRepository.DeletePendingFriendship(request.SenderId, request.ResponderId);
        }

        return isResponded;
    }

    private bool RemoveFriendshipForPlayer(PlayerPair pair)
    {
        if (pair is null || pair.PlayerId == Guid.Empty || pair.OtherPlayerId == Guid.Empty
            || pair.PlayerId == pair.OtherPlayerId)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return _friendshipRepository.DeleteFriendshipBetween(pair.PlayerId, pair.OtherPlayerId);
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }

    private static PlayerRelation ResolveRelation(
        Guid requesterId,
        Guid otherPlayerId,
        List<BlockedPlayer> blocks,
        List<Friendship> friendships)
    {
        PlayerRelation relation = PlayerRelation.None;

        bool isBlockedByRequester = blocks.Any(
            block => block.PlayerBlockerId == requesterId && block.PlayerBlockedId == otherPlayerId);
        Friendship? friendship = friendships.FirstOrDefault(
            candidate => candidate.PlayerRequesterId == otherPlayerId || candidate.PlayerAddresseeId == otherPlayerId);

        if (isBlockedByRequester)
        {
            relation = PlayerRelation.Blocked;
        }
        else if (friendship is not null && friendship.Status == FriendshipStatus.Accepted)
        {
            relation = PlayerRelation.Friends;
        }
        else if (friendship is not null && friendship.PlayerRequesterId == requesterId)
        {
            relation = PlayerRelation.RequestSent;
        }

        return relation;
    }

    private static PlayerSearchEntry CreateSearchEntry(Player player, PlayerRelation relation)
    {
        return new PlayerSearchEntry
        {
            PlayerId = player.PlayerId,
            Username = player.Username,
            Relation = relation
        };
    }

    private static FriendshipInvitationEntry CreateRequestEntry(Friendship friendship)
    {
        return new FriendshipInvitationEntry
        {
            SenderId = friendship.PlayerRequesterId,
            SenderUsername = friendship.PlayerRequester.Username
        };
    }

    private static void EnsureSearchRequestIsValid(PlayerSearch request)
    {
        bool isValid = request is not null
                       && request.RequesterId != Guid.Empty
                       && request.SearchTerm is not null
                       && request.SearchTerm.Trim().Length >= MinimumSearchLength;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static void EnsureFriendRequestIsValid(FriendshipInvitation request)
    {
        bool isValid = request is not null
                       && request.SenderId != Guid.Empty
                       && request.TargetId != Guid.Empty
                       && request.SenderId != request.TargetId;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static void EnsureResponseIsValid(FriendshipDecision request)
    {
        bool isValid = request is not null
                       && request.ResponderId != Guid.Empty
                       && request.SenderId != Guid.Empty
                       && request.ResponderId != request.SenderId;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static FriendEntry CreateFriendEntry(Guid playerId, Friendship friendship)
    {
        Player friend = friendship.PlayerRequesterId == playerId
            ? friendship.PlayerAddressee
            : friendship.PlayerRequester;

        return CreateFriendEntry(friend);
    }

    private static FriendEntry CreateFriendEntry(Player player)
    {
        return new FriendEntry
        {
            PlayerId = player.PlayerId,
            Username = player.Username
        };
    }

    private Player FindRegisteredPlayer(Guid playerId)
    {
        Player? player = _playerRepository.GetPlayerById(playerId);

        if (player is null)
        {
            throw CreateFault(ServiceErrorCodes.UserNotFound, ServiceMessageKeys.UserNotFound);
        }

        return player;
    }

    private void EnsurePlayersAreNotBlocked(FriendshipInvitation request)
    {
        List<Guid> targetIds = new List<Guid> { request.TargetId };
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(request.SenderId, targetIds);

        if (blocks.Count > 0)
        {
            throw CreateFault(ServiceErrorCodes.FriendshipBlocked, ServiceMessageKeys.FriendshipBlocked);
        }
    }

    private PlayerRelation CreatePendingFriendship(Player sender, Player target)
    {
        bool isCreated = _friendshipRepository.CreatePendingFriendship(sender.PlayerId, target.PlayerId);

        if (!isCreated)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        FriendshipInvitationEntry entry = new FriendshipInvitationEntry
        {
            SenderId = sender.PlayerId,
            SenderUsername = sender.Username
        };
        _notifier.NotifyFriendRequestReceived(target.PlayerId, entry);

        return PlayerRelation.RequestSent;
    }

    private PlayerRelation AcceptCrossedRequest(Friendship crossedFriendship, Player sender, Player target)
    {
        bool isAccepted = _friendshipRepository.AcceptPendingFriendship(
            crossedFriendship.PlayerRequesterId,
            crossedFriendship.PlayerAddresseeId);

        if (!isAccepted)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestNotFound, ServiceMessageKeys.FriendRequestNotFound);
        }

        _notifier.NotifyFriendshipEstablished(sender.PlayerId, CreateFriendEntry(target));
        _notifier.NotifyFriendshipEstablished(target.PlayerId, CreateFriendEntry(sender));

        return PlayerRelation.Friends;
    }

    private bool AcceptFriendRequest(FriendshipDecision request)
    {
        bool isAccepted = _friendshipRepository.AcceptPendingFriendship(request.SenderId, request.ResponderId);

        if (isAccepted)
        {
            Player responder = FindRegisteredPlayer(request.ResponderId);
            _notifier.NotifyFriendshipEstablished(request.SenderId, CreateFriendEntry(responder));
        }

        return isAccepted;
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Registration/Dependencies/AccountServiceDependencies.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.PlayerManagement.Registration.Dependencies;

public sealed record AccountServiceDependencies(
    IPlayerRepository PlayerRepository,
    ISessionTokenRepository SessionTokenRepository,
    IPasswordHasher PasswordHasher,
    ISessionIdentityResolver Sessions);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Registration/Services/AccountService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

public class AccountService : IAccountService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionIdentityResolver _sessions;

    public AccountService(AccountServiceDependencies dependencies)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _sessions = dependencies.Sessions;
    }

    public bool RegisterAccount(AccountRegistration registration)
    {
        if (registration is null || string.IsNullOrWhiteSpace(registration.Username)
            || string.IsNullOrWhiteSpace(registration.Email)
            || string.IsNullOrWhiteSpace(registration.Password)
            || registration.Username.Length > 32 || registration.Email.Length > 120)
        {
            throw CreateFault(ServiceErrorCodes.InvalidRegistrationData, ServiceMessageKeys.InvalidRegistrationData);
        }

        if (_playerRepository.UsernameExists(registration.Username))
        {
            throw CreateFault(ServiceErrorCodes.UsernameAlreadyRegistered,
                ServiceMessageKeys.UsernameAlreadyRegistered);
        }

        if (_playerRepository.EmailExists(registration.Email))
        {
            throw CreateFault(ServiceErrorCodes.EmailAlreadyRegistered,
                ServiceMessageKeys.EmailAlreadyRegistered);
        }

        string salt = _passwordHasher.GenerateSalt();
        Player player = new Player
        {
            Username = registration.Username.Trim(),
            Email = registration.Email.Trim(),
            PasswordSalt = salt,
            PasswordHash = _passwordHasher.HashPassword(registration.Password, salt),
            Stats = new PlayerStats()
        };

        return _playerRepository.CreatePlayer(player);
    }

    public bool ChangePassword(PasswordChange change, string sessionToken)
    {
        if (change is null || string.IsNullOrWhiteSpace(change.Username)
            || string.IsNullOrWhiteSpace(change.CurrentPassword)
            || string.IsNullOrWhiteSpace(change.NewPassword))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        Player? player = _playerRepository.GetPlayerById(identity.PlayerId);
        if (player is null || !string.Equals(player.Username, change.Username, StringComparison.OrdinalIgnoreCase) || !_passwordHasher.VerifyPassword(
                change.CurrentPassword, player.PasswordHash, player.PasswordSalt))
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        string salt = _passwordHasher.GenerateSalt();
        string hash = _passwordHasher.HashPassword(change.NewPassword, salt);
        bool isChanged = _playerRepository.UpdatePassword(player.PlayerId, hash, salt);
        if (isChanged)
        {
            _sessionTokenRepository.RevokePlayerSessions(player.PlayerId);
        }

        return isChanged;
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Shared/Vertigo.Contracts/Callbacks/IFriendshipCallback.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface IFriendshipCallback
{
    [OperationContract(IsOneWay = true)]
    void OnFriendRequestReceived(FriendshipInvitationEntry request);

    [OperationContract(IsOneWay = true)]
    void OnFriendshipEstablished(FriendEntry friend);

    [OperationContract(IsOneWay = true)]
    void OnLobbyInvitationReceived(LobbyInvitationEntry invitation);

    [OperationContract(IsOneWay = true)]
    void OnLobbyInvitationWithdrawn(string lobbyCode);
}
```

## File: src/Shared/Vertigo.Contracts/Callbacks/ILobbyCallback.cs
```csharp
#nullable enable

using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface ILobbyCallback
{
    [OperationContract(IsOneWay = true)]
    void PlayerJoined(LobbyMember player);

    [OperationContract(IsOneWay = true)]
    void PlayerLeft(Guid playerId);

    [OperationContract(IsOneWay = true)]
    void MessageReceived(LobbyChatMessage message);
}
```

## File: src/Shared/Vertigo.Contracts/Endpoints/ServiceEndpoints.cs
```csharp
namespace Vertigo.Contracts.Endpoints;

public static class ServiceEndpoints
{
    public const int TcpPort = 8081;

    public static readonly string AccountServiceAddress = $"net.tcp://localhost:{TcpPort}/AccountService";
    public static readonly string AuthServiceAddress = $"net.tcp://localhost:{TcpPort}/AuthService";
    public static readonly string FriendshipServiceAddress = $"net.tcp://localhost:{TcpPort}/FriendshipService";
    public static readonly string FriendshipNotificationServiceAddress = $"net.tcp://localhost:{TcpPort}/FriendshipNotificationService";
    public static readonly string LobbyInvitationServiceAddress = $"net.tcp://localhost:{TcpPort}/LobbyInvitationService";
    public static readonly string BlockedPlayerServiceAddress = $"net.tcp://localhost:{TcpPort}/BlockedPlayerService";
    public static readonly string ChatServiceAddress = $"net.tcp://localhost:{TcpPort}/ChatService";
    public static readonly string LobbyServiceAddress = $"net.tcp://localhost:{TcpPort}/LobbyService";
}
```

## File: src/Shared/Vertigo.Contracts/Faults/ServiceErrorCodes.cs
```csharp
namespace Vertigo.Contracts.Faults;

public static class ServiceErrorCodes
{
    public const string UsernameAlreadyRegistered = "USERNAME_ALREADY_REGISTERED";
    public const string EmailAlreadyRegistered = "EMAIL_ALREADY_REGISTERED";
    public const string InvalidRegistrationData = "INVALID_REGISTRATION_DATA";
    public const string LobbyNotFound = "ROOM_NOT_FOUND";
    public const string LobbyFull = "ROOM_FULL";
    public const string LobbyAlreadyStarted = "MSG-ROOM-009";
    public const string InvalidLobbyCapacity = "INVALID_ROOM_CAPACITY";
    public const string InvalidData = "INVALID_DATA";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string InvalidCode = "INVALID_CODE";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string SessionAlreadyActive = "SESSION_ALREADY_ACTIVE";
    public const string FriendRequestAlreadySent = "FRIEND_REQUEST_ALREADY_SENT";
    public const string AlreadyFriends = "ALREADY_FRIENDS";
    public const string FriendshipBlocked = "FRIENDSHIP_BLOCKED";
    public const string FriendRequestNotFound = "FRIEND_REQUEST_NOT_FOUND";
    public const string LobbyInviteNotAllowed = "LOBBY_INVITE_NOT_ALLOWED";
    
    public const string Unauthorized = "UNAUTHORIZED";
    public const string LobbyChatNotAllowed = "LOBBY_CHAT_NOT_ALLOWED";
}
```

## File: src/Shared/Vertigo.Contracts/Faults/ServiceMessageKeys.cs
```csharp
namespace Vertigo.Contracts.Faults;

public static class ServiceMessageKeys
{
    public const string UsernameAlreadyRegistered = "RegisterAccount.UsernameAlreadyRegistered";
    public const string EmailAlreadyRegistered = "RegisterAccount.EmailAlreadyRegistered";
    public const string InvalidRegistrationData = "RegisterAccount.ValidationSummary";
    public const string LobbyNotFound = "ROOM_NOT_FOUND";
    public const string LobbyFull = "ROOM_FULL";    
    public const string LobbyAlreadyStarted = "MSG-ROOM-009";
    public const string InvalidLobbyCapacity = "MSG-ROOM-011";
    public const string InvalidData = "ErrorInvalidData";
    public const string UserNotFound = "ErrorUserNotFound";
    public const string InvalidCode = "ErrorInvalidCode";
    public const string InvalidCredentials = "ErrorInvalidCredentials";
    public const string SessionAlreadyActive = "AccessMenu.SessionAlreadyActive";
    public const string FriendRequestAlreadySent = "Friends.RequestSent";
    public const string AlreadyFriends = "Friends.AlreadyFriends";
    public const string FriendshipBlocked = "Friends.RequestBlocked";
    public const string FriendRequestNotFound = "Notifications.FriendRequestNotFound";
    public const string LobbyInviteNotAllowed = "InviteFriends.NotAllowed";
    
    public const string Unauthorized = "Authentication.Unauthorized";
    public const string LobbyChatNotAllowed = "Lobby.ChatNotAllowed";
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IAccountService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IAccountService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RegisterAccount(AccountRegistration registration);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool ChangePassword(PasswordChange change, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IAuthService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IAuthService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    UserSession Login(LoginCredentials credentials);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    UserSession ResumeSession(string token);

    [OperationContract]
    UserSession CreateGuestSession();

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool Logout(SessionLogout logout);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IBlockedPlayerService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IBlockedPlayerService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<BlockedPlayerSummary> GetBlockedPlayers(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool BlockPlayer(Guid otherPlayerId, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool UnblockPlayer(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IChatService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IChatService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool SendMessage(string lobbyCode, string text, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IFriendshipNotificationService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract(CallbackContract = typeof(IFriendshipCallback), SessionMode = SessionMode.Required)]
public interface IFriendshipNotificationService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool Subscribe(string sessionToken);

    [OperationContract(IsOneWay = true)]
    void Unsubscribe();
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IFriendshipService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IFriendshipService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendEntry> GetFriends(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<PlayerSearchEntry> SearchPlayers(string searchTerm, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    PlayerRelation SendFriendRequest(Guid targetPlayerId, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendshipInvitationEntry> GetPendingFriendRequests(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RemoveFriendship(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/ILobbyInvitationService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface ILobbyInvitationService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendEntry> GetInvitableFriends(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<Guid> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<LobbyInvitationEntry> GetPendingLobbyInvitations(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool DeclineLobbyInvitation(string lobbyCode, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/ILobbyService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract(CallbackContract = typeof(ILobbyCallback), SessionMode = SessionMode.Required)]
public interface ILobbyService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool LeaveLobby(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<LobbySummary> GetPublicLobbies();
}
```

## File: src/Shared/Vertigo.Contracts/Internal/IFriendshipNotifier.cs
```csharp
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface IFriendshipNotifier
{
    void Subscribe(Guid playerId, IFriendshipCallback callback, string sessionId);

    void Unsubscribe(Guid playerId, string sessionId);

    void UnsubscribeSession(string sessionId);

    void NotifyFriendRequestReceived(Guid targetPlayerId, FriendshipInvitationEntry request);

    void NotifyFriendshipEstablished(Guid targetPlayerId, FriendEntry friend);

    void NotifyLobbyInvitationReceived(Guid targetPlayerId, LobbyInvitationEntry invitation);

    void NotifyLobbyInvitationWithdrawn(Guid targetPlayerId, string lobbyCode);

    bool IsOnline(Guid playerId);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/ILobbyDirectory.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface ILobbyDirectory
{
    LobbyOverview? FindLobby(string lobbyCode);

    bool IsInAnyLobby(Guid playerId);

    bool RegisterInvitation(string lobbyCode, Guid targetPlayerId, Guid senderPlayerId);

    bool CancelInvitation(string lobbyCode, Guid targetPlayerId);

    List<LobbyOverview> FindLobbiesInviting(Guid playerId);

    bool PublishChatMessage(LobbyChatMessage message);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/LobbyOverview.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public sealed record LobbyOverview(
    string LobbyCode,
    Guid HostPlayerId,
    int MaxPlayers,
    bool IsWaiting,
    List<LobbyMember> Members,
    Dictionary<Guid, Guid> Invitations);
```

## File: src/Shared/Vertigo.Contracts/Models/AccountRegistration.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class AccountRegistration
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Email { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/BlockedPlayerSummary.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class BlockedPlayerSummary
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public DateTime BlockedAt { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendEntry
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendshipDecision.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipDecision
{
    [DataMember]
    public Guid ResponderId { get; set; }

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public bool Accept { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendshipInvitation.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipInvitation
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public Guid TargetId { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendshipInvitationEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipInvitationEntry
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/InvitableFriendsQuery.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class InvitableFriendsQuery
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyAdmission.cs
```csharp
#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyAdmission
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid HostPlayerId { get; set; }

    [DataMember]
    public bool IsPublic { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }

    [DataMember]
    public List<LobbyMember> ExistingMembers { get; set; } = new();
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyChatMessage.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class LobbyChatMessage
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;

    [DataMember]
    public string Text { get; set; } = string.Empty;

    [DataMember]
    public DateTime SentAt { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyInvitationDecline.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyInvitationDecline
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyInvitationEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyInvitationEntry
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;

    [DataMember]
    public int CurrentPlayers { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyInvitations.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyInvitations
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public List<Guid> TargetIds { get; set; } = new();
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyMember.cs
```csharp
#nullable enable

using System;
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyMember
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public bool IsGuest { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbySummary.cs
```csharp
#nullable enable

using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbySummary
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public string HostUsername { get; set; } = string.Empty;

    [DataMember]
    public int CurrentPlayers { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LoginCredentials.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LoginCredentials
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/PasswordChange.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models
{
    [DataContract]
    public class PasswordChange
    {
        [DataMember] 
        public string Username { get; set; } = string.Empty;
        
        [DataMember] 
        public string CurrentPassword { get; set; } = string.Empty;
        
        [DataMember] 
        public string NewPassword { get; set; } = string.Empty;
    }
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerPair.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerPair
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public Guid OtherPlayerId { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerRelation.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum PlayerRelation
{
    [EnumMember]
    None,

    [EnumMember]
    RequestSent,

    [EnumMember]
    Friends,

    [EnumMember]
    Blocked
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerSearch.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerSearch
{
    [DataMember]
    public Guid RequesterId { get; set; }

    [DataMember]
    public string SearchTerm { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerSearchEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerSearchEntry
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public PlayerRelation Relation { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/ServiceFault.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class ServiceFault
{
    [DataMember]
    public string ErrorCode { get; set; } = string.Empty;

    [DataMember]
    public string MessageKey { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/SessionLogout.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class SessionLogout
{
    [DataMember]
    public string SessionToken { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/UserSession.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class UserSession
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Token { get; set; } = string.Empty;

    [DataMember]
    public DateTime ExpiresAt { get; set; }

    [DataMember]
    public bool IsGuest { get; set; }
}
```

## File: tests/Vertigo.Client.Networking.Smoke/Program.cs
```csharp
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
```

## File: tests/Vertigo.Client.Networking.Smoke/Vertigo.Client.Networking.Smoke.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
  </ItemGroup>
</Project>
```

## File: src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Client.Core\Vertigo.Client.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="System.ServiceModel.NetTcp" Version="10.1.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.1" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Client/Vertigo.Client.UI/ViewModels/MainViewModel.cs
```csharp
using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UI.ViewModels;

public sealed class MainViewModel : ViewModelBase, IDisposable
{
    private readonly INavigator _navigation;
    private readonly PlayerSession _playerSession;
    private readonly HomeViewModel _home;
    private bool _disposed;
    private NavigationPage _previousPage;

    public MainViewModel(INavigator navigation, AccessViewModel access, MainContext context)
    {
        _navigation = navigation;
        _playerSession = context.PlayerSession;
        Access = access;
        Home = context.Home;
        Lobby = context.Lobby;
        _home = context.Home;
        _previousPage = _navigation.CurrentPage;
        _navigation.PageChanged += OnPageChanged;
        if (_playerSession.IsAuthenticated) _navigation.NavigateTo(NavigationPage.Home);
        else Access.OnNavigatedTo();
    }

    public AccessViewModel Access { get; }
    public HomeViewModel Home { get; }
    public LobbyViewModel Lobby { get; }

    public NavigationPage CurrentPage => _navigation.CurrentPage;
    public object CurrentViewModel => CurrentPage switch
    {
        NavigationPage.Home => Home,
        NavigationPage.Lobby => Lobby,
        _ => Access
    };

    // Temporary binding for the current placeholder MainWindow; not the final UI.
    public string Greeting => $"Vértigo — {CurrentPage}";

    private void OnPageChanged(object? sender, NavigationPage page)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(() => OnPageChanged(sender, page));
            return;
        }
        if (_disposed) return;
        if (page != NavigationPage.Access && !_playerSession.IsAuthenticated)
        {
            _navigation.NavigateTo(NavigationPage.Access);
            return;
        }

        GetPage(_previousPage)?.OnNavigatedFrom();
        _previousPage = page;
        if (page == NavigationPage.Access) Lobby.ResetState();
        GetPage(page)?.OnNavigatedTo();
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(CurrentViewModel));
        OnPropertyChanged(nameof(Greeting));
    }

    private INavigationAware? GetPage(NavigationPage page)
    {
        return page switch
        {
            NavigationPage.Access => Access,
            NavigationPage.Home => Home,
            NavigationPage.Lobby => Lobby,
            _ => null
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _navigation.PageChanged -= OnPageChanged;
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/ViewModelBase.cs
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace Vertigo.Client.UI.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
}
```

## File: src/Client/Vertigo.Client.UI/Views/MainWindow.axaml
```
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:Vertigo.Client.UI.ViewModels"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="Vertigo.Client.UI.Views.MainWindow"
        x:DataType="vm:MainViewModel"
        Icon="/Assets/avalonia-logo.ico"
        Title="Vertigo.Client.UI">

    <Design.DataContext>
        <!-- This only sets the DataContext for the previewer in an IDE,
             to set the actual DataContext for runtime, set the DataContext property in code (look at App.axaml.cs) -->
        <vm:MainViewModel />
    </Design.DataContext>

    <TextBox Text="{Binding Greeting}"
             HorizontalAlignment="Center"
             VerticalAlignment="Center" />

</Window>
```

## File: src/Client/Vertigo.Client.UI/Views/MainWindow.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/App.axaml
```
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="Vertigo.Client.UI.App"
             xmlns:local="using:Vertigo.Client.UI"
             RequestedThemeVariant="Default">
             <!-- "Default" ThemeVariant follows system theme variant. "Dark" or "Light" are other available options. -->

    <Application.DataTemplates>
        <local:ViewLocator />
    </Application.DataTemplates>

    <Application.Styles>
        <FluentTheme />
    </Application.Styles>
</Application>
```

## File: src/Client/Vertigo.Client.UI/App.axaml.cs
```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.Views;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Services;

namespace Vertigo.Client.UI;

public partial class App : Application
{
    private ServiceProvider? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServiceCollection registrations = new();
            registrations.AddVertigoClientNetworking();
            registrations.AddSingleton<ITextLocalizer, TextLocalizer>();
            registrations.AddSingleton<IClientErrorText, ClientErrorText>();
            registrations.AddSingleton<ISessionGuard, SessionGuard>();
            registrations.AddTransient<UiServices>();
            registrations.AddTransient<UiNavigationContext>();
            registrations.AddTransient<AccessContext>();
            registrations.AddTransient<LobbyContext>();
            registrations.AddTransient<HomeContext>();
            registrations.AddTransient<MainContext>();
            registrations.AddSingleton<AccessViewModel>();
            registrations.AddSingleton<HomeViewModel>();
            registrations.AddSingleton<LobbyViewModel>();
            registrations.AddSingleton<MainViewModel>();
            _services = registrations.BuildServiceProvider();
            desktop.MainWindow = new MainWindow
            {
                DataContext = _services.GetRequiredService<MainViewModel>()
            };
            desktop.Exit += OnDesktopExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs eventArgs)
    {
        _services?.Dispose();
        _services = null;
    }
}
```

## File: src/Client/Vertigo.Client.UI/app.manifest
```
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <!-- This manifest is used on Windows only.
       Don't remove it as it might cause problems with window transparency and embedded controls.
       For more details visit https://learn.microsoft.com/en-us/windows/win32/sbscs/application-manifests -->
  <assemblyIdentity version="1.0.0.0" name="Vertigo.Client.UI.Desktop"/>

  <compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
      <!-- A list of the Windows versions that this application has been tested on
           and is designed to work with. Uncomment the appropriate elements
           and Windows will automatically select the most compatible environment. -->

      <!-- Windows 10 -->
      <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
    </application>
  </compatibility>
</assembly>
```

## File: src/Client/Vertigo.Client.UI/Program.cs
```csharp
using Avalonia;
using System;

namespace Vertigo.Client.UI;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
```

## File: src/Client/Vertigo.Client.UI/Vertigo.Client.UI.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ApplicationManifest>app.manifest</ApplicationManifest>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <Folder Include="Models\" />
    <AvaloniaResource Include="Assets\**" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.1.6" />
    <PackageReference Include="Avalonia.Desktop" Version="12.1.6" />
    <PackageReference Include="Avalonia.Themes.Fluent" Version="12.1.6" />
    <PackageReference Include="Avalonia.Fonts.Inter" Version="12.1.6" />
    <PackageReference Include="AvaloniaUI.DiagnosticsSupport" Version="2.2.3">
      <IncludeAssets Condition="'$(Configuration)' != 'Debug'">None</IncludeAssets>
      <PrivateAssets Condition="'$(Configuration)' != 'Debug'">All</PrivateAssets>
    </PackageReference>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" />
  </ItemGroup>
</Project>
```

## File: src/Client/Vertigo.Client.UI/ViewLocator.cs
```csharp
using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Vertigo.Client.UI.ViewModels;

namespace Vertigo.Client.UI;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Implementations/PasswordHasher.cs
```csharp
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
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Implementations/TokenGenerator.cs
```csharp
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
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Interfaces/IPasswordHasher.cs
```csharp
namespace Vertigo.Server.CrossCutting.Security.Interfaces;

public interface IPasswordHasher
{
    string GenerateSalt();

    string HashPassword(string password, string passwordSalt);

    bool VerifyPassword(string password, string passwordHash, string passwordSalt);
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Interfaces/ITokenGenerator.cs
```csharp
namespace Vertigo.Server.CrossCutting.Security.Interfaces;

public interface ITokenGenerator
{
    string GenerateSessionToken();

    string GenerateVerificationCode();

    string HashSessionToken(string sessionToken);
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Vertigo.Server.CrossCutting.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.DataAccess/Contexts/VertigoDatabaseContext.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Game;
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Contexts;

public class VertigoDatabaseContext : DbContext
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerStats> PlayerStats => Set<PlayerStats>();
    public DbSet<SessionToken> SessionTokens => Set<SessionToken>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<BlockedPlayer> BlockedPlayers => Set<BlockedPlayer>();
    public DbSet<MatchHistory> MatchHistory => Set<MatchHistory>();

    public VertigoDatabaseContext(DbContextOptions<VertigoDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigurePlayers(modelBuilder);
        ConfigurePlayerStats(modelBuilder);
        ConfigureSessionTokens(modelBuilder);
        ConfigureFriendships(modelBuilder);
        ConfigureBlockedPlayers(modelBuilder);
        ConfigureMatchHistory(modelBuilder);
    }

    private static void ConfigurePlayers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(player =>
        {
            player.ToTable("Players");
            player.HasKey(p => p.PlayerId);
            player.Property(p => p.PlayerId).HasDefaultValueSql("NEWID()");
            player.Property(p => p.Username).HasMaxLength(32).IsRequired();
            player.Property(p => p.Email).HasMaxLength(120).IsRequired();
            player.Property(p => p.PasswordHash).HasMaxLength(256).IsRequired();
            player.Property(p => p.PasswordSalt).HasMaxLength(128).IsRequired();
            player.Property(p => p.IsEmailVerified).HasDefaultValue(false);
            player.Property(p => p.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            player.Property(p => p.LastLoginAt).HasColumnType("datetime2");
            player.HasAlternateKey(p => p.Username);
            player.HasAlternateKey(p => p.Email);
            player.HasIndex(p => p.Username).HasDatabaseName("IX_Players_Username");
        });
    }

    private static void ConfigurePlayerStats(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerStats>(stats =>
        {
            stats.ToTable("PlayerStats");
            stats.HasKey(s => s.PlayerId);
            stats.Property(s => s.MatchesPlayed).HasDefaultValue(0);
            stats.Property(s => s.MatchesWon).HasDefaultValue(0);
            stats.Property(s => s.TotalFloorsBuilt).HasDefaultValue(0);
            stats.Property(s => s.HighestSkyscraper).HasDefaultValue(0);
            stats.HasIndex(s => new { s.MatchesWon, s.TotalFloorsBuilt })
                .IsDescending(true, true).HasDatabaseName("IX_PlayerStats_Ranking");
            stats.HasOne(s => s.Player).WithOne(p => p.Stats)
                .HasForeignKey<PlayerStats>(s => s.PlayerId)
                .HasConstraintName("FK_PlayerStats_Players")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSessionTokens(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SessionToken>(token =>
        {
            token.ToTable("SessionTokens");
            token.HasKey(t => t.TokenId);
            token.Property(t => t.TokenId).HasDefaultValueSql("NEWID()");
            token.Property(t => t.TokenValue).HasMaxLength(128).IsRequired();
            token.Property(t => t.ExpiresAt).HasColumnType("datetime2");
            token.Property(t => t.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            token.Property(t => t.IsRevoked).HasDefaultValue(false);
            token.HasAlternateKey(t => t.TokenValue);
            token.HasIndex(t => t.TokenValue).HasDatabaseName("IX_SessionTokens_TokenValue");
            token.HasOne(t => t.Player).WithMany(p => p.SessionTokens)
                .HasForeignKey(t => t.PlayerId)
                .HasConstraintName("FK_SessionTokens_Players")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureFriendships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>(friendship =>
        {
            friendship.ToTable("Friendships", table => table.HasCheckConstraint(
                "CK_Friendship_NotSelf", "[RequesterId] <> [AddresseeId]"));
            friendship.HasKey(f => f.FriendshipId);
            friendship.Property(f => f.FriendshipId).HasDefaultValueSql("NEWID()");
            friendship.Property(f => f.PlayerRequesterId).HasColumnName("RequesterId");
            friendship.Property(f => f.PlayerAddresseeId).HasColumnName("AddresseeId");
            friendship.Property(f => f.Status).HasConversion<byte>().HasColumnType("tinyint")
                .HasDefaultValue(FriendshipStatus.Pending);
            friendship.Property(f => f.CreatedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            friendship.Property(f => f.UpdatedAt).HasColumnType("datetime");
            friendship.HasAlternateKey(f => new { f.PlayerRequesterId, f.PlayerAddresseeId })
                .HasName("UQ_Friendship_Pair");
            friendship.HasOne(f => f.PlayerRequester).WithMany(p => p.SentFriendships)
                .HasForeignKey(f => f.PlayerRequesterId)
                .HasConstraintName("FK_Friendships_Requester")
                .OnDelete(DeleteBehavior.NoAction);
            friendship.HasOne(f => f.PlayerAddressee).WithMany(p => p.ReceivedFriendships)
                .HasForeignKey(f => f.PlayerAddresseeId)
                .HasConstraintName("FK_Friendships_Addressee")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureBlockedPlayers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlockedPlayer>(blocked =>
        {
            blocked.ToTable("BlockedPlayers", table => table.HasCheckConstraint(
                "CK_Blocked_NotSelf", "[BlockerId] <> [BlockedId]"));
            blocked.HasKey(b => b.BlockId);
            blocked.Property(b => b.BlockId).HasDefaultValueSql("NEWID()");
            blocked.Property(b => b.PlayerBlockerId).HasColumnName("BlockerId");
            blocked.Property(b => b.PlayerBlockedId).HasColumnName("BlockedId");
            blocked.Property(b => b.CreatedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            blocked.HasAlternateKey(b => new { b.PlayerBlockerId, b.PlayerBlockedId })
                .HasName("UQ_Blocked_Pair");
            blocked.HasOne(b => b.PlayerBlocker).WithMany(p => p.BlockedPlayers)
                .HasForeignKey(b => b.PlayerBlockerId)
                .HasConstraintName("FK_BlockedPlayers_Blocker")
                .OnDelete(DeleteBehavior.NoAction);
            blocked.HasOne(b => b.PlayerBlocked).WithMany(p => p.BlockedByPlayers)
                .HasForeignKey(b => b.PlayerBlockedId)
                .HasConstraintName("FK_BlockedPlayers_Blocked")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureMatchHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchHistory>(match =>
        {
            match.ToTable("MatchHistory");
            match.HasKey(m => m.MatchId);
            match.Property(m => m.MatchId).HasDefaultValueSql("NEWID()");
            match.Property(m => m.PlayedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            match.Property(m => m.RoundsPlayed).HasDefaultValue(3);
            match.Property(m => m.WinningHeight).HasDefaultValue(0);
            match.HasOne(m => m.Winner).WithMany(p => p.WonMatches)
                .HasForeignKey(m => m.WinnerId)
                .HasConstraintName("FK_MatchHistory_Winner")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Enums/FriendshipStatus.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Enums;

public enum FriendshipStatus : byte
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Game/MatchHistory.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Entities.Game;

public class MatchHistory
{
    public Guid MatchId { get; set; }
    public DateTime PlayedAt { get; set; }
    public int RoundsPlayed { get; set; }
    public Guid? WinnerId { get; set; }
    public int WinningHeight { get; set; }

    public Player? Winner { get; set; }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/BlockedPlayer.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Identity;

public class BlockedPlayer
{
    public Guid BlockId { get; set; }
    public Guid PlayerBlockerId { get; set; }
    public Guid PlayerBlockedId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Player PlayerBlocker { get; set; } = null!;
    public Player PlayerBlocked { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/Friendship.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Enums;

namespace Vertigo.Server.DataAccess.Entities.Identity;

public class Friendship
{
    public Guid FriendshipId { get; set; }
    public Guid PlayerRequesterId { get; set; }
    public Guid PlayerAddresseeId { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Player PlayerRequester { get; set; } = null!;
    public Player PlayerAddressee { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/Player.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Game;

namespace Vertigo.Server.DataAccess.Entities.Identity;

public class Player
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public int? AvatarId { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public PlayerStats? Stats { get; set; }
    public ICollection<SessionToken> SessionTokens { get; set; } = new List<SessionToken>();
    public ICollection<Friendship> SentFriendships { get; set; } = new List<Friendship>();
    public ICollection<Friendship> ReceivedFriendships { get; set; } = new List<Friendship>();
    public ICollection<BlockedPlayer> BlockedPlayers { get; set; } = new List<BlockedPlayer>();
    public ICollection<BlockedPlayer> BlockedByPlayers { get; set; } = new List<BlockedPlayer>();
    public ICollection<MatchHistory> WonMatches { get; set; } = new List<MatchHistory>();
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/PlayerStats.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Identity;

public class PlayerStats
{
    public Guid PlayerId { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int TotalFloorsBuilt { get; set; }
    public int HighestSkyscraper { get; set; }

    public Player Player { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/SessionToken.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Identity;

public class SessionToken
{
    public Guid TokenId { get; set; }
    public Guid PlayerId { get; set; }
    public string TokenValue { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }

    public Player Player { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/BlockedPlayerRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class BlockedPlayerRepository : IBlockedPlayerRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public BlockedPlayerRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public List<BlockedPlayer> GetBlockedPlayers(Guid playerId)
    {
        List<BlockedPlayer> blockedPlayers = _context.BlockedPlayers
            .AsNoTracking()
            .Include(b => b.PlayerBlocked)
            .Where(b => b.PlayerBlockerId == playerId)
            .OrderBy(b => b.PlayerBlocked.Username)
            .ToList();

        return blockedPlayers;
    }

    public bool IsPlayerBlocked(Guid playerId, Guid otherPlayerId)
    {
        bool isBlocked = _context.BlockedPlayers
            .Any(b => (b.PlayerBlockerId == playerId && b.PlayerBlockedId == otherPlayerId)
                      || (b.PlayerBlockerId == otherPlayerId && b.PlayerBlockedId == playerId));

        return isBlocked;
    }

    public bool BlockPlayer(Guid playerId, Guid otherPlayerId)
    {
        BlockedPlayer blockedPlayer = new BlockedPlayer
        {
            PlayerBlockerId = playerId,
            PlayerBlockedId = otherPlayerId
        };

        using IDbContextTransaction transaction = _context.Database.BeginTransaction();

        _context.Friendships
            .Where(friendship => (friendship.PlayerRequesterId == playerId
                                  && friendship.PlayerAddresseeId == otherPlayerId)
                                 || (friendship.PlayerRequesterId == otherPlayerId
                                     && friendship.PlayerAddresseeId == playerId))
            .ExecuteDelete();

        _context.BlockedPlayers.Add(blockedPlayer);
        int affectedRows = _context.SaveChanges();
        transaction.Commit();

        return affectedRows == ExpectedAffectedRows;
    }

    public bool UnblockPlayer(Guid playerId, Guid otherPlayerId)
    {
        int affectedRows = _context.BlockedPlayers
            .Where(b => b.PlayerBlockerId == playerId && b.PlayerBlockedId == otherPlayerId)
            .ExecuteDelete();

        return affectedRows == ExpectedAffectedRows;
    }

    public List<BlockedPlayer> GetBlocksBetween(Guid playerId, List<Guid> otherPlayerIds)
    {
        return _context.BlockedPlayers.AsNoTracking()
            .Where(block => (block.PlayerBlockerId == playerId && otherPlayerIds.Contains(block.PlayerBlockedId))
                            || (block.PlayerBlockedId == playerId && otherPlayerIds.Contains(block.PlayerBlockerId)))
            .ToList();
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/FriendshipRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class FriendshipRepository : IFriendshipRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public FriendshipRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public Friendship? GetFriendshipBetween(Guid playerId, Guid otherPlayerId)
    {
        Friendship? friendship = _context.Friendships
            .AsNoTracking()
            .FirstOrDefault(f => (f.PlayerRequesterId == playerId && f.PlayerAddresseeId == otherPlayerId)
                                 || (f.PlayerRequesterId == otherPlayerId && f.PlayerAddresseeId == playerId));

        return friendship;
    }

    public List<Friendship> GetFriendshipsBetween(Guid playerId, List<Guid> otherPlayerIds)
    {
        List<Friendship> friendships = _context.Friendships
            .AsNoTracking()
            .Where(f => (f.PlayerRequesterId == playerId && otherPlayerIds.Contains(f.PlayerAddresseeId))
                        || (f.PlayerAddresseeId == playerId && otherPlayerIds.Contains(f.PlayerRequesterId)))
            .ToList();

        return friendships;
    }

    public List<Friendship> GetAcceptedFriendships(Guid playerId)
    {
        List<Friendship> friendships = _context.Friendships
            .AsNoTracking()
            .Include(f => f.PlayerRequester)
            .Include(f => f.PlayerAddressee)
            .Where(f => (f.PlayerRequesterId == playerId || f.PlayerAddresseeId == playerId)
                        && f.Status == FriendshipStatus.Accepted)
            .ToList();

        return friendships;
    }

    public List<Friendship> GetPendingRequestsReceivedBy(Guid addresseeId)
    {
        List<Friendship> friendships = _context.Friendships
            .AsNoTracking()
            .Include(f => f.PlayerRequester)
            .Where(f => f.PlayerAddresseeId == addresseeId && f.Status == FriendshipStatus.Pending)
            .OrderBy(f => f.PlayerRequester.Username)
            .ToList();

        return friendships;
    }

    public bool CreatePendingFriendship(Guid requesterId, Guid addresseeId)
    {
        Friendship friendship = new Friendship
        {
            PlayerRequesterId = requesterId,
            PlayerAddresseeId = addresseeId,
            Status = FriendshipStatus.Pending
        };

        _context.Friendships.Add(friendship);
        int affectedRows = _context.SaveChanges();

        return affectedRows == ExpectedAffectedRows;
    }

    public bool AcceptPendingFriendship(Guid requesterId, Guid addresseeId)
    {
        int affectedRows = _context.Friendships
            .Where(f => f.PlayerRequesterId == requesterId
                        && f.PlayerAddresseeId == addresseeId
                        && f.Status == FriendshipStatus.Pending)
            .ExecuteUpdate(setters => setters.SetProperty(f => f.Status, FriendshipStatus.Accepted));

        return affectedRows == ExpectedAffectedRows;
    }

    public bool DeletePendingFriendship(Guid requesterId, Guid addresseeId)
    {
        int affectedRows = _context.Friendships
            .Where(f => f.PlayerRequesterId == requesterId
                        && f.PlayerAddresseeId == addresseeId
                        && f.Status == FriendshipStatus.Pending)
            .ExecuteDelete();

        return affectedRows == ExpectedAffectedRows;
    }
    public bool DeleteFriendshipBetween(Guid playerId, Guid otherPlayerId)
    {
        int affectedRows = _context.Friendships
            .Where(friendship => (friendship.PlayerRequesterId == playerId
                                  && friendship.PlayerAddresseeId == otherPlayerId)
                                 || (friendship.PlayerRequesterId == otherPlayerId
                                     && friendship.PlayerAddresseeId == playerId))
            .ExecuteDelete();

        return affectedRows == ExpectedAffectedRows;
    }

}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/PlayerRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class PlayerRepository : IPlayerRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public PlayerRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public Player? GetPlayerById(Guid playerId)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.PlayerId == playerId);

        return player;
    }

    public Player? GetPlayerByUsername(string username)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.Username == username);

        return player;
    }

    public Player? GetPlayerByEmail(string email)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.Email == email);

        return player;
    }

    public bool UsernameExists(string username)
    {
        bool exists = _context.Players.Any(p => p.Username == username);

        return exists;
    }

    public bool EmailExists(string email)
    {
        bool exists = _context.Players.Any(p => p.Email == email);

        return exists;
    }

    public bool CreatePlayer(Player player)
    {
        _context.Players.Add(player);
        int affectedRows = _context.SaveChanges();

        return affectedRows >= ExpectedAffectedRows;
    }

    public bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt)
    {
        int affectedRows = _context.Players
            .Where(p => p.PlayerId == playerId)
            .ExecuteUpdate(setters => setters.SetProperty(p => p.LastLoginAt, lastLoginAt));

        return affectedRows == ExpectedAffectedRows;
    }

    public List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults)
    {
        return _context.Players.AsNoTracking()
            .Where(player => player.PlayerId != excludedPlayerId && player.Username.Contains(searchTerm))
            .OrderBy(player => player.Username)
            .Take(maximumResults)
            .ToList();
    }

    public bool UpdatePassword(Guid playerId, string passwordHash, string passwordSalt)
    {
        int affectedRows = _context.Players
            .Where(player => player.PlayerId == playerId)
            .ExecuteUpdate(setters => setters
                .SetProperty(player => player.PasswordHash, passwordHash)
                .SetProperty(player => player.PasswordSalt, passwordSalt));

        return affectedRows == ExpectedAffectedRows;
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/SessionTokenRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class SessionTokenRepository : ISessionTokenRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public SessionTokenRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public bool CreateSessionToken(SessionToken token)
    {
        _context.SessionTokens.Add(token);
        int affectedRows = _context.SaveChanges();

        return affectedRows == ExpectedAffectedRows;
    }

    public SessionToken? GetActiveSessionToken(string tokenHash)
    {
        DateTime currentTime = DateTime.UtcNow;
        SessionToken? token = _context.SessionTokens
            .AsNoTracking()
            .Include(session => session.Player)
            .FirstOrDefault(session => session.TokenValue == tokenHash
                                       && !session.IsRevoked
                                       && session.ExpiresAt > currentTime);

        return token;
    }

    public bool RevokeSessionToken(string tokenHash)
    {
        int affectedRows = _context.SessionTokens
            .Where(session => session.TokenValue == tokenHash && !session.IsRevoked)
            .ExecuteUpdate(setters => setters.SetProperty(session => session.IsRevoked, true));

        return affectedRows == ExpectedAffectedRows;
    }

    public int RevokePlayerSessions(Guid playerId)
    {
        int affectedRows = _context.SessionTokens
            .Where(session => session.PlayerId == playerId && !session.IsRevoked)
            .ExecuteUpdate(setters => setters.SetProperty(session => session.IsRevoked, true));

        return affectedRows;
    }

}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/IBlockedPlayerRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IBlockedPlayerRepository
{
    List<BlockedPlayer> GetBlocksBetween(Guid playerId, List<Guid> otherPlayerIds);
    List<BlockedPlayer> GetBlockedPlayers(Guid playerId);
    bool IsPlayerBlocked(Guid playerId, Guid otherPlayerId);
    bool BlockPlayer(Guid playerId, Guid otherPlayerId);
    bool UnblockPlayer(Guid playerId, Guid otherPlayerId);
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/IFriendshipRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IFriendshipRepository
{
    Friendship? GetFriendshipBetween(Guid playerId, Guid otherPlayerId);
    List<Friendship> GetFriendshipsBetween(Guid playerId, List<Guid> otherPlayerIds);
    List<Friendship> GetAcceptedFriendships(Guid playerId);
    List<Friendship> GetPendingRequestsReceivedBy(Guid addresseeId);
    bool CreatePendingFriendship(Guid requesterId, Guid addresseeId);
    bool AcceptPendingFriendship(Guid requesterId, Guid addresseeId);
    bool DeleteFriendshipBetween(Guid playerId, Guid otherPlayerId);
    bool DeletePendingFriendship(Guid requesterId, Guid addresseeId);
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/IPlayerRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IPlayerRepository
{
    Player? GetPlayerById(Guid playerId);
    Player? GetPlayerByUsername(string username);
    Player? GetPlayerByEmail(string email);
    bool UsernameExists(string username);
    bool EmailExists(string email);
    bool CreatePlayer(Player player);
    List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults);
    bool UpdatePassword(Guid playerId, string passwordHash, string passwordSalt);
    bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt);
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/ISessionTokenRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface ISessionTokenRepository
{
    bool CreateSessionToken(SessionToken token);

    SessionToken? GetActiveSessionToken(string tokenHash);

    bool RevokeSessionToken(string tokenHash);

    int RevokePlayerSessions(Guid playerId);
}
```

## File: src/Server/Vertigo.Server.DataAccess/DependencyInjection.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Repositories.Implementations;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddVertigoDataAccess(
        this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("VertigoDb")
            ?? throw new InvalidOperationException("Connection string 'VertigoDb' is missing.");

        services.AddDbContext<VertigoDatabaseContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IBlockedPlayerRepository, BlockedPlayerRepository>();
        services.AddScoped<ISessionTokenRepository, SessionTokenRepository>();

        return services;
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Vertigo.Server.DataAccess.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="11.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="11.0.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.Game/Vertigo.Server.Game.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.Host/Program.cs
```csharp
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.DataAccess;
using CoreWCF;
using CoreWCF.Configuration;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.Matchmaking.Invitations.Dependencies;
using Vertigo.Server.Matchmaking.Invitations.Services;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Services;
using Vertigo.Server.PlayerManagement.Friendships.Dependencies;
using Vertigo.Server.PlayerManagement.Friendships.Services;
using Vertigo.Server.PlayerManagement.Registration.Services;
using Vertigo.Server.PlayerManagement.Blocking.Services;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.Matchmaking.Chat.Services;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseNetTcp(IPAddress.Loopback, ServiceEndpoints.TcpPort);
builder.Services.AddServiceModelServices();
builder.Services.AddVertigoDataAccess(builder.Configuration);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

builder.Services.AddSingleton<IGuestSessionRegistry, GuestSessionRegistry>();
builder.Services.AddScoped<ISessionIdentityResolver, SessionIdentityResolver>();
builder.Services.AddTransient<AccountServiceDependencies>();
builder.Services.AddTransient<AuthenticationDependencies>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddTransient<AccountService>();
builder.Services.AddTransient<BlockedPlayerService>();
builder.Services.AddSingleton<IFriendshipNotifier, FriendshipNotifier>();
builder.Services.AddTransient<FriendshipServiceDependencies>();
builder.Services.AddTransient<FriendshipService>();
builder.Services.AddSingleton<FriendshipNotificationService>();
builder.Services.AddSingleton<LobbyService>();
builder.Services.AddSingleton<ILobbyDirectory>(services => services.GetRequiredService<LobbyService>());
builder.Services.AddTransient<LobbyInvitationServiceDependencies>();
builder.Services.AddTransient<LobbyInvitationService>();
builder.Services.AddTransient<ChatService>();

WebApplication app = builder.Build();
((IApplicationBuilder)app).UseServiceModel(ConfigureServices);
app.Run();

static void ConfigureServices(IServiceBuilder services)
{
    NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
    services.AddService<AccountService>()
        .AddServiceEndpoint<AccountService, IAccountService>(binding, ServiceEndpoints.AccountServiceAddress);
    services.AddService<AuthenticationService>()
        .AddServiceEndpoint<AuthenticationService, IAuthService>(binding, ServiceEndpoints.AuthServiceAddress);
    services.AddService<BlockedPlayerService>()
        .AddServiceEndpoint<BlockedPlayerService, IBlockedPlayerService>(
            binding, ServiceEndpoints.BlockedPlayerServiceAddress);
    services.AddService<FriendshipService>()
        .AddServiceEndpoint<FriendshipService, IFriendshipService>(binding, ServiceEndpoints.FriendshipServiceAddress);
    services.AddService<FriendshipNotificationService>()
        .AddServiceEndpoint<FriendshipNotificationService, IFriendshipNotificationService>(
            binding, ServiceEndpoints.FriendshipNotificationServiceAddress);
    services.AddService<ChatService>()
        .AddServiceEndpoint<ChatService, IChatService>(binding, ServiceEndpoints.ChatServiceAddress);
    services.AddService<LobbyService>()
        .AddServiceEndpoint<LobbyService, ILobbyService>(binding, ServiceEndpoints.LobbyServiceAddress);
    services.AddService<LobbyInvitationService>()
        .AddServiceEndpoint<LobbyInvitationService, ILobbyInvitationService>(
            binding, ServiceEndpoints.LobbyInvitationServiceAddress);
}
```

## File: src/Server/Vertigo.Server.Host/Vertigo.Server.Host.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.PlayerManagement\Vertigo.Server.PlayerManagement.csproj" />
    <ProjectReference Include="..\Vertigo.Server.Matchmaking\Vertigo.Server.Matchmaking.csproj" />
    <ProjectReference Include="..\Vertigo.Server.Game\Vertigo.Server.Game.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="CoreWCF.NetTcp" Version="1.9.1" />
    <PackageReference Include="CoreWCF.Primitives" Version="1.9.1" />
  </ItemGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.Matchmaking/Vertigo.Server.Matchmaking.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
    <ProjectReference Include="..\Vertigo.Server.PlayerManagement\Vertigo.Server.PlayerManagement.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="CoreWCF.Primitives" Version="1.9.1" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Dependencies/AuthenticationDependencies.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Dependencies;

public sealed class AuthenticationDependencies
{
    public IPlayerRepository PlayerRepository { get; }

    public ISessionTokenRepository SessionTokenRepository { get; }

    public IPasswordHasher PasswordHasher { get; }

    public ITokenGenerator TokenGenerator { get; }

    public AuthenticationDependencies(
        IPlayerRepository playerRepository,
        ISessionTokenRepository sessionTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        PlayerRepository = playerRepository;
        SessionTokenRepository = sessionTokenRepository;
        PasswordHasher = passwordHasher;
        TokenGenerator = tokenGenerator;
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Models/AuthenticatedSession.cs
```csharp
namespace Vertigo.Server.PlayerManagement.Authentication.Models;

public sealed class AuthenticatedSession
{
    public Guid PlayerId { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }

    public bool IsGuest { get; init; }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/AuthService.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public class AuthService
{
    private const int SessionDurationDays = 7;

    private readonly IPlayerRepository _playerRepository;
    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IGuestSessionRegistry _guests;
    private readonly TimeProvider _timeProvider;

    public AuthService(AuthenticationDependencies dependencies, IGuestSessionRegistry guests, TimeProvider timeProvider)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _tokenGenerator = dependencies.TokenGenerator;
        _guests = guests;
        _timeProvider = timeProvider;
    }

    public AuthenticatedSession? Login(string username, string password)
    {
        Player? player = _playerRepository.GetPlayerByUsername(username);
        if (player is null || !_passwordHasher.VerifyPassword(
                password, player.PasswordHash, player.PasswordSalt))
        {
            return null;
        }

        string sessionToken = _tokenGenerator.GenerateSessionToken();
        DateTime expiresAt = _timeProvider.GetUtcNow().UtcDateTime.AddDays(SessionDurationDays);
        SessionToken token = new SessionToken
        {
            PlayerId = player.PlayerId,
            TokenValue = _tokenGenerator.HashSessionToken(sessionToken),
            ExpiresAt = expiresAt
        };

        bool isCreated = _sessionTokenRepository.CreateSessionToken(token);
        if (!isCreated)
        {
            throw new InvalidOperationException("The session could not be persisted.");
        }

        return new AuthenticatedSession
        {
            PlayerId = player.PlayerId,
            Username = player.Username,
            Token = sessionToken,
            ExpiresAt = expiresAt
        };
    }

    public AuthenticatedSession? ResumeSession(string sessionToken)
    {
        AuthenticatedSession? session = null;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
            session = _guests.GetSession(tokenHash);
            if (session is null)
            {
                SessionToken? storedToken = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
                if (storedToken is not null)
                {
                    session = new AuthenticatedSession
                    {
                        PlayerId = storedToken.PlayerId,
                        Username = storedToken.Player.Username,
                        Token = sessionToken,
                        ExpiresAt = storedToken.ExpiresAt
                    };
                }
            }
        }

        return session;
    }

    public bool Logout(string sessionToken)
    {
        bool isRevoked = false;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
            isRevoked = _guests.Revoke(tokenHash) || _sessionTokenRepository.RevokeSessionToken(tokenHash);
        }

        return isRevoked;
    }

    public AuthenticatedSession CreateGuestSession()
    {
        return _guests.Create();
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Vertigo.Server.PlayerManagement.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="CoreWCF.Primitives" Version="1.9.1" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Shared/Vertigo.Contracts/Vertigo.Contracts.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.ServiceModel.Primitives" Version="10.1.0" />
  </ItemGroup>

</Project>
```

## File: tests/Vertigo.Server.Game.Tests/UnitTest1.cs
```csharp
namespace Vertigo.Server.Game.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
    }
}
```

## File: tests/Vertigo.Server.Game.Tests/Vertigo.Server.Game.Tests.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Server\Vertigo.Server.Game\Vertigo.Server.Game.csproj" />
  </ItemGroup>

</Project>
```

## File: .gitignore
```
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.
##
## Get latest from `dotnet new gitignore`

# dotenv files
.env

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# User-specific files (MonoDevelop/Xamarin Studio)
*.userprefs

# Mono auto generated files
mono_crash.*

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio 2015/2017 cache/options directory
.vs/
# Uncomment if you have tasks that create the project's static files in wwwroot
#wwwroot/

# Visual Studio 2017 auto generated files
Generated\ Files/

# MSTest test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# NUnit
*.VisualState.xml
TestResult.xml
nunit-*.xml

# Build Results of an ATL Project
[Dd]ebugPS/
[Rr]eleasePS/
dlldata.c

# Benchmark Results
BenchmarkDotNet.Artifacts/

# .NET
project.lock.json
project.fragment.lock.json
artifacts/

# Tye
.tye/

# ASP.NET Scaffolding
ScaffoldingReadMe.txt

# StyleCop
StyleCopReport.xml

# Files built by Visual Studio
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
# but not Directory.Build.rsp, as it configures directory-level build defaults
!Directory.Build.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.tlog
*.vspscc
*.vssscc
.builds
*.pidb
*.svclog
*.scc

# Chutzpah Test files
_Chutzpah*

# Visual C++ cache files
ipch/
*.aps
*.ncb
*.opendb
*.opensdf
*.sdf
*.cachefile
*.VC.db
*.VC.VC.opendb

# Visual Studio profiler
*.psess
*.vsp
*.vspx
*.sap

# Visual Studio Trace Files
*.e2e
# but not directories ending in .e2e
!*.e2e/

# TFS 2012 Local Workspace
$tf/

# Guidance Automation Toolkit
*.gpState

# ReSharper is a .NET coding add-in
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user

# TeamCity is a build add-in
_TeamCity*

# DotCover is a Code Coverage Tool
*.dotCover

# AxoCover is a Code Coverage Tool
.axoCover/*
!.axoCover/settings.json

# Coverlet is a free, cross platform Code Coverage Tool
coverage*.json
coverage*.xml
coverage*.info

# Visual Studio code coverage results
*.coverage
*.coveragexml

# NCrunch
_NCrunch_*
.*crunch*.local.xml
nCrunchTemp_*

# MightyMoose
*.mm.*
AutoTest.Net/

# Web workbench (sass)
.sass-cache/

# Installshield output folder
[Ee]xpress/

# DocProject is a documentation generator add-in
DocProject/buildhelp/
DocProject/Help/*.HxT
DocProject/Help/*.HxC
DocProject/Help/*.hhc
DocProject/Help/*.hhk
DocProject/Help/*.hhp
DocProject/Help/Html2
DocProject/Help/html

# Click-Once directory
publish/

# Publish Web Output
*.[Pp]ublish.xml
*.azurePubxml
# Note: Comment the next line if you want to checkin your web deploy settings,
# but database connection strings (with potential passwords) will be unencrypted
*.pubxml
*.publishproj

# Microsoft Azure Web App publish settings. Comment the next line if you want to
# checkin your Azure Web App publish settings, but sensitive information contained
# in these scripts will be unencrypted
PublishScripts/

# NuGet Packages
*.nupkg
# NuGet Symbol Packages
*.snupkg
# The packages folder can be ignored because of Package Restore
**/[Pp]ackages/*
# except build/, which is used as an MSBuild target.
!**/[Pp]ackages/build/
# Uncomment if necessary however generally it will be regenerated when needed
#!**/[Pp]ackages/repositories.config
# NuGet v3's project.json files produces more ignorable files
*.nuget.props
*.nuget.targets

# Microsoft Azure Build Output
csx/
*.build.csdef

# Microsoft Azure Emulator
ecf/
rcf/

# Windows Store app package directories and files
AppPackages/
BundleArtifacts/
Package.StoreAssociation.xml
_pkginfo.txt
*.appx
*.appxbundle
*.appxupload

# Visual Studio cache files
# files ending in .cache can be ignored
*.[Cc]ache
# but keep track of directories ending in .cache
!?*.[Cc]ache/

# Others
ClientBin/
~$*
*~
*.dbmdl
*.dbproj.schemaview
*.jfm
*.pfx
*.publishsettings
orleans.codegen.cs

# Including strong name files can present a security risk
# (https://github.com/github/gitignore/pull/2483#issue-259490424)
#*.snk

# Since there are multiple workflows, uncomment next line to ignore bower_components
# (https://github.com/github/gitignore/pull/1529#issuecomment-104372622)
#bower_components/

# RIA/Silverlight projects
Generated_Code/

# Backup & report files from converting an old project file
# to a newer Visual Studio version. Backup files are not needed,
# because we have git ;-)
_UpgradeReport_Files/
Backup*/
UpgradeLog*.XML
UpgradeLog*.htm
ServiceFabricBackup/
*.rptproj.bak

# SQL Server files
*.mdf
*.ldf
*.ndf

# Business Intelligence projects
*.rdl.data
*.bim.layout
*.bim_*.settings
*.rptproj.rsuser
*- [Bb]ackup.rdl
*- [Bb]ackup ([0-9]).rdl
*- [Bb]ackup ([0-9][0-9]).rdl

# Microsoft Fakes
FakesAssemblies/

# GhostDoc plugin setting file
*.GhostDoc.xml

# Node.js Tools for Visual Studio
.ntvs_analysis.dat
node_modules/

# Visual Studio 6 build log
*.plg

# Visual Studio 6 workspace options file
*.opt

# Visual Studio 6 auto-generated workspace file (contains which files were open etc.)
*.vbw

# Visual Studio 6 auto-generated project file (contains which files were open etc.)
*.vbp

# Visual Studio 6 workspace and project file (working project files containing files to include in project)
*.dsw
*.dsp

# Visual Studio 6 technical files
*.ncb
*.aps

# Visual Studio LightSwitch build output
**/*.HTMLClient/GeneratedArtifacts
**/*.DesktopClient/GeneratedArtifacts
**/*.DesktopClient/ModelManifest.xml
**/*.Server/GeneratedArtifacts
**/*.Server/ModelManifest.xml
_Pvt_Extensions

# Paket dependency manager
.paket/paket.exe
paket-files/

# FAKE - F# Make
.fake/

# CodeRush personal settings
.cr/personal

# Python Tools for Visual Studio (PTVS)
__pycache__/
*.pyc

# Cake - Uncomment if you are using it
# tools/**
# !tools/packages.config

# Tabs Studio
*.tss

# Telerik's JustMock configuration file
*.jmconfig

# BizTalk build output
*.btp.cs
*.btm.cs
*.odx.cs
*.xsd.cs

# OpenCover UI analysis results
OpenCover/

# Azure Stream Analytics local run output
ASALocalRun/

# MSBuild Binary and Structured Log
*.binlog

# NVidia Nsight GPU debugger configuration file
*.nvuser

# MFractors (Xamarin productivity tool) working folder
.mfractor/

# Local History for Visual Studio
.localhistory/

# Visual Studio History (VSHistory) files
.vshistory/

# BeatPulse healthcheck temp database
healthchecksdb

# Backup folder for Package Reference Convert tool in Visual Studio 2017
MigrationBackup/

# Ionide (cross platform F# VS Code tools) working folder
.ionide/

# Fody - auto-generated XML schema
FodyWeavers.xsd

# VS Code files for those working on multiple tools
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
*.code-workspace

# Official VS Code C# Dev Kit Extension exclusion
*.lscache

# Local History for Visual Studio Code
.history/

# Windows Installer files from build outputs
*.cab
*.msi
*.msix
*.msm
*.msp

# JetBrains Rider
*.sln.iml
.idea/

##
## Visual studio for Mac
##


# globs
Makefile.in
*.userprefs
*.usertasks
config.make
config.status
aclocal.m4
install-sh
autom4te.cache/
*.tar.gz
tarballs/
test-results/

# content below from: https://github.com/github/gitignore/blob/main/Global/macOS.gitignore
# General
.DS_Store
.AppleDouble
.LSOverride

# Icon must end with two \r
Icon


# Thumbnails
._*

# Files that might appear in the root of a volume
.DocumentRevisions-V100
.fseventsd
.Spotlight-V100
.TemporaryItems
.Trashes
.VolumeIcon.icns
.com.apple.timemachine.donotpresent

# Directories potentially created on remote AFP share
.AppleDB
.AppleDesktop
Network Trash Folder
Temporary Items
.apdisk

# content below from: https://github.com/github/gitignore/blob/main/Global/Windows.gitignore
# Windows thumbnail cache files
Thumbs.db
ehthumbs.db
ehthumbs_vista.db

# Dump file
*.stackdump

# Folder config file
[Dd]esktop.ini

# Recycle Bin used on file shares
$RECYCLE.BIN/

# Windows Installer files
*.cab
*.msi
*.msix
*.msm
*.msp

# Windows shortcuts
*.lnk

# Vim temporary swap files
*.swp
```

## File: Vertigo.Client.slnx
```
<Solution>
  <Folder Name="/src/" />
  <Folder Name="/src/Client/">
    <Project Path="src/Client/Vertigo.Client.Core/Vertigo.Client.Core.csproj" />
    <Project Path="src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <Project Path="src/Client/Vertigo.Client.UI/Vertigo.Client.UI.csproj" />
  </Folder>
  <Folder Name="/src/Shared/">
    <Project Path="src/Shared/Vertigo.Contracts/Vertigo.Contracts.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/Vertigo.Client.Networking.Smoke/Vertigo.Client.Networking.Smoke.csproj" />
  </Folder>
</Solution>
```

## File: Vertigo.Server.slnx
```
<Solution>
  <Folder Name="/src/" />
  <Folder Name="/src/Server/">
    <Project Path="src/Server/Vertigo.Server.CrossCutting/Vertigo.Server.CrossCutting.csproj" />
    <Project Path="src/Server/Vertigo.Server.DataAccess/Vertigo.Server.DataAccess.csproj" />
    <Project Path="src/Server/Vertigo.Server.Game/Vertigo.Server.Game.csproj" />
    <Project Path="src/Server/Vertigo.Server.Host/Vertigo.Server.Host.csproj" />
    <Project Path="src/Server/Vertigo.Server.Matchmaking/Vertigo.Server.Matchmaking.csproj" />
    <Project Path="src/Server/Vertigo.Server.PlayerManagement/Vertigo.Server.PlayerManagement.csproj" />
  </Folder>
  <Folder Name="/src/Shared/">
    <Project Path="src/Shared/Vertigo.Contracts/Vertigo.Contracts.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/Vertigo.Server.Game.Tests/Vertigo.Server.Game.Tests.csproj" />
  </Folder>
</Solution>
```
