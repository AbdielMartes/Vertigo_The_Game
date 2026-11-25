using System;
using Avalonia.Media;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed record RankingRow(int Position, Guid PlayerId, string Username, int MatchesWon, int FloorsBuilt,
    IImage? Avatar);
