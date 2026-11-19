using System;

namespace Vertigo.Client.UI.Utilities.Validation;

public readonly record struct MatchBoardIdentity(string LobbyCode, Guid MatchId);
