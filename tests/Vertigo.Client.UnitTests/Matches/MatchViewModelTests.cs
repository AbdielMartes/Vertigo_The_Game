using System.Runtime.Serialization;
using System.Xml.Linq;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels;
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
    public void ApplyBoard_SnapshotWithoutRoundSummaries_AcceptsLegacyPayload()
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
        using System.Xml.XmlReader reader = payload.CreateReader();
        MatchBoardSnapshot board = Assert.IsType<MatchBoardSnapshot>(serializer.ReadObject(reader));

        bool accepted = model.ApplyBoard(board);

        Assert.True(accepted);
        Assert.Empty(model.RoundSummaries);
    }

    private static MatchViewModel CreateModel()
    {
        return new MatchViewModel(new FakeTextLocalizer());
    }

    private sealed class FakeTextLocalizer : ITextLocalizer
    {
        public string Get(string key)
        {
            return key;
        }
    }
}
