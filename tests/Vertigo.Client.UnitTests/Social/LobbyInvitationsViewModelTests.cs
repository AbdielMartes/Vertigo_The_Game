using System.ServiceModel;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.Utilities.Sessions.Services;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Social;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Social;

public sealed class LobbyInvitationsViewModelTests
{
    [Fact]
    public async Task AcceptInvitation_Valid_RemovesOnlyAcceptedInvitation()
    {
        TestContext test = Create();
        string accepted = "AAAAAA";
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = accepted });
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "BBBBBB" });

        bool result = await test.Invitations.AcceptInvitationAsync(accepted);

        Assert.True(result);
        Assert.Single(test.Invitations.PendingInvitations);
        Assert.Equal("BBBBBB", test.Invitations.PendingInvitations[0].LobbyCode);
        Assert.Equal(NavigationPage.Lobby, test.Navigator.CurrentPage);
    }

    [Fact]
    public async Task AcceptInvitation_ServerRejects_StaysInSocialAndKeepsInvitations()
    {
        TestContext test = Create();
        test.Navigator.NavigateTo(NavigationPage.Social);
        test.LobbyClient.JoinFailure = new FaultException<ServiceFault>(
            new ServiceFault { ErrorCode = ServiceErrorCodes.LobbyNotFound, MessageKey = ServiceMessageKeys.LobbyNotFound },
            new FaultReason("Not found"));
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "AAAAAA" });
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "BBBBBB" });

        bool result = await test.Invitations.AcceptInvitationAsync("AAAAAA");

        Assert.False(result);
        Assert.Equal(2, test.Invitations.PendingInvitations.Count);
        Assert.Equal("No se encontró la sala.", test.Invitations.ErrorMessage);
        Assert.Equal(NavigationPage.Social, test.Navigator.CurrentPage);
    }

    [Fact]
    public async Task AcceptInvitation_StaleResponse_DoesNotNavigateOrRemove()
    {
        TestContext test = Create();
        test.Navigator.NavigateTo(NavigationPage.Social);
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "AAAAAA" });
        using ManualResetEventSlim started = new(false);
        using ManualResetEventSlim release = new(false);
        test.LobbyClient.OnJoin = _ =>
        {
            started.Set();
            if (!release.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException("Synchronization timeout");
            }
            return new LobbyAdmission { LobbyCode = "AAAAAA", HostPlayerId = Guid.NewGuid() };
        };

        Task<bool> pending = test.Invitations.AcceptInvitationAsync("AAAAAA");
        Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
        try
        {
            test.PlayerSession.SetSession(new SessionSnapshot
            {
                PlayerId = Guid.NewGuid(),
                Username = "replacement",
                Token = "replacement-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsGuest = false
            });
        }
        finally
        {
            release.Set();
        }
        bool result = await pending;

        Assert.False(result);
        Assert.Single(test.Invitations.PendingInvitations);
        Assert.Equal(NavigationPage.Social, test.Navigator.CurrentPage);
    }

    private static TestContext Create()
    {
        PlayerSession sessions = new(TimeProvider.System);
        sessions.SetSession(new SessionSnapshot
        {
            PlayerId = Guid.NewGuid(),
            Username = "registered",
            Token = "test-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsGuest = false
        });
        Navigator navigator = new();
        ITextLocalizer text = new FakeLocalizer();
        IClientErrorText errors = new ClientErrorText(text);
        SessionGuard guard = new(sessions, navigator, new FakeTransportCleaner());
        UiServices uiServices = new(errors, text, guard);
        UiNavigationContext ui = new(navigator, uiServices);
        FakeLobbyClient lobbyClient = new();
        LobbyContext lobbyContext = new(sessions, ui, new MatchViewModel(text));
        LobbyViewModel lobby = new(lobbyClient, new FakeChatClient(), lobbyContext);
        SocialOperation operation = new(sessions, guard, errors);
        LobbyInvitationsViewModel invitations = new(new FakeInvitationClient(), operation, new LobbyInvitationContext(lobby, text));
        return new TestContext
        {
            PlayerSession = sessions,
            Navigator = navigator,
            LobbyClient = lobbyClient,
            Invitations = invitations
        };
    }

    private sealed class TestContext
    {
        public required PlayerSession PlayerSession { get; init; }
        public required Navigator Navigator { get; init; }
        public required FakeLobbyClient LobbyClient { get; init; }
        public required LobbyInvitationsViewModel Invitations { get; init; }
    }

    private sealed class FakeLocalizer : ITextLocalizer
    {
        public string Get(string key)
        {
            return key switch
            {
                "ROOM_NOT_FOUND" => "No se encontró la sala.",
                "Lobby.JoinFailed" => "No se pudo entrar a la sala.",
                _ => key
            };
        }
    }

    private sealed class FakeTransportCleaner : ISessionTransportCleaner
    {
        public void DisconnectAll() { }
    }

    private sealed class FakeChatClient : IChatClient
    {
        public ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken)
        {
            return ClientResult<bool>.Success(true);
        }
    }

    private sealed class FakeLobbyClient : ILobbyClient
    {
        public Exception? JoinFailure { get; set; }
        public Func<string, LobbyAdmission>? OnJoin { get; set; }
        public event EventHandler<LobbyMember>? PlayerJoined;
        public event EventHandler<Guid>? PlayerLeft;
        public event EventHandler<LobbyChatMessage>? MessageReceived;
        public event EventHandler<LobbyPreparation>? PreparationChanged;
        public event EventHandler<MatchBoardChanged>? MatchBoardUpdated { add { } remove { } }

        public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
        {
            return "AAAAAA";
        }

        public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
        {
            if (JoinFailure is not null)
            {
                throw JoinFailure;
            }
            return OnJoin?.Invoke(lobbyCode) ?? new LobbyAdmission { LobbyCode = lobbyCode, HostPlayerId = Guid.NewGuid() };
        }

        public LobbyPreparation? GetCurrentLobby(string sessionToken)
        {
            return null;
        }

        public MatchBoardSnapshot VoteToExpelMatchPlayer(MatchExpulsionVoteRequest request)
        {
            throw new NotSupportedException();
        }

        public bool LeaveMatch(MatchLeaveRequest request)
        {
            throw new NotSupportedException();
        }

        public MatchBoardSnapshot UseMatchMasterPlan(MatchMasterPlanRequest request)
        {
            throw new NotSupportedException();
        }

        public MatchBoardSnapshot DismantleMatchTower(MatchDismantleRequest request)
        {
            throw new NotSupportedException();
        }

        public MatchBoardSnapshot PlaceMatchCard(MatchPlaceRequest request)
        {
            throw new NotSupportedException();
        }

        public MatchPrivateHand GetMatchPrivateHand(string lobbyCode, string sessionToken)
        {
            throw new NotSupportedException();
        }

        public MatchBoardSnapshot GetMatchBoard(string lobbyCode, string sessionToken)
        {
            throw new NotSupportedException();
        }

        public LobbyPreparation GetPreparation(string lobbyCode, string sessionToken)
        {
            return new LobbyPreparation { LobbyCode = lobbyCode };
        }

        public LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken)
        {
            return GetPreparation(lobbyCode, sessionToken);
        }

        public LobbyPreparation StartMatch(string lobbyCode, string sessionToken)
        {
            return GetPreparation(lobbyCode, sessionToken);
        }

        public bool LeaveLobby(string lobbyCode, string sessionToken)
        {
            return true;
        }

        public List<LobbySummary> GetPublicLobbies()
        {
            return new();
        }

        public void ReplayPendingEvents() { }
        public void Disconnect() { }
        public void Dispose() { }
    }

    private sealed class FakeInvitationClient : ILobbyInvitationClient
    {
        public ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken)
        {
            return ClientResult<List<FriendEntry>>.Success(new());
        }

        public ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
        {
            return ClientResult<List<Guid>>.Success(new());
        }

        public ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken)
        {
            return ClientResult<List<LobbyInvitationEntry>>.Success(new());
        }

        public ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken)
        {
            return ClientResult<bool>.Success(true);
        }
    }
}
