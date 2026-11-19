using System.Collections.Generic;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed record MatchRoundResultRow(
    int RoundNumber, string Title, IReadOnlyList<MatchRoundPlayerScore> Scores);
