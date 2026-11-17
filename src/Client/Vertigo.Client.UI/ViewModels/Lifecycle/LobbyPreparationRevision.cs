using System;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Lifecycle;

/// <summary>Keeps the latest accepted preparation revision for the active lobby.</summary>
public sealed class LobbyPreparationRevision
{
    private long _revision = -1;

    public bool TryAdvance(LobbyPreparation state, string activeLobbyCode)
    {
        bool accepted = !string.IsNullOrWhiteSpace(activeLobbyCode)
            && string.Equals(state.LobbyCode, activeLobbyCode, StringComparison.OrdinalIgnoreCase)
            && state.Revision > _revision;

        if (accepted)
        {
            _revision = state.Revision;
        }

        return accepted;
    }

    public void Reset()
    {
        _revision = -1;
    }
}
