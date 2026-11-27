using CommunityToolkit.Mvvm.Input;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed record MatchExpulsionPrompt(
    long BallotId,
    string Heading,
    string CountText,
    string StatusText,
    bool CanRespond,
    bool CanHide,
    string AcceptText,
    string RejectText,
    string HideText,
    IAsyncRelayCommand AcceptCommand,
    IAsyncRelayCommand RejectCommand,
    IRelayCommand HideCommand)
{
    public bool HasStatus => StatusText.Length > 0;
}
