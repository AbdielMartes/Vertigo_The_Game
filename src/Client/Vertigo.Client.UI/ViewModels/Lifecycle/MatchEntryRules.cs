using System;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Lifecycle;

public static class MatchEntryRules
{
    public static bool CanEnter(LobbyPreparation preparation, string lobbyCode)
    {
        bool canEnter = preparation.IsInGame
            && preparation.MatchId != Guid.Empty
            && !string.IsNullOrWhiteSpace(lobbyCode)
            && string.Equals(preparation.LobbyCode, lobbyCode, StringComparison.OrdinalIgnoreCase);

        return canEnter;
    }
}
