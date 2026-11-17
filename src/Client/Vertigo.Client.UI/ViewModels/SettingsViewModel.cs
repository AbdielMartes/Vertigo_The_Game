using System;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Settings.Interfaces;
using Vertigo.Client.UI.Utilities.Settings.Models;
using Vertigo.Client.UI.Utilities.Settings.Services;
using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UI.ViewModels;

public sealed class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly INavigator _navigation;
    private readonly IUserSettingsStore _store;
    private readonly ITextLocalizer _text;
    private string _language = "es";
    private string _errorMessage = string.Empty;

    public SettingsViewModel(INavigator navigation, IUserSettingsStore store, ITextLocalizer text)
    {
        _navigation = navigation;
        _store = store;
        _text = text;
        BackCommand = new RelayCommand(() => _navigation.NavigateTo(NavigationPage.Home));
        SaveCommand = new RelayCommand(Save);
    }

    public string Language
    {
        get => _language;
        set
        {
            if (SetProperty(ref _language, value))
            {
                OnPropertyChanged(nameof(LanguageIndex));
            }
        }
    }

    public int LanguageIndex
    {
        get => Language == "en" ? 1 : 0;
        set
        {
            Language = value == 1 ? "en" : "es";
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public string HeadingText => _text.Get("Settings.Title");
    public string LanguageText => _text.Get("Settings.Language");
    public string SpanishText => _text.Get("Settings.Spanish");
    public string EnglishText => _text.Get("Settings.English");
    public string SaveText => _text.Get("Settings.Save");
    public string BackText => _text.Get("Navigation.Back");
    public IRelayCommand BackCommand { get; }
    public IRelayCommand SaveCommand { get; }

    public void OnNavigatedTo()
    {
        Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en" ? "en" : "es";
        ErrorMessage = string.Empty;
        RefreshText();
    }

    public void OnNavigatedFrom()
    {
        ErrorMessage = string.Empty;
    }

    private void Save()
    {
        if (JsonUserSettingsStore.IsSupportedLanguage(Language))
        {
            if (_store.TrySave(new UserSettings { Language = Language }))
            {
                UiLanguage.Apply(Language);
                ErrorMessage = string.Empty;
                RefreshText();
            }
            else
            {
                ErrorMessage = _text.Get("Settings.SaveFailed");
            }
        }
        else
        {
            ErrorMessage = _text.Get("Settings.LanguageInvalid");
        }
    }

    private void RefreshText()
    {
        OnPropertyChanged(nameof(HeadingText));
        OnPropertyChanged(nameof(LanguageText));
        OnPropertyChanged(nameof(SpanishText));
        OnPropertyChanged(nameof(EnglishText));
        OnPropertyChanged(nameof(SaveText));
        OnPropertyChanged(nameof(BackText));
    }
}
