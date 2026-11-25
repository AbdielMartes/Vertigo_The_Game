using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Resources;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Contracts.Models;
using Vertigo.Registration;

namespace Vertigo.Client.UI.ViewModels;

public sealed class ProfileViewModel : ObservableObject, INavigationAware
{
    private readonly PlayerSession _session;
    private readonly UiNavigationContext _ui;
    private readonly IStatisticsClient _statistics;
    private readonly IAccountClient _account;
    private readonly ProfileNavigation _profileNavigation;
    private (Guid PlayerId, NavigationPage Source)? _target;
    private string _publicUsername = string.Empty;
    private int _generation;
    private PlayerMatchRecord? _record;
    private string _recordStatus = string.Empty;
    private bool _isEditing;
    private string _draftUsername = string.Empty;
    private string? _draftCountryText;
    private ProfileAvatarOption? _draftAvatar;
    private string _editError = string.Empty;

    public ProfileViewModel(PlayerSession session, UiNavigationContext ui, IStatisticsClient statistics,
        IAccountClient account, ProfileNavigation profileNavigation)
    {
        _session = session;
        _ui = ui;
        _statistics = statistics;
        _account = account;
        _profileNavigation = profileNavigation;
        BackCommand = new RelayCommand(() => _ui.Navigation.NavigateTo(_target?.Source ?? NavigationPage.Home));
        EditCommand = new RelayCommand(BeginEdit);
        CancelEditCommand = new RelayCommand(CancelEdit);
        SaveProfileCommand = new AsyncRelayCommand(SaveProfileAsync);
    }

    public IRelayCommand BackCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IRelayCommand CancelEditCommand { get; }
    public IAsyncRelayCommand SaveProfileCommand { get; }
    public IReadOnlyList<ProfileCountryOption> CountryChoices { get; private set; } = [];
    public Avalonia.Controls.AutoCompleteFilterPredicate<object> CountryFilter { get; } =
        (query, item) => item is ProfileCountryOption option && option.Matches(query ?? string.Empty);
    public IReadOnlyList<ProfileAvatarOption> AvatarChoices { get; private set; } = [];

    public string HeadingText => _ui.Services.Text.Get("Profile.Title");
    public string BackText => _ui.Services.Text.Get("Navigation.Back");
    public string MatchesPlayedText => _ui.Services.Text.Get("Profile.MatchesPlayed");
    public string MatchesWonText => _ui.Services.Text.Get("Profile.MatchesWon");
    public string WinRateText => _ui.Services.Text.Get("Profile.WinRate");
    public string CreatedYearText => _ui.Services.Text.Get("Profile.CreatedYear");
    public string CountryText => _ui.Services.Text.Get("Profile.Country");
    public string CountrySearchText => _ui.Services.Text.Get("Profile.CountrySearch");
    public string EditProfileText => _ui.Services.Text.Get("Profile.Edit");
    public string UsernameText => _ui.Services.Text.Get("Profile.Username");
    public string AvatarText => _ui.Services.Text.Get("Profile.Avatar");
    public string SaveText => _ui.Services.Text.Get("Profile.Save");
    public string CancelText => _ui.Services.Text.Get("Profile.Cancel");
    public int UsernameMaximumLength => RegistrationRules.UsernameMaximumLength;

    public bool ShowStatistics => _target.HasValue || IsOwnRegisteredProfile;
    private bool IsOwnRegisteredProfile => !_target.HasValue && _session.GetActiveSession() is { IsGuest: false };
    public bool CanShowEdit => IsOwnRegisteredProfile && !IsEditing;
    public bool IsRecordAvailable => _record is not null;
    public string Username => _target.HasValue ? _publicUsername : _session.GetActiveSession()?.Username ?? string.Empty;
    public string AccountType => _ui.Services.Text.Get(ShowStatistics ? "Home.RegisteredAccount" : "Home.GuestAccount");
    public IImage? Avatar
    {
        get
        {
            int? avatarId = _record is null && !_target.HasValue ? _session.GetActiveSession()?.AvatarId : _record?.AvatarId;
            return AvatarCatalog.Resolve(avatarId);
        }
    }

    public bool IsEditing
    {
        get => _isEditing;
        private set
        {
            if (SetProperty(ref _isEditing, value))
            {
                OnPropertyChanged(nameof(CanShowEdit));
            }
        }
    }

    public string DraftUsername
    {
        get => _draftUsername;
        set
        {
            if (SetProperty(ref _draftUsername, value))
            {
                EditError = string.Empty;
            }
        }
    }

