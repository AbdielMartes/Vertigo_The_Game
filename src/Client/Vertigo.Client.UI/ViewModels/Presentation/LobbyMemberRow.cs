using Avalonia.Media;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class LobbyMemberRow
{
    public required string Username { get; init; }
    public IImage? Avatar { get; init; }
    public required string RoleText { get; init; }
    public required string ReadyText { get; init; }
    public bool IsReady { get; init; }
}
