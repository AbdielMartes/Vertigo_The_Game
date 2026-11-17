using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Contracts.Models;
using Xunit;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchBoardRevisionTests
{
    [Fact]
    public void TryAdvance_NewerRevision_ReturnsTrue()
    {
        MatchBoardRevision revisions = new();
        Guid matchId = Guid.NewGuid();

        bool result = revisions.TryAdvance(new MatchBoardSnapshot
        {
            MatchId = matchId,
            Revision = 2
        }, matchId);

        Assert.True(result);
    }

    [Fact]
    public void TryAdvance_OlderRevision_ReturnsFalse()
    {
        MatchBoardRevision revisions = new();
        Guid matchId = Guid.NewGuid();
        revisions.TryAdvance(new MatchBoardSnapshot { MatchId = matchId, Revision = 4 }, matchId);

        bool result = revisions.TryAdvance(new MatchBoardSnapshot
        {
            MatchId = matchId,
            Revision = 3
        }, matchId);

        Assert.False(result);
    }

    [Fact]
    public void TryAdvance_DifferentMatch_ReturnsFalse()
    {
        MatchBoardRevision revisions = new();

        bool result = revisions.TryAdvance(new MatchBoardSnapshot
        {
            MatchId = Guid.NewGuid(),
            Revision = 2
        }, Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public void TryAdvance_Reset_AllowsNewMatch()
    {
        MatchBoardRevision revisions = new();
        Guid first = Guid.NewGuid();
        Guid next = Guid.NewGuid();
        revisions.TryAdvance(new MatchBoardSnapshot { MatchId = first, Revision = 3 }, first);
        revisions.Reset();

        bool result = revisions.TryAdvance(new MatchBoardSnapshot
        {
            MatchId = next,
            Revision = 1
        }, next);

        Assert.True(result);
    }
}