    public string? DraftCountryText
    {
        get => _draftCountryText;
        set
        {
            if (SetProperty(ref _draftCountryText, value))
            {
                EditError = string.Empty;
            }
        }
    }

    public ProfileAvatarOption? DraftAvatar
    {
        get => _draftAvatar;
        set => SetProperty(ref _draftAvatar, value);
    }

    public string EditError
    {
        get => _editError;
        private set
        {
            if (SetProperty(ref _editError, value))
            {
                OnPropertyChanged(nameof(HasEditError));
            }
        }
    }

    public bool HasEditError => EditError.Length > 0;
    public string MatchesPlayed => _record?.MatchesPlayed.ToString(CultureInfo.CurrentUICulture) ?? string.Empty;
    public string MatchesWon => _record?.MatchesWon.ToString(CultureInfo.CurrentUICulture) ?? string.Empty;
    public string CreatedYear => _record?.CreatedYear?.ToString(CultureInfo.CurrentUICulture)
        ?? _ui.Services.Text.Get("Profile.YearUnavailable");
    public string Country => CountryCatalog.Normalize(_record?.CountryCode)
        ?? _ui.Services.Text.Get("Profile.CountryUnspecified");

    public string WinRate
    {
        get
        {
            return _record is { MatchesPlayed: > 0 } record
                ? ((double)record.MatchesWon / record.MatchesPlayed).ToString("P0", CultureInfo.CurrentUICulture)
                : _ui.Services.Text.Get("Profile.NoMatches");
        }
    }

    public string RecordStatus
    {
        get => _recordStatus;
        private set => SetProperty(ref _recordStatus, value);
    }

