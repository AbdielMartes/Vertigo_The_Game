using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Lobbies;

public sealed class LobbyPreparationRevisionTests
{
    [Fact]
    public void TryAdvance_OutOfOrderUpdates_KeepsNewest()
    {
        LobbyPreparationRevision tracker = new();
        LobbyPreparation newer = new() { LobbyCode = "ABCDEF", Revision = 12 };
        LobbyPreparation older = new() { LobbyCode = "ABCDEF", Revision = 11 };

        Assert.True(tracker.TryAdvance(newer, "ABCDEF"));
        Assert.False(tracker.TryAdvance(older, "ABCDEF"));
        Assert.False(tracker.TryAdvance(newer, "ABCDEF"));
    }

    [Fact]
    public void TryAdvance_AfterLeavingRoom_DiscardsQueuedUpdate()
    {
        LobbyPreparationRevision tracker = new();
        LobbyPreparation queued = new() { LobbyCode = "ABCDEF", Revision = 5 };
        tracker.TryAdvance(new LobbyPreparation { LobbyCode = "ABCDEF", Revision = 4 }, "ABCDEF");
        tracker.Reset();

        Assert.False(tracker.TryAdvance(queued, string.Empty));
        Assert.True(tracker.TryAdvance(new LobbyPreparation { LobbyCode = "GHIJKL", Revision = 1 }, "GHIJKL"));
    }

    [Fact]
    public void TryAdvance_AnotherRoom_DoesNotAdvanceRevision()
    {
        LobbyPreparationRevision tracker = new();
        Assert.False(tracker.TryAdvance(new LobbyPreparation { LobbyCode = "OLD", Revision = 50 }, "NEW"));
        Assert.True(tracker.TryAdvance(new LobbyPreparation { LobbyCode = "NEW", Revision = 1 }, "NEW"));
    }
}
