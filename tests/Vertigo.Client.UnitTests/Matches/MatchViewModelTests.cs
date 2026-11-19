using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Services;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Contracts.Models;
using Xunit;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchViewModelTests
{
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
        payload.Element(payload.Name.Namespace + nameof(MatchBoardSnapshot.ExpulsionVotes))!.Remove();
        using System.Xml.XmlReader reader = payload.CreateReader();
        MatchBoardSnapshot board = Assert.IsType<MatchBoardSnapshot>(serializer.ReadObject(reader));

        bool accepted = model.ApplyBoard(board);

        Assert.True(accepted);
        Assert.Empty(model.RoundSummaries);
        Assert.Empty(model.RecentMoves);
        Assert.Empty(board.ExpelledPlayerIds);
        Assert.Empty(board.ExpulsionVotes);
    }

    [Fact]
    public void ApplyBoard_WithRecentMoves_ShowsMostRecentFirst()
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

        string[] expected = { "Ana usó un Plano Maestro: 20 → 24 → 30 · torre 3",
            "Beto desmontó la torre 2.", "Ana jugó 12 → 15 · torre 1" };
        Assert.Equal(expected, model.RecentMoves.Select(move => move.Text));
        model.Reset();
        Assert.Empty(model.RecentMoves);
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
        session.SetSession(new SessionSnapshot(local, "Ana", "test-session", DateTime.UtcNow.AddHours(1), true));
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
        MatchViewModel model = new(provider.GetRequiredService<MatchBoardContext>());
        model.Activate(Guid.NewGuid(), "ABC234");
        Assert.False(model.ConfirmExitCommand.CanExecute(null));

        model.RequestExitCommand.Execute(null);

        Assert.True(model.IsExitConfirmationOpen);
        Assert.True(model.ConfirmExitCommand.CanExecute(null));
        Assert.Equal("Match.ExitConfirmTitle", model.ExitConfirmTitleText);
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
                _ => key
            };
        }
    }
}
