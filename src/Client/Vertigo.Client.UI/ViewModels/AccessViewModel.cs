using System;
using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Client.UI.Utilities.Validation;

namespace Vertigo.Client.UI.ViewModels;

public sealed class AccessViewModel : ObservableObject, INavigationAware
{
    private readonly ISessionCoordinator _sessions;
    private readonly IAccountClient _accounts;
    private readonly INavigator _navigation;
    private readonly IClientErrorText _errors;
    private readonly ITextLocalizer _text;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _isRegistering;
    private string _registerUsername = string.Empty;
    private string _registerEmail = string.Empty;
    private string _registerPassword = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _registrationMessage = string.Empty;
    private string _registerUsernameError = string.Empty;
    private string _registerEmailError = string.Empty;
    private string _registerPasswordError = string.Empty;
    private string _confirmPasswordError = string.Empty;

    public AccessViewModel(AccessContext context, IAccountClient accounts)
    {
        _sessions = context.Sessions;
        _accounts = accounts;
        _navigation = context.Ui.Navigation;
        _errors = context.Ui.Services.Errors;
        _text = context.Ui.Services.Text;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        GuestCommand = new AsyncRelayCommand(EnterAsGuestAsync);
        ResumeCommand = new AsyncRelayCommand<string>(ResumeSessionAsync);
        ToggleLanguageCommand = new RelayCommand(ToggleLanguage);
        ShowRegistrationCommand = new RelayCommand(ShowRegistration);
        CancelRegistrationCommand = new RelayCommand(CancelRegistration);
        RegisterCommand = new AsyncRelayCommand(RegisterAsync);
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

    public bool IsRegistering
    {
        get => _isRegistering;
        private set => SetProperty(ref _isRegistering, value);
    }

    public string RegisterUsername
    {
        get => _registerUsername;
        set => SetProperty(ref _registerUsername, value);
    }

    public string RegisterEmail
    {
        get => _registerEmail;
        set => SetProperty(ref _registerEmail, value);
    }

    public string RegisterPassword
    {
        get => _registerPassword;
        set => SetProperty(ref _registerPassword, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public string RegistrationMessage
    {
        get => _registrationMessage;
        private set => SetProperty(ref _registrationMessage, value);
    }

    public string RegisterUsernameError
    {
        get => _registerUsernameError;
        private set => SetProperty(ref _registerUsernameError, value);
    }

    public string RegisterEmailError
    {
        get => _registerEmailError;
        private set => SetProperty(ref _registerEmailError, value);
    }

    public string RegisterPasswordError
    {
        get => _registerPasswordError;
        private set => SetProperty(ref _registerPasswordError, value);
    }

    public string ConfirmPasswordError
    {
        get => _confirmPasswordError;
        private set => SetProperty(ref _confirmPasswordError, value);
    }

    public string UsernameHint => _text.Get("Access.UsernameHint");
    public string PasswordHint => _text.Get("Access.PasswordHint");

    public string TitleText => _text.Get("Access.Title");
    public string DescriptionText => _text.Get("Access.Description");
    public string UsernameLabel => _text.Get("Access.Username");
    public string PasswordLabel => _text.Get("Access.Password");
    public string LoginText => _text.Get("Access.Login");
    public string GuestText => _text.Get("Access.Guest");
    public string ProgressText => _text.Get("Access.Progress");
    public string ErrorHeadingText => _text.Get("Access.ErrorHeading");
    public string HelpText => _text.Get("Access.Help");
    public string RegisterText => _text.Get("Access.Register");
    public string RegisterHeadingText => _text.Get("Access.RegisterHeading");
    public string CreateAccountText => _text.Get("Access.CreateAccount");
    public string EmailLabel => _text.Get("Access.Email");
    public string ConfirmPasswordLabel => _text.Get("Access.ConfirmPassword");
    public string BackText => _text.Get("Access.Back");
    public string LanguageButtonText => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es" ? "English" : "Español";
    public string LanguageButtonHelp => _text.Get("Access.ChangeLanguage");

    public IRelayCommand ToggleLanguageCommand { get; }
    public IRelayCommand ShowRegistrationCommand { get; }
    public IRelayCommand CancelRegistrationCommand { get; }
    public IAsyncRelayCommand RegisterCommand { get; }

    private void ToggleLanguage()
    {
        if (!IsBusy)
        {
            string targetLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es" ? "en" : "es";
            CultureInfo target = CultureInfo.GetCultureInfo(targetLanguage);
            CultureInfo.CurrentUICulture = target;
            CultureInfo.DefaultThreadCurrentUICulture = target;
            ErrorMessage = string.Empty;
            RegistrationMessage = string.Empty;
            ClearRegistrationErrors();
            NotifyLocalizationChanged();
        }
    }

    private void NotifyLocalizationChanged()
    {
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(DescriptionText));
        OnPropertyChanged(nameof(UsernameLabel));
        OnPropertyChanged(nameof(PasswordLabel));
        OnPropertyChanged(nameof(LoginText));
        OnPropertyChanged(nameof(GuestText));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(ErrorHeadingText));
        OnPropertyChanged(nameof(HelpText));
        OnPropertyChanged(nameof(RegisterText));
        OnPropertyChanged(nameof(RegisterHeadingText));
        OnPropertyChanged(nameof(CreateAccountText));
        OnPropertyChanged(nameof(EmailLabel));
        OnPropertyChanged(nameof(ConfirmPasswordLabel));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(LanguageButtonText));
        OnPropertyChanged(nameof(LanguageButtonHelp));
        OnPropertyChanged(nameof(UsernameHint));
        OnPropertyChanged(nameof(PasswordHint));
    }

