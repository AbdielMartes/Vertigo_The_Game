using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchPlayerRow
{
    public required Guid PlayerId { get; init; }
    public required string Username { get; init; }
    public IImage? Avatar { get; init; }
    public int FloorsBuilt { get; init; }
    public bool IsCurrentTurn { get; init; }
    public bool IsMe { get; init; }
    public string VoteText { get; init; } = string.Empty;
    public bool HasVoteButton => VoteCommand is not null;
    public IAsyncRelayCommand? VoteCommand { get; init; }
}
