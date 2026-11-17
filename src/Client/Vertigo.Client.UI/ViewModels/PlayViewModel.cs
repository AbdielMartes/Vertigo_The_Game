using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Validation;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed class PlayViewModel : ObservableObject, INavigationAware
{
    private readonly INavigator _navigation;
    private readonly ISessionGuard _guard;
    private readonly PlayerSession _session;
    private readonly LobbyViewModel _lobby;
    private readonly ILobbyClient _lobbies;
    private readonly ITextLocalizer _text;
    private bool _isBusy;
    private bool _isPublic = true;
    private int _maxPlayers = 4;
    private string _lobbyCode = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isPublicListOpen;
    private LobbySummary? _selectedPublicLobby;
    private int _generation;
    private string _existingLobbyCode = string.Empty;
    private bool _isCheckingMembership;

    public PlayViewModel(HomeContext context, LobbyViewModel lobby, ILobbyClient lobbies)
    {
        _navigation = context.Ui.Navigation;
        _guard = context.Ui.Services.SessionGuard;
        _session = context.PlayerSession;
        _text = context.Ui.Services.Text;
        _lobby = lobby;
        _lobbies = lobbies;
        PublicLobbies = new ObservableCollection<LobbySummary>();
        BackCommand = new RelayCommand(Back);
        CreateCommand = new AsyncRelayCommand(CreateAsync);
        JoinCodeCommand = new AsyncRelayCommand(JoinCodeAsync);
        RefreshPublicCommand = new AsyncRelayCommand(RefreshPublicAsync);
        JoinSelectedCommand = new AsyncRelayCommand(JoinSelectedAsync);
        TogglePublicCommand = new AsyncRelayCommand(TogglePublicAsync);
        ResumeLobbyCommand = new AsyncRelayCommand(ResumeLobbyAsync);
    }

    public ObservableCollection<LobbySummary> PublicLobbies { get; }
    public bool HasPublicLobbies => PublicLobbies.Count > 0;
    public bool IsPublicListEmpty => !IsBusy && !HasPublicLobbies;
    public IAsyncRelayCommand ResumeLobbyCommand { get; }
    public string ExistingLobbyCode
    {
        get => _existingLobbyCode;
        private set
        {
            if (SetProperty(ref _existingLobbyCode, value))
            {
                OnPropertyChanged(nameof(HasExistingLobby));
            }
        }
    }
    public bool HasExistingLobby => !string.IsNullOrWhiteSpace(ExistingLobbyCode);
    public string ResumeLobbyText => _text.Get("Play.ResumeLobby");
    public IRelayCommand BackCommand { get; }
    public IAsyncRelayCommand CreateCommand { get; }
    public IAsyncRelayCommand JoinCodeCommand { get; }
    public IAsyncRelayCommand RefreshPublicCommand { get; }
    public IAsyncRelayCommand JoinSelectedCommand { get; }
    public IAsyncRelayCommand TogglePublicCommand { get; }

    public bool IsPublic
    {
        get => _isPublic;
        set => SetProperty(ref _isPublic, value);
    }

    public int MaxPlayers
    {
        get => _maxPlayers;
        set => SetProperty(ref _maxPlayers, value);
    }

    public string LobbyCode
    {
        get => _lobbyCode;
        set => SetProperty(ref _lobbyCode, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public bool IsPublicListOpen
    {
        get => _isPublicListOpen;
        private set => SetProperty(ref _isPublicListOpen, value);
    }

    public LobbySummary? SelectedPublicLobby
    {
        get => _selectedPublicLobby;
        set => SetProperty(ref _selectedPublicLobby, value);
    }

    public string TitleText => _text.Get("Play.Title");
    public string SubtitleText => _text.Get("Play.Subtitle");
    public string CreateTitleText => _text.Get("Play.CreateTitle");
    public string PublicText => _text.Get("Play.Public");
    public string MaxPlayersText => _text.Get("Play.MaxPlayers");
    public string CreateText => _text.Get("Play.Create");
    public string JoinTitleText => _text.Get("Play.JoinTitle");
    public string CodeText => _text.Get("Play.Code");
    public string JoinText => _text.Get("Play.Join");
    public string BrowseTitleText => _text.Get("Play.BrowseTitle");
    public string BrowseText => _text.Get("Play.Browse");
    public string RefreshText => _text.Get("Play.Refresh");
    public string EmptyText => _text.Get("Play.Empty");
    public string RoomText => _text.Get("Play.Room");
    public string HostText => _text.Get("Play.Host");
    public string PlayersText => _text.Get("Play.Players");
    public string BackText => _text.Get("Navigation.Back");

    public void OnNavigatedTo()
    {
        _generation++;
        _isCheckingMembership = false;
        ErrorMessage = string.Empty;
        IsBusy = false;
        RefreshLanguage();
        _ = CheckMembershipAsync();
    }

    public void OnNavigatedFrom()
    {
        _generation++;
        _isCheckingMembership = false;
        ExistingLobbyCode = string.Empty;
        ErrorMessage = string.Empty;
        IsPublicListOpen = false;
        SelectedPublicLobby = null;
        PublicLobbies.Clear();
        OnPropertyChanged(nameof(HasPublicLobbies));
        OnPropertyChanged(nameof(IsPublicListEmpty));
    }

    private void Back()
    {
        if (!IsBusy)
        {
            _navigation.NavigateTo(NavigationPage.Home);
        }
    }

    private async Task TogglePublicAsync()
    {
        if (!IsBusy)
        {
            if (IsPublicListOpen)
            {
                IsPublicListOpen = false;
                ErrorMessage = string.Empty;
            }
            else
            {
                await RefreshPublicAsync();
            }
        }
    }

    private bool IsCurrent(SessionSnapshot? original, int generation)
    {
        return original is not null
            && ReferenceEquals(_session.GetActiveSession(), original)
            && _generation == generation
            && _navigation.CurrentPage == NavigationPage.Play;
    }

    private async Task CreateAsync()
    {
        if (HasExistingLobby)
        {
            ErrorMessage = _text.Get("Lobby.PlayerAlreadyInLobby");
            return;
        }

        SessionSnapshot? current = _session.GetActiveSession();
        int generation = _generation;
        if (!IsBusy && _guard.HasActiveSession() && current is not null)
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                if (MaxPlayers < 2 || MaxPlayers > 6)
                {
                    ErrorMessage = _text.Get("Play.InvalidCapacity");
                }
                else
                {
                    bool success = await _lobby.CreateAsync(IsPublic, MaxPlayers);
                    if (IsCurrent(current, generation) && !success)
                    {
                        ErrorMessage = _lobby.ErrorMessage;
                        await CheckMembershipAsync();
                    }
                }
            }
            catch (Exception)
            {
                if (IsCurrent(current, generation))
                {
                    ErrorMessage = _text.Get("Play.NetworkError");
                }
            }
            finally
            {
                if (_generation == generation)
                {
                    IsBusy = false;
                }
            }
        }
    }

    private async Task CheckMembershipAsync()
    {
        SessionSnapshot? session = _session.GetActiveSession();
        int generation = _generation;
        if (_isCheckingMembership || !_guard.HasActiveSession() || session is null)
        {
            return;
        }

        _isCheckingMembership = true;
        try
        {
            LobbyPreparation? current = await Task.Run(() => _lobbies.GetCurrentLobby(session.Token));
            if (IsCurrent(session, generation))
            {
                ExistingLobbyCode = current?.LobbyCode ?? string.Empty;
            }
        }
        catch (Exception)
        {
            // A failed optional lookup must not prevent a normal join or creation.
            // The server remains authoritative for the one-lobby rule.
        }
        finally
        {
            if (generation == _generation)
            {
                _isCheckingMembership = false;
            }
        }
    }

    private async Task ResumeLobbyAsync()
    {
        string code = ExistingLobbyCode;
        if (!IsBusy && LobbyCodeRules.TryNormalize(code, out string normalized))
        {
            await JoinAsync(normalized);
        }
    }

    private async Task JoinCodeAsync()
    {
        if (LobbyCodeRules.TryNormalize(LobbyCode, out string normalized))
        {
            await JoinAsync(normalized);
        }
        else
        {
            ErrorMessage = _text.Get("Play.InvalidCode");
        }
    }

    private async Task JoinSelectedAsync()
    {
        LobbySummary? selected = SelectedPublicLobby;
        if (selected is not null && LobbyCodeRules.TryNormalize(selected.LobbyCode, out string code))
        {
            await JoinAsync(code);
        }
        else
        {
            ErrorMessage = _text.Get("Play.SelectRoom");
        }
    }

    private async Task JoinAsync(string code)
    {
        SessionSnapshot? current = _session.GetActiveSession();
        int generation = _generation;
        if (!IsBusy && _guard.HasActiveSession() && current is not null)
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                bool success = await _lobby.JoinAsync(code);
                if (IsCurrent(current, generation) && !success)
                {
                    ErrorMessage = _lobby.ErrorMessage;
                }
            }
            catch (Exception)
            {
                if (IsCurrent(current, generation))
                {
                    ErrorMessage = _text.Get("Play.NetworkError");
                }
            }
            finally
            {
                if (_generation == generation)
                {
                    IsBusy = false;
                }
            }
        }
    }

    private async Task RefreshPublicAsync()
    {
        SessionSnapshot? current = _session.GetActiveSession();
        int generation = _generation;
        if (!IsBusy && _guard.HasActiveSession() && current is not null)
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsPublicListEmpty));
            ErrorMessage = string.Empty;
            IsPublicListOpen = true;
            try
            {
                List<LobbySummary> result = await Task.Run(_lobbies.GetPublicLobbies);
                if (IsCurrent(current, generation))
                {
                    PublicLobbies.Clear();
                    SelectedPublicLobby = null;
                    foreach (LobbySummary lobby in result)
                    {
                        if (lobby.CurrentPlayers < lobby.MaxPlayers)
                        {
                            PublicLobbies.Add(lobby);
                        }
                    }
                    OnPropertyChanged(nameof(HasPublicLobbies));
                    OnPropertyChanged(nameof(IsPublicListEmpty));
                }
            }
            catch (Exception)
            {
                if (IsCurrent(current, generation))
                {
                    ErrorMessage = _text.Get("Play.NetworkError");
                }
            }
            finally
            {
                if (_generation == generation)
                {
                    IsBusy = false;
                    OnPropertyChanged(nameof(IsPublicListEmpty));
                }
            }
        }
    }

    private void RefreshLanguage()
    {
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(SubtitleText));
        OnPropertyChanged(nameof(CreateTitleText));
        OnPropertyChanged(nameof(PublicText));
        OnPropertyChanged(nameof(MaxPlayersText));
        OnPropertyChanged(nameof(CreateText));
        OnPropertyChanged(nameof(JoinTitleText));
        OnPropertyChanged(nameof(CodeText));
        OnPropertyChanged(nameof(JoinText));
        OnPropertyChanged(nameof(BrowseTitleText));
        OnPropertyChanged(nameof(BrowseText));
        OnPropertyChanged(nameof(RefreshText));
        OnPropertyChanged(nameof(EmptyText));
        OnPropertyChanged(nameof(RoomText));
        OnPropertyChanged(nameof(HostText));
        OnPropertyChanged(nameof(PlayersText));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(ResumeLobbyText));
    }
}