    private void ShowRegistration()
    {
        if (!IsBusy)
        {
            ErrorMessage = string.Empty;
            Password = string.Empty;
            RegistrationMessage = string.Empty;
            ClearRegistrationErrors();
            IsRegistering = true;
        }
    }

    private void CancelRegistration()
    {
        if (!IsBusy)
        {
            ClearRegistrationSecrets();
            ClearRegistrationErrors();
            ErrorMessage = string.Empty;
            RegistrationMessage = string.Empty;
            IsRegistering = false;
        }
    }

    private async Task RegisterAsync()
    {
        if (!IsBusy && IsRegistering)
        {
            bool isValid = ValidateRegistration();
            if (isValid)
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                RegistrationMessage = string.Empty;

                try
                {
                    AccountRegistration request = new()
                    {
                        Username = RegisterUsername.Trim(),
                        Email = RegisterEmail.Trim(),
                        Password = RegisterPassword
                    };
                    ClientResult<bool> result = await Task.Run(() => _accounts.RegisterAccount(request));
                    if (result.IsSuccess && result.Value)
                    {
                        Username = request.Username;
                        RegistrationMessage = _text.Get("Access.RegistrationSuccess");
                        IsRegistering = false;
                    }
                    else
                    {
                        ErrorMessage = _errors.FromResult(result, "Access.RegistrationFailed");
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = _text.Get("Access.RegistrationFailed");
                }
                finally
                {
                    ClearRegistrationSecrets();
                    IsBusy = false;
                }
            }
        }
    }

    private bool ValidateRegistration()
    {
        FieldValidationError username = RegistrationRules.ValidateUsername(RegisterUsername);
        FieldValidationError email = RegistrationRules.ValidateEmail(RegisterEmail);
        FieldValidationError password = RegistrationRules.ValidatePassword(RegisterPassword);
        FieldValidationError confirmation = RegistrationRules.ValidateConfirmation(RegisterPassword, ConfirmPassword);

        RegisterUsernameError = GetValidationText("Username", username);
        RegisterEmailError = GetValidationText("Email", email);
        RegisterPasswordError = GetValidationText("Password", password);
        ConfirmPasswordError = GetValidationText("ConfirmPassword", confirmation);

        bool isValid = username == FieldValidationError.None
            && email == FieldValidationError.None
            && password == FieldValidationError.None
            && confirmation == FieldValidationError.None;
        ErrorMessage = isValid ? string.Empty : _text.Get("Access.RegistrationInvalid");
        return isValid;
    }

    private string GetValidationText(string field, FieldValidationError error)
    {
        string text = string.Empty;
        if (error != FieldValidationError.None)
        {
            text = _text.Get($"Access.Validation.{field}.{error}");
        }
        return text;
    }

    private void ClearRegistrationErrors()
    {
        RegisterUsernameError = string.Empty;
        RegisterEmailError = string.Empty;
        RegisterPasswordError = string.Empty;
        ConfirmPasswordError = string.Empty;
    }

    private void ClearRegistrationSecrets()
    {
        RegisterPassword = string.Empty;
        ConfirmPassword = string.Empty;
    }

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
        ClearRegistrationSecrets();
        ClearRegistrationErrors();
        RegistrationMessage = string.Empty;
        IsRegistering = false;
    }

    public void OnNavigatedFrom()
    {
        Password = string.Empty;
        ClearRegistrationSecrets();
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
