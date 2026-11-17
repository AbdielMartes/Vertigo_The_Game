using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels;
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
