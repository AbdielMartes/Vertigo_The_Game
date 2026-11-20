using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed partial class MatchViewModel
{
    private const int MaximumVisibleChatMessages = 100;
    private const string ChatSendFailed = "Lobby.MessageFailed";

    private readonly IChatClient? _chatClient;
    private string _draftMessage = string.Empty;
    private string _chatErrorMessage = string.Empty;

    public IAsyncRelayCommand SendChatCommand { get; }
    public ObservableCollection<MatchChatRow> ChatMessages { get; } = new();

    public string DraftMessage
    {
        get => _draftMessage;
        set
        {
            if (SetProperty(ref _draftMessage, value ?? string.Empty))
            {
                OnPropertyChanged(nameof(ChatLengthLabel));
                SendChatCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string ChatLengthLabel => $"{DraftMessage.Length}/{MatchChatRules.MaximumLength}";

    public string ChatErrorMessage
    {
        get => _chatErrorMessage;
        private set
        {
            if (SetProperty(ref _chatErrorMessage, value))
            {
                OnPropertyChanged(nameof(HasChatError));
            }
        }
    }

    public bool HasChatError => !string.IsNullOrEmpty(ChatErrorMessage);

    private bool CanSendChat()
    {
        return _chatClient is not null && !NeedsReconnect && !MatchUnavailable && !_isLeavingMatch
            && !IsExitConfirmationOpen && MatchChatRules.CanSend(DraftMessage)
            && _session?.GetActiveSession() is { } session
            && _voting.Board is { EndReason: MatchEndReason.None, TurnDeadlineUtc: not null } board
            && board.Players.Exists(player => player.PlayerId == session.PlayerId);
    }

    private async Task SendChatAsync()
    {
        SessionSnapshot? session = _session?.GetActiveSession();
        if (session is null || _chatClient is null || !CanSendChat() || _guard?.HasActiveSession() != true)
        {
            return;
        }

        BoardRequestIdentity identity = new(session, _matchId, _generation);
        MatchChatRequest request = new()
        {
            LobbyCode = _lobbyCode, MatchId = _matchId, SessionToken = session.Token, Text = DraftMessage
        };
        ChatErrorMessage = string.Empty;
        ClientResult<bool> result;
        try
        {
            result = await Task.Run(() => _chatClient.SendMatchMessage(request));
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Match chat send failed: {exception.GetType().Name}.");
            result = ClientResult<bool>.TechnicalFailure(FailureKind.Unexpected);
        }

        await Dispatcher.UIThread.InvokeAsync(() => ApplyChatSendResult(identity, request.Text, result));
    }

    private void ApplyChatSendResult(BoardRequestIdentity identity, string sentText, ClientResult<bool> result)
    {
        if (!IsCurrent(identity.Session, identity.MatchId, identity.Generation))
        {
            return;
        }

        if (_errors?.IsUnauthorized(result) == true)
        {
            _guard?.Invalidate(identity.Session);
        }
        else if (!result.IsSuccess || !result.Value)
        {
            ChatErrorMessage = _errors?.FromResult(result, ChatSendFailed) ?? _text.Get(ChatSendFailed);
        }
        else if (DraftMessage == sentText)
        {
            DraftMessage = string.Empty;
        }
    }

    private void OnMatchChatReceived(object? sender, LobbyChatMessage message)
    {
        SessionSnapshot? session = _session?.GetActiveSession();
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (session is not null && message.MatchId is Guid matchId && IsCurrent(session, matchId, generation))
            {
                ApplyChatMessage(message);
            }
        });
    }

    internal void ApplyChatMessage(LobbyChatMessage message)
    {
        if (_matchId == Guid.Empty || message.MatchId != _matchId
            || !string.Equals(message.LobbyCode, _lobbyCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        ChatMessages.Insert(0, new MatchChatRow
        {
            Sender = message.SenderUsername,
            Text = message.Text,
            SentAt = message.SentAt
        });
        if (ChatMessages.Count > MaximumVisibleChatMessages)
        {
            ChatMessages.RemoveAt(ChatMessages.Count - 1);
        }
    }
}
