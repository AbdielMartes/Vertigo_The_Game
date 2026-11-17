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

public sealed class FriendsViewModel : SocialViewModelBase
{
    private readonly IFriendshipClient _client;
    private string _searchTerm = string.Empty;
    private Guid _selectedPlayerId;
    private Guid _pendingSenderId;

    public FriendsViewModel(IFriendshipClient client, SocialOperation operation, ITextLocalizer text)
        : base(operation, text)
    {
        _client = client;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        SendRequestCommand = new AsyncRelayCommand(SendRequestAsync);
        AcceptCommand = new AsyncRelayCommand(() => RespondAsync(true));
        RejectCommand = new AsyncRelayCommand(() => RespondAsync(false));
        RemoveCommand = new AsyncRelayCommand(RemoveAsync);
    }

    public ObservableCollection<FriendEntry> Friends { get; } = new();
    public ObservableCollection<PlayerSearchEntry> SearchResults { get; } = new();
    public ObservableCollection<FriendshipInvitationEntry> PendingRequests { get; } = new();
    public string SearchTerm { get => _searchTerm; set => SetProperty(ref _searchTerm, value); }
    public Guid SelectedPlayerId { get => _selectedPlayerId; set => SetProperty(ref _selectedPlayerId, value); }
    public Guid PendingSenderId { get => _pendingSenderId; set => SetProperty(ref _pendingSenderId, value); }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }
    public IAsyncRelayCommand SendRequestCommand { get; }
    public IAsyncRelayCommand AcceptCommand { get; }
    public IAsyncRelayCommand RejectCommand { get; }
    public IAsyncRelayCommand RemoveCommand { get; }

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
        ClearError();
        try
        {
            ClientResult<List<FriendEntry>> friends = await Task.Run(() => _client.GetFriends(token));
            if (!Operation.Check(friends, session)) return;
            if (!friends.IsSuccess || friends.Value is null) { ErrorMessage = Operation.Error(friends, "Social.FriendsFailed"); return; }
            ClientResult<List<FriendshipInvitationEntry>> pending = await Task.Run(() => _client.GetPendingFriendRequests(token));
            if (!Operation.Check(pending, session)) return;
            if (!pending.IsSuccess || pending.Value is null) { ErrorMessage = Operation.Error(pending, "Social.FriendsFailed"); return; }
            Friends.Clear();
            foreach (FriendEntry friend in friends.Value) Friends.Add(friend);
            PendingRequests.Clear();
            foreach (FriendshipInvitationEntry request in pending.Value) PendingRequests.Add(request);
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.FriendsFailed");
            }
        }
        finally { IsBusy = false; }
    }

    private async Task SearchAsync()
    {
        if (IsBusy || SearchTerm.Trim().Length < 3) { ErrorMessage = Text.Get("Social.SearchLength"); return; }
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            ClientResult<List<PlayerSearchEntry>> result = await Task.Run(() => _client.SearchPlayers(SearchTerm.Trim(), token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || result.Value is null) { ErrorMessage = Operation.Error(result, "Social.SearchFailed"); return; }
            SearchResults.Clear();
            foreach (PlayerSearchEntry player in result.Value) SearchResults.Add(player);
            ClearError();
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.SearchFailed");
            }
        }
        finally { IsBusy = false; }
    }

    private async Task SendRequestAsync()
    {
        if (IsBusy || SelectedPlayerId == Guid.Empty) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            ClientResult<PlayerRelation> result = await Task.Run(() => _client.SendFriendRequest(SelectedPlayerId, token));
            if (!Operation.Check(result, session)) return;
            ErrorMessage = result.IsSuccess ? string.Empty : Operation.Error(result, "Social.RequestFailed");
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.RequestFailed");
            }
        }
        finally { IsBusy = false; }
    }

    private async Task RespondAsync(bool accept)
    {
        if (IsBusy || PendingSenderId == Guid.Empty) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            ClientResult<bool> result = await Task.Run(() => _client.RespondToFriendship(PendingSenderId, accept, token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || !result.Value) { ErrorMessage = Operation.Error(result, "Social.RequestFailed"); return; }
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.RequestFailed");
            }
        }
        finally { IsBusy = false; }
        if (Operation.IsCurrent(session))
        {
            await RefreshAsync();
        }
    }

    private async Task RemoveAsync()
    {
        if (IsBusy || SelectedPlayerId == Guid.Empty) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        IsBusy = true;
        try
        {
            ClientResult<bool> result = await Task.Run(() => _client.RemoveFriendship(SelectedPlayerId, token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || !result.Value) { ErrorMessage = Operation.Error(result, "Social.RequestFailed"); return; }
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.RequestFailed");
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
        Friends.Clear(); SearchResults.Clear(); PendingRequests.Clear();
        SelectedPlayerId = Guid.Empty; PendingSenderId = Guid.Empty;
        SearchTerm = string.Empty; ClearError();
    }
}
