using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Services;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Xunit;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchViewModelTests
{
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    public void IsRetryableBoardFailure_TimeoutRequiresAReadWithNewerNotification(
        bool isBoardRead, bool newerNotification, bool retryable)
    {
        Assert.Equal(retryable,
            MatchViewModel.IsRetryableBoardFailure(new TimeoutException(), isBoardRead, newerNotification));
    }

    [Theory]
    [InlineData(ServiceErrorCodes.Unexpected, true, true)]
    [InlineData(ServiceErrorCodes.Unexpected, false, false)]
    [InlineData(ServiceErrorCodes.Unauthorized, true, false)]
    public void IsRetryableBoardFailure_OnlyRetriesUnexpectedServiceFaultOnRead(
        string errorCode, bool isBoardRead, bool retryable)
    {
        FaultException<ServiceFault> fault = new(new ServiceFault { ErrorCode = errorCode });
        Assert.Equal(retryable, MatchViewModel.IsRetryableBoardFailure(fault, isBoardRead, false));
    }

    [Theory]
    [InlineData("es-MX", "Plano Maestro")]
    [InlineData("en-US", "Master Plan")]
    public void MasterPlanTitle_UsesLocalizedResources(string cultureName, string expected)
    {
        CultureInfo previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            MatchViewModel model = new(new TextLocalizer());

            Assert.Equal(expected, model.MasterPlanTitle);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Fact]
    public void RefreshLocalization_NotifiesMatchPanelLabels()
    {
        MatchViewModel model = CreateModel();
        List<string?> changes = [];
        model.PropertyChanged += (_, args) => changes.Add(args.PropertyName);

        model.RefreshLocalization();

        Assert.Contains(nameof(MatchViewModel.ChatText), changes);
        Assert.Contains(nameof(MatchViewModel.RecentMovesText), changes);
        Assert.Contains(nameof(MatchViewModel.MasterPlanTitle), changes);
    }

    [Fact]
    public void Activate_ValidMatch_SetsMatchIdentity()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();

        bool activated = model.Activate(matchId, "ABC234");

        Assert.True(activated);
        Assert.Equal(matchId, model.MatchId);
        Assert.Equal("ABC234", model.LobbyCode);
    }

    [Fact]
    public void Activate_SameMatch_DoesNotReplaceDraft()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        model.DraftMessage = "Mensaje sin enviar";

        bool activated = model.Activate(matchId, "abc234");

        Assert.False(activated);
        Assert.Equal("Mensaje sin enviar", model.DraftMessage);
    }

    [Fact]
    public void ApplyChatMessage_IgnoresLobbyChatAndMessagesFromOtherMatches()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        model.ApplyChatMessage(new LobbyChatMessage { LobbyCode = "ABC234", Text = "Solo sala" });
        model.ApplyChatMessage(new LobbyChatMessage
        {
            LobbyCode = "ABC234", MatchId = Guid.NewGuid(), Text = "Otra partida"
        });
        model.ApplyChatMessage(new LobbyChatMessage { LobbyCode = "XYZ789", MatchId = matchId });
        DateTime sentAt = new(2026, 1, 15, 23, 7, 0, DateTimeKind.Utc);
        model.ApplyChatMessage(new LobbyChatMessage
        {
            LobbyCode = "abc234", MatchId = matchId, Text = "¡Vamos!", SenderUsername = "Ana", SentAt = sentAt
        });

        MatchChatRow message = Assert.Single(model.ChatMessages);
        Assert.Equal("¡Vamos!", message.Text);
        Assert.Equal("Ana", message.Sender);
        Assert.Equal(sentAt, message.SentAt);
        Assert.Equal(DateTimeKind.Utc, message.SentAt.Kind);
        Assert.Equal(sentAt.ToLocalTime().ToString("HH:mm", CultureInfo.InvariantCulture), message.LocalTimeText);
    }

    [Theory]
    [InlineData(-360, "17:07")]
    [InlineData(120, "01:07")]
    [InlineData(330, "04:37")]
    public void FormatTime_ServerTimestamp_ShowsTimeInPlayersZone(int offsetMinutes, string expected)
    {
        const string ZoneName = "Test zone";
        TimeZoneInfo zone = TimeZoneInfo.CreateCustomTimeZone(
            ZoneName,
            TimeSpan.FromMinutes(offsetMinutes),
            ZoneName,
            ZoneName);
        DateTime utc = new(2026, 1, 15, 23, 7, 0, DateTimeKind.Utc);

        Assert.Equal(expected, MatchChatRow.FormatTime(utc, zone));
        Assert.Equal(expected, MatchChatRow.FormatTime(DateTime.SpecifyKind(utc, DateTimeKind.Unspecified), zone));
    }

    [Fact]
    public void LocalTimeText_MissingTimestamp_IsEmpty()
    {
        MatchChatRow message = new() { Sender = "Ana", Text = "¡Vamos!" };

        Assert.Empty(message.LocalTimeText);
    }

    [Fact]
    public void ApplyChatMessage_HistoryAtCapacity_DropsOnlyOldestMessage()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");

        for (int index = 0; index <= 100; index++)
        {
            model.ApplyChatMessage(new LobbyChatMessage { LobbyCode = "ABC234", MatchId = matchId, Text = $"#{index}" });
        }

        Assert.Equal(100, model.ChatMessages.Count);
        Assert.Equal("#1", model.ChatMessages[0].Text);
        Assert.Equal("#100", model.ChatMessages[^1].Text);
    }

    [Fact]
    public void Activate_AnotherMatch_ClearsChatHistory()
    {
        MatchViewModel model = CreateModel();
        Guid firstMatch = Guid.NewGuid();
        model.Activate(firstMatch, "ABC234");
        model.ApplyChatMessage(new LobbyChatMessage { LobbyCode = "ABC234", MatchId = firstMatch, Text = "Hola" });
        model.Activate(Guid.NewGuid(), "ABC234");

        Assert.Empty(model.ChatMessages);
    }

    [Fact]
    public void Activate_EmptyMatchId_ReturnsFalse()
    {
        MatchViewModel model = CreateModel();

        bool activated = model.Activate(Guid.Empty, "ABC234");

        Assert.False(activated);
        Assert.Equal(Guid.Empty, model.MatchId);
    }

    [Fact]
    public void Reset_ActiveMatch_ClearsIdentityAndDraft()
    {
        MatchViewModel model = CreateModel();
        model.Activate(Guid.NewGuid(), "ABC234");
        model.DraftMessage = "Hola";

        model.Reset();

        Assert.Equal(Guid.Empty, model.MatchId);
        Assert.Empty(model.LobbyCode);
        Assert.Empty(model.DraftMessage);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ApplyBoard_PlayerSelection_FollowsPlayerIdWhenRowsChange(bool targetStillPresent)
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();
        MatchBoardPlayer otherPlayer = new() { PlayerId = Guid.NewGuid(), Username = "Ana" };
        model.Activate(matchId, "ABC234");
        model.ApplyBoard(new MatchBoardSnapshot
        {
            MatchId = matchId,
            Revision = 1,
            Players = [new MatchBoardPlayer { PlayerId = targetId, Username = "Luis" }, otherPlayer]
        });
        model.SelectedPlayer = model.Players[0];

        MatchBoardSnapshot updated = new()
        {
            MatchId = matchId,
            Revision = 2,
            Players = [otherPlayer]
        };
        if (targetStillPresent)
        {
            updated.Players.Add(new MatchBoardPlayer { PlayerId = targetId, Username = "Luis", FloorsBuilt = 2 });
        }

        Assert.True(model.ApplyBoard(updated));
        if (targetStillPresent)
        {
            Assert.Same(model.Players[1], model.SelectedPlayer);
            Assert.Equal(2, model.SelectedPlayer.FloorsBuilt);
        }
        else
        {
            Assert.Null(model.SelectedPlayer);
        }

        model.Activate(Guid.NewGuid(), "DEF234");
        Assert.Null(model.SelectedPlayer);
    }

    [Fact]
    public void ApplyBoard_ClosedRound_ExposesSummaryAndIgnoresOlderSnapshots()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        MatchRoundSummary summary = new() { RoundNumber = 1 };

        bool accepted = model.ApplyBoard(new MatchBoardSnapshot
        {
            MatchId = matchId,
            Revision = 20,
            RoundNumber = 2,
            RoundSummaries = new List<MatchRoundSummary> { summary }
        });
        bool stale = model.ApplyBoard(new MatchBoardSnapshot { MatchId = matchId, Revision = 19, RoundNumber = 1 });

        Assert.True(accepted);
        Assert.False(stale);
        Assert.Equal(1, Assert.Single(model.RoundSummaries).RoundNumber);
        model.Reset();
        Assert.Empty(model.RoundSummaries);
    }

    [Fact]
    public void ApplyBoard_SnapshotWithoutNewFields_AcceptsLegacyPayload()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        DataContractSerializer serializer = new(typeof(MatchBoardSnapshot));
        using MemoryStream stream = new();
        serializer.WriteObject(stream, new MatchBoardSnapshot { MatchId = matchId, Revision = 1 });
        stream.Position = 0;
        XElement payload = XElement.Load(stream);
        payload.Element(payload.Name.Namespace + nameof(MatchBoardSnapshot.RoundSummaries))!.Remove();
        payload.Element(payload.Name.Namespace + nameof(MatchBoardSnapshot.RecentMoves))!.Remove();
        payload.Element(payload.Name.Namespace + nameof(MatchBoardSnapshot.ExpelledPlayerIds))!.Remove();
        payload.Element(payload.Name.Namespace + nameof(MatchBoardSnapshot.ExpelledByVotePlayerIds))!.Remove();
        payload.Element(payload.Name.Namespace + nameof(MatchBoardSnapshot.ExpulsionVotes))!.Remove();
        using System.Xml.XmlReader reader = payload.CreateReader();
        MatchBoardSnapshot board = Assert.IsType<MatchBoardSnapshot>(serializer.ReadObject(reader));

        bool accepted = model.ApplyBoard(board);

        Assert.True(accepted);
        Assert.Empty(model.RoundSummaries);
        Assert.Empty(model.RecentMoves);
        Assert.Empty(board.ExpelledPlayerIds);
        Assert.Empty(board.ExpelledByVotePlayerIds);
        Assert.Empty(board.ExpulsionVotes);
    }

    [Fact]
    public void ApplyBoard_WithRecentMoves_ShowsMostRecentLast()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        MatchBoardSnapshot board = new()
        {
            MatchId = matchId,
            Revision = 5,
            RecentMoves =
            {
                new MatchBoardMove { PlayerUsername = "Ana", PilePosition = 1, PreviousTop = 12, CardNumbers = { 15 } },
                new MatchBoardMove { PlayerUsername = "Beto", PilePosition = 2,
                    ActionKind = MatchBoardActionKind.Dismantle },
                new MatchBoardMove { PlayerUsername = "Ana", PilePosition = 3, PreviousTop = 20,
                    ActionKind = MatchBoardActionKind.MasterPlan, CardNumbers = { 24, 30 } }
            }
        };

        Assert.True(model.ApplyBoard(board));
        Assert.False(model.ApplyBoard(new MatchBoardSnapshot { MatchId = matchId, Revision = 4 }));

        string[] expected = { "Ana jugó 12 → 15 · torre 1",
            "Beto desmontó la torre 2.", "Ana usó un Plano Maestro: 20 → 24 → 30 · torre 3" };
        Assert.Equal(expected, model.RecentMoves.Select(move => move.Text));
        model.Reset();
        Assert.Empty(model.RecentMoves);
    }

    [Fact]
    public void ApplyBoard_OnlyNewMovesNotifyHistoryObservers()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        int changes = 0;
        model.RecentMoves.CollectionChanged += (_, _) => changes++;
        ApplyMove(revision: 5, moveRevision: 5);
        Assert.True(changes > 0);

        changes = 0;
        ApplyMove(revision: 6, moveRevision: 5);
        Assert.Equal(0, changes);

        ApplyMove(revision: 7, moveRevision: 7);
        Assert.True(changes > 0);
        Assert.Equal(7, Assert.Single(model.RecentMoves).Revision);

        void ApplyMove(long revision, long moveRevision)
        {
            Assert.True(model.ApplyBoard(new MatchBoardSnapshot
            {
                MatchId = matchId,
                Revision = revision,
                RecentMoves = [new MatchBoardMove
                {
                    Revision = moveRevision,
                    PlayerUsername = "Ana",
                    ActionKind = MatchBoardActionKind.Dismantle,
                    PilePosition = 1
                }]
            }));
        }
    }

    [Fact]
    public void ApplyBoard_FinalResult_PersistsUntilAnotherMatchIsActivated()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        Guid winner = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        MatchBoardSnapshot final = new()
        {
            MatchId = matchId,
            Revision = 30,
            EndReason = MatchEndReason.Completed,
            WinnerIds = { winner },
            Players = { new MatchBoardPlayer { PlayerId = winner, Username = "Ana" } },
            RoundSummaries = { new MatchRoundSummary { RoundNumber = 3 } }
        };

        Assert.True(model.ApplyBoard(final));
        Assert.False(model.ApplyBoard(new MatchBoardSnapshot { MatchId = matchId, Revision = 29 }));
        Assert.True(model.Results.HasFinalResult);
        Assert.True(model.Results.IsExpanded);

        model.Activate(Guid.NewGuid(), "XYZ789");

        Assert.False(model.Results.HasResults);
        Assert.False(model.Results.IsExpanded);
        Assert.Empty(model.RoundSummaries);
    }

    [Fact]
    public void ApplyBoard_FinalResult_OffersReturnToMenuInsteadOfAbandonment()
    {
        MatchViewModel model = CreateModel();
        Guid matchId = Guid.NewGuid();
        model.Activate(matchId, "ABC234");
        Assert.Equal("Match.ExitConfirmTitle", model.ExitConfirmTitleText);

        Assert.True(model.ApplyBoard(new MatchBoardSnapshot
        {
            MatchId = matchId,
            Revision = 10,
            EndReason = MatchEndReason.Completed
        }));

        Assert.Equal("Match.Exit.ReturnHome", model.ExitText);
        Assert.Equal("Match.Exit.FinalBody", model.ExitConfirmBodyText);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ApplyBoard_OngoingMatch_OffersReturnToMenuOnlyToExpelledPlayer(bool localWasExpelled)
    {
        Guid matchId = Guid.NewGuid();
        Guid local = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        PlayerSession session = new(TimeProvider.System);
        session.SetSession(new SessionSnapshot
        {
            PlayerId = local,
            Username = "Ana",
            Token = "test-session",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsGuest = true
        });
        MatchViewModel model = new(new FakeTextLocalizer(), session, TimeProvider.System);
        model.Activate(matchId, "ABC234");
        model.ApplyBoard(new MatchBoardSnapshot { MatchId = matchId, Revision = 4 });
        Assert.Equal("Match.Exit", model.ExitText);
        List<string?> changed = new();
        model.PropertyChanged += (_, args) => changed.Add(args.PropertyName);

        Assert.True(model.ApplyBoard(new MatchBoardSnapshot
        {
            MatchId = matchId,
            Revision = 5,
            ExpelledPlayerIds = { localWasExpelled ? local : other }
        }));

        Assert.Equal(localWasExpelled ? "Match.Exit.ReturnHome" : "Match.Exit", model.ExitText);
        Assert.Equal(localWasExpelled ? "Match.Exit.ReturnHome" : "Match.ExitConfirmTitle", model.ExitConfirmTitleText);
        Assert.Equal(localWasExpelled ? "Match.Exit.FinalBody" : "Match.ExitConfirmBody", model.ExitConfirmBodyText);
        Assert.Contains(nameof(MatchViewModel.ExitText), changed);
        changed.Clear();
        model.Activate(Guid.NewGuid(), "XYZ789");
        Assert.Contains(nameof(MatchViewModel.ExitText), changed);
        Assert.Equal("Match.Exit", model.ExitText);
    }

    [Fact]
    public void RequestExit_ResolvedBoardContext_EnablesConfirmationWhenDialogOpens()
    {
        ServiceCollection services = new();
        services.AddVertigoClientNetworking();
        services.AddSingleton<ITextLocalizer>(new FakeTextLocalizer());
        services.AddSingleton<IClientErrorText, ClientErrorText>();
        services.AddSingleton<ISessionGuard, SessionGuard>();
        services.AddTransient<UiServices>();
        services.AddTransient<UiNavigationContext>();
        services.AddTransient<MatchBoardContext>();
        using ServiceProvider provider = services.BuildServiceProvider();
        MatchViewModel model = new(provider.GetRequiredService<MatchBoardContext>(),
            provider.GetRequiredService<IChatClient>());
        model.Activate(Guid.NewGuid(), "ABC234");
        Assert.False(model.ConfirmExitCommand.CanExecute(null));

        model.RequestExitCommand.Execute(null);

        Assert.True(model.IsExitConfirmationOpen);
        Assert.True(model.ConfirmExitCommand.CanExecute(null));
        Assert.Equal("Match.ExitConfirmTitle", model.ExitConfirmTitleText);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ApplyBoard_ExpulsionPrompt_ShowsTargetNoticeOrVotingControlsAndClosesOnExpulsion(bool isTarget)
    {
        Guid matchId = Guid.NewGuid();
        Guid local = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        Guid target = isTarget ? local : other;
        PlayerSession session = new(TimeProvider.System);
        session.SetSession(new SessionSnapshot
        {
            PlayerId = local,
            Username = "Ana",
            Token = "test-session",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsGuest = true
        });
        MatchViewModel model = new(new FakeTextLocalizer(), session, TimeProvider.System);
        model.Activate(matchId, "ABC234");

        MatchBoardSnapshot pending = new()
        {
            MatchId = matchId,
            Revision = 5,
            ExpulsionVotesRequired = 2,
            Players = [new() { PlayerId = local }, new() { PlayerId = other }, new() { PlayerId = Guid.NewGuid() }],
            ExpulsionVotes = [new() { BallotId = 5, TargetPlayerId = target, VoteCount = 1 }]
        };
        model.ApplyBoard(pending);
        MatchExpulsionPrompt prompt = Assert.Single(model.ExpulsionPrompts);
        Assert.Equal(isTarget ? "Match.Expulsion.TargetNotice" : "Match.Expulsion.Notice", prompt.Heading);
        Assert.Equal(!isTarget, prompt.CanRespond);

        if (!isTarget)
        {
            model.ApplyBoardAndHand(pending, new MatchPrivateHand
            {
                MatchId = matchId,
                PlayerId = local,
                Revision = 5,
                VotedExpulsionTargetIds = [target]
            });
            prompt = Assert.Single(model.ExpulsionPrompts);
            Assert.False(prompt.CanRespond);
            Assert.True(prompt.CanHide);
            prompt.HideCommand.Execute(null);
            Assert.Empty(model.ExpulsionPrompts);
            Assert.True(model.HasHiddenExpulsionPrompts);
            model.ShowExpulsionPromptsCommand.Execute(null);
            Assert.True(Assert.Single(model.ExpulsionPrompts).CanHide);
        }

        model.ApplyBoardAndHand(new MatchBoardSnapshot
        {
            MatchId = matchId,
            Revision = 6,
            Players = pending.Players.Where(player => player.PlayerId != target).ToList(),
            ExpelledPlayerIds = [target],
            ExpelledByVotePlayerIds = [target],
            RecentMoves = [new() { ActionKind = MatchBoardActionKind.ExpelledByVote, PlayerUsername = "Rafa" }]
        }, null);

        Assert.Empty(model.ExpulsionPrompts);
        Assert.Equal(isTarget, model.WasExpelledByVote);
        Assert.Equal(isTarget ? "Match.Exit.ReturnHome" : "Match.Exit", model.ExitText);
        Assert.Equal("Rafa salió de la partida.", Assert.Single(model.RecentMoves).Text);
    }

    private static MatchViewModel CreateModel()
    {
        return new MatchViewModel(new FakeTextLocalizer());
    }

    private sealed class FakeTextLocalizer : ITextLocalizer
    {
        public string Get(string key)
        {
            return key switch
            {
                "Match.Move.Placement" => "{0} jugó {1} · torre {2}",
                "Match.Move.MasterPlan" => "{0} usó un Plano Maestro: {1} · torre {2}",
                "Match.Move.Dismantle" => "{0} desmontó la torre {1}.",
                "Match.Move.LeftMatch" => "{0} salió de la partida.",
                _ => key
            };
        }
    }
}
