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

public sealed class BlockedPlayersViewModel : SocialViewModelBase
{
    private readonly IBlockedPlayerClient _client;
    private Guid _selectedPlayerId;

    public BlockedPlayersViewModel(IBlockedPlayerClient client, SocialOperation operation, ITextLocalizer text)
        : base(operation, text)
    {
        _client = client;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        BlockCommand = new AsyncRelayCommand(BlockAsync);
        UnblockCommand = new AsyncRelayCommand(UnblockAsync);
    }

    public ObservableCollection<BlockedPlayerSummary> BlockedPlayers { get; } = new();
    public Guid SelectedPlayerId { get => _selectedPlayerId; set => SetProperty(ref _selectedPlayerId, value); }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand BlockCommand { get; }
    public IAsyncRelayCommand UnblockCommand { get; }

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
            ClientResult<List<BlockedPlayerSummary>> result = await Task.Run(() => _client.GetBlockedPlayers(token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || result.Value is null) { ErrorMessage = Operation.Error(result, "Social.BlockFailed"); return; }
            BlockedPlayers.Clear();
            foreach (BlockedPlayerSummary player in result.Value) BlockedPlayers.Add(player);
            ClearError();
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.BlockFailed");
            }
        }
        finally { IsBusy = false; }
    }

    private Task BlockAsync() => ChangeAsync(true);
    private Task UnblockAsync() => ChangeAsync(false);

    private async Task ChangeAsync(bool block)
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
            ClientResult<bool> result = await Task.Run(() => block
                ? _client.BlockPlayer(SelectedPlayerId, token)
                : _client.UnblockPlayer(SelectedPlayerId, token));
            if (!Operation.Check(result, session)) return;
            if (!result.IsSuccess || !result.Value) { ErrorMessage = Operation.Error(result, "Social.BlockFailed"); return; }
        }
        catch (Exception)
        {
            if (Operation.IsCurrent(session))
            {
                ErrorMessage = Text.Get("Social.BlockFailed");
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
        SelectedPlayerId = Guid.Empty;
        BlockedPlayers.Clear();
        ClearError();
    }
}
