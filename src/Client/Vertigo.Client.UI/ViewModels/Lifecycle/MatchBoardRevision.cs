using System;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Lifecycle;

public sealed class MatchBoardRevision
{
    private long _revision = -1;

    public bool TryAdvance(MatchBoardSnapshot snapshot, Guid activeMatchId)
    {
        bool accepted = activeMatchId != Guid.Empty
            && snapshot.MatchId == activeMatchId
            && snapshot.Revision > _revision;
        if (accepted)
        {
            _revision = snapshot.Revision;
        }

        return accepted;
    }

    public void Reset()
    {
        _revision = -1;
    }
}
