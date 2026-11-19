using Vertigo.Client.UI.ViewModels.Presentation;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchMasterPlanDraftTests
{
    [Fact]
    public void TryAppend_ValidModules_UsesSelectionOrderAndRequiresTwoModules()
    {
        MatchMasterPlanDraft draft = new();

        Assert.True(draft.TryAppend(12, 15));
        Assert.False(draft.CanPlay);
        Assert.True(draft.TryAppend(12, 24));
        Assert.True(draft.TryAppend(12, 32));

        Assert.True(draft.CanPlay);
        Assert.Equal(new[] { 15, 24, 32 }, draft.Modules);
    }

    [Fact]
    public void TryAppend_DuplicateOrInvalidRise_KeepsTheExistingDraft()
    {
        MatchMasterPlanDraft draft = new();
        Assert.True(draft.TryAppend(12, 15));

        Assert.False(draft.TryAppend(12, 15));
        Assert.False(draft.TryAppend(12, 26));
        Assert.False(draft.TryAppend(12, 14));
        Assert.False(draft.TryAppend(null, 20));

        Assert.Equal(new[] { 15 }, draft.Modules);
        Assert.False(draft.CanPlay);
    }

    [Fact]
    public void RemoveLast_ChangesChainTopSoAnotherModuleCanBeAdded()
    {
        MatchMasterPlanDraft draft = new();
        Assert.True(draft.TryAppend(12, 15));
        Assert.True(draft.TryAppend(12, 24));

        bool removed = draft.RemoveLast();

        Assert.True(removed);
        Assert.False(draft.CanPlay);
        Assert.True(draft.TryAppend(12, 20));
        Assert.Equal(new[] { 15, 20 }, draft.Modules);
    }

    [Fact]
    public void IsStillInHand_MissingModule_InvalidatesDraft()
    {
        MatchMasterPlanDraft draft = new();
        Assert.True(draft.TryAppend(12, 15));
        Assert.True(draft.TryAppend(12, 20));

        Assert.False(draft.IsStillInHand(new[] { 15, 26 }));
        Assert.True(draft.IsStillInHand(new[] { 20, 15 }));

        draft.Clear();
        Assert.Empty(draft.Modules);
        Assert.False(draft.CanPlay);
    }
}
