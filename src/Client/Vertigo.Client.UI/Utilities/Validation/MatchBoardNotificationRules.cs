using System;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.Utilities.Validation;

public static class MatchBoardNotificationRules
{
    public static bool ShouldRefresh(MatchBoardChanged? change, string currentLobbyCode,
        Guid currentMatchId, long currentRevision)
    {
        return change is not null
            && currentMatchId != Guid.Empty
            && change.MatchId == currentMatchId
            && string.Equals(change.LobbyCode, currentLobbyCode, StringComparison.OrdinalIgnoreCase)
            && change.Revision > currentRevision;
    }
}
