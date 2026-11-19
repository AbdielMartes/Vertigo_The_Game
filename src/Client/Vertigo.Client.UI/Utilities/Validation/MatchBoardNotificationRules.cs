using System;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.Utilities.Validation;

public static class MatchBoardNotificationRules
{
    public static bool ShouldRefresh(
        MatchBoardChanged? change,
        MatchBoardIdentity currentBoard,
        long currentRevision)
    {
        return change is not null
            && currentBoard.MatchId != Guid.Empty
            && change.MatchId == currentBoard.MatchId
            && string.Equals(change.LobbyCode, currentBoard.LobbyCode, StringComparison.OrdinalIgnoreCase)
            && change.Revision > currentRevision;
    }
}