    public void OnNavigatedTo()
    {
        _target = _profileNavigation.Take();
        _publicUsername = string.Empty;
        _record = null;
        RecordStatus = ShowStatistics ? _ui.Services.Text.Get("Profile.Loading") : string.Empty;
        OnPropertyChanged(nameof(HeadingText));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(MatchesPlayedText));
        OnPropertyChanged(nameof(MatchesWonText));
        OnPropertyChanged(nameof(WinRateText));
        OnPropertyChanged(nameof(CreatedYearText));
        OnPropertyChanged(nameof(CountryText));
        OnPropertyChanged(nameof(CountrySearchText));
        OnPropertyChanged(nameof(EditProfileText));
        OnPropertyChanged(nameof(UsernameText));
        OnPropertyChanged(nameof(AvatarText));
        OnPropertyChanged(nameof(SaveText));
        OnPropertyChanged(nameof(CancelText));
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(AccountType));
        OnPropertyChanged(nameof(ShowStatistics));
        OnPropertyChanged(nameof(CanShowEdit));
        NotifyRecordChanged();
        int generation = ++_generation;
        if (_session.GetActiveSession() is { } session && ShowStatistics)
        {
            _ = _target is { } target
                ? LoadPublicRecordAsync(session, target.PlayerId, generation)
                : LoadRecordAsync(session, generation);
        }
    }

    public void OnNavigatedFrom()
    {
        _generation++;
        _record = null;
        RecordStatus = string.Empty;
        CancelEdit();
        NotifyRecordChanged();
    }

    private void BeginEdit()
    {
        if (IsOwnRegisteredProfile && _record is { } record)
        {
            CountryChoices = BuildCountryChoices();
            AvatarChoices = AvatarCatalog.Avatars.Select(avatar => new ProfileAvatarOption(
                (int)avatar, _ui.Services.Text.Get("Profile.Avatar." + avatar), AvatarCatalog.Resolve((int)avatar)))
                .ToArray();
            OnPropertyChanged(nameof(CountryChoices));
            OnPropertyChanged(nameof(AvatarChoices));
            DraftUsername = Username;
            string? code = CountryCatalog.Normalize(record.CountryCode);
            DraftCountryText = CountryChoices.First(option => option.Code == code).DisplayName;
            int selectedAvatarId = (int)AvatarCatalog.GetAvatar(record.AvatarId);
            DraftAvatar = AvatarChoices.First(option => option.Id == selectedAvatarId);
            EditError = string.Empty;
            IsEditing = true;
        }
    }

    private IReadOnlyList<ProfileCountryOption> BuildCountryChoices()
    {
        ProfileCountryOption unspecified = new(null, _ui.Services.Text.Get("Profile.CountryUnspecified"));
        IEnumerable<ProfileCountryOption> countries = CountryCatalog.Codes
            .Select(code => new ProfileCountryOption(code, GetCountryName(code)))
            .OrderBy(option => option.Name, StringComparer.Create(CultureInfo.CurrentUICulture, ignoreCase: true));
        return [unspecified, .. countries];
    }

    private string GetCountryName(string code)
    {
        string? name = TextLocalizer.FindTranslation("Country." + code);
        if (name is null)
        {
            try
            {
                RegionInfo region = new(code);
                name = region.TwoLetterISORegionName == code ? region.DisplayName : code;
            }
            catch (ArgumentException)
            {
                name = code;
            }
        }

        return name;
    }

    private void CancelEdit()
    {
        IsEditing = false;
        EditError = string.Empty;
    }

    private async Task SaveProfileAsync()
    {
        if (IsEditing && !_target.HasValue && _ui.Services.SessionGuard.HasActiveSession()
            && _session.GetActiveSession() is { IsGuest: false } session)
        {
            FieldValidationError validation = RegistrationRules.ValidateUsername(DraftUsername);
            ProfileCountryOption? country = CountryChoices.FirstOrDefault(
                option => option.MatchesExactly(DraftCountryText ?? string.Empty));
            if (validation != FieldValidationError.None)
            {
                EditError = _ui.Services.Text.Get("Access.Validation.Username." + validation);
            }
            else if (country is null)
            {
                EditError = _ui.Services.Text.Get("Profile.CountryInvalid");
            }
            else
            {
                EditError = string.Empty;
                ProfileUpdate profile = new()
                {
                    Username = DraftUsername.Trim(),
                    CountryCode = country.Code,
                    AvatarId = DraftAvatar?.Id
                };
                int generation = _generation;
                ClientResult<bool> result = await Task.Run(() => _account.UpdateProfile(profile, session.Token));
                ApplyProfileResult(session, profile, result, generation);
            }
        }
    }

    private void ApplyProfileResult(SessionSnapshot session, ProfileUpdate profile, ClientResult<bool> result,
        int generation)
    {
        if (result.IsSuccess && result.Value)
        {
            bool updated = _session.UpdateProfile(session, profile.Username, profile.AvatarId);
            if (updated && generation == _generation && _record is { } record)
            {
                record.CountryCode = profile.CountryCode;
                record.AvatarId = profile.AvatarId;
                OnPropertyChanged(nameof(Username));
                NotifyRecordChanged();
                CancelEdit();
            }
        }
        else if (generation == _generation && _ui.Services.Errors.IsUnauthorized(result))
        {
            _ui.Services.SessionGuard.Invalidate(session);
        }
        else if (generation == _generation && _ui.Services.SessionGuard.IsCurrent(session))
        {
            EditError = _ui.Services.Errors.FromResult(result, "Profile.SaveFailed");
        }
    }

    private async Task LoadRecordAsync(SessionSnapshot session, int generation)
    {
        ClientResult<PlayerMatchRecord> result = await Task.Run(() => _statistics.GetMyRecord(session.Token));
        if (generation == _generation && _ui.Services.SessionGuard.IsCurrent(session))
        {
            if (_ui.Services.Errors.IsUnauthorized(result))
            {
                _ui.Services.SessionGuard.Invalidate(session);
            }
            else if (result.IsSuccess && result.Value is { } record)
            {
                _record = record;
                RecordStatus = string.Empty;
                NotifyRecordChanged();
            }
            else
            {
                RecordStatus = _ui.Services.Errors.FromResult(result, "Error.Unknown");
            }
        }
    }

    private async Task LoadPublicRecordAsync(SessionSnapshot session, Guid playerId, int generation)
    {
        ClientResult<PublicPlayerProfile> result = await Task.Run(
            () => _statistics.GetPlayerProfile(playerId, session.Token));
        if (generation == _generation && _ui.Services.SessionGuard.IsCurrent(session))
        {
            if (_ui.Services.Errors.IsUnauthorized(result))
            {
                _ui.Services.SessionGuard.Invalidate(session);
            }
            else if (result.IsSuccess && result.Value is { } profile)
            {
                _publicUsername = profile.Username;
                _record = profile.Record;
                RecordStatus = string.Empty;
                OnPropertyChanged(nameof(Username));
                NotifyRecordChanged();
            }
            else
            {
                RecordStatus = _ui.Services.Errors.FromResult(result, "Error.Unknown");
            }
        }
    }

    private void NotifyRecordChanged()
    {
        OnPropertyChanged(nameof(IsRecordAvailable));
        OnPropertyChanged(nameof(Avatar));
        OnPropertyChanged(nameof(MatchesPlayed));
        OnPropertyChanged(nameof(MatchesWon));
        OnPropertyChanged(nameof(WinRate));
        OnPropertyChanged(nameof(CreatedYear));
        OnPropertyChanged(nameof(Country));
    }
}
