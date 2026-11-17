using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed class AccessViewModel : ObservableObject, INavigationAware
{
    private readonly ISessionCoordinator _sessions;
    private readonly INavigator _navigation;
    private readonly IClientErrorText _errors;
    private readonly ITextLocalizer _text;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public AccessViewModel(AccessContext context)
    {
        _sessions = context.Sessions;
        _navigation = context.Ui.Navigation;
        _errors = context.Ui.Services.Errors;
        _text = context.Ui.Services.Text;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        GuestCommand = new AsyncRelayCommand(EnterAsGuestAsync);
        ResumeCommand = new AsyncRelayCommand<string>(ResumeSessionAsync);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    // In-memory only. A future secure storage service may supply this value.
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

    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand GuestCommand { get; }
    public IAsyncRelayCommand ResumeCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = _text.Get("Access.MissingCredentials");
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            // Existing WCF proxies are synchronous; offload their blocking calls from the UI thread.
            string username = Username.Trim();
            string password = Password;
            ClientResult<UserSession> result = await Task.Run(() => _sessions.Login(username, password));
            CompleteAccess(result);
        }
        catch (Exception)
        {
            ErrorMessage = _text.Get("Access.LoginFailed");
        }
        finally
        {
            Password = string.Empty;
            IsBusy = false;
        }
    }

    private async Task EnterAsGuestAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            ClientResult<UserSession> result = await Task.Run(_sessions.CreateGuestSession);
            CompleteAccess(result);
        }
        catch (Exception)
        {
            ErrorMessage = _text.Get("Access.GuestFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ResumeSessionAsync(string? sessionToken)
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            ErrorMessage = _text.Get("Access.MissingToken");
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            ClientResult<UserSession> result = await Task.Run(() => _sessions.ResumeSession(sessionToken));
            CompleteAccess(result);
        }
        catch (Exception)
        {
            ErrorMessage = _text.Get("Access.ResumeFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void OnNavigatedTo()
    {
        ErrorMessage = string.Empty;
        Password = string.Empty;
    }

    public void OnNavigatedFrom()
    {
        Password = string.Empty;
    }

    private void CompleteAccess(ClientResult<UserSession> result)
    {
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessage = _errors.FromResult(result, "Access.LoginFailed");
            return;
        }

        _navigation.NavigateTo(NavigationPage.Home);
    }
}
