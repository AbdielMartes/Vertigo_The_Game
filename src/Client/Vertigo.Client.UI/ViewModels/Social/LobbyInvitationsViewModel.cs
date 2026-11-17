using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertigo.Client.Core.Sessions;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Social;

public sealed class LobbyInvitationsViewModel : SocialViewModelBase
{
    private readonly ILobbyInvitationClient _client;
    private string _lobbyCode = string.Empty;
    private string _declinedCode = string.Empty;
    private Guid _targetPlayerId;

    public LobbyInvitationsViewModel(ILobbyInvitationClient client, SocialOperation operation,
        ITextLocalizer text) : base(operation, text)
    {
        _client = client;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        GetInvitableCommand = new AsyncRelayCommand(GetInvitableAsync);
        InviteCommand = new AsyncRelayCommand(InviteAsync);
        DeclineCommand = new AsyncRelayCommand(DeclineAsync);
    }

    public ObservableCollection<LobbyInvitationEntry> PendingInvitations { get; } = new();
    public ObservableCollection<FriendEntry> InvitableFriends { get; } = new();
    public string LobbyCode { get => _lobbyCode; set => SetProperty(ref _lobbyCode, value); }
    public string DeclinedCode { get => _declinedCode; set => SetProperty(ref _declinedCode, value); }
    public Guid TargetPlayerId { get => _targetPlayerId; set => SetProperty(ref _targetPlayerId, value); }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand GetInvitableCommand { get; }
    public IAsyncRelayCommand InviteCommand { get; }
    public IAsyncRelayCommand DeclineCommand { get; }

    public async Task RefreshAsync()
    {
        if (IsBusy) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            ClientResult<List<LobbyInvitationEntry>> result = await Task.Run(() => _client.GetPendingLobbyInvitations(token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || result.Value is null) { ErrorMessage = Operation.Error(result, "Social.InvitationFailed"); return; }
            PendingInvitations.Clear();
            foreach (LobbyInvitationEntry invitation in result.Value) PendingInvitations.Add(invitation);
            ClearError();
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.InvitationFailed");
            }
        }
        finally { IsBusy = false; }
    }

    private async Task GetInvitableAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(LobbyCode)) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            ClientResult<List<FriendEntry>> result = await Task.Run(() => _client.GetInvitableFriends(LobbyCode.Trim(), token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || result.Value is null) { ErrorMessage = Operation.Error(result, "Social.InvitationFailed"); return; }
            InvitableFriends.Clear();
            foreach (FriendEntry friend in result.Value) InvitableFriends.Add(friend);
            ClearError();
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.InvitationFailed");
            }
        }
        finally { IsBusy = false; }
    }

    private async Task InviteAsync()
    {
        if (IsBusy || TargetPlayerId == Guid.Empty || string.IsNullOrWhiteSpace(LobbyCode)) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            List<Guid> targets = new() { TargetPlayerId };
            ClientResult<List<Guid>> result = await Task.Run(() => _client.SendLobbyInvitations(LobbyCode.Trim(), targets, token));
            if (!Operation.Check(result, session)) return;
            ErrorMessage = result.IsSuccess ? string.Empty : Operation.Error(result, "Social.InvitationFailed");
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.InvitationFailed");
            }
        }
        finally { IsBusy = false; }
    }

    private async Task DeclineAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(DeclinedCode)) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            ClientResult<bool> result = await Task.Run(() => _client.DeclineLobbyInvitation(DeclinedCode.Trim(), token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || !result.Value) { ErrorMessage = Operation.Error(result, "Social.InvitationFailed"); return; }
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.InvitationFailed");
            }
        }
        finally { IsBusy = false; }
        if (Operation.IsCurrent(session))
        {
            await RefreshAsync();
        }
    }

    public void Reset()
    {
        PendingInvitations.Clear(); InvitableFriends.Clear();
        LobbyCode = string.Empty; DeclinedCode = string.Empty;
        TargetPlayerId = Guid.Empty; ClearError();
    }
}
