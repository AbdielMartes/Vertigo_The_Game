This file is a merged representation of the entire codebase, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of the entire repository's contents.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
src/
  Client/
    Vertigo.Client.Core/
      Navigation/
        INavigator.cs
        NavigationPage.cs
        Navigator.cs
      Sessions/
        PlayerSession.cs
        SessionSnapshot.cs
      Vertigo.Client.Core.csproj
    Vertigo.Client.Networking/
      Callbacks/
        FriendshipCallbackHandler.cs
        LobbyCallbackHandler.cs
        PendingLobbyEvent.cs
      Interfaces/
        IAccountClient.cs
        IAuthClient.cs
        IBlockedPlayerClient.cs
        IChatClient.cs
        IFriendshipClient.cs
        IFriendshipNotificationClient.cs
        ILobbyClient.cs
        ILobbyInvitationClient.cs
        IServiceCaller.cs
      Proxies/
        AccountClient.cs
        AuthClient.cs
        BlockedPlayerClient.cs
        ChatClient.cs
        FriendshipClient.cs
        FriendshipNotificationClient.cs
        LobbyClient.cs
        LobbyInvitationClient.cs
        ServiceCaller.cs
      Results/
        ClientResult.cs
        FailureKind.cs
      Sessions/
        Interfaces/
          ISessionCoordinator.cs
          ISessionTransportCleaner.cs
        Services/
          SessionTransportCleaner.cs
        SessionCoordinator.cs
      DependencyInjection.cs
      Vertigo.Client.Networking.csproj
    Vertigo.Client.UI/
      Assets/
        avalonia-logo.ico
      Resources/
        Localization/
          Strings.en.resx
          Strings.resx
      Themes/
        Controls/
          Alerts.axaml
          Buttons.axaml
          Cards.axaml
          Lists.axaml
          StatusIndicators.axaml
          TextInputs.axaml
        Layouts/
          Panels.axaml
        Tokens/
          Colors.axaml
          Icons.axaml
          Shapes.axaml
          Spacing.axaml
          Typography.axaml
        VertigoTheme.axaml
      Utilities/
        Localization/
          Interfaces/
            IClientErrorText.cs
            ITextLocalizer.cs
          Markup/
            LocalizeExtension.cs
          Services/
            ClientErrorText.cs
            TextLocalizer.cs
        Sessions/
          Interfaces/
            ISessionGuard.cs
          Services/
            SessionGuard.cs
        Settings/
          Interfaces/
            IUserSettingsStore.cs
          Models/
            UserSettings.cs
          Services/
            JsonUserSettingsStore.cs
            UiLanguage.cs
        Validation/
          FieldValidationError.cs
          LobbyCodeRules.cs
          MatchChatRules.cs
          RegistrationRules.cs
      ViewModels/
        Dependencies/
          AccessContext.cs
          HomeContext.cs
          LobbyContext.cs
          LobbyInvitationContext.cs
          MainContext.cs
          MainScreensContext.cs
          MainSocialContext.cs
          SocialNavigationContext.cs
          SocialViewModelContext.cs
          UiNavigationContext.cs
          UiServices.cs
        Lifecycle/
          INavigationAware.cs
          LobbyPreparationRevision.cs
        Presentation/
          LobbyMemberRow.cs
          MatchCardRow.cs
          MatchChatRow.cs
          MatchPileRow.cs
          MatchPlayerRow.cs
        Social/
          BlockedPlayersViewModel.cs
          FriendsViewModel.cs
          LobbyInvitationsViewModel.cs
          SocialViewModelBase.cs
        Utilities/
          SocialOperation.cs
        AccessViewModel.cs
        HomeViewModel.cs
        LobbyViewModel.cs
        MainViewModel.cs
        MatchViewModel.cs
        PlayViewModel.cs
        ProfileViewModel.cs
        SettingsViewModel.cs
        SocialViewModel.cs
        ViewModelBase.cs
      Views/
        AccessView.axaml
        AccessView.axaml.cs
        HomeView.axaml
        HomeView.axaml.cs
        LobbyView.axaml
        LobbyView.axaml.cs
        MainWindow.axaml
        MainWindow.axaml.cs
        MatchView.axaml
        MatchView.axaml.cs
        PlayView.axaml
        PlayView.axaml.cs
        ProfileView.axaml
        ProfileView.axaml.cs
        SettingsView.axaml
        SettingsView.axaml.cs
        SocialView.axaml
        SocialView.axaml.cs
      App.axaml
      App.axaml.cs
      app.manifest
      Program.cs
      Vertigo.Client.UI.csproj
      ViewLocator.cs
  Server/
    Vertigo.Server.CrossCutting/
      Security/
        Implementations/
          PasswordHasher.cs
          TokenGenerator.cs
        Interfaces/
          IPasswordHasher.cs
          ITokenGenerator.cs
      Vertigo.Server.CrossCutting.csproj
    Vertigo.Server.DataAccess/
      Contexts/
        VertigoDatabaseContext.cs
      Entities/
        Enums/
          FriendshipStatus.cs
        Game/
          MatchHistory.cs
        Identity/
          BlockedPlayer.cs
          Friendship.cs
          Player.cs
          PlayerStats.cs
          SessionToken.cs
      Repositories/
        Implementations/
          BlockedPlayerRepository.cs
          FriendshipRepository.cs
          PlayerRepository.cs
          SessionTokenRepository.cs
        Interfaces/
          IBlockedPlayerRepository.cs
          IFriendshipRepository.cs
          IPlayerRepository.cs
          ISessionTokenRepository.cs
      DependencyInjection.cs
      Vertigo.Server.DataAccess.csproj
    Vertigo.Server.Game/
      Matches/
        DependencyInjection/
          MatchLifecycleServiceCollectionExtensions.cs
        Implementations/
          InMemoryMatchSessionStarter.cs
        Models/
          MatchRecord.cs
      Vertigo.Server.Game.csproj
    Vertigo.Server.Host/
      Program.cs
      Vertigo.Server.Host.csproj
    Vertigo.Server.Matchmaking/
      Chat/
        Services/
          ChatService.cs
      Invitations/
        Dependencies/
          LobbyInvitationServiceDependencies.cs
        Services/
          LobbyInvitationService.cs
      Lobbies/
        Services/
          LobbyGameOperations.cs
          LobbyMatchActivation.cs
          LobbyService.cs
        LobbyConnection.cs
        LobbySettings.cs
        LobbyState.cs
        LobbyStatus.cs
      Vertigo.Server.Matchmaking.csproj
    Vertigo.Server.PlayerManagement/
      Authentication/
        Dependencies/
          AuthenticationDependencies.cs
        Interfaces/
          IGuestSessionRegistry.cs
          ISessionIdentityResolver.cs
        Models/
          AuthenticatedSession.cs
          SessionIdentity.cs
        Services/
          AuthenticationService.cs
          AuthService.cs
          GuestSessionRegistry.cs
          SessionIdentityResolver.cs
      Blocking/
        Services/
          BlockedPlayerService.cs
      Friendships/
        Dependencies/
          FriendshipServiceDependencies.cs
        Models/
          FriendshipSubscription.cs
        Services/
          FriendshipNotificationService.cs
          FriendshipNotifier.cs
          FriendshipService.cs
      Registration/
        Dependencies/
          AccountServiceDependencies.cs
        Services/
          AccountService.cs
      Vertigo.Server.PlayerManagement.csproj
  Shared/
    Vertigo.Contracts/
      Callbacks/
        IFriendshipCallback.cs
        ILobbyCallback.cs
      Endpoints/
        ServiceEndpoints.cs
      Faults/
        ServiceErrorCodes.cs
        ServiceMessageKeys.cs
      Interfaces/
        IAccountService.cs
        IAuthService.cs
        IBlockedPlayerService.cs
        IChatService.cs
        IFriendshipNotificationService.cs
        IFriendshipService.cs
        ILobbyInvitationService.cs
        ILobbyService.cs
      Internal/
        IFriendshipNotifier.cs
        ILobbyDirectory.cs
        IMatchLifecycle.cs
        IMatchPresence.cs
        IMatchSessionStarter.cs
        LobbyOverview.cs
        MatchLifecycleSnapshot.cs
        MatchLifecycleStatus.cs
        MatchPresenceSnapshot.cs
        MatchReconnectResult.cs
        MatchSessionSnapshot.cs
        MatchStartRequest.cs
      Models/
        AccountRegistration.cs
        BlockedPlayerSummary.cs
        FriendEntry.cs
        FriendshipDecision.cs
        FriendshipInvitation.cs
        FriendshipInvitationEntry.cs
        InvitableFriendsQuery.cs
        LobbyAdmission.cs
        LobbyChatMessage.cs
        LobbyInvitationDecline.cs
        LobbyInvitationEntry.cs
        LobbyInvitations.cs
        LobbyMember.cs
        LobbyPreparation.cs
        LobbySummary.cs
        LoginCredentials.cs
        PasswordChange.cs
        PlayerPair.cs
        PlayerRelation.cs
        PlayerSearch.cs
        PlayerSearchEntry.cs
        ServiceFault.cs
        SessionLogout.cs
        UserSession.cs
      Vertigo.Contracts.csproj
tests/
  Vertigo.Client.LobbyPreparation.Smoke/
    Program.cs
    Vertigo.Client.LobbyPreparation.Smoke.csproj
  Vertigo.Client.Multiplayer.Smoke/
    Program.cs
    Vertigo.Client.Multiplayer.Smoke.csproj
  Vertigo.Client.Networking.Smoke/
    Program.cs
    Vertigo.Client.Networking.Smoke.csproj
  Vertigo.Client.PrivateInvitations.Smoke/
    Program.cs
    Vertigo.Client.PrivateInvitations.Smoke.csproj
  Vertigo.Client.Reconnection.Smoke/
    Program.cs
    Vertigo.Client.Reconnection.Smoke.csproj
  Vertigo.Client.TestAccounts.Setup/
    Program.cs
    Vertigo.Client.TestAccounts.Setup.csproj
  Vertigo.Client.UnitTests/
    Errors/
      ClientErrorTextTests.cs
    Lobbies/
      LobbyPreparationRevisionTests.cs
    Localization/
      AccessLanguageSwitchTests.cs
      AccessLocalizationTests.cs
      HomeLocalizationTests.cs
    Navigation/
      NavigatorTests.cs
    Sessions/
      PlayerSessionTests.cs
      SessionCoordinatorTests.cs
      SessionGuardTests.cs
    Settings/
      JsonUserSettingsStoreTests.cs
    Social/
      LobbyInvitationsViewModelTests.cs
    Validation/
      LobbyCodeRulesTests.cs
      MatchChatRulesTests.cs
      RegistrationRulesTests.cs
    Vertigo.Client.UnitTests.csproj
  Vertigo.Server.Game.Tests/
    Matches/
      InMemoryMatchSessionStarterTests.cs
      MatchLifecycleRegistrationTests.cs
      MatchLifecycleTests.cs
      MatchPresenceTests.cs
    UnitTest1.cs
    Vertigo.Server.Game.Tests.csproj
  Vertigo.Server.Matchmaking.Tests/
    Authentication/
      GuestSessionTests.cs
    Lobbies/
      LobbyDisconnectionTests.cs
      LobbyMatchActivationTests.cs
      LobbyStateTests.cs
    Vertigo.Server.Matchmaking.Tests.csproj
.gitignore
Vertigo.Client.slnx
Vertigo.Server.slnx
```

# Files

## File: src/Client/Vertigo.Client.UI/Themes/Controls/Alerts.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style Selector="Border.vertigo-alert">
        <Setter Property="Background" Value="{DynamicResource ColorErrorBackground}"/>
        <Setter Property="BorderBrush" Value="{DynamicResource ColorError}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
        <Setter Property="CornerRadius" Value="{DynamicResource RadiusSmall}"/>
        <Setter Property="Padding" Value="{DynamicResource PaddingCard}"/>
    </Style>
    <Style Selector="Border.vertigo-lobby-panel">
        <Setter Property="Background" Value="{DynamicResource ColorWhite}"/>
        <Setter Property="BorderBrush" Value="{DynamicResource ColorBorder}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
        <Setter Property="CornerRadius" Value="{DynamicResource RadiusSmall}"/>
        <Setter Property="Padding" Value="{DynamicResource PaddingCard}"/>
    </Style>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Utilities/Validation/LobbyCodeRules.cs
```csharp
namespace Vertigo.Client.UI.Utilities.Validation;

public static class LobbyCodeRules
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static bool TryNormalize(string? input, out string code)
    {
        string candidate = (input ?? string.Empty).Trim().ToUpperInvariant();
        bool isValid = candidate.Length == 6;

        if (isValid)
        {
            foreach (char character in candidate)
            {
                if (!Alphabet.Contains(character))
                {
                    isValid = false;
                    break;
                }
            }
        }

        code = isValid ? candidate : string.Empty;
        return isValid;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Validation/MatchChatRules.cs
```csharp
namespace Vertigo.Client.UI.Utilities.Validation;

public static class MatchChatRules
{
    public const int MaximumLength = 40;

    public static bool CanSend(string? message)
    {
        bool isValid = !string.IsNullOrWhiteSpace(message);

        if (isValid)
        {
            isValid = message!.Length <= MaximumLength;
        }

        return isValid;
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Presentation/LobbyMemberRow.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class LobbyMemberRow
{
    public required string Username { get; init; }
    public required string RoleText { get; init; }
    public required string ReadyText { get; init; }
    public bool IsReady { get; init; }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Presentation/MatchCardRow.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchCardRow
{
    public required string DisplayValue { get; init; }
    public bool IsWildcard { get; init; }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Presentation/MatchChatRow.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchChatRow
{
    public required string Sender { get; init; }
    public required string Text { get; init; }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Presentation/MatchPileRow.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchPileRow
{
    public required string Label { get; init; }
    public string TopCardText { get; init; } = string.Empty;
    public int Count { get; init; }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Presentation/MatchPlayerRow.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchPlayerRow
{
    public required string Username { get; init; }
    public int FloorsBuilt { get; init; }
    public bool IsCurrentTurn { get; init; }
    public bool IsMe { get; init; }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/MatchViewModel.cs
```csharp
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Validation;
using Vertigo.Client.UI.ViewModels.Presentation;

namespace Vertigo.Client.UI.ViewModels;

public sealed class MatchViewModel : ObservableObject
{
    private readonly ITextLocalizer _text;
    private bool _isExitConfirmationOpen;
    private string _draftMessage = string.Empty;
    private string _roundLabel = "—";
    private string _currentTurnLabel = "—";
    private string _errorMessage = string.Empty;
    private MatchCardRow? _selectedCard;

    public MatchViewModel(ITextLocalizer text)
    {
        _text = text;
        RequestExitCommand = new RelayCommand(RequestExit);
        CancelExitCommand = new RelayCommand(CancelExit);
    }

    public ObservableCollection<MatchPlayerRow> Players { get; } = new();
    public ObservableCollection<MatchPileRow> Piles { get; } = new();
    public ObservableCollection<MatchCardRow> Hand { get; } = new();
    public ObservableCollection<MatchCardRow> Wildcards { get; } = new();
    public ObservableCollection<MatchChatRow> ChatMessages { get; } = new();

    public string RoundLabel
    {
        get => _roundLabel;
        set => SetProperty(ref _roundLabel, value);
    }

    public string CurrentTurnLabel
    {
        get => _currentTurnLabel;
        set => SetProperty(ref _currentTurnLabel, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public string DraftMessage
    {
        get => _draftMessage;
        set
        {
            string nextValue = value ?? string.Empty;
            if (SetProperty(ref _draftMessage, nextValue))
            {
                OnPropertyChanged(nameof(ChatLengthLabel));
                OnPropertyChanged(nameof(CanSendChat));
            }
        }
    }

    public bool CanSendChat => MatchChatRules.CanSend(DraftMessage);
    public string ChatLengthLabel => $"{DraftMessage.Length}/{MatchChatRules.MaximumLength}";

    public MatchCardRow? SelectedCard
    {
        get => _selectedCard;
        set => SetProperty(ref _selectedCard, value);
    }

    public bool IsExitConfirmationOpen
    {
        get => _isExitConfirmationOpen;
        private set => SetProperty(ref _isExitConfirmationOpen, value);
    }

    public string TitleText => _text.Get("Match.Title");
    public string RoundText => _text.Get("Match.Round");
    public string TurnText => _text.Get("Match.Turn");
    public string PlayersText => _text.Get("Match.Players");
    public string FloorsText => _text.Get("Match.Floors");
    public string BoardText => _text.Get("Match.Board");
    public string BoardHintText => _text.Get("Match.BoardHint");
    public string HandText => _text.Get("Match.Hand");
    public string WildcardText => _text.Get("Match.Wildcards");
    public string ChatText => _text.Get("Match.Chat");
    public string MessageHintText => _text.Get("Match.MessageHint");
    public string SendText => _text.Get("Match.Send");
    public string PlayCardText => _text.Get("Match.PlayCard");
    public string ExitText => _text.Get("Match.Exit");
    public string ExitConfirmTitleText => _text.Get("Match.ExitConfirmTitle");
    public string ExitConfirmBodyText => _text.Get("Match.ExitConfirmBody");
    public string CancelText => _text.Get("Match.Cancel");
    public string ConfirmText => _text.Get("Match.Confirm");
    public string ErrorTitleText => _text.Get("Room.ErrorHeading");

    public IRelayCommand RequestExitCommand { get; }
    public IRelayCommand CancelExitCommand { get; }

    public void RefreshLocalization()
    {
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(RoundText));
        OnPropertyChanged(nameof(TurnText));
        OnPropertyChanged(nameof(PlayersText));
        OnPropertyChanged(nameof(FloorsText));
        OnPropertyChanged(nameof(BoardText));
        OnPropertyChanged(nameof(BoardHintText));
        OnPropertyChanged(nameof(HandText));
        OnPropertyChanged(nameof(WildcardText));
        OnPropertyChanged(nameof(ChatText));
        OnPropertyChanged(nameof(MessageHintText));
        OnPropertyChanged(nameof(SendText));
        OnPropertyChanged(nameof(PlayCardText));
        OnPropertyChanged(nameof(ExitText));
        OnPropertyChanged(nameof(ExitConfirmTitleText));
        OnPropertyChanged(nameof(ExitConfirmBodyText));
        OnPropertyChanged(nameof(CancelText));
        OnPropertyChanged(nameof(ConfirmText));
        OnPropertyChanged(nameof(ErrorTitleText));
    }

    private void RequestExit()
    {
        IsExitConfirmationOpen = true;
    }

    private void CancelExit()
    {
        IsExitConfirmationOpen = false;
    }

}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/PlayViewModel.cs
```csharp
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
    }

    public ObservableCollection<LobbySummary> PublicLobbies { get; }
    public bool HasPublicLobbies => PublicLobbies.Count > 0;
    public bool IsPublicListEmpty => !IsBusy && !HasPublicLobbies;
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
        ErrorMessage = string.Empty;
        IsBusy = false;
        RefreshLanguage();
    }

    public void OnNavigatedFrom()
    {
        _generation++;
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
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/LobbyView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             xmlns:presentation="using:Vertigo.Client.UI.ViewModels.Presentation"
             xmlns:models="using:Vertigo.Contracts.Models"
             x:Class="Vertigo.Client.UI.Views.LobbyView"
             x:DataType="vm:LobbyViewModel"
             x:CompileBindings="True">
    <UserControl.Styles>
        <Style Selector="Button:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
        <Style Selector="TextBox:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <ScrollViewer Background="{DynamicResource ColorCanvas}"
                  HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
        <StackPanel MinWidth="880" MaxWidth="1380" Margin="28" Spacing="18"
                    HorizontalAlignment="Center">
            <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
                <StackPanel Spacing="6">
                    <TextBlock Text="{Binding RoomTitleText}" FontFamily="{DynamicResource FontTitleDisplay}"
                               FontSize="34" FontWeight="Bold"/>
                    <StackPanel Orientation="Horizontal" Spacing="10">
                        <TextBlock Text="{Binding CodeTitleText}" VerticalAlignment="Center"/>
                        <Border Background="{DynamicResource ColorInfoBackground}"
                                BorderBrush="{DynamicResource ColorSnesBlue}" BorderThickness="1"
                                CornerRadius="{DynamicResource RadiusSmall}" Padding="12,6">
                            <TextBlock Text="{Binding LobbyCode}" FontSize="20" FontWeight="Bold"
                                       FontFamily="{DynamicResource FontMono}"/>
                        </Border>
                    </StackPanel>
                </StackPanel>
                <Button Grid.Column="1" Classes="vertigo-button danger" TabIndex="0"
                        Command="{Binding LeaveCommand}" Content="{Binding LeaveText}"
                        IsEnabled="{Binding !IsBusy}"
                        AutomationProperties.Name="{Binding LeaveText}"/>
            </Grid>
            <Border Classes="vertigo-alert" IsVisible="{Binding HasError}"
                    AutomationProperties.Name="{Binding ErrorHeadingText}">
                <Grid ColumnDefinitions="Auto,*" ColumnSpacing="12">
                    <Path Width="22" Height="22" Stretch="Uniform" VerticalAlignment="Top"
                          Stroke="{DynamicResource ColorError}" StrokeThickness="2"
                          Data="M 12,2 L 22,20 L 2,20 Z M 12,8 L 12,14 M 12,17 L 12,18"/>
                    <StackPanel Grid.Column="1" Spacing="5">
                        <TextBlock Text="{Binding ErrorHeadingText}" FontWeight="Bold"
                                   Foreground="{DynamicResource ColorError}"/>
                        <TextBlock Text="{Binding ErrorMessage}" TextWrapping="Wrap"
                                   Foreground="{DynamicResource ColorTextPrimary}"/>
                    </StackPanel>
                </Grid>
            </Border>
            <Grid ColumnDefinitions="1.05*,0.9*,1.2*" ColumnSpacing="16" MinHeight="480">
                <Border Grid.Column="0" Classes="vertigo-lobby-panel">
                    <StackPanel Spacing="12">
                        <TextBlock Text="{Binding MembersTitleText}" FontSize="21" FontWeight="SemiBold"/>
                        <TextBlock Text="{Binding MembersDescriptionText}"
                                   Foreground="{DynamicResource ColorTextSecondary}" TextWrapping="Wrap"/>
                        <ItemsControl ItemsSource="{Binding MemberRows}">
                            <ItemsControl.ItemTemplate>
                                <DataTemplate x:DataType="presentation:LobbyMemberRow">
                                    <Border BorderBrush="{DynamicResource ColorDivider}" BorderThickness="0,0,0,1"
                                            Padding="6,12">
                                        <Grid ColumnDefinitions="Auto,*,Auto" ColumnSpacing="10">
                                            <Path Width="28" Height="28" Stretch="Uniform"
                                                  Stroke="{DynamicResource ColorSnesBlue}" StrokeThickness="1.7"
                                                  Data="M 12,3 A 5,5 0 1 1 11.99,3 Z M 3,23 Q 3,14 12,14 Q 21,14 21,23 Z"/>
                                            <StackPanel Grid.Column="1" Spacing="4">
                                                <TextBlock Text="{Binding Username}" FontWeight="SemiBold"
                                                           TextTrimming="CharacterEllipsis"/>
                                                <TextBlock Text="{Binding RoleText}" FontSize="12"
                                                           Foreground="{DynamicResource ColorTextSecondary}"/>
                                            </StackPanel>
                                            <Border Grid.Column="2" Classes="vertigo-status"
                                                    
                                                    VerticalAlignment="Center">
                                                <TextBlock Text="{Binding ReadyText}" FontSize="12"
                                                           FontWeight="SemiBold"/>
                                            </Border>
                                        </Grid>
                                    </Border>
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </StackPanel>
                </Border>
                <Border Grid.Column="1" Classes="vertigo-lobby-panel">
                    <StackPanel Spacing="16">
                        <TextBlock Text="{Binding PreparationTitleText}" FontSize="21" FontWeight="SemiBold"/>
                        <Path Height="116" Width="110" HorizontalAlignment="Center" Stretch="Uniform"
                              Stroke="{DynamicResource ColorCoal}" StrokeThickness="1.5"
                              Fill="{DynamicResource ColorInfoBackground}"
                              Data="M 16,108 L 16,55 L 35,55 L 35,28 L 53,28 L 53,10 L 87,10 L 87,108 Z
                                    M 8,108 L 102,108 M 25,66 L 29,66 M 25,77 L 29,77
                                    M 44,39 L 48,39 M 44,51 L 48,51 M 62,24 L 76,24 M 62,40 L 76,40"/>
                        <TextBlock Text="{Binding PreparationHintText}" TextWrapping="Wrap"
                                   Foreground="{DynamicResource ColorTextSecondary}"
                                   TextAlignment="Center"/>
                        <Border Background="{DynamicResource ColorSurfaceMuted}" Padding="10"
                                CornerRadius="{DynamicResource RadiusSmall}">
                            <StackPanel Spacing="7">
                                <TextBlock Text="{Binding HostLabelText}" FontWeight="SemiBold"/>
                                <TextBlock Text="{Binding HostUsername}" TextTrimming="CharacterEllipsis"/>
                                <TextBlock Text="{Binding MatchStatusText}" FontWeight="SemiBold"/>
                            </StackPanel>
                        </Border>
                        <Button Classes="vertigo-button secondary" TabIndex="1"
                                Command="{Binding ToggleReadyCommand}"
                                Content="{Binding ReadyActionText}" IsEnabled="{Binding CanPrepare}"/>
                        <Button Classes="vertigo-button primary" TabIndex="2"
                                IsVisible="{Binding IsHost}" IsEnabled="{Binding CanPrepare}"
                                Command="{Binding StartMatchCommand}" Content="{Binding StartText}"/>
                    </StackPanel>
                </Border>
                <Border Grid.Column="2" Classes="vertigo-lobby-panel">
                    <Grid RowDefinitions="Auto,*,Auto,Auto" RowSpacing="12">
                        <TextBlock Text="{Binding ChatTitleText}" FontSize="21" FontWeight="SemiBold"/>
                        <ListBox Grid.Row="1" ItemsSource="{Binding Messages}" TabIndex="3"
                                 MinHeight="260" MaxHeight="520"
                                 AutomationProperties.Name="{Binding ChatTitleText}">
                            <ListBox.ItemTemplate>
                                <DataTemplate x:DataType="models:LobbyChatMessage">
                                    <StackPanel Margin="4,8" Spacing="3">
                                        <TextBlock Text="{Binding SenderUsername}" FontWeight="Bold"
                                                   Foreground="{DynamicResource ColorSnesBlue}"/>
                                        <TextBlock Text="{Binding Text}" TextWrapping="Wrap"/>
                                    </StackPanel>
                                </DataTemplate>
                            </ListBox.ItemTemplate>
                        </ListBox>
                        <TextBox Grid.Row="2" Classes="vertigo-input" TabIndex="4"
                                 MaxLength="500" AcceptsReturn="False"
                                 Text="{Binding MessageText, Mode=TwoWay}"
                                 PlaceholderText="{Binding MessagePlaceholderText}"
                                 AutomationProperties.Name="{Binding MessagePlaceholderText}"/>
                        <Button Grid.Row="3" Classes="vertigo-button primary" TabIndex="5"
                                Command="{Binding SendMessageCommand}"
                                Content="{Binding SendText}" IsEnabled="{Binding !IsBusy}"/>
                    </Grid>
                </Border>
            </Grid>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/LobbyView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class LobbyView : UserControl
{
    public LobbyView()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/MatchView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             xmlns:rows="using:Vertigo.Client.UI.ViewModels.Presentation"
             x:Class="Vertigo.Client.UI.Views.MatchView"
             x:DataType="vm:MatchViewModel"
             xmlns:validation="using:Vertigo.Client.UI.Utilities.Validation"
             x:CompileBindings="True">
    <UserControl.KeyBindings>
        <KeyBinding Gesture="Escape" Command="{Binding CancelExitCommand}"/>
    </UserControl.KeyBindings>
    <UserControl.Styles>
        <Style Selector="Border.match-panel">
            <Setter Property="Background" Value="{DynamicResource ColorWhite}"/>
            <Setter Property="BorderBrush" Value="{DynamicResource ColorBorderStrong}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
            <Setter Property="CornerRadius" Value="{DynamicResource RadiusSmall}"/>
            <Setter Property="Padding" Value="14"/>
        </Style>
        <Style Selector="Border.match-pile">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorSnesBlue}"/>
            <Setter Property="BorderThickness" Value="2"/>
            <Setter Property="Background" Value="{DynamicResource ColorInfoBackground}"/>
            <Setter Property="CornerRadius" Value="{DynamicResource RadiusSmall}"/>
            <Setter Property="Padding" Value="12"/>
            <Setter Property="MinWidth" Value="86"/>
            <Setter Property="MinHeight" Value="132"/>
        </Style>
        <Style Selector="Button:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
        <Style Selector="TextBox:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <Grid Background="{DynamicResource ColorCanvas}">
        <ScrollViewer HorizontalScrollBarVisibility="Auto"
                      VerticalScrollBarVisibility="Auto"
                      IsEnabled="{Binding !IsExitConfirmationOpen}">
            <StackPanel MinWidth="980" MaxWidth="1510" Margin="24" Spacing="14"
                        HorizontalAlignment="Center">
                <Grid ColumnDefinitions="*,Auto,Auto" ColumnSpacing="20">
                    <StackPanel Spacing="2">
                        <TextBlock Text="VÉRTIGO" FontSize="32" FontWeight="Bold"
                                   FontFamily="{DynamicResource FontTitleDisplay}"/>
                        <TextBlock Text="{Binding TitleText}" FontSize="12"
                                   Foreground="{DynamicResource ColorTextSecondary}"/>
                    </StackPanel>
                    <StackPanel Grid.Column="1" Orientation="Horizontal" Spacing="12"
                                VerticalAlignment="Center">
                        <TextBlock Text="{Binding RoundText}" FontWeight="SemiBold"/>
                        <TextBlock Text="{Binding RoundLabel}" FontFamily="{DynamicResource FontMono}"
                                   Foreground="{DynamicResource ColorSnesBlue}"/>
                        <TextBlock Text="{Binding TurnText}" FontWeight="SemiBold" Margin="12,0,0,0"/>
                        <TextBlock Text="{Binding CurrentTurnLabel}"/>
                    </StackPanel>
                    <Button Grid.Column="2" Classes="vertigo-button danger" TabIndex="0"
                            Content="{Binding ExitText}" Command="{Binding RequestExitCommand}"/>
                </Grid>
                <Border IsVisible="{Binding HasError}" Classes="match-panel"
                        Background="{DynamicResource ColorErrorBackground}"
                        BorderBrush="{DynamicResource ColorError}">
                    <StackPanel Orientation="Horizontal" Spacing="10">
                        <Path Width="18" Height="18" Stretch="Uniform"
                              Stroke="{DynamicResource ColorError}" StrokeThickness="2"
                              Data="M 12,2 L 22,20 L 2,20 Z M 12,8 L 12,14 M 12,17 L 12,18"/>
                        <TextBlock Text="{Binding ErrorMessage}" TextWrapping="Wrap"/>
                    </StackPanel>
                </Border>
                <Grid ColumnDefinitions="210,*,260" ColumnSpacing="14" MinHeight="450">
                    <Border Grid.Column="0" Classes="match-panel">
                        <StackPanel Spacing="12">
                            <TextBlock Text="{Binding PlayersText}" FontSize="18" FontWeight="Bold"/>
                            <ListBox ItemsSource="{Binding Players}" TabIndex="1" MinHeight="300"
                                     AutomationProperties.Name="{Binding PlayersText}">
                                <ListBox.ItemTemplate>
                                    <DataTemplate x:DataType="rows:MatchPlayerRow">
                                        <Border BorderBrush="{DynamicResource ColorDivider}"
                                                BorderThickness="0,0,0,1" Padding="4,10">
                                            <StackPanel Spacing="5">
                                                <TextBlock Text="{Binding Username}" FontWeight="SemiBold"
                                                           TextTrimming="CharacterEllipsis"/>
                                                <StackPanel Orientation="Horizontal" Spacing="6">
                                                    <Path Width="15" Height="15" Stroke="{DynamicResource ColorSnesBlue}"
                                                          StrokeThickness="1.6" Data="M 2,13 L 13,13 L 13,2 L 2,2 Z"/>
                                                    <TextBlock Text="{Binding FloorsBuilt}"
                                                               FontFamily="{DynamicResource FontMono}"/>
                                                </StackPanel>
                                            </StackPanel>
                                        </Border>
                                    </DataTemplate>
                                </ListBox.ItemTemplate>
                            </ListBox>
                        </StackPanel>
                    </Border>
                    <Border Grid.Column="1" Classes="match-panel">
                        <Grid RowDefinitions="Auto,*,Auto" RowSpacing="12">
                            <TextBlock Text="{Binding BoardText}" FontSize="18" FontWeight="Bold"/>
                            <ScrollViewer Grid.Row="1" HorizontalScrollBarVisibility="Auto"
                                          VerticalScrollBarVisibility="Disabled">
                                <ItemsControl ItemsSource="{Binding Piles}">
                                    <ItemsControl.ItemsPanel>
                                        <ItemsPanelTemplate>
                                            <StackPanel Orientation="Horizontal" Spacing="14"/>
                                        </ItemsPanelTemplate>
                                    </ItemsControl.ItemsPanel>
                                    <ItemsControl.ItemTemplate>
                                        <DataTemplate x:DataType="rows:MatchPileRow">
                                            <StackPanel Spacing="8">
                                                <TextBlock Text="{Binding Label}" HorizontalAlignment="Center"
                                                           FontWeight="SemiBold"/>
                                                <Border Classes="match-pile">
                                                    <Grid RowDefinitions="*,Auto">
                                                        <Path Stretch="Uniform" Width="46" Height="55"
                                                              Stroke="{DynamicResource ColorSnesBlue}"
                                                              StrokeThickness="1.5"
                                                              Data="M 5,5 L 41,5 L 41,50 L 5,50 Z M 12,12 L 34,12 M 12,18 L 34,18"/>
                                                        <TextBlock Grid.Row="1" Text="{Binding TopCardText}"
                                                                   FontSize="24" FontWeight="Bold"
                                                                   HorizontalAlignment="Center"
                                                                   FontFamily="{DynamicResource FontMono}"/>
                                                    </Grid>
                                                </Border>
                                            </StackPanel>
                                        </DataTemplate>
                                    </ItemsControl.ItemTemplate>
                                </ItemsControl>
                            </ScrollViewer>
                            <TextBlock Grid.Row="2" Text="{Binding BoardHintText}"
                                       TextWrapping="Wrap" Foreground="{DynamicResource ColorTextSecondary}"
                                       FontSize="12"/>
                        </Grid>
                    </Border>
                    <Border Grid.Column="2" Classes="match-panel">
                        <Grid RowDefinitions="Auto,*,Auto,Auto,Auto" RowSpacing="10">
                            <TextBlock Text="{Binding ChatText}" FontSize="18" FontWeight="Bold"/>
                            <ListBox Grid.Row="1" TabIndex="2" MinHeight="240"
                                     ItemsSource="{Binding ChatMessages}"
                                     AutomationProperties.Name="{Binding ChatText}">
                                <ListBox.ItemTemplate>
                                    <DataTemplate x:DataType="rows:MatchChatRow">
                                        <StackPanel Margin="4,6" Spacing="3">
                                            <TextBlock Text="{Binding Sender}" FontWeight="Bold"
                                                       Foreground="{DynamicResource ColorSnesBlue}"/>
                                            <TextBlock Text="{Binding Text}" TextWrapping="Wrap"/>
                                        </StackPanel>
                                    </DataTemplate>
                                </ListBox.ItemTemplate>
                            </ListBox>
                            <TextBox Grid.Row="2" TabIndex="3" MaxLength="{x:Static validation:MatchChatRules.MaximumLength}"
                                     Text="{Binding DraftMessage, Mode=TwoWay}"
                                     PlaceholderText="{Binding MessageHintText}" AcceptsReturn="False"
                                     AutomationProperties.Name="{Binding MessageHintText}"/>
                            <TextBlock Grid.Row="3" Text="{Binding ChatLengthLabel}"
                                       HorizontalAlignment="Right" FontSize="11"
                                       Foreground="{DynamicResource ColorTextSecondary}"/>
                            <Button Grid.Row="4" Classes="vertigo-button primary" TabIndex="4"
                                    Content="{Binding SendText}" IsEnabled="False"/>
                        </Grid>
                    </Border>
                </Grid>
                <Border Classes="match-panel">
                    <Grid ColumnDefinitions="*,200" ColumnSpacing="24">
                        <StackPanel Spacing="10">
                            <TextBlock Text="{Binding HandText}" FontSize="19" FontWeight="Bold"/>
                            <ListBox ItemsSource="{Binding Hand}" SelectedItem="{Binding SelectedCard, Mode=TwoWay}"
                                     TabIndex="5" MaxHeight="168" SelectionMode="Single">
                                <ListBox.ItemsPanel>
                                    <ItemsPanelTemplate>
                                        <StackPanel Orientation="Horizontal" Spacing="9"/>
                                    </ItemsPanelTemplate>
                                </ListBox.ItemsPanel>
                                <ListBox.ItemTemplate>
                                    <DataTemplate x:DataType="rows:MatchCardRow">
                                        <Border Classes="match-pile" MinHeight="100" MinWidth="75"
                                                Background="{DynamicResource ColorWhite}">
                                            <TextBlock Text="{Binding DisplayValue}" FontSize="28"
                                                       FontWeight="Bold" HorizontalAlignment="Center"
                                                       VerticalAlignment="Center"
                                                       FontFamily="{DynamicResource FontMono}"/>
                                        </Border>
                                    </DataTemplate>
                                </ListBox.ItemTemplate>
                            </ListBox>
                        </StackPanel>
                        <StackPanel Grid.Column="1" Spacing="8">
                            <TextBlock Text="{Binding WildcardText}" FontWeight="SemiBold"/>
                            <ItemsControl ItemsSource="{Binding Wildcards}">
                                <ItemsControl.ItemTemplate>
                                    <DataTemplate x:DataType="rows:MatchCardRow">
                                        <Border Classes="match-pile" MinHeight="52">
                                            <TextBlock Text="{Binding DisplayValue}" FontSize="22"/>
                                        </Border>
                                    </DataTemplate>
                                </ItemsControl.ItemTemplate>
                            </ItemsControl>
                            <Button Classes="vertigo-button primary" TabIndex="6"
                                    Content="{Binding PlayCardText}" IsEnabled="False"/>
                        </StackPanel>
                    </Grid>
                </Border>
            </StackPanel>
        </ScrollViewer>
        <Border IsVisible="{Binding IsExitConfirmationOpen}" Background="{DynamicResource ColorCanvas}"
                BorderBrush="{DynamicResource ColorCoal}" BorderThickness="2"
                CornerRadius="{DynamicResource RadiusSmall}"
                Width="450" MaxWidth="450" Padding="24"
                HorizontalAlignment="Center" VerticalAlignment="Center"
                Focusable="True">
            <StackPanel Spacing="14">
                <TextBlock Text="{Binding ExitConfirmTitleText}" FontWeight="Bold" FontSize="21"/>
                <TextBlock Text="{Binding ExitConfirmBodyText}" TextWrapping="Wrap"/>
                <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Spacing="10">
                    <Button Content="{Binding CancelText}" Command="{Binding CancelExitCommand}"
                            Classes="vertigo-button neutral" TabIndex="7"/>
                    <Button Content="{Binding ConfirmText}"
                            Classes="vertigo-button danger" TabIndex="8" IsEnabled="False"/>
                </StackPanel>
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/MatchView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class MatchView : UserControl
{
    public MatchView()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/PlayView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             xmlns:contracts="using:Vertigo.Contracts.Models"
             x:Class="Vertigo.Client.UI.Views.PlayView"
             x:DataType="vm:PlayViewModel"
             x:CompileBindings="True">
    <UserControl.Styles>
        <Style Selector="Button:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
        <Style Selector="ListBox:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <ScrollViewer Background="{DynamicResource ColorCanvas}">
        <StackPanel Margin="30" Spacing="24" MinWidth="730" MaxWidth="1080"
                    HorizontalAlignment="Center">
            <DockPanel LastChildFill="True">
                <Button DockPanel.Dock="Left" Classes="vertigo-button neutral"
                        Command="{Binding BackCommand}" TabIndex="0"
                        AutomationProperties.Name="{Binding BackText}">
                    <Path Width="24" Height="24" Stretch="Uniform"
                          Stroke="{DynamicResource ColorCoal}" StrokeThickness="2"
                          Data="M 20,12 L 4,12 M 11,5 L 4,12 L 11,19"/>
                </Button>
                <StackPanel Margin="22,0,0,0" VerticalAlignment="Center" Spacing="4">
                    <TextBlock Text="{Binding TitleText}" FontSize="34" FontWeight="Bold"
                               FontFamily="{DynamicResource FontTitleDisplay}"/>
                    <TextBlock Text="{Binding SubtitleText}" FontSize="14"
                               Foreground="{DynamicResource ColorTextSecondary}"/>
                </StackPanel>
            </DockPanel>
            <Grid ColumnDefinitions="*,*" ColumnSpacing="22">
                <Border Grid.Column="0" Background="{DynamicResource ColorWhite}"
                        BorderBrush="{DynamicResource ColorCoal}"
                        BorderThickness="{DynamicResource BorderThin}"
                        CornerRadius="{DynamicResource RadiusSmall}" Padding="22">
                    <StackPanel Spacing="16">
                        <Path Width="40" Height="42" HorizontalAlignment="Left"
                              Stroke="{DynamicResource ColorSnesBlue}" StrokeThickness="2"
                              Data="M 5,38 L 5,20 L 18,20 L 18,38 M 18,38 L 18,8 L 35,8 L 35,38 M 2,38 L 39,38"/>
                        <TextBlock Text="{Binding CreateTitleText}" FontSize="21" FontWeight="SemiBold"/>
                        <CheckBox IsChecked="{Binding IsPublic, Mode=TwoWay}"
                                  Content="{Binding PublicText}" TabIndex="1"
                                  AutomationProperties.Name="{Binding PublicText}"/>
                        <TextBlock Text="{Binding MaxPlayersText}"/>
                        <NumericUpDown Minimum="2" Maximum="6" Increment="1"
                                       Value="{Binding MaxPlayers, Mode=TwoWay}"
                                       Width="120" HorizontalAlignment="Left" TabIndex="2"
                                       AutomationProperties.Name="{Binding MaxPlayersText}"/>
                        <Button Classes="vertigo-button primary" Command="{Binding CreateCommand}"
                                Content="{Binding CreateText}" IsEnabled="{Binding !IsBusy}"
                                TabIndex="3"/>
                    </StackPanel>
                </Border>
                <Border Grid.Column="1" Background="{DynamicResource ColorWhite}"
                        BorderBrush="{DynamicResource ColorCoal}"
                        BorderThickness="{DynamicResource BorderThin}"
                        CornerRadius="{DynamicResource RadiusSmall}" Padding="22">
                    <StackPanel Spacing="16">
                        <Path Width="40" Height="42" HorizontalAlignment="Left"
                              Stroke="{DynamicResource ColorSnesGreen}" StrokeThickness="2"
                              Data="M 2,15 L 14,15 L 14,28 L 2,28 Z M 14,21 L 32,21 M 25,13 L 33,21 L 25,29"/>
                        <TextBlock Text="{Binding JoinTitleText}" FontSize="21" FontWeight="SemiBold"/>
                        <TextBlock Text="{Binding CodeText}"/>
                        <TextBox Text="{Binding LobbyCode, Mode=TwoWay}" MaxLength="6"
                                 Classes="vertigo-input" TabIndex="4"
                                 AutomationProperties.Name="{Binding CodeText}"/>
                        <Button Classes="vertigo-button secondary" Command="{Binding JoinCodeCommand}"
                                Content="{Binding JoinText}" IsEnabled="{Binding !IsBusy}"
                                TabIndex="5"/>
                    </StackPanel>
                </Border>
            </Grid>
            <Border Background="{DynamicResource ColorWhite}"
                    BorderBrush="{DynamicResource ColorCoal}"
                    BorderThickness="{DynamicResource BorderThin}"
                    CornerRadius="{DynamicResource RadiusSmall}" Padding="22">
                <StackPanel Spacing="16">
                    <TextBlock Text="{Binding BrowseTitleText}" FontSize="21" FontWeight="SemiBold"/>
                    <StackPanel Orientation="Horizontal" Spacing="12">
                        <Button Classes="vertigo-button secondary" Command="{Binding TogglePublicCommand}"
                                Content="{Binding BrowseText}" TabIndex="6"/>
                        <Button Classes="vertigo-button neutral" Command="{Binding RefreshPublicCommand}"
                                Content="{Binding RefreshText}" IsEnabled="{Binding !IsBusy}"
                                IsVisible="{Binding IsPublicListOpen}" TabIndex="7"/>
                    </StackPanel>
                    <StackPanel IsVisible="{Binding IsPublicListOpen}" Spacing="12">
                        <TextBlock Text="{Binding EmptyText}" IsVisible="{Binding IsPublicListEmpty}"
                                   Foreground="{DynamicResource ColorTextSecondary}"/>
                        <ListBox ItemsSource="{Binding PublicLobbies}"
                                 SelectedItem="{Binding SelectedPublicLobby, Mode=TwoWay}"
                                 TabIndex="8" MaxHeight="260"
                                 AutomationProperties.Name="{Binding BrowseTitleText}">
                            <ListBox.ItemTemplate>
                                <DataTemplate x:DataType="contracts:LobbySummary">
                                    <Grid ColumnDefinitions="140,*,Auto" ColumnSpacing="16" Margin="8">
                                        <TextBlock Grid.Column="0" Text="{Binding LobbyCode}"
                                                   FontFamily="{DynamicResource FontMono}" FontWeight="Bold"/>
                                        <TextBlock Grid.Column="1" Text="{Binding HostUsername}"/>
                                        <StackPanel Grid.Column="2" Orientation="Horizontal" Spacing="4">
                                            <TextBlock Text="{Binding CurrentPlayers}"/>
                                            <TextBlock Text="/"/>
                                            <TextBlock Text="{Binding MaxPlayers}"/>
                                        </StackPanel>
                                    </Grid>
                                </DataTemplate>
                            </ListBox.ItemTemplate>
                        </ListBox>
                        <Button Classes="vertigo-button primary" Command="{Binding JoinSelectedCommand}"
                                Content="{Binding JoinText}" IsEnabled="{Binding !IsBusy}"
                                HorizontalAlignment="Right" TabIndex="9"/>
                    </StackPanel>
                </StackPanel>
            </Border>
            <TextBlock Text="{Binding ErrorMessage}" TextWrapping="Wrap"
                       Foreground="{DynamicResource ColorError}"/>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/PlayView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class PlayView : UserControl
{
    public PlayView()
    {
        InitializeComponent();
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Validation/LobbyCodeRulesTests.cs
```csharp
using Vertigo.Client.UI.Utilities.Validation;
using Xunit;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class LobbyCodeRulesTests
{
    [Theory]
    [InlineData("ABC234", "ABC234")]
    [InlineData(" abc234 ", "ABC234")]
    [InlineData("Z9K8T7", "Z9K8T7")]
    public void TryNormalize_ValidCode_ReturnsNormalized(string input, string expected)
    {
        bool result = LobbyCodeRules.TryNormalize(input, out string code);

        Assert.True(result);
        Assert.Equal(expected, code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC23")]
    [InlineData("ABC2345")]
    [InlineData("ABC_34")]
    [InlineData("ABO234")]
    [InlineData("ABI234")]
    [InlineData("AB1234;")]
    public void TryNormalize_InvalidCode_ReturnsFalse(string input)
    {
        bool result = LobbyCodeRules.TryNormalize(input, out string code);

        Assert.False(result);
        Assert.Empty(code);
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Validation/MatchChatRulesTests.cs
```csharp
using Vertigo.Client.UI.Utilities.Validation;
using Xunit;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class MatchChatRulesTests
{
    [Fact]
    public void CanSend_ExactlyFortyCharacters_ReturnsTrue()
    {
        Assert.True(MatchChatRules.CanSend(new string('a', 40)));
    }

    [Fact]
    public void CanSend_FortyOneCharacters_ReturnsFalse()
    {
        Assert.False(MatchChatRules.CanSend(new string('a', 41)));
    }

    [Fact]
    public void CanSend_WhitespaceOnly_ReturnsFalse()
    {
        Assert.False(MatchChatRules.CanSend("   "));
    }

    [Fact]
    public void CanSend_ValidShortMessage_ReturnsTrue()
    {
        Assert.True(MatchChatRules.CanSend("¡Vamos!"));
    }
}
```

## File: src/Client/Vertigo.Client.Core/Navigation/INavigator.cs
```csharp
namespace Vertigo.Client.Core.Navigation;

public interface INavigator
{
    NavigationPage CurrentPage { get; }
    event EventHandler<NavigationPage>? PageChanged;
    void NavigateTo(NavigationPage page);
}
```

## File: src/Client/Vertigo.Client.Core/Navigation/Navigator.cs
```csharp
namespace Vertigo.Client.Core.Navigation;

public sealed class Navigator : INavigator
{
    private readonly object _syncRoot = new();
    private NavigationPage _currentPage = NavigationPage.Access;

    public event EventHandler<NavigationPage>? PageChanged;

    public NavigationPage CurrentPage
    {
        get
        {
            lock (_syncRoot)
            {
                return _currentPage;
            }
        }
    }

    public void NavigateTo(NavigationPage page)
    {
        bool changed;
        lock (_syncRoot)
        {
            changed = _currentPage != page;
            _currentPage = page;
        }

        if (changed)
        {
            PageChanged?.Invoke(this, page);
        }
    }
}
```

## File: src/Client/Vertigo.Client.Core/Sessions/PlayerSession.cs
```csharp
namespace Vertigo.Client.Core.Sessions;

public sealed class PlayerSession
{
    private readonly object _syncRoot = new();
    private readonly TimeProvider _timeProvider;
    private SessionSnapshot? _session;

    public PlayerSession(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public bool IsAuthenticated => GetActiveSession() is not null;

    public SessionSnapshot? GetActiveSession()
    {
        lock (_syncRoot)
        {
            if (_session is not null && _session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
            {
                _session = null;
            }

            return _session;
        }
    }

    public void SetSession(SessionSnapshot session)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (session.PlayerId == Guid.Empty || string.IsNullOrWhiteSpace(session.Username)
            || string.IsNullOrWhiteSpace(session.Token) || session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            throw new ArgumentException("Session data is incomplete or expired.", nameof(session));
        }

        lock (_syncRoot)
        {
            _session = session;
        }
    }

    public void Clear(SessionSnapshot session)
    {
        lock (_syncRoot)
        {
            if (ReferenceEquals(_session, session))
            {
                _session = null;
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Core/Sessions/SessionSnapshot.cs
```csharp
namespace Vertigo.Client.Core.Sessions;

public sealed record SessionSnapshot(
    Guid PlayerId,
    string Username,
    string Token,
    DateTime ExpiresAt,
    bool IsGuest)
{
    public override string ToString()
    {
        return nameof(SessionSnapshot);
    }
}
```

## File: src/Client/Vertigo.Client.Core/Vertigo.Client.Core.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

## File: src/Client/Vertigo.Client.Networking/Callbacks/FriendshipCallbackHandler.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
public sealed class FriendshipCallbackHandler : IFriendshipCallback
{
    public event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived;

    public event EventHandler<FriendEntry>? FriendshipEstablished;

    public event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived;

    public event EventHandler<string>? LobbyInvitationWithdrawn;

    void IFriendshipCallback.OnFriendRequestReceived(FriendshipInvitationEntry request)
    {
        FriendRequestReceived?.Invoke(this, request);
    }

    void IFriendshipCallback.OnFriendshipEstablished(FriendEntry friend)
    {
        FriendshipEstablished?.Invoke(this, friend);
    }

    void IFriendshipCallback.OnLobbyInvitationReceived(LobbyInvitationEntry invitation)
    {
        LobbyInvitationReceived?.Invoke(this, invitation);
    }

    void IFriendshipCallback.OnLobbyInvitationWithdrawn(string lobbyCode)
    {
        LobbyInvitationWithdrawn?.Invoke(this, lobbyCode);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IAccountClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IAccountClient
{
    ClientResult<bool> RegisterAccount(AccountRegistration registration);

    ClientResult<bool> ChangePassword(PasswordChange change, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IAuthClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IAuthClient
{
    ClientResult<UserSession> Login(LoginCredentials request);

    ClientResult<UserSession> ResumeSession(string sessionToken);

    ClientResult<UserSession> CreateGuestSession();

    ClientResult<bool> Logout(string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IBlockedPlayerClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IBlockedPlayerClient
{
    ClientResult<List<BlockedPlayerSummary>> GetBlockedPlayers(string sessionToken);

    ClientResult<bool> BlockPlayer(Guid otherPlayerId, string sessionToken);

    ClientResult<bool> UnblockPlayer(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IChatClient.cs
```csharp
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.Networking.Interfaces;

public interface IChatClient
{
    ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IFriendshipClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IFriendshipClient
{
    ClientResult<List<FriendEntry>> GetFriends(string sessionToken);

    ClientResult<List<PlayerSearchEntry>> SearchPlayers(string searchTerm, string sessionToken);

    ClientResult<PlayerRelation> SendFriendRequest(Guid targetPlayerId, string sessionToken);

    ClientResult<List<FriendshipInvitationEntry>> GetPendingFriendRequests(string sessionToken);

    ClientResult<bool> RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken);

    ClientResult<bool> RemoveFriendship(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IFriendshipNotificationClient.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IFriendshipNotificationClient : IDisposable
{
    event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived;

    event EventHandler<FriendEntry>? FriendshipEstablished;

    event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived;

    event EventHandler<string>? LobbyInvitationWithdrawn;

    bool Subscribe(string sessionToken);

    void Disconnect();

    void Unsubscribe();
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/ILobbyInvitationClient.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyInvitationClient
{
    ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken);

    ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken);

    ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken);

    ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken);
}
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/IServiceCaller.cs
```csharp
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.Networking.Interfaces;

public interface IServiceCaller
{
    ClientResult<TResult> Call<TChannel, TArgument, TResult>(
        string address,
        Func<TChannel, TArgument, TResult> operation,
        TArgument argument)
        where TChannel : class;
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/AccountClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class AccountClient : IAccountClient
{
    private readonly IServiceCaller _serviceCaller;

    public AccountClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<bool> RegisterAccount(AccountRegistration request)
    {
        return _serviceCaller.Call<IAccountService, AccountRegistration, bool>(
            ServiceEndpoints.AccountServiceAddress,
            RegisterAccountOperation,
            request);
    }

    public ClientResult<bool> ChangePassword(PasswordChange change, string sessionToken)
    {
        return _serviceCaller.Call<IAccountService, (PasswordChange, string), bool>(
            ServiceEndpoints.AccountServiceAddress,
            (channel, args) => channel.ChangePassword(args.Item1, args.Item2),
            (change, sessionToken));
    }

    private static bool RegisterAccountOperation(IAccountService channel, AccountRegistration request)
    {
        return channel.RegisterAccount(request);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/AuthClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class AuthClient : IAuthClient
{
    private readonly IServiceCaller _serviceCaller;

    public AuthClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<UserSession> Login(LoginCredentials request)
    {
        return _serviceCaller.Call<IAuthService, LoginCredentials, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            LoginOperation,
            request);
    }

    public ClientResult<UserSession> ResumeSession(string sessionToken)
    {
        return _serviceCaller.Call<IAuthService, string, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            ResumeSessionOperation,
            sessionToken);
    }

    public ClientResult<UserSession> CreateGuestSession()
    {
        return _serviceCaller.Call<IAuthService, object?, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            CreateGuestSessionOperation,
            null);
    }

    public ClientResult<bool> Logout(string sessionToken)
    {
        SessionLogout request = new()
        {
            SessionToken = sessionToken
        };

        return _serviceCaller.Call<IAuthService, SessionLogout, bool>(
            ServiceEndpoints.AuthServiceAddress,
            LogoutOperation,
            request);
    }

    private static UserSession LoginOperation(IAuthService channel, LoginCredentials request)
    {
        return channel.Login(request);
    }

    private static UserSession ResumeSessionOperation(IAuthService channel, string sessionToken)
    {
        return channel.ResumeSession(sessionToken);
    }

    private static UserSession CreateGuestSessionOperation(IAuthService channel, object? unused)
    {
        return channel.CreateGuestSession();
    }

    private static bool LogoutOperation(IAuthService channel, SessionLogout request)
    {
        return channel.Logout(request);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/BlockedPlayerClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class BlockedPlayerClient : IBlockedPlayerClient
{
    private readonly IServiceCaller _serviceCaller;

    public BlockedPlayerClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<BlockedPlayerSummary>> GetBlockedPlayers(string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, string, List<BlockedPlayerSummary>>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, token) => channel.GetBlockedPlayers(token),
            sessionToken);
    }

    public ClientResult<bool> BlockPlayer(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, (Guid, string), bool>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, args) => channel.BlockPlayer(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }

    public ClientResult<bool> UnblockPlayer(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, (Guid, string), bool>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, args) => channel.UnblockPlayer(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/ChatClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class ChatClient : IChatClient
{
    private readonly IServiceCaller _serviceCaller;

    public ChatClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken)
    {
        return _serviceCaller.Call<IChatService, (string, string, string), bool>(
            ServiceEndpoints.ChatServiceAddress,
            (channel, args) => channel.SendMessage(args.Item1, args.Item2, args.Item3),
            (lobbyCode, text, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/FriendshipClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class FriendshipClient : IFriendshipClient
{
    private readonly IServiceCaller _serviceCaller;

    public FriendshipClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<FriendEntry>> GetFriends(string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, string, List<FriendEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, token) => channel.GetFriends(token),
            sessionToken);
    }

    public ClientResult<List<PlayerSearchEntry>> SearchPlayers(string searchTerm, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (string, string), List<PlayerSearchEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.SearchPlayers(args.Item1, args.Item2),
            (searchTerm, sessionToken));
    }

    public ClientResult<PlayerRelation> SendFriendRequest(Guid targetPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, string), PlayerRelation>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.SendFriendRequest(args.Item1, args.Item2),
            (targetPlayerId, sessionToken));
    }

    public ClientResult<List<FriendshipInvitationEntry>> GetPendingFriendRequests(string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, string, List<FriendshipInvitationEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, token) => channel.GetPendingFriendRequests(token),
            sessionToken);
    }

    public ClientResult<bool> RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, bool, string), bool>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.RespondToFriendship(args.Item1, args.Item2, args.Item3),
            (senderPlayerId, accept, sessionToken));
    }

    public ClientResult<bool> RemoveFriendship(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, string), bool>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.RemoveFriendship(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/FriendshipNotificationClient.cs
```csharp
using System.ServiceModel;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class FriendshipNotificationClient : IFriendshipNotificationClient
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    private FriendshipCallbackHandler? _callbackHandler;
    private DuplexChannelFactory<IFriendshipNotificationService>? _channelFactory;
    private readonly object _syncRoot = new();
    private IFriendshipNotificationService? _channel;
    private bool _isDisposed;

    public FriendshipNotificationClient()
    {
    }

    private event EventHandler<FriendshipInvitationEntry>? _friendRequestReceived;
    private event EventHandler<FriendEntry>? _friendshipEstablished;
    private event EventHandler<LobbyInvitationEntry>? _lobbyInvitationReceived;
    private event EventHandler<string>? _lobbyInvitationWithdrawn;


    public event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived
    {
        add => _friendRequestReceived += value;
        remove => _friendRequestReceived -= value;
    }

    public event EventHandler<FriendEntry>? FriendshipEstablished
    {
        add => _friendshipEstablished += value;
        remove => _friendshipEstablished -= value;
    }

    public event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived
    {
        add => _lobbyInvitationReceived += value;
        remove => _lobbyInvitationReceived -= value;
    }

    public event EventHandler<string>? LobbyInvitationWithdrawn
    {
        add => _lobbyInvitationWithdrawn += value;
        remove => _lobbyInvitationWithdrawn -= value;
    }

    public bool Subscribe(string sessionToken)
    {
        IFriendshipNotificationService channel = GetOrCreateChannel();

        try
        {
            return channel.Subscribe(sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public void Unsubscribe()
    {
        IFriendshipNotificationService channel = GetOrCreateChannel();

        try
        {
            channel.Unsubscribe();
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
        }
    }

    public void Disconnect()
    {
        IFriendshipNotificationService? oldChannel;
        DuplexChannelFactory<IFriendshipNotificationService>? oldFactory;
        lock (_syncRoot)
        {
            oldChannel = _channel;
            oldFactory = _channelFactory;
            _channel = null;
            _channelFactory = null;
            _callbackHandler = null;
        }
        (oldChannel as ICommunicationObject)?.Abort();
        if (oldFactory is not null)
        {
            oldFactory.Abort();
        }
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
        }
        Disconnect();
    }

    private static bool IsTransportFailure(Exception exception)
    {
        bool isCommunicationFailure = exception is CommunicationException and not FaultException;
        bool isTimeout = exception is TimeoutException;

        return isCommunicationFailure || isTimeout;
    }

    private static bool IsUnusable(ICommunicationObject communicationObject)
    {
        bool isUnusable = communicationObject.State is CommunicationState.Faulted
            or CommunicationState.Closing
            or CommunicationState.Closed;

        return isUnusable;
    }

    private static void CloseSafely(ICommunicationObject communicationObject)
    {
        try
        {
            communicationObject.Close();
        }
        catch (CommunicationException)
        {
            communicationObject.Abort();
        }
        catch (TimeoutException)
        {
            communicationObject.Abort();
        }
    }

    private IFriendshipNotificationService GetOrCreateChannel()
    {
        lock (_syncRoot)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_channel is not null && !IsUnusable((ICommunicationObject)_channel))
            {
                return _channel;
            }
            _channelFactory?.Abort();
            _callbackHandler = new FriendshipCallbackHandler();
            FriendshipCallbackHandler handler = _callbackHandler;
            handler.FriendRequestReceived += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _friendRequestReceived?.Invoke(this, value);
                }
            };
            handler.FriendshipEstablished += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _friendshipEstablished?.Invoke(this, value);
                }
            };
            handler.LobbyInvitationReceived += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _lobbyInvitationReceived?.Invoke(this, value);
                }
            };
            handler.LobbyInvitationWithdrawn += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _lobbyInvitationWithdrawn?.Invoke(this, value);
                }
            };
            NetTcpBinding binding = new(SecurityMode.None)
            {
                OpenTimeout = OperationTimeout,
                SendTimeout = OperationTimeout
            };
            _channelFactory = new DuplexChannelFactory<IFriendshipNotificationService>(
                new InstanceContext(handler), binding,
                new EndpointAddress(ServiceEndpoints.FriendshipNotificationServiceAddress));
            _channel = _channelFactory.CreateChannel();
            return _channel;
        }
    }

    private void AbortChannel(IFriendshipNotificationService channel)
    {
        ((ICommunicationObject)channel).Abort();

        lock (_syncRoot)
        {
            if (ReferenceEquals(_channel, channel))
            {
                _channel = null;
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/LobbyInvitationClient.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class LobbyInvitationClient : ILobbyInvitationClient
{
    private readonly IServiceCaller _serviceCaller;

    public LobbyInvitationClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, string), List<FriendEntry>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.GetInvitableFriends(args.Item1, args.Item2),
            (lobbyCode, sessionToken));
    }

    public ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, List<Guid>, string), List<Guid>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.SendLobbyInvitations(args.Item1, args.Item2, args.Item3),
            (lobbyCode, targetIds, sessionToken));
    }

    public ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, string, List<LobbyInvitationEntry>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, token) => channel.GetPendingLobbyInvitations(token),
            sessionToken);
    }

    public ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, string), bool>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.DeclineLobbyInvitation(args.Item1, args.Item2),
            (lobbyCode, sessionToken));
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/ServiceCaller.cs
```csharp
using System.Diagnostics;
using System.ServiceModel;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class ServiceCaller : IServiceCaller
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    public ClientResult<TResult> Call<TChannel, TArgument, TResult>(
        string address,
        Func<TChannel, TArgument, TResult> operation,
        TArgument argument)
        where TChannel : class
    {
        ChannelFactory<TChannel>? channelFactory = null;
        TChannel? channel = null;
        ClientResult<TResult> result;

        try
        {
            channelFactory = new ChannelFactory<TChannel>(CreateBinding(), new EndpointAddress(address));
            channel = channelFactory.CreateChannel();
            TResult value = operation(channel, argument);
            result = ClientResult<TResult>.Success(value);
        }
        catch (FaultException<ServiceFault> exception)
        {
            result = ClientResult<TResult>.BusinessFailure(exception.Detail.ErrorCode, exception.Detail.MessageKey);
        }
        catch (FaultException exception)
        {
            Debug.WriteLine($"ServiceCaller: server fault at {address}: {exception}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Unexpected);
        }
        catch (TimeoutException exception)
        {
            Debug.WriteLine($"ServiceCaller: timeout at {address}: {exception.Message}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Timeout);
        }
        catch (CommunicationException exception)
        {
            Debug.WriteLine($"ServiceCaller: unreachable {address}: {exception.Message}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.ServerUnreachable);
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"ServiceCaller: unexpected failure at {address}: {exception}");
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Unexpected);
        }
        finally
        {
            CloseCommunicationObject(channel as ICommunicationObject);
            CloseCommunicationObject(channelFactory);
        }

        return result;
    }

    private static NetTcpBinding CreateBinding()
    {
        return new NetTcpBinding(SecurityMode.None)
        {
            OpenTimeout = OperationTimeout,
            SendTimeout = OperationTimeout
        };
    }

    private static void CloseCommunicationObject(ICommunicationObject? communicationObject)
    {
        if (communicationObject is not null)
        {
            try
            {
                if (communicationObject.State == CommunicationState.Faulted)
                {
                    communicationObject.Abort();
                }
                else
                {
                    communicationObject.Close();
                }
            }
            catch (CommunicationException)
            {
                communicationObject.Abort();
            }
            catch (TimeoutException)
            {
                communicationObject.Abort();
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Results/ClientResult.cs
```csharp
using System.Diagnostics;

namespace Vertigo.Client.Networking.Results;

[DebuggerDisplay("{ToString(),nq}")]
public sealed class ClientResult<TValue>
{
    private ClientResult()
    {
    }

    public bool IsSuccess { get; private set; }

    public TValue? Value { get; private set; }

    public FailureKind Failure { get; private set; }
    
    public string ErrorCode { get; private set; } = string.Empty;

    public string MessageKey { get; private set; } = string.Empty;

    public bool IsConnectionFailure => Failure is FailureKind.ServerUnreachable or FailureKind.Timeout;

    public static ClientResult<TValue> Success(TValue value)
    {
        return new ClientResult<TValue> { IsSuccess = true, Value = value, Failure = FailureKind.None };
    }

    public static ClientResult<TValue> BusinessFailure(string errorCode, string messageKey)
    {
        return new ClientResult<TValue>
        {
            Failure = FailureKind.Business,
            ErrorCode = errorCode,
            MessageKey = messageKey
        };
    }

    public static ClientResult<TValue> TechnicalFailure(FailureKind failure)
    {
        return new ClientResult<TValue> { Failure = failure };
    }

    public override string ToString()
    {
        string text = IsSuccess ? "Success" : $"Failure {Failure}";

        if (Failure == FailureKind.Business)
        {
            text += $" {ErrorCode}";
        }

        return text;
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Results/FailureKind.cs
```csharp
namespace Vertigo.Client.Networking.Results;

public enum FailureKind
{
    None,
    Business,
    ServerUnreachable,
    Timeout,
    Unexpected
}
```

## File: src/Client/Vertigo.Client.Networking/Sessions/Interfaces/ISessionCoordinator.cs
```csharp
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Sessions.Interfaces;

public interface ISessionCoordinator
{
    ClientResult<UserSession> Login(string username, string password);
    ClientResult<UserSession> ResumeSession(string token);
    ClientResult<UserSession> CreateGuestSession();
    ClientResult<bool> Logout();
}
```

## File: src/Client/Vertigo.Client.Networking/Sessions/Interfaces/ISessionTransportCleaner.cs
```csharp
namespace Vertigo.Client.Networking.Sessions.Interfaces;

public interface ISessionTransportCleaner
{
    void DisconnectAll();
}
```

## File: src/Client/Vertigo.Client.Networking/Sessions/Services/SessionTransportCleaner.cs
```csharp
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.Networking.Sessions.Services;

public sealed class SessionTransportCleaner : ISessionTransportCleaner
{
    private readonly ILobbyClient _lobby;
    private readonly IFriendshipNotificationClient _notifications;

    public SessionTransportCleaner(ILobbyClient lobby, IFriendshipNotificationClient notifications)
    {
        _lobby = lobby;
        _notifications = notifications;
    }

    public void DisconnectAll()
    {
        _lobby.Disconnect();
        _notifications.Disconnect();
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Sessions/SessionCoordinator.cs
```csharp
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Sessions;

public sealed class SessionCoordinator : ISessionCoordinator
{
    private readonly IAuthClient _authClient;
    private readonly PlayerSession _playerSession;

    public SessionCoordinator(IAuthClient authClient, PlayerSession playerSession)
    {
        _authClient = authClient;
        _playerSession = playerSession;
    }

    public ClientResult<UserSession> Login(string username, string password)
    {
        ClientResult<UserSession> result = _authClient.Login(new LoginCredentials
        {
            Username = username,
            Password = password
        });
        ApplySession(result);
        return result;
    }

    public ClientResult<UserSession> ResumeSession(string token)
    {
        ClientResult<UserSession> result = _authClient.ResumeSession(token);
        ApplySession(result);
        return result;
    }

    public ClientResult<UserSession> CreateGuestSession()
    {
        ClientResult<UserSession> result = _authClient.CreateGuestSession();
        ApplySession(result);
        return result;
    }

    public ClientResult<bool> Logout()
    {
        SessionSnapshot? session = _playerSession.GetActiveSession();
        ClientResult<bool> result = ClientResult<bool>.Success(true);
        if (session is not null)
        {
            result = _authClient.Logout(session.Token);
            if (result.IsSuccess && result.Value)
            {
                _playerSession.Clear(session);
            }
        }

        return result;
    }

    private void ApplySession(ClientResult<UserSession> result)
    {
        if (result.IsSuccess && result.Value is UserSession session)
        {
            _playerSession.SetSession(new SessionSnapshot(
                session.PlayerId, session.Username, session.Token, session.ExpiresAt, session.IsGuest));
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/DependencyInjection.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Proxies;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.Networking.Sessions;
using Vertigo.Client.Networking.Sessions.Services;

namespace Vertigo.Client.Networking;

public static class DependencyInjection
{
    public static IServiceCollection AddVertigoClientNetworking(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<PlayerSession>();
        services.AddSingleton<INavigator, Navigator>();
        services.AddSingleton<IServiceCaller, ServiceCaller>();
        services.AddTransient<IAuthClient, AuthClient>();
        services.AddTransient<IAccountClient, AccountClient>();
        services.AddTransient<IBlockedPlayerClient, BlockedPlayerClient>();
        services.AddTransient<IFriendshipClient, FriendshipClient>();
        services.AddTransient<ILobbyInvitationClient, LobbyInvitationClient>();
        services.AddTransient<IChatClient, ChatClient>();
        services.AddSingleton<ISessionCoordinator, SessionCoordinator>();
        services.AddSingleton<ILobbyClient, LobbyClient>();
        services.AddSingleton<IFriendshipNotificationClient, FriendshipNotificationClient>();
        services.AddSingleton<ISessionTransportCleaner, SessionTransportCleaner>();
        return services;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Themes/Controls/Buttons.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style Selector="Button.vertigo-button">
        <Setter Property="MinHeight" Value="{DynamicResource ControlHeight}"/>
        <Setter Property="Padding" Value="{DynamicResource PaddingControl}"/>
        <Setter Property="CornerRadius" Value="{DynamicResource RadiusControl}"/>
        <Setter Property="FontFamily" Value="{DynamicResource FontBody}"/>
        <Setter Property="FontSize" Value="{DynamicResource FontSizeBody}"/>
        <Setter Property="FontWeight" Value="SemiBold"/>
        <Setter Property="HorizontalContentAlignment" Value="Center"/>
    </Style>
    <Style Selector="Button.vertigo-button.primary">
        <Setter Property="Background" Value="{DynamicResource ColorSuccess}"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextOnSuccess}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderNone}"/>
    </Style>
    <Style Selector="Button.vertigo-button.primary:pointerover">
        <Setter Property="Background" Value="{DynamicResource ColorSuccessHover}"/>
    </Style>
    <Style Selector="Button.vertigo-button.primary:pressed">
        <Setter Property="Background" Value="{DynamicResource ColorSuccessPressed}"/>
    </Style>
    <Style Selector="Button.vertigo-button.secondary">
        <Setter Property="Background" Value="{DynamicResource ColorAccent}"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextOnAccent}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderNone}"/>
    </Style>
    <Style Selector="Button.vertigo-button.secondary:pointerover">
        <Setter Property="Background" Value="{DynamicResource ColorAccentHover}"/>
    </Style>
    <Style Selector="Button.vertigo-button.secondary:pressed">
        <Setter Property="Background" Value="{DynamicResource ColorAccentPressed}"/>
    </Style>
    <Style Selector="Button.vertigo-button.danger">
        <Setter Property="Background" Value="{DynamicResource ColorError}"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextOnDanger}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderNone}"/>
    </Style>
    <Style Selector="Button.vertigo-button.danger:pointerover">
        <Setter Property="Background" Value="{DynamicResource ColorDangerHover}"/>
    </Style>
    <Style Selector="Button.vertigo-button.danger:pressed">
        <Setter Property="Background" Value="{DynamicResource ColorDangerPressed}"/>
    </Style>
    <Style Selector="Button.vertigo-button.neutral">
        <Setter Property="Background" Value="{DynamicResource ColorSurface}"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextPrimary}"/>
        <Setter Property="BorderBrush" Value="{DynamicResource ColorBorderStrong}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
    </Style>
    <Style Selector="Button.vertigo-button:disabled">
        <Setter Property="Background" Value="{DynamicResource ColorDisabledBackground}"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorDisabledForeground}"/>
    </Style>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Themes/Controls/Cards.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style Selector="Border.vertigo-card">
        <Setter Property="Background" Value="{DynamicResource ColorSurface}"/>
        <Setter Property="BorderBrush" Value="{DynamicResource ColorBorder}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
        <Setter Property="CornerRadius" Value="{DynamicResource RadiusCard}"/>
        <Setter Property="Padding" Value="{DynamicResource PaddingCard}"/>
    </Style>
    <Style Selector="Border.vertigo-card.muted">
        <Setter Property="Background" Value="{DynamicResource ColorSurfaceMuted}"/>
    </Style>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Themes/Controls/Lists.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style Selector="ListBox.vertigo-list">
        <Setter Property="Background" Value="{DynamicResource ColorSurface}"/>
        <Setter Property="BorderBrush" Value="{DynamicResource ColorDivider}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
    </Style>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Themes/Controls/StatusIndicators.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style Selector="Border.vertigo-status">
        <Setter Property="Padding" Value="{DynamicResource PaddingStatus}"/>
        <Setter Property="CornerRadius" Value="{DynamicResource RadiusBadge}"/>
        <Setter Property="Background" Value="{DynamicResource ColorSurfaceMuted}"/>
    </Style>
    <Style Selector="Border.vertigo-status.ready">
        <Setter Property="Background" Value="{DynamicResource ColorSuccessBackground}"/>
    </Style>
    <Style Selector="Border.vertigo-status.warning">
        <Setter Property="Background" Value="{DynamicResource ColorWarningBackground}"/>
    </Style>
    <Style Selector="Border.vertigo-status.error">
        <Setter Property="Background" Value="{DynamicResource ColorErrorBackground}"/>
    </Style>
    <Style Selector="TextBlock.vertigo-status-text">
        <Setter Property="FontFamily" Value="{DynamicResource FontBody}"/>
        <Setter Property="FontSize" Value="{DynamicResource FontSizeSmall}"/>
        <Setter Property="FontWeight" Value="SemiBold"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextPrimary}"/>
    </Style>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Themes/Controls/TextInputs.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style Selector="TextBox.vertigo-input">
        <Setter Property="MinHeight" Value="{DynamicResource ControlHeight}"/>
        <Setter Property="CornerRadius" Value="{DynamicResource RadiusInput}"/>
        <Setter Property="FontFamily" Value="{DynamicResource FontBody}"/>
        <Setter Property="FontSize" Value="{DynamicResource FontSizeBody}"/>
        <Setter Property="Background" Value="{DynamicResource ColorInputBackground}"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextPrimary}"/>
        <Setter Property="BorderBrush" Value="{DynamicResource ColorBorder}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
    </Style>
    <Style Selector="TextBox.vertigo-input:focus">
        <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
    </Style>
    <Style Selector="TextBox.vertigo-input.invalid">
        <Setter Property="BorderBrush" Value="{DynamicResource ColorError}"/>
        <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
    </Style>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Themes/Layouts/Panels.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style Selector="Border.vertigo-screen">
        <Setter Property="Background" Value="{DynamicResource ColorCanvas}"/>
    </Style>
    <Style Selector="TextBlock.vertigo-title">
        <Setter Property="FontFamily" Value="{DynamicResource FontTitleDisplay}"/>
        <Setter Property="FontSize" Value="{DynamicResource FontSizeTitle}"/>
        <Setter Property="FontWeight" Value="Bold"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextPrimary}"/>
    </Style>
    <Style Selector="TextBlock.vertigo-section">
        <Setter Property="FontFamily" Value="{DynamicResource FontSection}"/>
        <Setter Property="FontSize" Value="{DynamicResource FontSizeSection}"/>
        <Setter Property="FontWeight" Value="SemiBold"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextPrimary}"/>
    </Style>
    <Style Selector="TextBlock.vertigo-body">
        <Setter Property="FontFamily" Value="{DynamicResource FontBody}"/>
        <Setter Property="FontSize" Value="{DynamicResource FontSizeBody}"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextPrimary}"/>
    </Style>
    <Style Selector="TextBlock.vertigo-code">
        <Setter Property="FontFamily" Value="{DynamicResource FontMono}"/>
        <Setter Property="FontSize" Value="{DynamicResource FontSizeMono}"/>
        <Setter Property="FontWeight" Value="Bold"/>
        <Setter Property="Foreground" Value="{DynamicResource ColorTextPrimary}"/>
    </Style>
    <Style Selector="Path.vertigo-icon">
        <Setter Property="Width" Value="{DynamicResource IconSize}"/>
        <Setter Property="Height" Value="{DynamicResource IconSize}"/>
        <Setter Property="Stretch" Value="Uniform"/>
        <Setter Property="Stroke" Value="{DynamicResource ColorTextPrimary}"/>
        <Setter Property="StrokeThickness" Value="{DynamicResource StrokeStrong}"/>
        <Setter Property="StrokeLineCap" Value="Round"/>
    </Style>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Themes/Tokens/Colors.axaml
```
<ResourceDictionary xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <SolidColorBrush x:Key="ColorShellGray" Color="#D3D3D8"/>
    <SolidColorBrush x:Key="ColorCoal" Color="#1E1F22"/>
    <SolidColorBrush x:Key="ColorWhite" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorSnesBlue" Color="#2B6CB0"/>
    <SolidColorBrush x:Key="ColorSnesGreen" Color="#38A169"/>
    <SolidColorBrush x:Key="ColorSnesYellow" Color="#D69E2E"/>
    <SolidColorBrush x:Key="ColorSnesRed" Color="#E53E3E"/>
    <SolidColorBrush x:Key="ColorPrimary" Color="#1E1F22"/>
    <SolidColorBrush x:Key="ColorAccent" Color="#2B6CB0"/>
    <SolidColorBrush x:Key="ColorSuccess" Color="#24784B"/>
    <SolidColorBrush x:Key="ColorError" Color="#B82525"/>
    <SolidColorBrush x:Key="ColorWarningText" Color="#6B4A12"/>
    <SolidColorBrush x:Key="ColorWarningBackground" Color="#FFF5D9"/>
    <SolidColorBrush x:Key="ColorInfoBackground" Color="#EAF2FC"/>
    <SolidColorBrush x:Key="ColorErrorBackground" Color="#FFF0F0"/>
    <SolidColorBrush x:Key="ColorSuccessBackground" Color="#E9F7EF"/>
    <SolidColorBrush x:Key="ColorSurface" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorSurfaceMuted" Color="#F5F5F7"/>
    <SolidColorBrush x:Key="ColorSurfaceRaised" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorCanvas" Color="#F3F4F6"/>
    <SolidColorBrush x:Key="ColorBorder" Color="#B7BAC3"/>
    <SolidColorBrush x:Key="ColorBorderStrong" Color="#646873"/>
    <SolidColorBrush x:Key="ColorDivider" Color="#D5D7DC"/>
    <SolidColorBrush x:Key="ColorTextPrimary" Color="#1E1F22"/>
    <SolidColorBrush x:Key="ColorTextSecondary" Color="#4B4E56"/>
    <SolidColorBrush x:Key="ColorTextMuted" Color="#62656E"/>
    <SolidColorBrush x:Key="ColorTextOnDark" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorTextOnAccent" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorTextOnSuccess" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorTextOnDanger" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorPrimaryHover" Color="#303237"/>
    <SolidColorBrush x:Key="ColorAccentHover" Color="#21578F"/>
    <SolidColorBrush x:Key="ColorAccentPressed" Color="#194570"/>
    <SolidColorBrush x:Key="ColorSuccessHover" Color="#276D49"/>
    <SolidColorBrush x:Key="ColorSuccessPressed" Color="#205C3D"/>
    <SolidColorBrush x:Key="ColorDangerHover" Color="#B52C2C"/>
    <SolidColorBrush x:Key="ColorDangerPressed" Color="#922323"/>
    <SolidColorBrush x:Key="ColorFocus" Color="#2B6CB0"/>
    <SolidColorBrush x:Key="ColorInputBackground" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="ColorDisabledBackground" Color="#E6E7EA"/>
    <SolidColorBrush x:Key="ColorDisabledForeground" Color="#62656E"/>
</ResourceDictionary>
```

## File: src/Client/Vertigo.Client.UI/Themes/Tokens/Icons.axaml
```
<ResourceDictionary xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StreamGeometry x:Key="IconBuilding">
        M 5,21 L 5,5 L 15,5 L 15,21 M 15,10 L 20,10 L 20,21 M 3,21 L 22,21 M 8,9 L 11,9 M 8,13 L
        11,13 M 8,17 L 11,17 M 17,14 L 18,14 M 17,18 L 18,18
    </StreamGeometry>
    <StreamGeometry x:Key="IconFloors">
        M 4,20 L 20,20 M 5,15 L 19,15 M 6,10 L 18,10 M 7,5 L 17,5
    </StreamGeometry>
    <StreamGeometry x:Key="IconReady">
        M 4,12 L 9,17 L 20,6
    </StreamGeometry>
    <StreamGeometry x:Key="IconArrowUp">
        M 12,20 L 12,4 M 5,11 L 12,4 L 19,11
    </StreamGeometry>
    <StreamGeometry x:Key="IconChat">
        M 4,5 L 20,5 L 20,17 L 9,17 L 4,21 Z
    </StreamGeometry>
    <StreamGeometry x:Key="IconPeople">
        M 12,11 A 3,3 0 1 0 12,5 A 3,3 0 1 0 12,11 Z M 5,20 C 5,15 8,13 12,13 C 16,13 19,15 19,20
        M 18,7 C 21,8 21,11 18,12
    </StreamGeometry>
    <StreamGeometry x:Key="IconWarning">
        M 12,3 L 22,21 L 2,21 Z M 12,9 L 12,15 M 12,18 L 12,18.5
    </StreamGeometry>
    <StreamGeometry x:Key="IconLock">
        M 5,11 L 19,11 L 19,21 L 5,21 Z M 8,11 L 8,8 A 4,4 0 0 1 16,8 L 16,11
    </StreamGeometry>
</ResourceDictionary>
```

## File: src/Client/Vertigo.Client.UI/Themes/Tokens/Shapes.axaml
```
<ResourceDictionary xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <CornerRadius x:Key="RadiusSmall">4</CornerRadius>
    <CornerRadius x:Key="RadiusInput">8</CornerRadius>
    <CornerRadius x:Key="RadiusControl">8</CornerRadius>
    <CornerRadius x:Key="RadiusCard">12</CornerRadius>
    <CornerRadius x:Key="RadiusBadge">6</CornerRadius>
    <x:Double x:Key="StrokeThin">1</x:Double>
    <x:Double x:Key="StrokeStrong">2</x:Double>
    <Thickness x:Key="BorderNone">0</Thickness>
    <Thickness x:Key="BorderThin">1</Thickness>
    <Thickness x:Key="BorderStrong">2</Thickness>
</ResourceDictionary>
```

## File: src/Client/Vertigo.Client.UI/Themes/Tokens/Spacing.axaml
```
<ResourceDictionary xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <x:Double x:Key="Space4">4</x:Double>
    <x:Double x:Key="Space8">8</x:Double>
    <x:Double x:Key="Space12">12</x:Double>
    <x:Double x:Key="Space16">16</x:Double>
    <x:Double x:Key="Space24">24</x:Double>
    <x:Double x:Key="Space32">32</x:Double>
    <x:Double x:Key="ControlHeight">44</x:Double>
    <x:Double x:Key="ControlHeightCompact">36</x:Double>
    <x:Double x:Key="ContentMaxWidth">1200</x:Double>
    <x:Double x:Key="FormMaxWidth">440</x:Double>
    <Thickness x:Key="PaddingCard">24</Thickness>
    <Thickness x:Key="PaddingPanel">24,20</Thickness>
    <Thickness x:Key="PaddingControl">16,10</Thickness>
    <Thickness x:Key="MarginScreen">24</Thickness>
    <Thickness x:Key="PaddingStatus">8,4</Thickness>
    <x:Double x:Key="IconSize">24</x:Double>
</ResourceDictionary>
```

## File: src/Client/Vertigo.Client.UI/Themes/Tokens/Typography.axaml
```
<ResourceDictionary xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <FontFamily x:Key="FontTitleDisplay">Space Grotesk, Montserrat, Inter</FontFamily>
    <FontFamily x:Key="FontSection">Space Grotesk, Montserrat, Inter</FontFamily>
    <FontFamily x:Key="FontBody">Inter, Arial, sans-serif</FontFamily>
    <FontFamily x:Key="FontBodyEmphasis">Inter, Arial, sans-serif</FontFamily>
    <FontFamily x:Key="FontMono">JetBrains Mono, Consolas, Menlo, monospace</FontFamily>
    <x:Double x:Key="FontSizeDisplay">36</x:Double>
    <x:Double x:Key="FontSizeTitle">32</x:Double>
    <x:Double x:Key="FontSizeSection">20</x:Double>
    <x:Double x:Key="FontSizeBody">15</x:Double>
    <x:Double x:Key="FontSizeSmall">13</x:Double>
    <x:Double x:Key="FontSizeMono">24</x:Double>
</ResourceDictionary>
```

## File: src/Client/Vertigo.Client.UI/Themes/VertigoTheme.axaml
```
<Styles xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Styles.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceInclude Source="avares://Vertigo.Client.UI/Themes/Tokens/Colors.axaml"/>
                <ResourceInclude Source="avares://Vertigo.Client.UI/Themes/Tokens/Typography.axaml"/>
                <ResourceInclude Source="avares://Vertigo.Client.UI/Themes/Tokens/Spacing.axaml"/>
                <ResourceInclude Source="avares://Vertigo.Client.UI/Themes/Tokens/Shapes.axaml"/>
                <ResourceInclude Source="avares://Vertigo.Client.UI/Themes/Tokens/Icons.axaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Styles.Resources>
    <StyleInclude Source="avares://Vertigo.Client.UI/Themes/Controls/Buttons.axaml"/>
    <StyleInclude Source="avares://Vertigo.Client.UI/Themes/Controls/TextInputs.axaml"/>
    <StyleInclude Source="avares://Vertigo.Client.UI/Themes/Controls/Cards.axaml"/>
    <StyleInclude Source="avares://Vertigo.Client.UI/Themes/Controls/StatusIndicators.axaml"/>
    <StyleInclude Source="avares://Vertigo.Client.UI/Themes/Controls/Lists.axaml"/>
    <StyleInclude Source="avares://Vertigo.Client.UI/Themes/Layouts/Panels.axaml"/>
    <StyleInclude Source="avares://Vertigo.Client.UI/Themes/Controls/Alerts.axaml"/>
</Styles>
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Interfaces/IClientErrorText.cs
```csharp
using System;
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.UI.Utilities.Localization.Interfaces;

public interface IClientErrorText
{
    string FromResult<T>(ClientResult<T> result, string fallbackKey);
    string FromException(Exception exception, string fallbackKey);
    bool IsUnauthorized<T>(ClientResult<T> result);
    bool IsUnauthorized(Exception exception);
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Interfaces/ITextLocalizer.cs
```csharp
namespace Vertigo.Client.UI.Utilities.Localization.Interfaces;

public interface ITextLocalizer
{
    string Get(string key);
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Markup/LocalizeExtension.cs
```csharp
using System;
using Avalonia.Markup.Xaml;
using Vertigo.Client.UI.Utilities.Localization.Services;

namespace Vertigo.Client.UI.Utilities.Localization.Markup;

public sealed class LocalizeExtension : MarkupExtension
{
    private static readonly TextLocalizer Localizer = new();

    public LocalizeExtension(string key)
    {
        Key = key;
    }

    public string Key { get; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        string value = Localizer.Get(Key);
        return value;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Services/ClientErrorText.cs
```csharp
using System;
using System.ServiceModel;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.Utilities.Localization.Services;

public sealed class ClientErrorText : IClientErrorText
{
    private readonly ITextLocalizer _localizer;

    public ClientErrorText(ITextLocalizer localizer)
    {
        _localizer = localizer;
    }

    public string FromResult<T>(ClientResult<T> result, string fallbackKey)
    {
        if (result.Failure == FailureKind.Business)
        {
            return _localizer.Get(string.IsNullOrWhiteSpace(result.MessageKey) ? fallbackKey : result.MessageKey);
        }

        string key = result.Failure switch
        {
            FailureKind.Timeout => "Error.Timeout",
            FailureKind.ServerUnreachable => "Error.Connection",
            _ => fallbackKey
        };
        return _localizer.Get(key);
    }

    public string FromException(Exception exception, string fallbackKey)
    {
        if (exception is FaultException<ServiceFault> fault)
        {
            return _localizer.Get(string.IsNullOrWhiteSpace(fault.Detail?.MessageKey)
                ? fallbackKey : fault.Detail.MessageKey);
        }

        string key = exception switch
        {
            TimeoutException => "Error.Timeout",
            CommunicationException => "Error.Connection",
            _ => fallbackKey
        };
        return _localizer.Get(key);
    }

    public bool IsUnauthorized<T>(ClientResult<T> result)
    {
        return result.Failure == FailureKind.Business
            && result.ErrorCode == ServiceErrorCodes.Unauthorized;
    }

    public bool IsUnauthorized(Exception exception)
    {
        return exception is FaultException<ServiceFault> fault
            && fault.Detail?.ErrorCode == ServiceErrorCodes.Unauthorized;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Localization/Services/TextLocalizer.cs
```csharp
using System.Globalization;
using System.Resources;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.Utilities.Localization.Services;

public sealed class TextLocalizer : ITextLocalizer
{
    private const string ResourceBaseName = "Vertigo.Client.UI.Resources.Localization.Strings";
    private readonly ResourceManager _resources = new(ResourceBaseName, typeof(TextLocalizer).Assembly);

    public string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return _resources.GetString("Error.Unknown", CultureInfo.CurrentUICulture) ?? "Ocurrió un error.";
        }

        return _resources.GetString(key, CultureInfo.CurrentUICulture)
            ?? _resources.GetString("Error.Unknown", CultureInfo.CurrentUICulture)
            ?? "Ocurrió un error.";
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Sessions/Interfaces/ISessionGuard.cs
```csharp
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.Utilities.Sessions.Interfaces;

public interface ISessionGuard
{
    bool HasActiveSession();
    bool IsCurrent(SessionSnapshot session);
    void Invalidate(SessionSnapshot? session = null);
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Sessions/Services/SessionGuard.cs
```csharp
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.Utilities.Sessions.Services;

public sealed class SessionGuard : ISessionGuard
{
    private readonly PlayerSession _session;
    private readonly INavigator _navigation;
    private readonly ISessionTransportCleaner _transport;

    public SessionGuard(PlayerSession session, INavigator navigation, ISessionTransportCleaner transport)
    {
        _session = session;
        _navigation = navigation;
        _transport = transport;
    }

    public bool HasActiveSession()
    {
        if (_session.GetActiveSession() is not null)
        {
            return true;
        }

        Invalidate();
        return false;
    }

    public bool IsCurrent(SessionSnapshot session)
    {
        return ReferenceEquals(_session.GetActiveSession(), session);
    }

    public void Invalidate(SessionSnapshot? session = null)
    {
        // A late response from an older session cannot close the current channels.
        if (session is not null)
        {
            SessionSnapshot? current = _session.GetActiveSession();
            if (current is not null && !ReferenceEquals(current, session))
            {
                return;
            }
            _session.Clear(session);
        }

        if (_session.GetActiveSession() is not null)
        {
            return;
        }

        _transport.DisconnectAll();
        _navigation.NavigateTo(NavigationPage.Access);
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Settings/Interfaces/IUserSettingsStore.cs
```csharp
using Vertigo.Client.UI.Utilities.Settings.Models;

namespace Vertigo.Client.UI.Utilities.Settings.Interfaces;

public interface IUserSettingsStore
{
    UserSettings Load();
    bool TrySave(UserSettings settings);
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Settings/Models/UserSettings.cs
```csharp
namespace Vertigo.Client.UI.Utilities.Settings.Models;

public sealed record UserSettings
{
    public string Language { get; init; } = "es";
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Settings/Services/JsonUserSettingsStore.cs
```csharp
using System;
using System.IO;
using System.Text.Json;
using Vertigo.Client.UI.Utilities.Settings.Interfaces;
using Vertigo.Client.UI.Utilities.Settings.Models;

namespace Vertigo.Client.UI.Utilities.Settings.Services;

public sealed class JsonUserSettingsStore : IUserSettingsStore
{
    private readonly string _path;

    public JsonUserSettingsStore()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Vertigo", "settings.json"))
    {
    }

    public JsonUserSettingsStore(string path)
    {
        _path = path;
    }

    public UserSettings Load()
    {
        UserSettings result = new();
        try
        {
            if (File.Exists(_path))
            {
                UserSettings? stored = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(_path));
                if (stored is not null && IsSupportedLanguage(stored.Language))
                {
                    result = stored;
                }
            }
        }
        catch (IOException)
        {
            result = new UserSettings();
        }
        catch (UnauthorizedAccessException)
        {
            result = new UserSettings();
        }
        catch (JsonException)
        {
            result = new UserSettings();
        }
        return result;
    }

    public bool TrySave(UserSettings settings)
    {
        bool success = false;
        if (IsSupportedLanguage(settings.Language))
        {
            string? tempPath = null;
            try
            {
                string directory = Path.GetDirectoryName(_path)
                    ?? throw new InvalidOperationException("Settings directory was not provided.");
                Directory.CreateDirectory(directory);
                tempPath = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(tempPath, JsonSerializer.Serialize(settings));
                File.Move(tempPath, _path, true);
                success = true;
            }
            catch (IOException)
            {
                success = false;
            }
            catch (UnauthorizedAccessException)
            {
                success = false;
            }
            finally
            {
                if (tempPath is not null)
                {
                    try
                    {
                        if (File.Exists(tempPath))
                        {
                            File.Delete(tempPath);
                        }
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
        }
        return success;
    }

    public static bool IsSupportedLanguage(string? language)
    {
        return language is "es" or "en";
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Settings/Services/UiLanguage.cs
```csharp
using System.Globalization;

namespace Vertigo.Client.UI.Utilities.Settings.Services;

public static class UiLanguage
{
    public static void Apply(string language)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(language == "en" ? "en" : "es");
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Validation/FieldValidationError.cs
```csharp
namespace Vertigo.Client.UI.Utilities.Validation;

public enum FieldValidationError
{
    None,
    Required,
    TooShort,
    TooLong,
    InvalidFormat,
    PasswordRequirementsNotMet,
    PasswordsDoNotMatch
}
```

## File: src/Client/Vertigo.Client.UI/Utilities/Validation/RegistrationRules.cs
```csharp
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

namespace Vertigo.Client.UI.Utilities.Validation;

public static class RegistrationRules
{
    public const int UsernameMinimumLength = 3;
    public const int UsernameMaximumLength = 20;
    public const int EmailMaximumLength = 60;
    public const int PasswordMinimumLength = 8;
    public const int PasswordMaximumLength = 20;

    public static FieldValidationError ValidateUsername(string? username)
    {
        FieldValidationError error = FieldValidationError.None;
        string normalized = username?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
        {
            error = FieldValidationError.Required;
        }
        else if (normalized.Length < UsernameMinimumLength)
        {
            error = FieldValidationError.TooShort;
        }
        else if (normalized.Length > UsernameMaximumLength)
        {
            error = FieldValidationError.TooLong;
        }
        else if (!normalized.All(IsAllowedUsernameCharacter) || !normalized.Any(IsAsciiLetter))
        {
            error = FieldValidationError.InvalidFormat;
        }

        return error;
    }

    public static FieldValidationError ValidateEmail(string? email)
    {
        FieldValidationError error = FieldValidationError.None;
        string normalized = email?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
        {
            error = FieldValidationError.Required;
        }
        else if (normalized.Length > EmailMaximumLength)
        {
            error = FieldValidationError.TooLong;
        }
        else if (!IsValidEmail(normalized))
        {
            error = FieldValidationError.InvalidFormat;
        }

        return error;
    }

    public static FieldValidationError ValidatePassword(string? password)
    {
        FieldValidationError error = FieldValidationError.None;

        if (string.IsNullOrEmpty(password))
        {
            error = FieldValidationError.Required;
        }
        else if (password.Length < PasswordMinimumLength)
        {
            error = FieldValidationError.TooShort;
        }
        else if (password.Length > PasswordMaximumLength)
        {
            error = FieldValidationError.TooLong;
        }
        else if (!password.All(IsAllowedPasswordCharacter)
                 || !password.Any(char.IsUpper)
                 || !password.Any(char.IsDigit)
                 || !password.Any(IsSpecialPasswordCharacter))
        {
            error = FieldValidationError.PasswordRequirementsNotMet;
        }

        return error;
    }

    public static FieldValidationError ValidateConfirmation(string? password, string? confirmation)
    {
        FieldValidationError error = FieldValidationError.None;

        if (string.IsNullOrEmpty(confirmation))
        {
            error = FieldValidationError.Required;
        }
        else if (!string.Equals(password, confirmation, StringComparison.Ordinal))
        {
            error = FieldValidationError.PasswordsDoNotMatch;
        }

        return error;
    }

    private static bool IsAllowedUsernameCharacter(char character)
    {
        UnicodeCategory category = char.GetUnicodeCategory(character);
        bool isLetter = char.IsLetter(character);
        bool isNumber = category == UnicodeCategory.DecimalDigitNumber;
        return isLetter || isNumber;
    }

    private static bool IsAsciiLetter(char character)
    {
        bool isLetter = char.IsAsciiLetter(character);
        return isLetter;
    }

    private static bool IsSpecialPasswordCharacter(char character)
    {
        bool isSpecial = AllowedPasswordSymbols.Contains(character);
        return isSpecial;
    }

    private static bool IsAllowedPasswordCharacter(char character)
    {
        bool isAllowed = char.IsAsciiLetterOrDigit(character)
            || IsSpecialPasswordCharacter(character);
        return isAllowed;
    }

    private const string AllowedPasswordSymbols = "!@#$%&*?_+-.";

    private static bool IsValidEmail(string email)
    {
        bool isValid = false;
        int separator = email.IndexOf('@');
        int lastSeparator = email.LastIndexOf('@');

        if (separator > 0 && separator == lastSeparator && separator < email.Length - 1
            && !email.Any(char.IsWhiteSpace))
        {
            string local = email[..separator];
            string domain = email[(separator + 1)..];
            string[] labels = domain.Split('.');
            bool validLocal = local.Length <= 64 && !local.StartsWith('.')
                && !local.EndsWith('.') && !local.Contains("..", StringComparison.Ordinal)
                && local.All(IsAllowedEmailLocalCharacter);
            bool validDomain = labels.Length >= 2 && labels.All(IsValidDomainLabel);

            if (validLocal && validDomain)
            {
                try
                {
                    MailAddress address = new(email);
                    isValid = string.Equals(address.Address, email, StringComparison.Ordinal);
                }
                catch (FormatException)
                {
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    private static bool IsAllowedEmailLocalCharacter(char character)
    {
        bool isAllowed = char.IsAsciiLetterOrDigit(character)
            || character is '.' or '_' or '%' or '+' or '-';
        return isAllowed;
    }

    private static bool IsValidDomainLabel(string label)
    {
        bool valid = label.Length is >= 1 and <= 63
            && label[0] != '-' && label[^1] != '-'
            && label.All(character => char.IsAsciiLetterOrDigit(character) || character == '-');
        return valid;
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/AccessContext.cs
```csharp
using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record AccessContext(ISessionCoordinator Sessions, UiNavigationContext Ui);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/HomeContext.cs
```csharp
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record HomeContext(ISessionCoordinator Sessions, PlayerSession PlayerSession,
    UiNavigationContext Ui);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/LobbyContext.cs
```csharp
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record LobbyContext(PlayerSession PlayerSession, UiNavigationContext Ui);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/LobbyInvitationContext.cs
```csharp
using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record LobbyInvitationContext(LobbyViewModel Lobby, ITextLocalizer Text);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/MainScreensContext.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MainScreensContext(LobbyViewModel Lobby, SettingsViewModel Settings,
    ProfileViewModel Profile);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/MainSocialContext.cs
```csharp
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MainSocialContext(PlayerSession PlayerSession, SocialViewModel Social);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/SocialNavigationContext.cs
```csharp
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record SocialNavigationContext(PlayerSession Session, INavigator Navigation,
    UiServices Services);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/SocialViewModelContext.cs
```csharp
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.ViewModels.Social;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record SocialViewModelContext(
    LobbyInvitationsViewModel Invitations,
    IFriendshipNotificationClient Notifications,
    SocialNavigationContext NavigationContext);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/UiNavigationContext.cs
```csharp
using Vertigo.Client.Core.Navigation;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record UiNavigationContext(INavigator Navigation, UiServices Services);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/UiServices.cs
```csharp
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record UiServices(IClientErrorText Errors, ITextLocalizer Text,
    ISessionGuard SessionGuard);
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Lifecycle/INavigationAware.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Lifecycle;

public interface INavigationAware
{
    void OnNavigatedTo();
    void OnNavigatedFrom();
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Lifecycle/LobbyPreparationRevision.cs
```csharp
using System;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Lifecycle;

/// <summary>Keeps the latest accepted preparation revision for the active lobby.</summary>
public sealed class LobbyPreparationRevision
{
    private long _revision = -1;

    public bool TryAdvance(LobbyPreparation state, string activeLobbyCode)
    {
        bool accepted = !string.IsNullOrWhiteSpace(activeLobbyCode)
            && string.Equals(state.LobbyCode, activeLobbyCode, StringComparison.OrdinalIgnoreCase)
            && state.Revision > _revision;

        if (accepted)
        {
            _revision = state.Revision;
        }

        return accepted;
    }

    public void Reset()
    {
        _revision = -1;
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Social/BlockedPlayersViewModel.cs
```csharp
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
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Social/SocialViewModelBase.cs
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Social;

public abstract class SocialViewModelBase : ObservableObject
{
    protected readonly SocialOperation Operation;
    protected readonly ITextLocalizer Text;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    protected SocialViewModelBase(SocialOperation operation, ITextLocalizer text)
    {
        Operation = operation;
        Text = text;
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        protected set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    public void ClearError() => ErrorMessage = string.Empty;
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Utilities/SocialOperation.cs
```csharp
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Utilities;

public sealed class SocialOperation
{
    private readonly PlayerSession _session;
    private readonly ISessionGuard _guard;
    private readonly IClientErrorText _errors;

    public SocialOperation(PlayerSession session, ISessionGuard guard, IClientErrorText errors)
    {
        _session = session;
        _guard = guard;
        _errors = errors;
    }

    public SessionSnapshot? GetRegisteredSession()
    {
        SessionSnapshot? session = _session.GetActiveSession();
        if (session is null)
        {
            _guard.Invalidate();
            return null;
        }
        return session.IsGuest ? null : session;
    }

    public bool IsCurrent(SessionSnapshot original)
    {
        return _guard.IsCurrent(original);
    }

    public bool Check<T>(ClientResult<T> result, SessionSnapshot original)
    {
        if (!_guard.IsCurrent(original))
        {
            return false;
        }
        if (_errors.IsUnauthorized(result))
        {
            _guard.Invalidate(original);
            return false;
        }
        return true;
    }

    public string Error<T>(ClientResult<T> result, string fallbackKey)
    {
        return _errors.FromResult(result, fallbackKey);
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/ProfileViewModel.cs
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UI.ViewModels;

public sealed class ProfileViewModel : ObservableObject, INavigationAware
{
    private readonly PlayerSession _session;
    private readonly INavigator _navigation;
    private readonly ITextLocalizer _text;

    public ProfileViewModel(PlayerSession session, INavigator navigation, ITextLocalizer text)
    {
        _session = session;
        _navigation = navigation;
        _text = text;
        BackCommand = new RelayCommand(() => _navigation.NavigateTo(NavigationPage.Home));
    }

    public IRelayCommand BackCommand { get; }
    public string HeadingText => _text.Get("Profile.Title");
    public string UsernameText => _text.Get("Profile.Username");
    public string AccountText => _text.Get("Profile.Account");
    public string BackText => _text.Get("Navigation.Back");
    public string Username => _session.GetActiveSession()?.Username ?? string.Empty;
    public string AccountType => _text.Get(_session.GetActiveSession()?.IsGuest == true
        ? "Home.GuestAccount" : "Home.RegisteredAccount");

    public void OnNavigatedTo()
    {
        OnPropertyChanged(nameof(HeadingText));
        OnPropertyChanged(nameof(UsernameText));
        OnPropertyChanged(nameof(AccountText));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(AccountType));
    }

    public void OnNavigatedFrom()
    {
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/SettingsViewModel.cs
```csharp
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
```

## File: src/Client/Vertigo.Client.UI/ViewModels/ViewModelBase.cs
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace Vertigo.Client.UI.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
}
```

## File: src/Client/Vertigo.Client.UI/Views/AccessView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             xmlns:validation="using:Vertigo.Client.UI.Utilities.Validation"
             x:Class="Vertigo.Client.UI.Views.AccessView"
             x:DataType="vm:AccessViewModel"
             x:CompileBindings="True">
    <UserControl.Styles>
        <Style Selector="Button.vertigo-button:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <ScrollViewer HorizontalScrollBarVisibility="Disabled"
                  VerticalScrollBarVisibility="Auto"
                  Background="{DynamicResource ColorCanvas}">
        <Grid RowDefinitions="Auto,*" MinHeight="560">
            <Border Grid.Row="0"
                    Background="{DynamicResource ColorCoal}"
                    Padding="{DynamicResource PaddingPanel}">
                <Grid ColumnDefinitions="Auto,*,Auto">
                    <Path Width="24"
                          Height="28"
                          Fill="{DynamicResource ColorWhite}"
                          Stretch="Uniform"
                          Data="M1,28 L1,13 8,13 8,4 15,4 15,19 22,19 22,28 Z 
                                M4,16 L4,19 6,19 6,16 Z M11,8 L11,11 13,11 13,8 Z"/>
                    <TextBlock Grid.Column="1"
                               Margin="12,0,0,0"
                               VerticalAlignment="Center"
                               Classes="vertigo-section"
                               Foreground="{DynamicResource ColorWhite}"
                               Text="VÉRTIGO"/>
                    <Button Grid.Column="2"
                            Classes="vertigo-button neutral"
                            TabIndex="0"
                            IsEnabled="{Binding !IsBusy}"
                            Command="{Binding ToggleLanguageCommand}"
                            Content="{Binding LanguageButtonText}"
                            AutomationProperties.Name="{Binding LanguageButtonHelp}"/>
                </Grid>
            </Border>
            <Grid Grid.Row="1"
                  MaxWidth="1100"
                  HorizontalAlignment="Center"
                  Margin="{DynamicResource MarginScreen}"
                  ColumnDefinitions="*,Auto">
                <Border Grid.Column="0"
                        MinWidth="300"
                        MaxWidth="420"
                        VerticalAlignment="Center"
                        Background="{DynamicResource ColorSurface}"
                        BorderBrush="{DynamicResource ColorBorder}"
                        BorderThickness="{DynamicResource BorderThin}"
                        CornerRadius="{DynamicResource RadiusCard}"
                        Padding="{DynamicResource PaddingCard}">
                    <StackPanel Spacing="{DynamicResource Space12}">
                        <TextBlock Classes="vertigo-title"
                                   Text="{Binding TitleText}"/>
                        <TextBlock Text="{Binding DescriptionText}"
                                   Foreground="{DynamicResource ColorTextSecondary}"
                                   TextWrapping="Wrap"/>
                        <StackPanel IsVisible="{Binding !IsRegistering}" Spacing="{DynamicResource Space12}">
                            <TextBlock Text="{Binding UsernameLabel}"
                                       FontWeight="SemiBold"/>
                            <TextBox
                                     Classes="vertigo-input"
                                     TabIndex="1"
                                     Text="{Binding Username, Mode=TwoWay}"
                                     PlaceholderText="{Binding UsernameLabel}"
                                     AutomationProperties.Name="{Binding UsernameLabel}"/>
                            <TextBlock Text="{Binding PasswordLabel}"
                                       FontWeight="SemiBold"/>
                            <TextBox Classes="vertigo-input"
                                     TabIndex="2"
                                     PasswordChar="●"
                                     Text="{Binding Password, Mode=TwoWay}"
                                     PlaceholderText="{Binding PasswordLabel}"
                                     AutomationProperties.Name="{Binding PasswordLabel}"/>
                            <Button Classes="vertigo-button primary"
                                    TabIndex="3"
                                    IsEnabled="{Binding !IsBusy}"
                                    Command="{Binding LoginCommand}"
                                    Content="{Binding LoginText}"/>
                            <Button Classes="vertigo-button secondary"
                                    TabIndex="4"
                                    IsEnabled="{Binding !IsBusy}"
                                    Command="{Binding GuestCommand}"
                                    Content="{Binding GuestText}"/>
                        </StackPanel>
                        <TextBlock Text="{Binding ErrorMessage}"
                                   Foreground="{DynamicResource ColorError}"
                                   FontWeight="SemiBold"
                                   TextWrapping="Wrap"
                                   AutomationProperties.HelpText="{Binding ErrorHeadingText}"/>
                        <TextBlock IsVisible="{Binding IsBusy}"
                                   Text="{Binding ProgressText}"
                                   Foreground="{DynamicResource ColorTextSecondary}"/>
                        <Button Classes="vertigo-button neutral"
                                TabIndex="5"
                                IsEnabled="{Binding !IsBusy}"
                                IsVisible="{Binding !IsRegistering}"
                                Command="{Binding ShowRegistrationCommand}"
                                Content="{Binding RegisterText}"/>
                        <Button Classes="vertigo-button neutral"
                                TabIndex="6"
                                IsVisible="{Binding IsRegistering}"
                                Command="{Binding CancelRegistrationCommand}"
                                Content="{Binding BackText}"/>
                        <StackPanel IsVisible="{Binding IsRegistering}" Spacing="{DynamicResource Space12}">
                            <TextBlock Text="{Binding RegisterHeadingText}"
                                       Classes="vertigo-section"/>
                            <TextBlock Text="{Binding UsernameLabel}"/>
                            <TextBox Classes="vertigo-input"
                                     TabIndex="7"
                                     Text="{Binding RegisterUsername, Mode=TwoWay}"
                                     MaxLength="{x:Static validation:RegistrationRules.UsernameMaximumLength}"
                                     AutomationProperties.Name="{Binding UsernameLabel}"/>
                            <TextBlock Text="{Binding RegisterUsernameError}"
                                       Foreground="{DynamicResource ColorError}"
                                       TextWrapping="Wrap"/>
                            <TextBlock Text="{Binding UsernameHint}"
                                       Foreground="{DynamicResource ColorTextSecondary}"
                                       TextWrapping="Wrap"/>
                            <TextBlock Text="{Binding EmailLabel}"/>
                            <TextBox Classes="vertigo-input"
                                     TabIndex="8"
                                     Text="{Binding RegisterEmail, Mode=TwoWay}"
                                     MaxLength="{x:Static validation:RegistrationRules.EmailMaximumLength}"
                                     AutomationProperties.Name="{Binding EmailLabel}"/>
                            <TextBlock Text="{Binding RegisterEmailError}"
                                       Foreground="{DynamicResource ColorError}"
                                       TextWrapping="Wrap"/>
                            <TextBlock Text="{Binding PasswordLabel}"/>
                            <TextBox Classes="vertigo-input"
                                     TabIndex="9"
                                     PasswordChar="●"
                                     Text="{Binding RegisterPassword, Mode=TwoWay}"
                                     MaxLength="{x:Static validation:RegistrationRules.PasswordMaximumLength}"
                                     AutomationProperties.Name="{Binding PasswordLabel}"/>
                            <TextBlock Text="{Binding RegisterPasswordError}"
                                       Foreground="{DynamicResource ColorError}"
                                       TextWrapping="Wrap"/>
                            <TextBlock Text="{Binding PasswordHint}"
                                       Foreground="{DynamicResource ColorTextSecondary}"
                                       TextWrapping="Wrap"/>
                            <TextBlock Text="{Binding ConfirmPasswordLabel}"/>
                            <TextBox Classes="vertigo-input"
                                     TabIndex="10"
                                     PasswordChar="●"
                                     Text="{Binding ConfirmPassword, Mode=TwoWay}"
                                     MaxLength="{x:Static validation:RegistrationRules.PasswordMaximumLength}"
                                     AutomationProperties.Name="{Binding ConfirmPasswordLabel}"/>
                            <TextBlock Text="{Binding ConfirmPasswordError}"
                                       Foreground="{DynamicResource ColorError}"
                                       TextWrapping="Wrap"/>
                            <Button Classes="vertigo-button primary"
                                    TabIndex="11"
                                    IsEnabled="{Binding !IsBusy}"
                                    Command="{Binding RegisterCommand}"
                                    Content="{Binding CreateAccountText}"/>
                        </StackPanel>
                        <TextBlock Text="{Binding RegistrationMessage}"
                                   TextWrapping="Wrap"
                                   Foreground="{DynamicResource ColorSuccess}"/>
                        <TextBlock Text="{Binding HelpText}"
                                   TextWrapping="Wrap"
                                   Foreground="{DynamicResource ColorTextMuted}"/>
                    </StackPanel>
                </Border>
                <Grid Grid.Column="1" Width="300" Margin="48,0,0,0"
                      VerticalAlignment="Center" RowDefinitions="Auto,Auto">
                    <Canvas Height="330" Width="280" IsHitTestVisible="False">
                        <Rectangle Canvas.Left="5" Canvas.Top="210" Width="92" Height="115"
                                   Fill="{DynamicResource ColorShellGray}"/>
                        <Rectangle Canvas.Left="101" Canvas.Top="116" Width="86" Height="209"
                                   Fill="{DynamicResource ColorSnesBlue}"/>
                        <Rectangle Canvas.Left="192" Canvas.Top="165" Width="83" Height="160"
                                   Fill="{DynamicResource ColorCoal}"/>
                        <Rectangle Canvas.Left="119" Canvas.Top="82" Width="50" Height="34"
                                   Fill="{DynamicResource ColorSnesGreen}"/>
                        <Rectangle Canvas.Left="28" Canvas.Top="235" Width="14" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="60" Canvas.Top="235" Width="14" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="121" Canvas.Top="143" Width="16" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="151" Canvas.Top="143" Width="16" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="121" Canvas.Top="183" Width="16" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="151" Canvas.Top="183" Width="16" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="212" Canvas.Top="191" Width="15" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="244" Canvas.Top="191" Width="15" Height="20"
                                   Fill="{DynamicResource ColorWhite}"/>
                        <Rectangle Canvas.Left="0" Canvas.Top="324" Width="280" Height="6"
                                   Fill="{DynamicResource ColorBorderStrong}"/>
                    </Canvas>

                </Grid>
            </Grid>
        </Grid>
    </ScrollViewer>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/AccessView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class AccessView : UserControl
{
    public AccessView()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/HomeView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             x:Class="Vertigo.Client.UI.Views.HomeView"
             x:DataType="vm:HomeViewModel"
             x:CompileBindings="True">
    <UserControl.Styles>
        <Style Selector="Button.vertigo-home-action">
            <Setter Property="MinHeight" Value="62"/>
            <Setter Property="FontSize" Value="22"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="HorizontalContentAlignment" Value="Left"/>
            <Setter Property="Padding" Value="20,10"/>
            <Setter Property="Background" Value="{DynamicResource ColorWhite}"/>
            <Setter Property="Foreground" Value="{DynamicResource ColorCoal}"/>
            <Setter Property="BorderBrush" Value="{DynamicResource ColorCoal}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderThin}"/>
            <Setter Property="CornerRadius" Value="{DynamicResource RadiusSmall}"/>
        </Style>
        <Style Selector="Button.vertigo-home-action:pointerover">
            <Setter Property="Background" Value="{DynamicResource ColorInfoBackground}"/>
        </Style>
        <Style Selector="Button.vertigo-home-action:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
        <Style Selector="Button.vertigo-home-action:disabled">
            <Setter Property="Foreground" Value="{DynamicResource ColorDisabledForeground}"/>
            <Setter Property="Background" Value="{DynamicResource ColorDisabledBackground}"/>
        </Style>
        <Style Selector="Button.vertigo-home-utility:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <ScrollViewer HorizontalScrollBarVisibility="Auto"
                  VerticalScrollBarVisibility="Auto"
                  Background="{DynamicResource ColorCanvas}">
        <Grid MinHeight="720" MinWidth="900" RowDefinitions="Auto,Auto,*,Auto" Margin="30">
            <Grid Grid.Row="0" ColumnDefinitions="Auto,*,Auto" ColumnSpacing="14">
                <Border Grid.Column="0"
                        Background="{DynamicResource ColorWhite}"
                        BorderBrush="{DynamicResource ColorCoal}"
                        BorderThickness="{DynamicResource BorderThin}"
                        CornerRadius="{DynamicResource RadiusSmall}"
                        Padding="12,10">
                    <StackPanel Orientation="Horizontal" Spacing="12" VerticalAlignment="Center">
                        <Path Width="45" Height="45" Stretch="Uniform"
                              Stroke="{DynamicResource ColorCoal}" StrokeThickness="1.7"
                              Data="{StaticResource IconPeople}"/>
                        <StackPanel Spacing="2" VerticalAlignment="Center" MinWidth="132">
                            <TextBlock Text="{Binding PlayerTag}" FontWeight="Bold" FontSize="17"/>
                            <TextBlock Text="{Binding AccountKindText}" FontSize="12"
                                       Foreground="{DynamicResource ColorTextSecondary}"/>
                        </StackPanel>
                        <Button Classes="vertigo-button neutral vertigo-home-utility"
                                TabIndex="0" Command="{Binding OpenProfileCommand}"
                                Content="{Binding ProfileText}"
                                AutomationProperties.Name="{Binding ProfileText}"/>
                    </StackPanel>
                </Border>
                <StackPanel Grid.Column="2" Orientation="Horizontal" Spacing="12"
                            VerticalAlignment="Center">
                    <Button Classes="vertigo-button neutral vertigo-home-utility"
                            TabIndex="2" Command="{Binding ToggleNotificationsCommand}"
                            AutomationProperties.Name="{Binding NotificationsText}">
                        <Path Width="22" Height="22" Stretch="Uniform"
                              Stroke="{DynamicResource ColorCoal}" StrokeThickness="2"
                              Data="M 12,2 C 8,2 7,5 7,9 L 7,15 L 4,18 L 20,18 L 17,15 L 17,9 C 17,5 16,2 12,2 Z M
                                    10,21 L 14,21"/>
                    </Button>
                    <Button Classes="vertigo-button neutral vertigo-home-utility"
                            TabIndex="3" Command="{Binding LogoutCommand}"
                            IsEnabled="{Binding !IsBusy}" Content="{Binding LogoutText}"/>
                </StackPanel>
            </Grid>
            <Grid Grid.Row="1" ColumnDefinitions="Auto,*,Auto" Margin="0,14,0,0">
                <Border Grid.Column="2" IsVisible="{Binding IsNotificationsOpen}"
                        Background="{DynamicResource ColorWhite}"
                        BorderBrush="{DynamicResource ColorBorderStrong}"
                        BorderThickness="{DynamicResource BorderThin}"
                        Padding="16" CornerRadius="{DynamicResource RadiusSmall}"
                        MinWidth="230">
                    <StackPanel Spacing="10">
                        <TextBlock Text="{Binding NotificationsText}" FontWeight="Bold"/>
                        <TextBlock Text="{Binding NoNotificationsText}" TextWrapping="Wrap"
                                   Foreground="{DynamicResource ColorTextSecondary}"/>
                        <Button Classes="vertigo-button secondary vertigo-home-utility"
                                TabIndex="4" Command="{Binding OpenSocialCommand}"
                                Content="{Binding OpenSocialNotificationsText}"/>
                    </StackPanel>
                </Border>
            </Grid>
            <Grid Grid.Row="2" ColumnDefinitions="420,*" ColumnSpacing="48" Margin="25,30,0,0">
                <StackPanel Grid.Column="0" VerticalAlignment="Center" Spacing="14">
                    <TextBlock Text="VÉRTIGO" FontSize="62" FontWeight="ExtraBold"
                               Foreground="{DynamicResource ColorCoal}"
                               FontFamily="{DynamicResource FontTitleDisplay}"/>
                    <TextBlock Text="{Binding BrandSubtitle}" FontSize="18"
                               Margin="0,-10,0,24"
                               Foreground="{DynamicResource ColorTextSecondary}"/>
                    <Button Classes="vertigo-home-action" TabIndex="5"
                            Command="{Binding OpenPlayCommand}"
                            AutomationProperties.Name="{Binding PlayText}">
                        <StackPanel Orientation="Horizontal" Spacing="18">
                            <Path Width="32" Height="32" Stretch="Uniform"
                                  Fill="{DynamicResource ColorSnesGreen}"
                                  Stroke="{DynamicResource ColorCoal}" StrokeThickness="1.6"
                                  Data="M 6,2 L 24,16 L 6,30 Z"/>
                            <TextBlock Text="{Binding PlayText}" VerticalAlignment="Center"/>
                        </StackPanel>
                    </Button>
                    <Button Classes="vertigo-home-action" TabIndex="6"
                            Command="{Binding OpenSocialCommand}">
                        <StackPanel Orientation="Horizontal" Spacing="18">
                            <Path Width="32" Height="32" Stretch="Uniform"
                                  Fill="{DynamicResource ColorSnesBlue}"
                                  Stroke="{DynamicResource ColorCoal}" StrokeThickness="1.5"
                                  Data="{StaticResource IconPeople}"/>
                            <TextBlock Text="{Binding SocialText}" VerticalAlignment="Center"/>
                        </StackPanel>
                    </Button>
                    <Button Classes="vertigo-home-action" TabIndex="7"
                            IsEnabled="False" AutomationProperties.HelpText="{Binding NotAvailableText}">
                        <StackPanel Orientation="Horizontal" Spacing="18">
                            <Path Width="32" Height="32" Stretch="Uniform"
                                  Stroke="{DynamicResource ColorCoal}" StrokeThickness="1.6"
                                  Fill="{DynamicResource ColorSnesYellow}"
                                  Data="M 4,4 L 20,4 L 20,12 Q 20,19 12,21 Q 4,19 4,12 Z M 7,21 L 17,21 M 12,21 L
                                        12,28 M 6,28 L 18,28"/>
                            <TextBlock Text="{Binding RankingText}" VerticalAlignment="Center"/>
                        </StackPanel>
                    </Button>
                    <Button Classes="vertigo-home-action" TabIndex="8"
                            Command="{Binding OpenSettingsCommand}">
                        <StackPanel Orientation="Horizontal" Spacing="18">
                            <Path Width="32" Height="32" Stretch="Uniform"
                                  Stroke="{DynamicResource ColorCoal}" StrokeThickness="1.7"
                                  Data="M 12,3 L 17,3 L 18,7 L 22,9 L 25,6 L 28,10 L 25,13 L 26,18 L 30,19 L 28,24 L
                                        24,24 L 21,28 L 18,25 L 13,26 L 11,30 L 6,28 L 7,24 L 3,21 L 6,17 L 3,14 L
                                        6,9 L 10,11 Z"/>
                            <TextBlock Text="{Binding SettingsText}" VerticalAlignment="Center"/>
                        </StackPanel>
                    </Button>
                    <TextBlock Text="{Binding ErrorMessage}" TextWrapping="Wrap"
                               Foreground="{DynamicResource ColorError}"/>
                </StackPanel>
                <Border Grid.Column="1" VerticalAlignment="Center"
                        Background="{DynamicResource ColorWhite}"
                        BorderBrush="{DynamicResource ColorCoal}"
                        BorderThickness="{DynamicResource BorderThin}"
                        CornerRadius="{DynamicResource RadiusSmall}"
                        ClipToBounds="True">
                    <Viewbox Stretch="Uniform" HorizontalAlignment="Center" VerticalAlignment="Center">
                        <Canvas Width="590" Height="470" IsHitTestVisible="False">
                            <Path Fill="{DynamicResource ColorInfoBackground}"
                                  Data="M 0,350 L 65,310 L 92,330 L 92,455 L 0,455 Z M 85,320 L 145,240 L 190,305 L
                                        190,455 L 85,455 Z M 415,345 L 460,280 L 510,320 L 510,455 L 415,455 Z M
                                        505,315 L 545,260 L 590,290 L 590,455 L 505,455 Z"/>
                            <Path Fill="{DynamicResource ColorShellGray}"
                                  Data="M 150,455 L 150,355 L 215,355 L 215,455 Z M 375,455 L 375,345 L 420,345 L
                                        420,455 Z"/>
                            <Path Stroke="{DynamicResource ColorSnesBlue}" StrokeThickness="2" Fill="Transparent"
                                  Data="M 22,98 C 25,78 53,80 60,95 C 75,85 93,94 94,105 L 112,105"/>
                            <Path Stroke="{DynamicResource ColorSnesBlue}" StrokeThickness="2" Fill="Transparent"
                                  Data="M 450,145 C 460,124 485,125 494,141 C 506,132 526,141 532,152 L 560,152"/>
                            <Path Stroke="{DynamicResource ColorCoal}" StrokeThickness="3" Fill="Transparent"
                                  Data="M 473,455 L 473,95 L 490,95 L 490,455 M 468,110 L 495,137 L 468,164 L
                                        495,191 L 468,218 L 495,245 L 468,272 L 495,299 L 468,326 L 495,353 L
                                        468,380"/>
                            <Path Stroke="{DynamicResource ColorCoal}" StrokeThickness="3" Fill="Transparent"
                                  Data="M 300,95 L 545,95 L 475,48 L 300,95 L 545,95 M 475,48 L 475,95 L 547,95 M
                                        313,95 L 327,80 L 343,95 L 359,80 L 375,95 L 391,80 L 407,95 L 423,80 L
                                        439,95"/>
                            <Path Stroke="{DynamicResource ColorSnesBlue}" StrokeThickness="6"
                                  Data="M 300,97 L 544,97"/>
                            <Path Stroke="{DynamicResource ColorCoal}" StrokeThickness="2" Fill="Transparent"
                                  Data="M 390,99 L 390,138 M 383,138 L 397,138 L 397,145 L 390,154 L 383,145 Z M
                                        390,154 L 365,177 L 415,177 Z"/>
                            <Path Fill="{DynamicResource ColorSnesYellow}"
                                  Data="M 384,132 L 396,132 L 396,145 L 390,150 L 384,145 Z"/>
                            <Path Fill="{DynamicResource ColorWhite}" Stroke="{DynamicResource ColorCoal}"
                                  StrokeThickness="2"
                                  Data="M 338,179 L 424,179 L 424,220 L 338,220 Z"/>
                            <Path Fill="{DynamicResource ColorSnesBlue}"
                                  Data="M 345,185 L 412,185 L 412,214 L 345,214 Z"/>
                            <Path Fill="{DynamicResource ColorSnesGreen}"
                                  Data="M 413,185 L 421,182 L 421,215 L 413,214 Z"/>
                            <Path Fill="{DynamicResource ColorCoal}"
                                  Data="M 220,455 L 220,280 L 265,265 L 415,265 L 430,280 L 430,455 Z"/>
                            <Path Fill="{DynamicResource ColorShellGray}"
                                  Data="M 225,285 L 270,271 L 410,271 L 424,285 L 424,453 L 225,453 Z"/>
                            <Path Fill="{DynamicResource ColorWhite}" Stroke="{DynamicResource ColorCoal}"
                                  StrokeThickness="2"
                                  Data="M 205,302 L 440,302 L 440,320 L 205,320 Z M 205,363 L 440,363 L 440,381 L
                                        205,381 Z M 205,425 L 440,425 L 440,443 L 205,443 Z"/>
                            <Path Fill="{DynamicResource ColorSnesBlue}"
                                  Data="M 235,326 L 305,326 L 305,359 L 235,359 Z M 312,326 L 415,326 L 415,359 L
                                        312,359 Z M 235,387 L 305,387 L 305,421 L 235,421 Z M 312,387 L 415,387 L
                                        415,421 L 312,421 Z M 235,445 L 415,445 L 415,455 L 235,455 Z"/>
                            <Path Fill="{DynamicResource ColorSnesGreen}"
                                  Data="M 306,326 L 314,326 L 314,359 L 306,359 Z M 306,387 L 314,387 L 314,421 L
                                        306,421 Z"/>
                            <Path Stroke="{DynamicResource ColorSnesYellow}" StrokeThickness="4"
                                  Data="M 228,278 L 228,255 L 315,255 M 340,255 L 420,255 L 420,278"/>
                            <Path Stroke="{DynamicResource ColorCoal}" StrokeThickness="2"
                                  Data="M 250,255 L 250,229 M 300,255 L 300,229 M 368,255 L 368,229 M 405,255 L
                                        405,229"/>
                            <Path Stroke="{DynamicResource ColorCoal}" StrokeThickness="3"
                                  Data="M 0,458 L 590,458"/>
                        </Canvas>
                    </Viewbox>
                </Border>
            </Grid>
            <Grid Grid.Row="3" ColumnDefinitions="*,Auto" Margin="0,24,0,0">
                <TextBlock Text="{Binding VersionText}" Foreground="{DynamicResource ColorTextSecondary}"/>
                <Button Grid.Column="1" Classes="vertigo-button neutral"
                        Content="{Binding GlossaryText}" IsEnabled="False"
                        AutomationProperties.HelpText="{Binding NotAvailableText}"/>
            </Grid>
        </Grid>
    </ScrollViewer>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/HomeView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/MainWindow.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/ProfileView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             x:Class="Vertigo.Client.UI.Views.ProfileView" x:DataType="vm:ProfileViewModel"
             x:CompileBindings="True">
    <UserControl.Styles>
        <Style Selector="Button:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <ScrollViewer Background="{DynamicResource ColorCanvas}">
        <StackPanel Margin="32" MaxWidth="660" HorizontalAlignment="Center" Spacing="24">
            <DockPanel LastChildFill="True">
                <Button DockPanel.Dock="Left" Classes="vertigo-button neutral"
                        Command="{Binding BackCommand}" TabIndex="0"
                        AutomationProperties.Name="{Binding BackText}">
                    <Path Width="22" Height="22" Stroke="{DynamicResource ColorCoal}"
                          StrokeThickness="2" Data="M 20,12 L 4,12 M 11,5 L 4,12 L 11,19"/>
                </Button>
                <TextBlock Text="{Binding HeadingText}" Margin="24,0,0,0" FontSize="32"
                           FontWeight="Bold" VerticalAlignment="Center"/>
            </DockPanel>
            <Border Background="{DynamicResource ColorWhite}" BorderBrush="{DynamicResource ColorBorderStrong}"
                    BorderThickness="{DynamicResource BorderThin}" CornerRadius="4" Padding="24">
                <StackPanel Spacing="16">
                    <Path Width="60" Height="60" Stroke="{DynamicResource ColorSnesBlue}"
                          StrokeThickness="2" Data="M 30,7 C 20,7 18,15 18,21 C 18,29 23,34 30,34 C 37,34 42,29 42,21 C 42,15 40,7 30,7 Z M 8,55 C 9,40 19,36 30,36 C 41,36 51,40 52,55"/>
                    <TextBlock Text="{Binding UsernameText}" Foreground="{DynamicResource ColorTextSecondary}"/>
                    <TextBlock Text="{Binding Username}" FontSize="22" FontWeight="Bold"/>
                    <TextBlock Text="{Binding AccountText}" Foreground="{DynamicResource ColorTextSecondary}"/>
                    <TextBlock Text="{Binding AccountType}" FontSize="18"/>
                </StackPanel>
            </Border>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/ProfileView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class ProfileView : UserControl
{
    public ProfileView()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/SettingsView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             x:Class="Vertigo.Client.UI.Views.SettingsView"
             x:DataType="vm:SettingsViewModel"
             x:CompileBindings="True">
    <UserControl.Styles>
        <Style Selector="Button:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <ScrollViewer Background="{DynamicResource ColorCanvas}">
        <StackPanel Margin="32" Spacing="24" MaxWidth="660" HorizontalAlignment="Center">
            <DockPanel LastChildFill="True">
                <Button DockPanel.Dock="Left" Classes="vertigo-button neutral"
                        Command="{Binding BackCommand}" TabIndex="0"
                        AutomationProperties.Name="{Binding BackText}">
                    <Path Width="22" Height="22" Stroke="{DynamicResource ColorCoal}"
                          StrokeThickness="2" Data="M 20,12 L 4,12 M 11,5 L 4,12 L 11,19"/>
                </Button>
                <TextBlock Text="{Binding HeadingText}" Margin="24,0,0,0" FontSize="32"
                           FontWeight="Bold" VerticalAlignment="Center"/>
            </DockPanel>
            <Border Background="{DynamicResource ColorWhite}" BorderBrush="{DynamicResource ColorBorderStrong}"
                    BorderThickness="{DynamicResource BorderThin}" CornerRadius="4" Padding="24">
                <StackPanel Spacing="14">
                    <TextBlock Text="{Binding LanguageText}" FontSize="18" FontWeight="SemiBold"/>
                    <ComboBox SelectedIndex="{Binding LanguageIndex, Mode=TwoWay}" TabIndex="1"
                              AutomationProperties.Name="{Binding LanguageText}">
                        <ComboBoxItem Content="{Binding SpanishText}"/>
                        <ComboBoxItem Content="{Binding EnglishText}"/>
                    </ComboBox>
                    <Button Classes="vertigo-button primary" Command="{Binding SaveCommand}"
                            Content="{Binding SaveText}" TabIndex="2"/>
                    <TextBlock Text="{Binding ErrorMessage}" Foreground="{DynamicResource ColorError}" TextWrapping="Wrap"/>
                </StackPanel>
            </Border>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/SettingsView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/Views/SocialView.axaml
```
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Vertigo.Client.UI.ViewModels"
             xmlns:models="using:Vertigo.Contracts.Models"
             x:Class="Vertigo.Client.UI.Views.SocialView"
             x:DataType="vm:SocialViewModel" x:CompileBindings="True">
    <UserControl.Styles>
        <Style Selector="Button:focus-visible">
            <Setter Property="BorderBrush" Value="{DynamicResource ColorFocus}"/>
            <Setter Property="BorderThickness" Value="{DynamicResource BorderStrong}"/>
        </Style>
    </UserControl.Styles>
    <ScrollViewer Background="{DynamicResource ColorCanvas}" HorizontalScrollBarVisibility="Auto">
        <StackPanel MinWidth="760" Margin="28" Spacing="20">
            <Grid ColumnDefinitions="Auto,*,Auto" ColumnSpacing="18">
                <Button Grid.Column="0" Classes="vertigo-button neutral" Command="{Binding BackCommand}"
                        AutomationProperties.Name="{Binding BackText}" TabIndex="0">
                    <Path Width="22" Height="22" Stroke="{DynamicResource ColorCoal}" StrokeThickness="2"
                          Data="M 20,12 L 4,12 M 11,5 L 4,12 L 11,19"/>
                </Button>
                <TextBlock Grid.Column="1" Text="{Binding HeadingText}" FontSize="34"
                           FontFamily="{DynamicResource FontTitleDisplay}" FontWeight="Bold"
                           VerticalAlignment="Center"/>
                <Button Grid.Column="2" Classes="vertigo-button neutral" TabIndex="1"
                        Command="{Binding RefreshCommand}" Content="{Binding RefreshText}"/>
            </Grid>
            <TextBlock Text="{Binding ErrorMessage}" Foreground="{DynamicResource ColorError}"
                       TextWrapping="Wrap"/>
            <TabControl SelectedIndex="0" TabIndex="2">
                <TabItem Header="{Binding FriendsTabText}">
                    <Grid RowDefinitions="Auto,Auto,*,Auto" RowSpacing="14" Margin="16">
                        <StackPanel Orientation="Horizontal" Spacing="12">
                            <TextBox Classes="vertigo-input" Width="260" MaxLength="20" TabIndex="3"
                                     PlaceholderText="{Binding SearchText}" Text="{Binding Friends.SearchTerm, Mode=TwoWay}"
                                     AutomationProperties.Name="{Binding SearchText}"/>
                            <Button Classes="vertigo-button secondary" TabIndex="4"
                                    Content="{Binding SearchText}" Command="{Binding Friends.SearchCommand}"/>
                        </StackPanel>
                        <TextBlock Grid.Row="1" Text="{Binding Friends.ErrorMessage}"
                                   Foreground="{DynamicResource ColorError}" TextWrapping="Wrap"/>
                        <Grid Grid.Row="2" ColumnDefinitions="*,*" ColumnSpacing="18">
                            <StackPanel Spacing="8">
                                <TextBlock Text="{Binding FriendsTabText}" FontWeight="Bold"/>
                                <ListBox ItemsSource="{Binding Friends.Friends}"
                                         SelectedItem="{Binding SelectedFriend, Mode=TwoWay}" TabIndex="5"
                                         MinHeight="180" MaxHeight="320">
                                    <ListBox.ItemTemplate>
                                        <DataTemplate x:DataType="models:FriendEntry">
                                            <TextBlock Text="{Binding Username}" Margin="8"/>
                                        </DataTemplate>
                                    </ListBox.ItemTemplate>
                                </ListBox>
                                <Button Classes="vertigo-button neutral" Command="{Binding Friends.RemoveCommand}"
                                        Content="{Binding RemoveFriendText}" TabIndex="6"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Spacing="8">
                                <TextBlock Text="{Binding SearchResultsText}" FontWeight="Bold"/>
                                <ListBox ItemsSource="{Binding Friends.SearchResults}"
                                         SelectedItem="{Binding SelectedSearch, Mode=TwoWay}" TabIndex="7"
                                         MinHeight="180" MaxHeight="320">
                                    <ListBox.ItemTemplate>
                                        <DataTemplate x:DataType="models:PlayerSearchEntry">
                                            <TextBlock Text="{Binding Username}" Margin="8"/>
                                        </DataTemplate>
                                    </ListBox.ItemTemplate>
                                </ListBox>
                                <Button Classes="vertigo-button primary" TabIndex="8"
                                        Content="{Binding SendRequestText}" Command="{Binding Friends.SendRequestCommand}"/>
                            </StackPanel>
                        </Grid>
                    </Grid>
                </TabItem>
                <TabItem Header="{Binding RequestsTabText}">
                    <StackPanel Margin="16" Spacing="12">
                        <ListBox ItemsSource="{Binding Friends.PendingRequests}" TabIndex="9"
                                 SelectedItem="{Binding SelectedRequest, Mode=TwoWay}" MinHeight="200">
                            <ListBox.ItemTemplate>
                                <DataTemplate x:DataType="models:FriendshipInvitationEntry">
                                    <TextBlock Text="{Binding SenderUsername}" Margin="8"/>
                                </DataTemplate>
                            </ListBox.ItemTemplate>
                        </ListBox>
                        <TextBlock Text="{Binding Friends.ErrorMessage}" Foreground="{DynamicResource ColorError}"/>
                        <StackPanel Orientation="Horizontal" Spacing="12">
                            <Button Classes="vertigo-button primary" Content="{Binding AcceptText}" TabIndex="10"
                                    Command="{Binding Friends.AcceptCommand}"/>
                            <Button Classes="vertigo-button neutral" Content="{Binding RejectText}" TabIndex="11"
                                    Command="{Binding Friends.RejectCommand}"/>
                        </StackPanel>
                    </StackPanel>
                </TabItem>
                <TabItem Header="{Binding InvitationsTabText}">
                    <StackPanel Margin="16" Spacing="12">
                        <ListBox ItemsSource="{Binding Invitations.PendingInvitations}" TabIndex="12"
                                 SelectedItem="{Binding SelectedInvitation, Mode=TwoWay}" MinHeight="200">
                            <ListBox.ItemTemplate>
                                <DataTemplate x:DataType="models:LobbyInvitationEntry">
                                    <TextBlock Text="{Binding LobbyCode}" Margin="8"/>
                                </DataTemplate>
                            </ListBox.ItemTemplate>
                        </ListBox>
                        <TextBlock Text="{Binding Invitations.ErrorMessage}" Foreground="{DynamicResource ColorError}"/>
                        <StackPanel Orientation="Horizontal" Spacing="12">
                            <Button Classes="vertigo-button primary" Content="{Binding AcceptText}" TabIndex="13"
                                    Command="{Binding Invitations.AcceptCommand}"/>
                            <Button Classes="vertigo-button neutral" Content="{Binding RejectText}" TabIndex="14"
                                    Command="{Binding Invitations.DeclineCommand}"/>
                        </StackPanel>
                    </StackPanel>
                </TabItem>
                <TabItem Header="{Binding BlockedTabText}">
                    <StackPanel Margin="16" Spacing="12">
                        <ListBox ItemsSource="{Binding Blocked.BlockedPlayers}" TabIndex="15"
                                 SelectedItem="{Binding SelectedBlocked, Mode=TwoWay}" MinHeight="200">
                            <ListBox.ItemTemplate>
                                <DataTemplate x:DataType="models:BlockedPlayerSummary">
                                    <TextBlock Text="{Binding Username}" Margin="8"/>
                                </DataTemplate>
                            </ListBox.ItemTemplate>
                        </ListBox>
                        <TextBlock Text="{Binding Blocked.ErrorMessage}" Foreground="{DynamicResource ColorError}"/>
                        <Button Classes="vertigo-button neutral" Content="{Binding UnblockText}"
                                TabIndex="16" Command="{Binding Blocked.UnblockCommand}"/>
                    </StackPanel>
                </TabItem>
            </TabControl>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

## File: src/Client/Vertigo.Client.UI/Views/SocialView.axaml.cs
```csharp
using Avalonia.Controls;

namespace Vertigo.Client.UI.Views;

public partial class SocialView : UserControl
{
    public SocialView()
    {
        InitializeComponent();
    }
}
```

## File: src/Client/Vertigo.Client.UI/app.manifest
```
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <!-- This manifest is used on Windows only.
       Don't remove it as it might cause problems with window transparency and embedded controls.
       For more details visit https://learn.microsoft.com/en-us/windows/win32/sbscs/application-manifests -->
  <assemblyIdentity version="1.0.0.0" name="Vertigo.Client.UI.Desktop"/>

  <compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
      <!-- A list of the Windows versions that this application has been tested on
           and is designed to work with. Uncomment the appropriate elements
           and Windows will automatically select the most compatible environment. -->

      <!-- Windows 10 -->
      <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
    </application>
  </compatibility>
</assembly>
```

## File: src/Client/Vertigo.Client.UI/Program.cs
```csharp
using Avalonia;
using System;

namespace Vertigo.Client.UI;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
```

## File: src/Client/Vertigo.Client.UI/ViewLocator.cs
```csharp
using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Vertigo.Client.UI.ViewModels;

namespace Vertigo.Client.UI;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Implementations/PasswordHasher.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using System.Security.Cryptography;

namespace Vertigo.Server.CrossCutting.Security.Implementations;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltByteLength = 16;
    private const int HashByteLength = 32;
    private const int Iterations = 600000;

    public string GenerateSalt()
    {
        byte[] randomSalt = RandomNumberGenerator.GetBytes(SaltByteLength);
        string passwordSalt = Convert.ToBase64String(randomSalt);

        return passwordSalt;
    }

    public string HashPassword(string password, string passwordSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(passwordSalt);
        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            HashByteLength);
        string passwordHash = Convert.ToHexString(hashBytes);

        return passwordHash;
    }

    public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
    {
        string computedHash = HashPassword(password, passwordSalt);
        byte[] computedBytes = Convert.FromHexString(computedHash);
        byte[] storedBytes = Convert.FromHexString(passwordHash);
        bool isValid = CryptographicOperations.FixedTimeEquals(computedBytes, storedBytes);

        return isValid;
    }
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Implementations/TokenGenerator.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Vertigo.Server.CrossCutting.Security.Implementations;

public class TokenGenerator : ITokenGenerator
{
    private const int SessionTokenByteLength = 32;
    private const int MinimumVerificationCode = 100000;
    private const int MaximumVerificationCodeExclusive = 1000000;

    public string GenerateSessionToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(SessionTokenByteLength);
        string sessionToken = Convert.ToHexString(randomBytes);

        return sessionToken;
    }

    public string GenerateVerificationCode()
    {
        int code = RandomNumberGenerator.GetInt32(
            MinimumVerificationCode,
            MaximumVerificationCodeExclusive);
        string verificationCode = code.ToString(System.Globalization.CultureInfo.InvariantCulture);

        return verificationCode;
    }

    public string HashSessionToken(string sessionToken)
    {
        byte[] tokenBytes = Encoding.UTF8.GetBytes(sessionToken);
        byte[] hashBytes = SHA256.HashData(tokenBytes);
        string tokenHash = Convert.ToHexString(hashBytes);

        return tokenHash;
    }
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Interfaces/IPasswordHasher.cs
```csharp
namespace Vertigo.Server.CrossCutting.Security.Interfaces;

public interface IPasswordHasher
{
    string GenerateSalt();

    string HashPassword(string password, string passwordSalt);

    bool VerifyPassword(string password, string passwordHash, string passwordSalt);
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Security/Interfaces/ITokenGenerator.cs
```csharp
namespace Vertigo.Server.CrossCutting.Security.Interfaces;

public interface ITokenGenerator
{
    string GenerateSessionToken();

    string GenerateVerificationCode();

    string HashSessionToken(string sessionToken);
}
```

## File: src/Server/Vertigo.Server.CrossCutting/Vertigo.Server.CrossCutting.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.DataAccess/Contexts/VertigoDatabaseContext.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Game;
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Contexts;

public class VertigoDatabaseContext : DbContext
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerStats> PlayerStats => Set<PlayerStats>();
    public DbSet<SessionToken> SessionTokens => Set<SessionToken>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<BlockedPlayer> BlockedPlayers => Set<BlockedPlayer>();
    public DbSet<MatchHistory> MatchHistory => Set<MatchHistory>();

    public VertigoDatabaseContext(DbContextOptions<VertigoDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigurePlayers(modelBuilder);
        ConfigurePlayerStats(modelBuilder);
        ConfigureSessionTokens(modelBuilder);
        ConfigureFriendships(modelBuilder);
        ConfigureBlockedPlayers(modelBuilder);
        ConfigureMatchHistory(modelBuilder);
    }

    private static void ConfigurePlayers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(player =>
        {
            player.ToTable("Players");
            player.HasKey(p => p.PlayerId);
            player.Property(p => p.PlayerId).HasDefaultValueSql("NEWID()");
            player.Property(p => p.Username).HasMaxLength(32).IsRequired();
            player.Property(p => p.Email).HasMaxLength(120).IsRequired();
            player.Property(p => p.PasswordHash).HasMaxLength(256).IsRequired();
            player.Property(p => p.PasswordSalt).HasMaxLength(128).IsRequired();
            player.Property(p => p.IsEmailVerified).HasDefaultValue(false);
            player.Property(p => p.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            player.Property(p => p.LastLoginAt).HasColumnType("datetime2");
            player.HasAlternateKey(p => p.Username);
            player.HasAlternateKey(p => p.Email);
            player.HasIndex(p => p.Username).HasDatabaseName("IX_Players_Username");
        });
    }

    private static void ConfigurePlayerStats(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerStats>(stats =>
        {
            stats.ToTable("PlayerStats");
            stats.HasKey(s => s.PlayerId);
            stats.Property(s => s.MatchesPlayed).HasDefaultValue(0);
            stats.Property(s => s.MatchesWon).HasDefaultValue(0);
            stats.Property(s => s.TotalFloorsBuilt).HasDefaultValue(0);
            stats.Property(s => s.HighestSkyscraper).HasDefaultValue(0);
            stats.HasIndex(s => new { s.MatchesWon, s.TotalFloorsBuilt })
                .IsDescending(true, true).HasDatabaseName("IX_PlayerStats_Ranking");
            stats.HasOne(s => s.Player).WithOne(p => p.Stats)
                .HasForeignKey<PlayerStats>(s => s.PlayerId)
                .HasConstraintName("FK_PlayerStats_Players")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSessionTokens(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SessionToken>(token =>
        {
            token.ToTable("SessionTokens");
            token.HasKey(t => t.TokenId);
            token.Property(t => t.TokenId).HasDefaultValueSql("NEWID()");
            token.Property(t => t.TokenValue).HasMaxLength(128).IsRequired();
            token.Property(t => t.ExpiresAt).HasColumnType("datetime2");
            token.Property(t => t.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            token.Property(t => t.IsRevoked).HasDefaultValue(false);
            token.HasAlternateKey(t => t.TokenValue);
            token.HasIndex(t => t.TokenValue).HasDatabaseName("IX_SessionTokens_TokenValue");
            token.HasOne(t => t.Player).WithMany(p => p.SessionTokens)
                .HasForeignKey(t => t.PlayerId)
                .HasConstraintName("FK_SessionTokens_Players")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureFriendships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>(friendship =>
        {
            friendship.ToTable("Friendships", table => table.HasCheckConstraint(
                "CK_Friendship_NotSelf", "[RequesterId] <> [AddresseeId]"));
            friendship.HasKey(f => f.FriendshipId);
            friendship.Property(f => f.FriendshipId).HasDefaultValueSql("NEWID()");
            friendship.Property(f => f.PlayerRequesterId).HasColumnName("RequesterId");
            friendship.Property(f => f.PlayerAddresseeId).HasColumnName("AddresseeId");
            friendship.Property(f => f.Status).HasConversion<byte>().HasColumnType("tinyint")
                .HasDefaultValue(FriendshipStatus.Pending);
            friendship.Property(f => f.CreatedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            friendship.Property(f => f.UpdatedAt).HasColumnType("datetime");
            friendship.HasAlternateKey(f => new { f.PlayerRequesterId, f.PlayerAddresseeId })
                .HasName("UQ_Friendship_Pair");
            friendship.HasOne(f => f.PlayerRequester).WithMany(p => p.SentFriendships)
                .HasForeignKey(f => f.PlayerRequesterId)
                .HasConstraintName("FK_Friendships_Requester")
                .OnDelete(DeleteBehavior.NoAction);
            friendship.HasOne(f => f.PlayerAddressee).WithMany(p => p.ReceivedFriendships)
                .HasForeignKey(f => f.PlayerAddresseeId)
                .HasConstraintName("FK_Friendships_Addressee")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureBlockedPlayers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlockedPlayer>(blocked =>
        {
            blocked.ToTable("BlockedPlayers", table => table.HasCheckConstraint(
                "CK_Blocked_NotSelf", "[BlockerId] <> [BlockedId]"));
            blocked.HasKey(b => b.BlockId);
            blocked.Property(b => b.BlockId).HasDefaultValueSql("NEWID()");
            blocked.Property(b => b.PlayerBlockerId).HasColumnName("BlockerId");
            blocked.Property(b => b.PlayerBlockedId).HasColumnName("BlockedId");
            blocked.Property(b => b.CreatedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            blocked.HasAlternateKey(b => new { b.PlayerBlockerId, b.PlayerBlockedId })
                .HasName("UQ_Blocked_Pair");
            blocked.HasOne(b => b.PlayerBlocker).WithMany(p => p.BlockedPlayers)
                .HasForeignKey(b => b.PlayerBlockerId)
                .HasConstraintName("FK_BlockedPlayers_Blocker")
                .OnDelete(DeleteBehavior.NoAction);
            blocked.HasOne(b => b.PlayerBlocked).WithMany(p => p.BlockedByPlayers)
                .HasForeignKey(b => b.PlayerBlockedId)
                .HasConstraintName("FK_BlockedPlayers_Blocked")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureMatchHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchHistory>(match =>
        {
            match.ToTable("MatchHistory");
            match.HasKey(m => m.MatchId);
            match.Property(m => m.MatchId).HasDefaultValueSql("NEWID()");
            match.Property(m => m.PlayedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            match.Property(m => m.RoundsPlayed).HasDefaultValue(3);
            match.Property(m => m.WinningHeight).HasDefaultValue(0);
            match.HasOne(m => m.Winner).WithMany(p => p.WonMatches)
                .HasForeignKey(m => m.WinnerId)
                .HasConstraintName("FK_MatchHistory_Winner")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Enums/FriendshipStatus.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Enums;

public enum FriendshipStatus : byte
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Game/MatchHistory.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Entities.Game;

public class MatchHistory
{
    public Guid MatchId { get; set; }
    public DateTime PlayedAt { get; set; }
    public int RoundsPlayed { get; set; }
    public Guid? WinnerId { get; set; }
    public int WinningHeight { get; set; }

    public Player? Winner { get; set; }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/BlockedPlayer.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Identity;

public class BlockedPlayer
{
    public Guid BlockId { get; set; }
    public Guid PlayerBlockerId { get; set; }
    public Guid PlayerBlockedId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Player PlayerBlocker { get; set; } = null!;
    public Player PlayerBlocked { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/Friendship.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Enums;

namespace Vertigo.Server.DataAccess.Entities.Identity;

public class Friendship
{
    public Guid FriendshipId { get; set; }
    public Guid PlayerRequesterId { get; set; }
    public Guid PlayerAddresseeId { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Player PlayerRequester { get; set; } = null!;
    public Player PlayerAddressee { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/Player.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Game;

namespace Vertigo.Server.DataAccess.Entities.Identity;

public class Player
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public int? AvatarId { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public PlayerStats? Stats { get; set; }
    public ICollection<SessionToken> SessionTokens { get; set; } = new List<SessionToken>();
    public ICollection<Friendship> SentFriendships { get; set; } = new List<Friendship>();
    public ICollection<Friendship> ReceivedFriendships { get; set; } = new List<Friendship>();
    public ICollection<BlockedPlayer> BlockedPlayers { get; set; } = new List<BlockedPlayer>();
    public ICollection<BlockedPlayer> BlockedByPlayers { get; set; } = new List<BlockedPlayer>();
    public ICollection<MatchHistory> WonMatches { get; set; } = new List<MatchHistory>();
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/PlayerStats.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Identity;

public class PlayerStats
{
    public Guid PlayerId { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int TotalFloorsBuilt { get; set; }
    public int HighestSkyscraper { get; set; }

    public Player Player { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Entities/Identity/SessionToken.cs
```csharp
namespace Vertigo.Server.DataAccess.Entities.Identity;

public class SessionToken
{
    public Guid TokenId { get; set; }
    public Guid PlayerId { get; set; }
    public string TokenValue { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }

    public Player Player { get; set; } = null!;
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/ISessionTokenRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface ISessionTokenRepository
{
    bool CreateSessionToken(SessionToken token);

    SessionToken? GetActiveSessionToken(string tokenHash);

    bool RevokeSessionToken(string tokenHash);

    int RevokePlayerSessions(Guid playerId);
}
```

## File: src/Server/Vertigo.Server.DataAccess/DependencyInjection.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Repositories.Implementations;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddVertigoDataAccess(
        this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("VertigoDb")
            ?? throw new InvalidOperationException("Connection string 'VertigoDb' is missing.");

        services.AddDbContext<VertigoDatabaseContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IBlockedPlayerRepository, BlockedPlayerRepository>();
        services.AddScoped<ISessionTokenRepository, SessionTokenRepository>();

        return services;
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Vertigo.Server.DataAccess.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="11.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="11.0.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.Game/Matches/DependencyInjection/MatchLifecycleServiceCollectionExtensions.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Matches.DependencyInjection;

public static class MatchLifecycleServiceCollectionExtensions
{
    public static IServiceCollection AddMatchLifecycleServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<InMemoryMatchSessionStarter>();
        services.AddSingleton<IMatchSessionStarter>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());
        services.AddSingleton<IMatchLifecycle>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());
        services.AddSingleton<IMatchPresence>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());

        return services;
    }
}
```

## File: src/Server/Vertigo.Server.Game/Matches/Models/MatchRecord.cs
```csharp
using Vertigo.Contracts.Internal;

namespace Vertigo.Server.Game.Matches.Models;

// Protected by the owning starter's synchronization lock.
internal sealed class MatchRecord
{
    public MatchRecord(MatchSessionSnapshot session, DateTimeOffset createdAtUtc)
    {
        Session = session;
        Lifecycle = new MatchLifecycleSnapshot
        {
            MatchId = session.MatchId,
            LobbyInstanceId = session.LobbyInstanceId,
            Status = MatchLifecycleStatus.Running,
            Revision = 1,
            UpdatedAtUtc = createdAtUtc
        };
    }

    private readonly HashSet<Guid> _disconnectedPlayerIds = new();

    public MatchSessionSnapshot Session { get; }

    public MatchLifecycleSnapshot Lifecycle { get; private set; }

    public MatchPresenceSnapshot ToPresence()
    {
        return new MatchPresenceSnapshot
        {
            MatchId = Session.MatchId,
            Status = Lifecycle.Status,
            Revision = Lifecycle.Revision,
            DisconnectedPlayerIds = _disconnectedPlayerIds.OrderBy(id => id).ToArray()
        };
    }

    public bool TrySetConnected(Guid playerId, bool isConnected, DateTimeOffset now)
    {
        bool changed = false;
        if (!IsTerminal && Session.PlayerIds.Contains(playerId))
        {
            changed = isConnected
                ? _disconnectedPlayerIds.Remove(playerId)
                : _disconnectedPlayerIds.Add(playerId);
            if (changed)
            {
                MatchLifecycleStatus status = _disconnectedPlayerIds.Count == Session.PlayerIds.Count
                    ? MatchLifecycleStatus.Suspended : MatchLifecycleStatus.Running;
                Lifecycle = Lifecycle with
                {
                    Status = status,
                    Revision = Lifecycle.Revision + 1,
                    UpdatedAtUtc = now
                };
            }
        }

        return changed;
    }

    public MatchReconnectResult TryReconnect(Guid playerId, DateTimeOffset now)
    {
        MatchReconnectResult result = MatchReconnectResult.Rejected;
        if (!IsTerminal && Session.PlayerIds.Contains(playerId))
        {
            if (_disconnectedPlayerIds.Remove(playerId))
            {
                Lifecycle = Lifecycle with
                {
                    Status = MatchLifecycleStatus.Running,
                    Revision = Lifecycle.Revision + 1,
                    UpdatedAtUtc = now
                };
                result = MatchReconnectResult.Reconnected;
            }
            else
            {
                result = MatchReconnectResult.AlreadyConnected;
            }
        }

        return result;
    }

    public bool IsTerminal => Lifecycle.Status is MatchLifecycleStatus.Finished or MatchLifecycleStatus.Cancelled;

    public bool TryTransition(MatchLifecycleStatus next, DateTimeOffset changedAtUtc)
    {
        bool changed = false;
        if (!IsTerminal && (next is MatchLifecycleStatus.Finished or MatchLifecycleStatus.Cancelled))
        {
            Lifecycle = Lifecycle with
            {
                Status = next,
                Revision = Lifecycle.Revision + 1,
                UpdatedAtUtc = changedAtUtc
            };
            changed = true;
        }

        return changed;
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Chat/Services/ChatService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Chat.Services;

public sealed class ChatService : IChatService
{
    private const int MaximumMessageLength = 500;

    private readonly ILobbyDirectory _lobbies;
    private readonly ISessionIdentityResolver _sessions;
    private readonly TimeProvider _timeProvider;

    public ChatService(ILobbyDirectory lobbies, ISessionIdentityResolver sessions, TimeProvider timeProvider)
    {
        _lobbies = lobbies;
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public bool SendMessage(string lobbyCode, string text, string sessionToken)
    {
        SessionIdentity sender = _sessions.Require(sessionToken);
        if (string.IsNullOrWhiteSpace(lobbyCode) || string.IsNullOrWhiteSpace(text)
            || text.Length > MaximumMessageLength)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        LobbyChatMessage message = new LobbyChatMessage
        {
            LobbyCode = lobbyCode.Trim(),
            SenderId = sender.PlayerId,
            SenderUsername = sender.Username,
            Text = text.Trim(),
            SentAt = _timeProvider.GetUtcNow().UtcDateTime
        };

        if (!_lobbies.PublishChatMessage(message))
        {
            throw CreateFault(ServiceErrorCodes.LobbyChatNotAllowed, ServiceMessageKeys.LobbyChatNotAllowed);
        }

        return true;
    }

    private static FaultException<ServiceFault> CreateFault(string code, string key)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = code, MessageKey = key };
        return new FaultException<ServiceFault>(fault, new FaultReason(code));
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Invitations/Dependencies/LobbyInvitationServiceDependencies.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.Matchmaking.Invitations.Dependencies;

public sealed record LobbyInvitationServiceDependencies(
    IFriendshipRepository FriendshipRepository,
    IBlockedPlayerRepository BlockedPlayerRepository,
    ILobbyDirectory LobbyDirectory,
    IFriendshipNotifier Notifier);
```

## File: src/Server/Vertigo.Server.Matchmaking/Invitations/Services/LobbyInvitationService.cs
```csharp
using Vertigo.Server.Matchmaking.Invitations.Dependencies;
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Invitations.Services;

public class LobbyInvitationService : ILobbyInvitationService
{
    private const int MaximumInvitationsPerRequest = 5;

    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly ILobbyDirectory _lobbyDirectory;
    private readonly IFriendshipNotifier _notifier;
    private readonly ISessionIdentityResolver _sessions;

    public LobbyInvitationService(LobbyInvitationServiceDependencies dependencies, ISessionIdentityResolver sessions)
    {
        _friendshipRepository = dependencies.FriendshipRepository;
        _blockedPlayerRepository = dependencies.BlockedPlayerRepository;
        _lobbyDirectory = dependencies.LobbyDirectory;
        _notifier = dependencies.Notifier;
        _sessions = sessions;
    }

    public List<FriendEntry> GetInvitableFriends(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        InvitableFriendsQuery query = new InvitableFriendsQuery
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return GetInvitableFriendsForPlayer(query);
    }

    public List<Guid> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitations invitations = new LobbyInvitations
        {
            SenderId = identity.PlayerId,
            LobbyCode = lobbyCode,
            TargetIds = targetIds
        };

        return SendLobbyInvitationsForPlayer(invitations);
    }

    public List<LobbyInvitationEntry> GetPendingLobbyInvitations(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetPendingInvitationsForPlayer(identity.PlayerId);
    }

    public bool DeclineLobbyInvitation(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitationDecline decline = new LobbyInvitationDecline
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return DeclineInvitationForPlayer(decline);
    }

    private List<FriendEntry> GetInvitableFriendsForPlayer(InvitableFriendsQuery request)
    {
        bool isValid = request is not null && request.PlayerId != Guid.Empty;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        LobbyOverview lobby = FindLobbyOfMember(request!.LobbyCode, request.PlayerId);

        return FindInvitableFriends(request.PlayerId, lobby);
    }

    private List<Guid> SendLobbyInvitationsForPlayer(LobbyInvitations request)
    {
        List<Guid> invitedIds = new List<Guid>();

        EnsureInvitationRequestIsValid(request);

        LobbyOverview lobby = FindLobbyOfMember(request.LobbyCode, request.SenderId);
        LobbyMember sender = lobby.Members.First(member => member.PlayerId == request.SenderId);
        int freeSlots = lobby.MaxPlayers - lobby.Members.Count;

        if (!lobby.IsWaiting)
        {
            throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
        }

        if (sender.IsGuest)
        {
            throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
        }

        if (freeSlots <= 0)
        {
            throw CreateFault(ServiceErrorCodes.LobbyFull, ServiceMessageKeys.LobbyFull);
        }

        List<FriendEntry> invitableFriends = FindInvitableFriends(request.SenderId, lobby)
            .Where(friend => request.TargetIds.Contains(friend.PlayerId))
            .ToList();

        foreach (FriendEntry friend in invitableFriends)
        {
            bool isRegistered = invitedIds.Count < freeSlots
                                && _lobbyDirectory.RegisterInvitation(lobby.LobbyCode, friend.PlayerId, sender.PlayerId);

            if (isRegistered)
            {
                invitedIds.Add(friend.PlayerId);
                _notifier.NotifyLobbyInvitationReceived(friend.PlayerId, CreateEntry(lobby, sender.Username, sender.PlayerId));
            }
        }

        return invitedIds;
    }

    private List<LobbyInvitationEntry> GetPendingInvitationsForPlayer(Guid playerId)
    {
        List<LobbyInvitationEntry> entries = new List<LobbyInvitationEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        foreach (LobbyOverview lobby in _lobbyDirectory.FindLobbiesInviting(playerId))
        {
            Guid senderId = lobby.Invitations[playerId];
            LobbyMember? sender = lobby.Members.FirstOrDefault(member => member.PlayerId == senderId)
                                  ?? lobby.Members.FirstOrDefault(member => member.PlayerId == lobby.HostPlayerId);

            entries.Add(CreateEntry(lobby, sender?.Username ?? string.Empty, senderId));
        }

        return entries;
    }

    private bool DeclineInvitationForPlayer(LobbyInvitationDecline request)
    {
        bool isValid = request is not null && request.PlayerId != Guid.Empty;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        bool isDeclined = _lobbyDirectory.CancelInvitation(request!.LobbyCode, request.PlayerId);

        return isDeclined;
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }

    private static void EnsureInvitationRequestIsValid(LobbyInvitations request)
    {
        bool isValid = request is not null
                       && request.SenderId != Guid.Empty
                       && request.TargetIds is not null
                       && request.TargetIds.Count > 0
                       && request.TargetIds.Count <= MaximumInvitationsPerRequest
                       && !request.TargetIds.Contains(request.SenderId);

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static LobbyInvitationEntry CreateEntry(LobbyOverview lobby, string senderUsername, Guid senderId)
    {
        return new LobbyInvitationEntry
        {
            LobbyCode = lobby.LobbyCode,
            SenderId = senderId,
            SenderUsername = senderUsername,
            CurrentPlayers = lobby.Members.Count,
            MaxPlayers = lobby.MaxPlayers
        };
    }

    private static FriendEntry CreateFriendEntry(Guid playerId, Friendship friendship)
    {
        Player friend = friendship.PlayerRequesterId == playerId
            ? friendship.PlayerAddressee
            : friendship.PlayerRequester;

        return new FriendEntry
        {
            PlayerId = friend.PlayerId,
            Username = friend.Username
        };
    }

    private LobbyOverview FindLobbyOfMember(string lobbyCode, Guid playerId)
    {
        LobbyOverview? lobby = _lobbyDirectory.FindLobby(lobbyCode);

        if (lobby is null)
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }

        if (lobby.Members.All(member => member.PlayerId != playerId))
        {
            throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
        }

        return lobby;
    }

    private List<FriendEntry> FindInvitableFriends(Guid playerId, LobbyOverview lobby)
    {
        List<Friendship> friendships = _friendshipRepository.GetAcceptedFriendships(playerId);
        List<FriendEntry> friends = friendships
            .Select(friendship => CreateFriendEntry(playerId, friendship))
            .ToList();
        List<Guid> friendIds = friends.Select(friend => friend.PlayerId).ToList();
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(playerId, friendIds);

        List<FriendEntry> invitableFriends = friends
            .Where(friend => lobby.Members.All(member => member.PlayerId != friend.PlayerId))
            .Where(friend => !lobby.Invitations.ContainsKey(friend.PlayerId))
            .Where(friend => blocks.All(block => block.PlayerBlockerId != friend.PlayerId && block.PlayerBlockedId != friend.PlayerId))
            .Where(friend => _notifier.IsOnline(friend.PlayerId))
            .Where(friend => !_lobbyDirectory.IsInAnyLobby(friend.PlayerId))
            .OrderBy(friend => friend.Username)
            .ToList();

        return invitableFriends;
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/Services/LobbyGameOperations.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

// One dependency for Matchmaking's internal Game interactions.
// All methods are invoked while the owning LobbyService holds its lobby lock.
public sealed class LobbyGameOperations
{
    private readonly LobbyMatchActivation _activation;
    private readonly IMatchPresence _presence;

    public LobbyGameOperations(LobbyMatchActivation activation, IMatchPresence presence)
    {
        _activation = activation;
        _presence = presence;
    }

    public void Activate(LobbyState lobby)
    {
        _activation.Activate(lobby);
    }

    public bool TryReconnect(LobbyState lobby, Guid playerId)
    {
        MatchReconnectResult result = _presence.TryReconnect(lobby.MatchId, playerId);
        bool accepted = result is MatchReconnectResult.Reconnected
            or MatchReconnectResult.AlreadyConnected;

        return accepted;
    }

    public void Disconnect(LobbyState lobby, Guid playerId)
    {
        _presence.TryDisconnect(lobby.MatchId, playerId);
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/Services/LobbyMatchActivation.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

// The caller must hold the lobby synchronization lock during this operation.
// No asynchronous or network operations are performed by the in-process starter.
public sealed class LobbyMatchActivation
{
    private readonly IMatchSessionStarter _starter;

    public LobbyMatchActivation(IMatchSessionStarter starter)
    {
        _starter = starter;
    }

    public void Activate(LobbyState lobby)
    {
        ArgumentNullException.ThrowIfNull(lobby);
        if (!lobby.CanStartMatch(lobby.HostPlayerId))
        {
            throw new InvalidOperationException("The lobby is not eligible to start a match.");
        }

        Guid[] players = lobby.GetMembersExcept(Guid.Empty).Select(member => member.PlayerId).ToArray();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = lobby.LobbyInstanceId,
            LobbyCode = lobby.LobbyCode,
            HostPlayerId = lobby.HostPlayerId,
            PlayerIds = Array.AsReadOnly(players)
        };

        // If Game fails, LobbyState is unchanged and remains Waiting.
        MatchSessionSnapshot match = _starter.Start(request);
        if (match.MatchId == Guid.Empty || match.LobbyInstanceId != lobby.LobbyInstanceId
            || match.LobbyCode != lobby.LobbyCode || match.InitialHostPlayerId != lobby.HostPlayerId
            || !match.PlayerIds.SequenceEqual(players))
        {
            throw new InvalidOperationException("Game returned an inconsistent match session.");
        }

        lobby.StartMatch(match.MatchId);
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbyConnection.cs
```csharp
#nullable enable

using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed class LobbyConnection
{
    public LobbyConnection(LobbyMember member, ILobbyCallback callback, string sessionId)
    {
        Member = member;
        Callback = callback;
        SessionId = sessionId;
    }

    public string SessionId { get; }

    public LobbyMember Member { get; }

    public ILobbyCallback Callback { get; }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbySettings.cs
```csharp
namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed record LobbySettings(
    string LobbyCode,
    Guid HostPlayerId,
    bool IsPublic,
    int MaxPlayers,
    DateTimeOffset CreatedAtUtc);
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbyStatus.cs
```csharp
namespace Vertigo.Server.Matchmaking.Lobbies;

public enum LobbyStatus
{
    Waiting,
    InGame
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Dependencies/AuthenticationDependencies.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Dependencies;

public sealed class AuthenticationDependencies
{
    public IPlayerRepository PlayerRepository { get; }

    public ISessionTokenRepository SessionTokenRepository { get; }

    public IPasswordHasher PasswordHasher { get; }

    public ITokenGenerator TokenGenerator { get; }

    public AuthenticationDependencies(
        IPlayerRepository playerRepository,
        ISessionTokenRepository sessionTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        PlayerRepository = playerRepository;
        SessionTokenRepository = sessionTokenRepository;
        PasswordHasher = passwordHasher;
        TokenGenerator = tokenGenerator;
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Interfaces/IGuestSessionRegistry.cs
```csharp
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Interfaces;

public interface IGuestSessionRegistry
{
    AuthenticatedSession Create();

    SessionIdentity? Find(string tokenHash);

    AuthenticatedSession? GetSession(string tokenHash);

    bool Revoke(string tokenHash);
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Interfaces/ISessionIdentityResolver.cs
```csharp
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Interfaces;

public interface ISessionIdentityResolver
{
    SessionIdentity? Resolve(string token);

    SessionIdentity Require(string token);

    SessionIdentity RequireRegistered(string token);
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Models/AuthenticatedSession.cs
```csharp
namespace Vertigo.Server.PlayerManagement.Authentication.Models;

public sealed class AuthenticatedSession
{
    public Guid PlayerId { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }

    public bool IsGuest { get; init; }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Models/SessionIdentity.cs
```csharp
namespace Vertigo.Server.PlayerManagement.Authentication.Models;

public sealed record SessionIdentity(Guid PlayerId, string Username, bool IsGuest);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/AuthenticationService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public class AuthenticationService : IAuthService
{
    private readonly AuthService _authentication;

    public AuthenticationService(AuthService authentication)
    {
        _authentication = authentication;
    }

    public UserSession Login(LoginCredentials credentials)
    {
        if (credentials is null || string.IsNullOrWhiteSpace(credentials.Username)
            || string.IsNullOrWhiteSpace(credentials.Password))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        AuthenticatedSession? session = _authentication.Login(credentials.Username, credentials.Password);
        if (session is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        return ConvertSession(session);
    }

    public UserSession ResumeSession(string token)
    {
        AuthenticatedSession? session = _authentication.ResumeSession(token);
        if (session is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        return ConvertSession(session);
    }

    public UserSession CreateGuestSession()
    {
        return ConvertSession(_authentication.CreateGuestSession());
    }

    public bool Logout(SessionLogout logout)
    {
        return logout is not null && _authentication.Logout(logout.SessionToken);
    }

    private static UserSession ConvertSession(AuthenticatedSession session)
    {
        return new UserSession
        {
            PlayerId = session.PlayerId,
            Username = session.Username,
            Token = session.Token,
            ExpiresAt = session.ExpiresAt,
            IsGuest = session.IsGuest
        };
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/GuestSessionRegistry.cs
```csharp
using System.Collections.Concurrent;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class GuestSessionRegistry : IGuestSessionRegistry
{
    private const int GuestDurationHours = 12;

    private readonly ConcurrentDictionary<string, AuthenticatedSession> _sessions = new();
    private readonly ITokenGenerator _tokenGenerator;
    private readonly TimeProvider _timeProvider;

    public GuestSessionRegistry(ITokenGenerator tokenGenerator, TimeProvider timeProvider)
    {
        _tokenGenerator = tokenGenerator;
        _timeProvider = timeProvider;
    }

    public AuthenticatedSession Create()
    {
        string token = _tokenGenerator.GenerateSessionToken();
        string guestCode = _tokenGenerator.GenerateVerificationCode();
        AuthenticatedSession session = new AuthenticatedSession
        {
            PlayerId = Guid.NewGuid(),
            Username = $"Guest_{guestCode}",
            Token = token,
            ExpiresAt = _timeProvider.GetUtcNow().UtcDateTime.AddHours(GuestDurationHours),
            IsGuest = true
        };

        string tokenHash = _tokenGenerator.HashSessionToken(token);
        if (!_sessions.TryAdd(tokenHash, session))
        {
            throw new InvalidOperationException("A guest session could not be registered.");
        }

        return session;
    }

    public SessionIdentity? Find(string tokenHash)
    {
        if (!_sessions.TryGetValue(tokenHash, out AuthenticatedSession? session))
        {
            return null;
        }

        if (session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            _sessions.TryRemove(tokenHash, out _);
            return null;
        }

        return new SessionIdentity(session.PlayerId, session.Username, true);
    }

    public AuthenticatedSession? GetSession(string tokenHash)
    {
        if (Find(tokenHash) is null)
        {
            return null;
        }

        return _sessions.TryGetValue(tokenHash, out AuthenticatedSession? session) ? session : null;
    }

    public bool Revoke(string tokenHash)
    {
        return _sessions.TryRemove(tokenHash, out _);
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/SessionIdentityResolver.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class SessionIdentityResolver : ISessionIdentityResolver
{
    private const int SessionTokenLength = 64;

    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IGuestSessionRegistry _guests;

    public SessionIdentityResolver(
        ISessionTokenRepository sessionTokenRepository,
        ITokenGenerator tokenGenerator,
        IGuestSessionRegistry guests)
    {
        _sessionTokenRepository = sessionTokenRepository;
        _tokenGenerator = tokenGenerator;
        _guests = guests;
    }

    public SessionIdentity? Resolve(string token)
    {
        SessionIdentity? identity = null;
        if (!string.IsNullOrWhiteSpace(token) && token.Length == SessionTokenLength)
        {
            string tokenHash = _tokenGenerator.HashSessionToken(token);
            identity = _guests.Find(tokenHash);
            if (identity is null)
            {
                SessionToken? session = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
                if (session is not null)
                {
                    identity = new SessionIdentity(session.PlayerId, session.Player.Username, false);
                }
            }
        }

        return identity;
    }

    public SessionIdentity Require(string token)
    {
        return Resolve(token) ?? throw CreateUnauthorizedFault();
    }

    public SessionIdentity RequireRegistered(string token)
    {
        SessionIdentity identity = Require(token);
        if (identity.IsGuest)
        {
            throw CreateUnauthorizedFault();
        }

        return identity;
    }

    private static FaultException<ServiceFault> CreateUnauthorizedFault()
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = ServiceErrorCodes.Unauthorized,
            MessageKey = ServiceMessageKeys.Unauthorized
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(fault.ErrorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Blocking/Services/BlockedPlayerService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Blocking.Services;

public class BlockedPlayerService : IBlockedPlayerService
{
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly ISessionIdentityResolver _sessions;

    public BlockedPlayerService(IBlockedPlayerRepository blockedPlayerRepository, ISessionIdentityResolver sessions)
    {
        _blockedPlayerRepository = blockedPlayerRepository;
        _sessions = sessions;
    }

    public List<BlockedPlayerSummary> GetBlockedPlayers(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetBlockedPlayersForPlayer(identity.PlayerId);
    }

    public bool BlockPlayer(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return BlockPlayerBetween(new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId });
    }

    public bool UnblockPlayer(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return UnblockPlayerBetween(new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId });
    }

    private List<BlockedPlayerSummary> GetBlockedPlayersForPlayer(Guid playerId)
    {
        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<BlockedPlayer> blockedPlayers = _blockedPlayerRepository.GetBlockedPlayers(playerId);
        return blockedPlayers.Select(blocked => new BlockedPlayerSummary
        {
            PlayerId = blocked.PlayerBlockedId,
            Username = blocked.PlayerBlocked.Username,
            BlockedAt = blocked.CreatedAt
        }).ToList();
    }

    private bool BlockPlayerBetween(PlayerPair pair)
    {
        EnsureValidPair(pair);
        if (_blockedPlayerRepository.IsPlayerBlocked(pair.PlayerId, pair.OtherPlayerId))
        {
            return false;
        }

        return _blockedPlayerRepository.BlockPlayer(pair.PlayerId, pair.OtherPlayerId);
    }

    private bool UnblockPlayerBetween(PlayerPair pair)
    {
        EnsureValidPair(pair);
        return _blockedPlayerRepository.UnblockPlayer(pair.PlayerId, pair.OtherPlayerId);
    }

    private static void EnsureValidPair(PlayerPair pair)
    {
        if (pair is null || pair.PlayerId == Guid.Empty || pair.OtherPlayerId == Guid.Empty
            || pair.PlayerId == pair.OtherPlayerId)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Dependencies/FriendshipServiceDependencies.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Friendships.Dependencies;

public sealed record FriendshipServiceDependencies(
    IPlayerRepository PlayerRepository,
    IFriendshipRepository FriendshipRepository,
    IBlockedPlayerRepository BlockedPlayerRepository,
    IFriendshipNotifier Notifier);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Models/FriendshipSubscription.cs
```csharp
using Vertigo.Contracts.Callbacks;

namespace Vertigo.Server.PlayerManagement.Friendships.Models;

public sealed record FriendshipSubscription(IFriendshipCallback Callback, string SessionId);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Services/FriendshipNotificationService.cs
```csharp
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class FriendshipNotificationService : IFriendshipNotificationService
{
    private readonly IFriendshipNotifier _notifier;
    private readonly IServiceScopeFactory _scopeFactory;

    public FriendshipNotificationService(IFriendshipNotifier notifier, IServiceScopeFactory scopeFactory)
    {
        _notifier = notifier;
        _scopeFactory = scopeFactory;
    }

    public bool Subscribe(string sessionToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISessionIdentityResolver resolver = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        SessionIdentity identity = resolver.RequireRegistered(sessionToken);

        IFriendshipCallback callback = OperationContext.Current.GetCallbackChannel<IFriendshipCallback>();
        IContextChannel channel = (IContextChannel)callback;
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        _notifier.Subscribe(identity.PlayerId, callback, sessionId);

        channel.Closed -= OnChannelClosed;
        channel.Faulted -= OnChannelClosed;
        channel.Closed += OnChannelClosed;
        channel.Faulted += OnChannelClosed;

        return true;
    }

    public void Unsubscribe()
    {
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        _notifier.UnsubscribeSession(sessionId);
    }

    private void OnChannelClosed(object? sender, EventArgs eventArgs)
    {
        string sessionId = (sender as IContextChannel)?.SessionId ?? string.Empty;

        _notifier.UnsubscribeSession(sessionId);
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Services/FriendshipNotifier.cs
```csharp
using Vertigo.Server.PlayerManagement.Friendships.Models;
using Vertigo.Contracts.Internal;
using System.Collections.Concurrent;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public sealed class FriendshipNotifier : IFriendshipNotifier
{
    private readonly ConcurrentDictionary<Guid, FriendshipSubscription> _subscriptions = new();

    public void Subscribe(Guid playerId, IFriendshipCallback callback, string sessionId)
    {
        _subscriptions[playerId] = new FriendshipSubscription(callback, sessionId);
    }

    public void Unsubscribe(Guid playerId, string sessionId)
    {
        if (_subscriptions.TryGetValue(playerId, out FriendshipSubscription? subscription)
            && subscription.SessionId == sessionId)
        {
            ICollection<KeyValuePair<Guid, FriendshipSubscription>> entries = _subscriptions;
            entries.Remove(new KeyValuePair<Guid, FriendshipSubscription>(playerId, subscription));
        }
    }

    public void UnsubscribeSession(string sessionId)
    {
        List<Guid> playerIds = _subscriptions
            .Where(pair => pair.Value.SessionId == sessionId)
            .Select(pair => pair.Key)
            .ToList();

        foreach (Guid playerId in playerIds)
        {
            Unsubscribe(playerId, sessionId);
        }
    }

    public void NotifyFriendRequestReceived(Guid targetPlayerId, FriendshipInvitationEntry request)
    {
        Send(targetPlayerId, SendFriendRequestReceived, request);
    }

    public void NotifyFriendshipEstablished(Guid targetPlayerId, FriendEntry friend)
    {
        Send(targetPlayerId, SendFriendshipEstablished, friend);
    }

    public void NotifyLobbyInvitationReceived(Guid targetPlayerId, LobbyInvitationEntry invitation)
    {
        Send(targetPlayerId, SendLobbyInvitationReceived, invitation);
    }

    public void NotifyLobbyInvitationWithdrawn(Guid targetPlayerId, string lobbyCode)
    {
        Send(targetPlayerId, SendLobbyInvitationWithdrawn, lobbyCode);
    }

    public bool IsOnline(Guid playerId)
    {
        bool isOnline = _subscriptions.ContainsKey(playerId);

        return isOnline;
    }

    private static void SendLobbyInvitationReceived(IFriendshipCallback callback, LobbyInvitationEntry invitation)
    {
        callback.OnLobbyInvitationReceived(invitation);
    }

    private static void SendLobbyInvitationWithdrawn(IFriendshipCallback callback, string lobbyCode)
    {
        callback.OnLobbyInvitationWithdrawn(lobbyCode);
    }

    private static void SendFriendRequestReceived(IFriendshipCallback callback, FriendshipInvitationEntry request)
    {
        callback.OnFriendRequestReceived(request);
    }

    private static void SendFriendshipEstablished(IFriendshipCallback callback, FriendEntry friend)
    {
        callback.OnFriendshipEstablished(friend);
    }

    private void Send<TArgument>(
        Guid targetPlayerId,
        Action<IFriendshipCallback, TArgument> notification,
        TArgument argument)
    {
        if (_subscriptions.TryGetValue(targetPlayerId, out FriendshipSubscription? subscription))
        {
            try
            {
                notification(subscription.Callback, argument);
            }
            catch (CommunicationException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
            catch (TimeoutException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
            catch (ObjectDisposedException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
        }
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Friendships/Services/FriendshipService.cs
```csharp
using Vertigo.Server.PlayerManagement.Friendships.Dependencies;
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public class FriendshipService : IFriendshipService
{
    private const int MinimumSearchLength = 3;
    private const int MaximumSearchResults = 20;

    private readonly IPlayerRepository _playerRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly IFriendshipNotifier _notifier;
    private readonly ISessionIdentityResolver _sessions;

    public FriendshipService(FriendshipServiceDependencies dependencies, ISessionIdentityResolver sessions)
    {
        _playerRepository = dependencies.PlayerRepository;
        _friendshipRepository = dependencies.FriendshipRepository;
        _blockedPlayerRepository = dependencies.BlockedPlayerRepository;
        _notifier = dependencies.Notifier;
        _sessions = sessions;
    }

    public List<FriendEntry> GetFriends(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetFriendsForPlayer(identity.PlayerId);
    }

    public List<PlayerSearchEntry> SearchPlayers(string searchTerm, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerSearch search = new PlayerSearch { RequesterId = identity.PlayerId, SearchTerm = searchTerm };
        return SearchPlayersForPlayer(search);
    }

    public PlayerRelation SendFriendRequest(Guid targetPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipInvitation invitation = new FriendshipInvitation
        {
            SenderId = identity.PlayerId,
            TargetId = targetPlayerId
        };

        return SendFriendRequestForPlayer(invitation);
    }

    public List<FriendshipInvitationEntry> GetPendingFriendRequests(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetPendingRequestsForPlayer(identity.PlayerId);
    }

    public bool RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipDecision decision = new FriendshipDecision
        {
            SenderId = senderPlayerId,
            ResponderId = identity.PlayerId,
            Accept = accept
        };

        return DecideFriendshipForPlayer(decision);
    }

    public bool RemoveFriendship(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerPair pair = new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId };
        return RemoveFriendshipForPlayer(pair);
    }

    private List<FriendEntry> GetFriendsForPlayer(Guid playerId)
    {
        List<FriendEntry> entries = new List<FriendEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<Friendship> friendships = _friendshipRepository.GetAcceptedFriendships(playerId);

        foreach (Friendship friendship in friendships)
        {
            entries.Add(CreateFriendEntry(playerId, friendship));
        }

        return entries.OrderBy(entry => entry.Username).ToList();
    }

    private List<PlayerSearchEntry> SearchPlayersForPlayer(PlayerSearch request)
    {
        List<PlayerSearchEntry> entries = new List<PlayerSearchEntry>();

        EnsureSearchRequestIsValid(request);

        string searchTerm = request.SearchTerm.Trim();
        List<Player> players = _playerRepository.SearchPlayersByUsername(
            searchTerm,
            request.RequesterId,
            MaximumSearchResults);
        List<Guid> playerIds = players.Select(player => player.PlayerId).ToList();
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(request.RequesterId, playerIds);
        List<Friendship> friendships = _friendshipRepository.GetFriendshipsBetween(request.RequesterId, playerIds);

        foreach (Player player in players)
        {
            bool hasBlockedRequester = blocks.Any(block => block.PlayerBlockerId == player.PlayerId);

            if (!hasBlockedRequester)
            {
                PlayerRelation relation = ResolveRelation(request.RequesterId, player.PlayerId, blocks, friendships);
                entries.Add(CreateSearchEntry(player, relation));
            }
        }

        return entries;
    }

    private PlayerRelation SendFriendRequestForPlayer(FriendshipInvitation request)
    {
        PlayerRelation relation = PlayerRelation.None;

        EnsureFriendRequestIsValid(request);

        Player sender = FindRegisteredPlayer(request.SenderId);
        Player target = FindRegisteredPlayer(request.TargetId);
        EnsurePlayersAreNotBlocked(request);

        Friendship? existingFriendship = _friendshipRepository.GetFriendshipBetween(request.SenderId, request.TargetId);

        if (existingFriendship is null)
        {
            relation = CreatePendingFriendship(sender, target);
        }
        else if (existingFriendship.Status == FriendshipStatus.Accepted)
        {
            throw CreateFault(ServiceErrorCodes.AlreadyFriends, ServiceMessageKeys.AlreadyFriends);
        }
        else if (existingFriendship.PlayerRequesterId == request.SenderId)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestAlreadySent, ServiceMessageKeys.FriendRequestAlreadySent);
        }
        else
        {
            relation = AcceptCrossedRequest(existingFriendship, sender, target);
        }

        return relation;
    }

    private List<FriendshipInvitationEntry> GetPendingRequestsForPlayer(Guid playerId)
    {
        List<FriendshipInvitationEntry> entries = new List<FriendshipInvitationEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<Friendship> pendingFriendships = _friendshipRepository.GetPendingRequestsReceivedBy(playerId);

        foreach (Friendship friendship in pendingFriendships)
        {
            entries.Add(CreateRequestEntry(friendship));
        }

        return entries;
    }

    private bool DecideFriendshipForPlayer(FriendshipDecision request)
    {
        bool isResponded = false;

        EnsureResponseIsValid(request);

        Friendship? friendship = _friendshipRepository.GetFriendshipBetween(request.SenderId, request.ResponderId);
        bool isPendingForResponder = friendship is not null
                                     && friendship.Status == FriendshipStatus.Pending
                                     && friendship.PlayerRequesterId == request.SenderId;

        if (!isPendingForResponder)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestNotFound, ServiceMessageKeys.FriendRequestNotFound);
        }

        if (request.Accept)
        {
            isResponded = AcceptFriendRequest(request);
        }
        else
        {
            isResponded = _friendshipRepository.DeletePendingFriendship(request.SenderId, request.ResponderId);
        }

        return isResponded;
    }

    private bool RemoveFriendshipForPlayer(PlayerPair pair)
    {
        if (pair is null || pair.PlayerId == Guid.Empty || pair.OtherPlayerId == Guid.Empty
            || pair.PlayerId == pair.OtherPlayerId)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return _friendshipRepository.DeleteFriendshipBetween(pair.PlayerId, pair.OtherPlayerId);
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }

    private static PlayerRelation ResolveRelation(
        Guid requesterId,
        Guid otherPlayerId,
        List<BlockedPlayer> blocks,
        List<Friendship> friendships)
    {
        PlayerRelation relation = PlayerRelation.None;

        bool isBlockedByRequester = blocks.Any(
            block => block.PlayerBlockerId == requesterId && block.PlayerBlockedId == otherPlayerId);
        Friendship? friendship = friendships.FirstOrDefault(
            candidate => candidate.PlayerRequesterId == otherPlayerId || candidate.PlayerAddresseeId == otherPlayerId);

        if (isBlockedByRequester)
        {
            relation = PlayerRelation.Blocked;
        }
        else if (friendship is not null && friendship.Status == FriendshipStatus.Accepted)
        {
            relation = PlayerRelation.Friends;
        }
        else if (friendship is not null && friendship.PlayerRequesterId == requesterId)
        {
            relation = PlayerRelation.RequestSent;
        }

        return relation;
    }

    private static PlayerSearchEntry CreateSearchEntry(Player player, PlayerRelation relation)
    {
        return new PlayerSearchEntry
        {
            PlayerId = player.PlayerId,
            Username = player.Username,
            Relation = relation
        };
    }

    private static FriendshipInvitationEntry CreateRequestEntry(Friendship friendship)
    {
        return new FriendshipInvitationEntry
        {
            SenderId = friendship.PlayerRequesterId,
            SenderUsername = friendship.PlayerRequester.Username
        };
    }

    private static void EnsureSearchRequestIsValid(PlayerSearch request)
    {
        bool isValid = request is not null
                       && request.RequesterId != Guid.Empty
                       && request.SearchTerm is not null
                       && request.SearchTerm.Trim().Length >= MinimumSearchLength;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static void EnsureFriendRequestIsValid(FriendshipInvitation request)
    {
        bool isValid = request is not null
                       && request.SenderId != Guid.Empty
                       && request.TargetId != Guid.Empty
                       && request.SenderId != request.TargetId;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static void EnsureResponseIsValid(FriendshipDecision request)
    {
        bool isValid = request is not null
                       && request.ResponderId != Guid.Empty
                       && request.SenderId != Guid.Empty
                       && request.ResponderId != request.SenderId;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static FriendEntry CreateFriendEntry(Guid playerId, Friendship friendship)
    {
        Player friend = friendship.PlayerRequesterId == playerId
            ? friendship.PlayerAddressee
            : friendship.PlayerRequester;

        return CreateFriendEntry(friend);
    }

    private static FriendEntry CreateFriendEntry(Player player)
    {
        return new FriendEntry
        {
            PlayerId = player.PlayerId,
            Username = player.Username
        };
    }

    private Player FindRegisteredPlayer(Guid playerId)
    {
        Player? player = _playerRepository.GetPlayerById(playerId);

        if (player is null)
        {
            throw CreateFault(ServiceErrorCodes.UserNotFound, ServiceMessageKeys.UserNotFound);
        }

        return player;
    }

    private void EnsurePlayersAreNotBlocked(FriendshipInvitation request)
    {
        List<Guid> targetIds = new List<Guid> { request.TargetId };
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(request.SenderId, targetIds);

        if (blocks.Count > 0)
        {
            throw CreateFault(ServiceErrorCodes.FriendshipBlocked, ServiceMessageKeys.FriendshipBlocked);
        }
    }

    private PlayerRelation CreatePendingFriendship(Player sender, Player target)
    {
        bool isCreated = _friendshipRepository.CreatePendingFriendship(sender.PlayerId, target.PlayerId);

        if (!isCreated)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        FriendshipInvitationEntry entry = new FriendshipInvitationEntry
        {
            SenderId = sender.PlayerId,
            SenderUsername = sender.Username
        };
        _notifier.NotifyFriendRequestReceived(target.PlayerId, entry);

        return PlayerRelation.RequestSent;
    }

    private PlayerRelation AcceptCrossedRequest(Friendship crossedFriendship, Player sender, Player target)
    {
        bool isAccepted = _friendshipRepository.AcceptPendingFriendship(
            crossedFriendship.PlayerRequesterId,
            crossedFriendship.PlayerAddresseeId);

        if (!isAccepted)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestNotFound, ServiceMessageKeys.FriendRequestNotFound);
        }

        _notifier.NotifyFriendshipEstablished(sender.PlayerId, CreateFriendEntry(target));
        _notifier.NotifyFriendshipEstablished(target.PlayerId, CreateFriendEntry(sender));

        return PlayerRelation.Friends;
    }

    private bool AcceptFriendRequest(FriendshipDecision request)
    {
        bool isAccepted = _friendshipRepository.AcceptPendingFriendship(request.SenderId, request.ResponderId);

        if (isAccepted)
        {
            Player responder = FindRegisteredPlayer(request.ResponderId);
            _notifier.NotifyFriendshipEstablished(request.SenderId, CreateFriendEntry(responder));
        }

        return isAccepted;
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Registration/Dependencies/AccountServiceDependencies.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.PlayerManagement.Registration.Dependencies;

public sealed record AccountServiceDependencies(
    IPlayerRepository PlayerRepository,
    ISessionTokenRepository SessionTokenRepository,
    IPasswordHasher PasswordHasher,
    ISessionIdentityResolver Sessions);
```

## File: src/Server/Vertigo.Server.PlayerManagement/Registration/Services/AccountService.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

public class AccountService : IAccountService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionIdentityResolver _sessions;

    public AccountService(AccountServiceDependencies dependencies)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _sessions = dependencies.Sessions;
    }

    public bool RegisterAccount(AccountRegistration registration)
    {
        if (registration is null || string.IsNullOrWhiteSpace(registration.Username)
            || string.IsNullOrWhiteSpace(registration.Email)
            || string.IsNullOrWhiteSpace(registration.Password)
            || registration.Username.Length > 32 || registration.Email.Length > 120)
        {
            throw CreateFault(ServiceErrorCodes.InvalidRegistrationData, ServiceMessageKeys.InvalidRegistrationData);
        }

        if (_playerRepository.UsernameExists(registration.Username))
        {
            throw CreateFault(ServiceErrorCodes.UsernameAlreadyRegistered,
                ServiceMessageKeys.UsernameAlreadyRegistered);
        }

        if (_playerRepository.EmailExists(registration.Email))
        {
            throw CreateFault(ServiceErrorCodes.EmailAlreadyRegistered,
                ServiceMessageKeys.EmailAlreadyRegistered);
        }

        string salt = _passwordHasher.GenerateSalt();
        Player player = new Player
        {
            Username = registration.Username.Trim(),
            Email = registration.Email.Trim(),
            PasswordSalt = salt,
            PasswordHash = _passwordHasher.HashPassword(registration.Password, salt),
            Stats = new PlayerStats()
        };

        return _playerRepository.CreatePlayer(player);
    }

    public bool ChangePassword(PasswordChange change, string sessionToken)
    {
        if (change is null || string.IsNullOrWhiteSpace(change.Username)
            || string.IsNullOrWhiteSpace(change.CurrentPassword)
            || string.IsNullOrWhiteSpace(change.NewPassword))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        Player? player = _playerRepository.GetPlayerById(identity.PlayerId);
        if (player is null || !string.Equals(player.Username, change.Username, StringComparison.OrdinalIgnoreCase) || !_passwordHasher.VerifyPassword(
                change.CurrentPassword, player.PasswordHash, player.PasswordSalt))
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        string salt = _passwordHasher.GenerateSalt();
        string hash = _passwordHasher.HashPassword(change.NewPassword, salt);
        bool isChanged = _playerRepository.UpdatePassword(player.PlayerId, hash, salt);
        if (isChanged)
        {
            _sessionTokenRepository.RevokePlayerSessions(player.PlayerId);
        }

        return isChanged;
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Shared/Vertigo.Contracts/Callbacks/IFriendshipCallback.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface IFriendshipCallback
{
    [OperationContract(IsOneWay = true)]
    void OnFriendRequestReceived(FriendshipInvitationEntry request);

    [OperationContract(IsOneWay = true)]
    void OnFriendshipEstablished(FriendEntry friend);

    [OperationContract(IsOneWay = true)]
    void OnLobbyInvitationReceived(LobbyInvitationEntry invitation);

    [OperationContract(IsOneWay = true)]
    void OnLobbyInvitationWithdrawn(string lobbyCode);
}
```

## File: src/Shared/Vertigo.Contracts/Endpoints/ServiceEndpoints.cs
```csharp
namespace Vertigo.Contracts.Endpoints;

public static class ServiceEndpoints
{
    public const int TcpPort = 8081;

    public static readonly string AccountServiceAddress = $"net.tcp://localhost:{TcpPort}/AccountService";
    public static readonly string AuthServiceAddress = $"net.tcp://localhost:{TcpPort}/AuthService";
    public static readonly string FriendshipServiceAddress = $"net.tcp://localhost:{TcpPort}/FriendshipService";
    public static readonly string FriendshipNotificationServiceAddress = $"net.tcp://localhost:{TcpPort}/FriendshipNotificationService";
    public static readonly string LobbyInvitationServiceAddress = $"net.tcp://localhost:{TcpPort}/LobbyInvitationService";
    public static readonly string BlockedPlayerServiceAddress = $"net.tcp://localhost:{TcpPort}/BlockedPlayerService";
    public static readonly string ChatServiceAddress = $"net.tcp://localhost:{TcpPort}/ChatService";
    public static readonly string LobbyServiceAddress = $"net.tcp://localhost:{TcpPort}/LobbyService";
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IAccountService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IAccountService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RegisterAccount(AccountRegistration registration);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool ChangePassword(PasswordChange change, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IAuthService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IAuthService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    UserSession Login(LoginCredentials credentials);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    UserSession ResumeSession(string token);

    [OperationContract]
    UserSession CreateGuestSession();

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool Logout(SessionLogout logout);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IBlockedPlayerService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IBlockedPlayerService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<BlockedPlayerSummary> GetBlockedPlayers(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool BlockPlayer(Guid otherPlayerId, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool UnblockPlayer(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IChatService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IChatService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool SendMessage(string lobbyCode, string text, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IFriendshipNotificationService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract(CallbackContract = typeof(IFriendshipCallback), SessionMode = SessionMode.Required)]
public interface IFriendshipNotificationService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool Subscribe(string sessionToken);

    [OperationContract(IsOneWay = true)]
    void Unsubscribe();
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/IFriendshipService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IFriendshipService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendEntry> GetFriends(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<PlayerSearchEntry> SearchPlayers(string searchTerm, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    PlayerRelation SendFriendRequest(Guid targetPlayerId, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendshipInvitationEntry> GetPendingFriendRequests(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RemoveFriendship(Guid otherPlayerId, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/ILobbyInvitationService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface ILobbyInvitationService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendEntry> GetInvitableFriends(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<Guid> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<LobbyInvitationEntry> GetPendingLobbyInvitations(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool DeclineLobbyInvitation(string lobbyCode, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/IFriendshipNotifier.cs
```csharp
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface IFriendshipNotifier
{
    void Subscribe(Guid playerId, IFriendshipCallback callback, string sessionId);

    void Unsubscribe(Guid playerId, string sessionId);

    void UnsubscribeSession(string sessionId);

    void NotifyFriendRequestReceived(Guid targetPlayerId, FriendshipInvitationEntry request);

    void NotifyFriendshipEstablished(Guid targetPlayerId, FriendEntry friend);

    void NotifyLobbyInvitationReceived(Guid targetPlayerId, LobbyInvitationEntry invitation);

    void NotifyLobbyInvitationWithdrawn(Guid targetPlayerId, string lobbyCode);

    bool IsOnline(Guid playerId);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/ILobbyDirectory.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface ILobbyDirectory
{
    LobbyOverview? FindLobby(string lobbyCode);

    bool IsInAnyLobby(Guid playerId);

    bool RegisterInvitation(string lobbyCode, Guid targetPlayerId, Guid senderPlayerId);

    bool CancelInvitation(string lobbyCode, Guid targetPlayerId);

    List<LobbyOverview> FindLobbiesInviting(Guid playerId);

    bool PublishChatMessage(LobbyChatMessage message);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/IMatchLifecycle.cs
```csharp
namespace Vertigo.Contracts.Internal;

// In-process API only; deliberately not a CoreWCF endpoint.
public interface IMatchLifecycle
{
    MatchLifecycleSnapshot? Get(Guid matchId);

    bool TryFinish(Guid matchId);

    bool TryCancel(Guid matchId);

    int PurgeTerminated(TimeSpan retention);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/IMatchPresence.cs
```csharp
namespace Vertigo.Contracts.Internal;

// In-process contract only. Identity is verified by Matchmaking before calling Game.
public interface IMatchPresence
{
    bool TryDisconnect(Guid matchId, Guid playerId);

    MatchReconnectResult TryReconnect(Guid matchId, Guid playerId);

    MatchPresenceSnapshot? GetPresence(Guid matchId);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/IMatchSessionStarter.cs
```csharp
namespace Vertigo.Contracts.Internal;

// In-process contract: not a CoreWCF service and not a public endpoint.
public interface IMatchSessionStarter
{
    MatchSessionSnapshot Start(MatchStartRequest request);
}
```

## File: src/Shared/Vertigo.Contracts/Internal/LobbyOverview.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public sealed record LobbyOverview(
    string LobbyCode,
    Guid HostPlayerId,
    int MaxPlayers,
    bool IsWaiting,
    List<LobbyMember> Members,
    Dictionary<Guid, Guid> Invitations);
```

## File: src/Shared/Vertigo.Contracts/Internal/MatchLifecycleSnapshot.cs
```csharp
namespace Vertigo.Contracts.Internal;

public sealed record MatchLifecycleSnapshot
{
    public required Guid MatchId { get; init; }

    public required Guid LobbyInstanceId { get; init; }

    public required MatchLifecycleStatus Status { get; init; }

    public required long Revision { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
```

## File: src/Shared/Vertigo.Contracts/Internal/MatchLifecycleStatus.cs
```csharp
namespace Vertigo.Contracts.Internal;

// Internal game lifecycle, independent of LobbyStatus and CoreWCF.
public enum MatchLifecycleStatus
{
    Running,
    Suspended,
    Finished,
    Cancelled
}
```

## File: src/Shared/Vertigo.Contracts/Internal/MatchPresenceSnapshot.cs
```csharp
namespace Vertigo.Contracts.Internal;

public sealed record MatchPresenceSnapshot
{
    public required Guid MatchId { get; init; }

    public required MatchLifecycleStatus Status { get; init; }

    public required long Revision { get; init; }

    public required IReadOnlyList<Guid> DisconnectedPlayerIds { get; init; }
}
```

## File: src/Shared/Vertigo.Contracts/Internal/MatchReconnectResult.cs
```csharp
namespace Vertigo.Contracts.Internal;

// Result of a single atomic attempt inside the Game registry lock.
public enum MatchReconnectResult
{
    Reconnected,
    AlreadyConnected,
    Rejected
}
```

## File: src/Shared/Vertigo.Contracts/Internal/MatchSessionSnapshot.cs
```csharp
namespace Vertigo.Contracts.Internal;

public sealed record MatchSessionSnapshot(
    Guid MatchId,
    Guid LobbyInstanceId,
    string LobbyCode,
    Guid InitialHostPlayerId,
    IReadOnlyList<Guid> PlayerIds,
    DateTime CreatedAtUtc);
```

## File: src/Shared/Vertigo.Contracts/Internal/MatchStartRequest.cs
```csharp
namespace Vertigo.Contracts.Internal;

public sealed record MatchStartRequest
{
    public required Guid LobbyInstanceId { get; init; }

    public required string LobbyCode { get; init; }

    public required Guid HostPlayerId { get; init; }

    public required IReadOnlyList<Guid> PlayerIds { get; init; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/AccountRegistration.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class AccountRegistration
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Email { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/BlockedPlayerSummary.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class BlockedPlayerSummary
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public DateTime BlockedAt { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendEntry
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendshipDecision.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipDecision
{
    [DataMember]
    public Guid ResponderId { get; set; }

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public bool Accept { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendshipInvitation.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipInvitation
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public Guid TargetId { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/FriendshipInvitationEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipInvitationEntry
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/InvitableFriendsQuery.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class InvitableFriendsQuery
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyAdmission.cs
```csharp
#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyAdmission
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid HostPlayerId { get; set; }

    [DataMember]
    public bool IsPublic { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }

    [DataMember]
    public List<LobbyMember> ExistingMembers { get; set; } = new();
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyChatMessage.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class LobbyChatMessage
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;

    [DataMember]
    public string Text { get; set; } = string.Empty;

    [DataMember]
    public DateTime SentAt { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyInvitationDecline.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyInvitationDecline
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyInvitationEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyInvitationEntry
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;

    [DataMember]
    public int CurrentPlayers { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyInvitations.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyInvitations
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public List<Guid> TargetIds { get; set; } = new();
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyMember.cs
```csharp
#nullable enable

using System;
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyMember
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public bool IsGuest { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbySummary.cs
```csharp
#nullable enable

using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbySummary
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public string HostUsername { get; set; } = string.Empty;

    [DataMember]
    public int CurrentPlayers { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/LoginCredentials.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LoginCredentials
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/PasswordChange.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models
{
    [DataContract]
    public class PasswordChange
    {
        [DataMember] 
        public string Username { get; set; } = string.Empty;
        
        [DataMember] 
        public string CurrentPassword { get; set; } = string.Empty;
        
        [DataMember] 
        public string NewPassword { get; set; } = string.Empty;
    }
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerPair.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerPair
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public Guid OtherPlayerId { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerRelation.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum PlayerRelation
{
    [EnumMember]
    None,

    [EnumMember]
    RequestSent,

    [EnumMember]
    Friends,

    [EnumMember]
    Blocked
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerSearch.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerSearch
{
    [DataMember]
    public Guid RequesterId { get; set; }

    [DataMember]
    public string SearchTerm { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/PlayerSearchEntry.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerSearchEntry
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public PlayerRelation Relation { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Models/ServiceFault.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class ServiceFault
{
    [DataMember]
    public string ErrorCode { get; set; } = string.Empty;

    [DataMember]
    public string MessageKey { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/SessionLogout.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class SessionLogout
{
    [DataMember]
    public string SessionToken { get; set; } = string.Empty;
}
```

## File: src/Shared/Vertigo.Contracts/Models/UserSession.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class UserSession
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Token { get; set; } = string.Empty;

    [DataMember]
    public DateTime ExpiresAt { get; set; }

    [DataMember]
    public bool IsGuest { get; set; }
}
```

## File: src/Shared/Vertigo.Contracts/Vertigo.Contracts.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.ServiceModel.Primitives" Version="10.1.0" />
  </ItemGroup>

</Project>
```

## File: tests/Vertigo.Client.LobbyPreparation.Smoke/Program.cs
```csharp
using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

using ServiceProvider first = CreateClient();
using ServiceProvider second = CreateClient();
using ILobbyClient hostLobby = first.GetRequiredService<ILobbyClient>();
using ILobbyClient guestLobby = second.GetRequiredService<ILobbyClient>();
using ManualResetEventSlim hostReadyEvent = new(false);
using ManualResetEventSlim guestStartedEvent = new(false);
object matchSync = new();
Guid callbackMatchId = Guid.Empty;
long callbackRevision = -1;
using ManualResetEventSlim hostTransferredEvent = new(false);

string? firstCode = null;
string? secondCode = null;
try
{
    string hostToken = CreateGuest(first);
    string guestToken = CreateGuest(second);
    Guid hostId = first.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;
    Guid guestId = second.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;

    hostLobby.PreparationChanged += (_, preparation) =>
    {
        if (preparation.LobbyCode == firstCode && preparation.ReadyPlayerIds.Contains(guestId))
        {
            hostReadyEvent.Set();
        }
    };
    guestLobby.PreparationChanged += (_, preparation) =>
    {
        if (preparation.LobbyCode == firstCode && preparation.IsInGame)
        {
            lock (matchSync)
            {
                callbackMatchId = preparation.MatchId;
                callbackRevision = preparation.Revision;
            }
            guestStartedEvent.Set();
        }
        if (preparation.LobbyCode == secondCode && preparation.HostPlayerId == guestId)
        {
            hostTransferredEvent.Set();
        }
    };

    firstCode = hostLobby.CreateLobby(true, 2, hostToken);
    guestLobby.JoinLobby(firstCode, guestToken);
    bool notReadyDenied = false;
    try
    {
        hostLobby.StartMatch(firstCode, hostToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        notReadyDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyNotReady;
    }
    Require(notReadyDenied, "Host could start without all players being ready");
    Console.WriteLine("PASS: NOT-READY REJECTED");

    LobbyPreparation firstReady = hostLobby.SetReady(firstCode, true, hostToken);
    Require(firstReady.ReadyPlayerIds.Contains(hostId), "Host ready flag missing");
    LobbyPreparation secondReady = guestLobby.SetReady(firstCode, true, guestToken);
    Require(secondReady.ReadyPlayerIds.Contains(guestId), "Guest ready flag missing");
    Require(secondReady.Revision > firstReady.Revision, "Ready revisions did not advance");
    Require(hostReadyEvent.Wait(TimeSpan.FromSeconds(10)), "Host missed Ready callback");
    Console.WriteLine("PASS: READY CALLBACK");

    bool guestDenied = false;
    try
    {
        guestLobby.StartMatch(firstCode, guestToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        guestDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyNotHost;
    }
    Require(guestDenied, "Non-host unexpectedly started match");
    Console.WriteLine("PASS: NON-HOST REJECTED");

    LobbyPreparation started = hostLobby.StartMatch(firstCode, hostToken);
    Require(started.IsInGame, "StartMatch did not change the lobby state");
    Require(guestStartedEvent.Wait(TimeSpan.FromSeconds(10)), "Guest missed match-start callback");
    Require(started.MatchId != Guid.Empty, "StartMatch returned an empty MatchId");
    LobbyPreparation queried = guestLobby.GetPreparation(firstCode, guestToken);
    Require(queried.MatchId == started.MatchId, "RPC GetPreparation MatchId differs from StartMatch");
    Require(queried.Revision == started.Revision, "RPC revisions differ after match start");
    lock (matchSync)
    {
        Require(callbackMatchId == started.MatchId, "Callback MatchId differs from RPC results");
        Require(callbackRevision == started.Revision, "Callback revision differs from RPC results");
    }
    Console.WriteLine("PASS: MATCH ID CONSISTENT ACROSS RPC AND CALLBACK");

    bool repeatStartDenied = false;
    try
    {
        hostLobby.StartMatch(firstCode, hostToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        repeatStartDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyAlreadyInGame;
    }
    Require(repeatStartDenied, "Second StartMatch was not rejected as AlreadyInGame");
    LobbyPreparation afterRejectedStart = hostLobby.GetPreparation(firstCode, hostToken);
    Require(afterRejectedStart.MatchId == started.MatchId, "Rejected start changed MatchId");
    Require(afterRejectedStart.Revision == started.Revision, "Rejected start changed revision");
    Console.WriteLine("PASS: SECOND START REJECTED WITHOUT CHANGING MATCH");
    bool readyAfterStartDenied = false;
    try
    {
        guestLobby.SetReady(firstCode, false, guestToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        readyAfterStartDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyAlreadyInGame;
    }
    Require(readyAfterStartDenied, "SetReady succeeded after match start");
    Console.WriteLine("PASS: READY AFTER START REJECTED");
    Console.WriteLine("PASS: START CALLBACK");

    Require(guestLobby.LeaveLobby(firstCode, guestToken), "Guest could not leave first lobby");
    Require(hostLobby.LeaveLobby(firstCode, hostToken), "Host could not leave first lobby");

    secondCode = hostLobby.CreateLobby(true, 2, hostToken);
    hostLobby.SetReady(secondCode, true, hostToken);
    guestLobby.JoinLobby(secondCode, guestToken);
    LobbyPreparation afterJoining = guestLobby.GetPreparation(secondCode, guestToken);
    Require(afterJoining.ReadyPlayerIds.Count == 0, "New arrival did not reset Ready flags");
    Console.WriteLine("PASS: NEW PLAYER RESETS READY");
    Require(hostLobby.LeaveLobby(secondCode, hostToken), "Host could not leave second lobby");
    Require(hostTransferredEvent.Wait(TimeSpan.FromSeconds(10)), "Guest missed host-transfer callback");
    LobbyPreparation afterTransfer = guestLobby.GetPreparation(secondCode, guestToken);
    Require(afterTransfer.HostPlayerId == guestId, "Host was not reassigned");
    Console.WriteLine("PASS: HOST TRANSFER CALLBACK");
    Require(guestLobby.LeaveLobby(secondCode, guestToken), "New host could not leave");
    Console.WriteLine("PASS: Lobby preparation smoke");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {exception.Message}");
    return 1;
}
finally
{
    guestLobby.Disconnect();
    hostLobby.Disconnect();
    LogoutQuietly(first);
    LogoutQuietly(second);
}

static ServiceProvider CreateClient()
{
    ServiceCollection registrations = new();
    registrations.AddVertigoClientNetworking();
    return registrations.BuildServiceProvider();
}

static string CreateGuest(ServiceProvider provider)
{
    var result = provider.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Could not authenticate guest");
    }
    return result.Value.Token;
}

static void LogoutQuietly(ServiceProvider provider)
{
    try
    {
        provider.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (CommunicationException)
    {
        // Cleanup is best-effort when the server has disconnected.
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
```

## File: tests/Vertigo.Client.LobbyPreparation.Smoke/Vertigo.Client.LobbyPreparation.Smoke.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <ProjectReference Include="../../src/Client/Vertigo.Client.Core/Vertigo.Client.Core.csproj" />
  </ItemGroup>
</Project>
```

## File: tests/Vertigo.Client.Multiplayer.Smoke/Program.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

// Two independent service providers represent two client applications.
using ServiceProvider first = CreateClient();
using ServiceProvider second = CreateClient();
using ILobbyClient hostLobby = first.GetRequiredService<ILobbyClient>();
using ILobbyClient guestLobby = second.GetRequiredService<ILobbyClient>();
using ManualResetEventSlim joinedEvent = new(false);
using ManualResetEventSlim chatEvent = new(false);
using ManualResetEventSlim leftEvent = new(false);

string? code = null;
string? hostToken = null;
string? guestToken = null;
Guid guestId = Guid.Empty;

try
{
    hostToken = CreateGuestToken(first);
    guestToken = CreateGuestToken(second);
    guestId = second.GetRequiredService<Vertigo.Client.Core.Sessions.PlayerSession>()
        .GetActiveSession()!.PlayerId;

    hostLobby.PlayerJoined += (_, player) =>
    {
        if (player.PlayerId == guestId)
        {
            joinedEvent.Set();
        }
    };
    hostLobby.PlayerLeft += (_, playerId) =>
    {
        if (playerId == guestId)
        {
            leftEvent.Set();
        }
    };
    hostLobby.MessageReceived += (_, message) =>
    {
        if (message.SenderId == guestId && message.Text == "Vertigo multiplayer smoke")
        {
            chatEvent.Set();
        }
    };

    code = hostLobby.CreateLobby(true, 2, hostToken);
    LobbyAdmission admission = guestLobby.JoinLobby(code, guestToken);
    Require(admission.LobbyCode == code, "Joined lobby code mismatch");
    Require(joinedEvent.Wait(TimeSpan.FromSeconds(10)), "Host did not receive PlayerJoined");
    Console.WriteLine("Two clients: JOIN OK");

    var sent = second.GetRequiredService<IChatClient>()
        .SendMessage(code, "Vertigo multiplayer smoke", guestToken);
    Require(sent.IsSuccess && sent.Value, "Chat request failed");
    Require(chatEvent.Wait(TimeSpan.FromSeconds(10)), "Host did not receive chat callback");
    Console.WriteLine("Two clients: CHAT CALLBACK OK");

    Require(guestLobby.LeaveLobby(code, guestToken), "Guest could not leave");
    Require(leftEvent.Wait(TimeSpan.FromSeconds(10)), "Host did not receive PlayerLeft");
    Console.WriteLine("Two clients: LEAVE CALLBACK OK");

    Require(hostLobby.LeaveLobby(code, hostToken), "Host could not leave");
    Console.WriteLine("PASS: Multiplayer smoke");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {exception.Message}");
    return 1;
}
finally
{
    guestLobby.Disconnect();
    hostLobby.Disconnect();
    LogoutQuietly(first);
    LogoutQuietly(second);
}

static ServiceProvider CreateClient()
{
    ServiceCollection registrations = new();
    registrations.AddVertigoClientNetworking();
    return registrations.BuildServiceProvider();
}

static string CreateGuestToken(ServiceProvider provider)
{
    var result = provider.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Could not create guest session");
    }
    return result.Value.Token;
}

static void LogoutQuietly(ServiceProvider provider)
{
    try
    {
        provider.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (Exception)
    {
        // Best-effort cleanup after a failed smoke test.
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
```

## File: tests/Vertigo.Client.Multiplayer.Smoke/Vertigo.Client.Multiplayer.Smoke.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
  </ItemGroup>
</Project>
```

## File: tests/Vertigo.Client.Networking.Smoke/Program.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

ServiceCollection registrations = new();
registrations.AddVertigoClientNetworking();
using ServiceProvider services = registrations.BuildServiceProvider();
ISessionCoordinator authentication = services.GetRequiredService<ISessionCoordinator>();
PlayerSession session = services.GetRequiredService<PlayerSession>();
INavigator navigation = services.GetRequiredService<INavigator>();

string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "guest";
ClientResult<UserSession> login;
if (mode == "login" && args.Length == 3)
{
    login = authentication.Login(args[1], args[2]);
}
else if (mode == "guest")
{
    login = authentication.CreateGuestSession();
}
else
{
    Console.Error.WriteLine("Usage: dotnet run -- guest OR dotnet run -- login USERNAME PASSWORD");
    return 2;
}

if (!login.IsSuccess || login.Value is null)
{
    Console.Error.WriteLine($"Login failed: {login.Failure}, {login.ErrorCode}");
    return 1;
}

SessionSnapshot? active = session.GetActiveSession();
if (active is null)
{
    Console.Error.WriteLine("The session was not stored locally.");
    return 1;
}

Console.WriteLine($"Authenticated: {active.Username} (Guest={active.IsGuest})");
Console.WriteLine("Token received: YES (value not printed)");
navigation.NavigateTo(NavigationPage.Home);
Console.WriteLine($"Navigation: {navigation.CurrentPage}");

using ILobbyClient lobby = services.GetRequiredService<ILobbyClient>();
lobby.PlayerJoined += (_, member) => Console.WriteLine($"Joined: {member.Username}");
lobby.MessageReceived += (_, message) => Console.WriteLine($"Chat: {message.SenderUsername}: {message.Text}");

int exitCode = 0;
try
{
    string lobbyCode = lobby.CreateLobby(true, 2, active.Token);
    Console.WriteLine($"Lobby created: {lobbyCode}");
    navigation.NavigateTo(NavigationPage.Lobby);
    IChatClient chat = services.GetRequiredService<IChatClient>();
    ClientResult<bool> sent = chat.SendMessage(lobbyCode, "Hello from Vertigo smoke test", active.Token);
    Console.WriteLine($"Chat sent: {sent.IsSuccess && sent.Value} ({sent.Failure})");
    bool left = lobby.LeaveLobby(lobbyCode, active.Token);
    Console.WriteLine($"Lobby left: {left}");
    if (!sent.IsSuccess || !sent.Value || !left)
    {
        exitCode = 1;
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Lobby transport/business failure: {exception.GetType().Name}: {exception.Message}");
    exitCode = 1;
}
finally
{
    ClientResult<bool> logout = authentication.Logout();
    bool isLoggedOut = logout.IsSuccess && logout.Value && !session.IsAuthenticated;
    Console.WriteLine($"Logout: {isLoggedOut} ({logout.Failure})");
    if (!isLoggedOut)
    {
        exitCode = 1;
    }
}

return exitCode;
```

## File: tests/Vertigo.Client.Networking.Smoke/Vertigo.Client.Networking.Smoke.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
  </ItemGroup>
</Project>
```

## File: tests/Vertigo.Client.PrivateInvitations.Smoke/Program.cs
```csharp
using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

const string HostUsernameVariable = "VERTIGO_TEST_HOST_USER";
const string HostPasswordVariable = "VERTIGO_TEST_HOST_PASSWORD";
const string GuestUsernameVariable = "VERTIGO_TEST_INVITEE_USER";
const string GuestPasswordVariable = "VERTIGO_TEST_INVITEE_PASSWORD";

string? hostUsername = Environment.GetEnvironmentVariable(HostUsernameVariable);
string? hostPassword = Environment.GetEnvironmentVariable(HostPasswordVariable);
string? inviteeUsername = Environment.GetEnvironmentVariable(GuestUsernameVariable);
string? inviteePassword = Environment.GetEnvironmentVariable(GuestPasswordVariable);

if (string.IsNullOrWhiteSpace(hostUsername) || string.IsNullOrWhiteSpace(hostPassword)
    || string.IsNullOrWhiteSpace(inviteeUsername) || string.IsNullOrWhiteSpace(inviteePassword))
{
    Console.Error.WriteLine("CONFIGURATION REQUIRED: set the four VERTIGO_TEST_* environment variables in Rider or your shell.");
    return 2;
}

using ServiceProvider host = CreateClient();
using ServiceProvider invitee = CreateClient();
ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
ILobbyClient inviteeLobby = invitee.GetRequiredService<ILobbyClient>();
IFriendshipNotificationClient inviteeNotifications = invitee.GetRequiredService<IFriendshipNotificationClient>();
string? hostToken = null;
string? inviteeToken = null;
string? liveLobbyCode = null;
string? closedLobbyCode = null;

try
{
    UserSession hostSession = Login(host, hostUsername, hostPassword);
    UserSession inviteeSession = Login(invitee, inviteeUsername, inviteePassword);
    Require(hostSession.PlayerId != inviteeSession.PlayerId, "Use two different registered accounts.");
    hostToken = hostSession.Token;
    inviteeToken = inviteeSession.Token;

    EnsureFriendship(host, invitee, hostSession, inviteeSession);
    Require(inviteeNotifications.Subscribe(inviteeToken), "Invitee notification subscription was rejected.");
    Console.WriteLine("Registered accounts: AUTHENTICATED, FRIENDS, INVITEE ONLINE");

    // Positive scenario: the server must authorize admission to a PRIVATE room.
    liveLobbyCode = hostLobby.CreateLobby(false, 2, hostToken);
    SendInvitation(host, liveLobbyCode, inviteeSession.PlayerId, hostToken);
    Require(HasInvitation(invitee, liveLobbyCode, inviteeToken), "Valid invitation not visible to invitee.");
    LobbyAdmission admission = inviteeLobby.JoinLobby(liveLobbyCode, inviteeToken);
    Require(string.Equals(admission.LobbyCode, liveLobbyCode, StringComparison.OrdinalIgnoreCase),
        "Private lobby admission did not return the expected code.");
    Require(!HasInvitation(invitee, liveLobbyCode, inviteeToken),
        "Accepted invitation was still pending.");
    Console.WriteLine("PASS: valid private invitation accepted and consumed by server");

    Require(inviteeLobby.LeaveLobby(liveLobbyCode, inviteeToken), "Invitee failed to leave private room.");
    Require(hostLobby.LeaveLobby(liveLobbyCode, hostToken), "Host failed to leave private room.");
    liveLobbyCode = null;

    // Negative scenario: an already issued invitation must not admit the player
    // after the host closes the room, even if a stale UI item remains visible.
    closedLobbyCode = hostLobby.CreateLobby(false, 2, hostToken);
    SendInvitation(host, closedLobbyCode, inviteeSession.PlayerId, hostToken);
    Require(HasInvitation(invitee, closedLobbyCode, inviteeToken), "Second invitation was not created.");
    Require(hostLobby.LeaveLobby(closedLobbyCode, hostToken), "Host did not close second room.");

    bool rejected = false;
    try
    {
        inviteeLobby.JoinLobby(closedLobbyCode, inviteeToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        Require(fault.Detail.ErrorCode == ServiceErrorCodes.LobbyNotFound,
            $"Expected LobbyNotFound; received '{fault.Detail.ErrorCode}'.");
        string playerMessage = new ClientErrorText(new TextLocalizer())
            .FromException(fault, "Social.InvitationUnavailable");
        Require(!string.IsNullOrWhiteSpace(playerMessage)
            && !string.Equals(playerMessage, fault.Detail.MessageKey, StringComparison.Ordinal),
            "The server error was not converted into localized player text.");
        rejected = true;
    }
    Require(rejected, "Server admitted the invitee to a closed private room.");
    Console.WriteLine("PASS: closed private room rejected admission with localized error");
    Console.WriteLine("PASS: Private invitations smoke");
    return 0;
}
catch (Exception exception)
{
    // Never print session tokens or passwords.
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {SafeError(exception)}");
    return 1;
}
finally
{
    inviteeNotifications.Disconnect();
    inviteeLobby.Disconnect();
    hostLobby.Disconnect();
    LogoutQuietly(host);
    LogoutQuietly(invitee);
}

static string SafeError(Exception exception)
{
    // Avoid copying transport exceptions that could contain sensitive request details.
    return exception is FaultException<ServiceFault> fault
        ? $"Server business error {fault.Detail.ErrorCode}"
        : exception is CommunicationException or TimeoutException
            ? "CoreWCF connection or timeout failure"
            : exception.Message;
}

static ServiceProvider CreateClient()
{
    ServiceCollection services = new();
    services.AddVertigoClientNetworking();
    return services.BuildServiceProvider();
}

static UserSession Login(ServiceProvider provider, string username, string password)
{
    ClientResult<UserSession> result = provider.GetRequiredService<ISessionCoordinator>()
        .Login(username, password);
    Require(result.IsSuccess && result.Value is not null && !result.Value.IsGuest,
        $"Could not authenticate registered test account '{username}'. Failure: {result.Failure} ({result.ErrorCode}).");
    return result.Value!;
}

static void EnsureFriendship(ServiceProvider host, ServiceProvider invitee, UserSession owner, UserSession other)
{
    IFriendshipClient ownerFriends = host.GetRequiredService<IFriendshipClient>();
    ClientResult<List<FriendEntry>> friends = ownerFriends.GetFriends(owner.Token);
    Require(friends.IsSuccess && friends.Value is not null, "Could not read host friendships.");
    bool areFriends = friends.Value!.Any(friend => friend.PlayerId == other.PlayerId);
    if (!areFriends)
    {
        ClientResult<PlayerRelation> request = ownerFriends.SendFriendRequest(other.PlayerId, owner.Token);
        Require(request.IsSuccess, $"Could not send test friend request ({request.ErrorCode}).");
        if (request.Value != PlayerRelation.Friends)
        {
            IFriendshipClient inviteeFriends = invitee.GetRequiredService<IFriendshipClient>();
            ClientResult<bool> response = inviteeFriends.RespondToFriendship(owner.PlayerId, true, other.Token);
            Require(response.IsSuccess && response.Value, $"Could not accept test friend request ({response.ErrorCode}).");
        }
    }
    friends = ownerFriends.GetFriends(owner.Token);
    Require(friends.IsSuccess && friends.Value?.Any(friend => friend.PlayerId == other.PlayerId) == true,
        "Test accounts are not friends.");
}

static void SendInvitation(ServiceProvider host, string code, Guid inviteeId, string hostToken)
{
    ILobbyInvitationClient invitations = host.GetRequiredService<ILobbyInvitationClient>();
    ClientResult<List<Guid>> result = invitations.SendLobbyInvitations(code, new List<Guid> { inviteeId }, hostToken);
    Require(result.IsSuccess && result.Value?.Contains(inviteeId) == true,
        $"Invitation was not sent. Failure={result.Failure}; ErrorCode={result.ErrorCode}. Check invitee subscription.");
}

static bool HasInvitation(ServiceProvider invitee, string code, string inviteeToken)
{
    ClientResult<List<LobbyInvitationEntry>> result = invitee.GetRequiredService<ILobbyInvitationClient>()
        .GetPendingLobbyInvitations(inviteeToken);
    Require(result.IsSuccess && result.Value is not null, "Could not query pending invitations.");
    return result.Value!.Any(entry => string.Equals(entry.LobbyCode, code, StringComparison.OrdinalIgnoreCase));
}

static void LogoutQuietly(ServiceProvider provider)
{
    try
    {
        provider.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (CommunicationException)
    {
        // Connection may already be gone during best-effort cleanup.
    }
    catch (TimeoutException)
    {
        // Connection may already be gone during best-effort cleanup.
    }
}

static void Require(bool condition, string reason)
{
    if (!condition)
    {
        throw new InvalidOperationException(reason);
    }
}
```

## File: tests/Vertigo.Client.PrivateInvitations.Smoke/Vertigo.Client.PrivateInvitations.Smoke.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <ProjectReference Include="../../src/Client/Vertigo.Client.UI/Vertigo.Client.UI.csproj" />
  </ItemGroup>
</Project>
```

## File: tests/Vertigo.Client.Reconnection.Smoke/Program.cs
```csharp
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

// Integration smoke: requires a running Vertigo.Server.Host with Fase 7B + Codex fixes.
// Each ServiceProvider has its own WCF duplex client and its own guest identity.
using ServiceProvider host = CreateClient();
using ServiceProvider guest = CreateClient();
using ServiceProvider outsider = CreateClient();
using ServiceProvider hostReconnect = CreateClient();
using ServiceProvider guestReconnect = CreateClient();
using ServiceProvider suspensionObserver = CreateClient();

ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
ILobbyClient guestLobby = guest.GetRequiredService<ILobbyClient>();
ILobbyClient outsiderLobby = outsider.GetRequiredService<ILobbyClient>();
ILobbyClient resumedHostLobby = hostReconnect.GetRequiredService<ILobbyClient>();
ILobbyClient resumedGuestLobby = guestReconnect.GetRequiredService<ILobbyClient>();
ILobbyClient observerLobby = suspensionObserver.GetRequiredService<ILobbyClient>();
string? code = null;

try
{
    string hostToken = CreateGuest(host);
    string guestToken = CreateGuest(guest);
    string outsiderToken = CreateGuest(outsider);
    Guid hostId = host.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;
    Guid guestId = guest.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;

    code = hostLobby.CreateLobby(true, 2, hostToken);
    LobbyCredentials hostCredentials = new(code, hostToken);
    guestLobby.JoinLobby(code, guestToken);
    hostLobby.SetReady(code, true, hostToken);
    guestLobby.SetReady(code, true, guestToken);
    LobbyPreparation started = hostLobby.StartMatch(code, hostToken);
    Require(started.IsInGame && started.MatchId != Guid.Empty, "Match was not initialized");
    Guid matchId = started.MatchId;
    long revision = started.Revision;
    Console.WriteLine("PASS: MATCH STARTED WITH STABLE ID");

    // An unrelated account cannot enter an in-progress match.
    RequireFault(() => outsiderLobby.JoinLobby(code, outsiderToken), ServiceErrorCodes.LobbyAlreadyStarted);
    Console.WriteLine("PASS: THIRD PLAYER REJECTED");

    // Voluntary exit is not yet supported in active games; neither participant should be removed.
    Require(!guestLobby.LeaveLobby(code, guestToken), "LeaveLobby was accepted during a match");
    Require(guestLobby.GetPreparation(code, guestToken).MatchId == matchId,
        "Voluntary exit changed the match identifier");
    Console.WriteLine("PASS: IN-GAME LEAVE REJECTED");

    // Drop one duplex channel; keep the other online to observe the server-side change.
    guestLobby.Disconnect();
    LobbyPreparation afterGuestLoss = AwaitPreparation(hostLobby, hostCredentials,
        state => state.DisconnectedPlayerIds.Contains(guestId));
    Require(!afterGuestLoss.IsSuspended, "Game suspended with host still connected");
    Require(afterGuestLoss.MatchId == matchId && afterGuestLoss.Revision > revision,
        "Guest disconnect changed MatchId or failed to advance revision");
    revision = afterGuestLoss.Revision;
    Console.WriteLine("PASS: SINGLE DISCONNECT KEEPS MATCH RUNNING");

    // The same identity reclaims its seat through a NEW duplex channel, without a new login.
    resumedGuestLobby.JoinLobby(code, guestToken);
    LobbyPreparation afterGuestReturn = resumedGuestLobby.GetPreparation(code, guestToken);
    Require(!afterGuestReturn.DisconnectedPlayerIds.Contains(guestId), "Guest still marked disconnected");
    Require(afterGuestReturn.MatchId == matchId && afterGuestReturn.Revision > revision,
        "Guest reconnect changed MatchId or failed to advance revision");
    revision = afterGuestReturn.Revision;
    Console.WriteLine("PASS: NEW GUEST CHANNEL RECONNECTS");

    // Replace a channel while the old one remains alive. Its late Closed callback must be ignored.
    resumedHostLobby.JoinLobby(code, hostToken);
    LobbyPreparation replaced = resumedHostLobby.GetPreparation(code, hostToken);
    Require(replaced.Revision > revision, "Host replacement did not advance revision");
    revision = replaced.Revision;
    hostLobby.Disconnect();
    // Allow the server to process Closed from the superseded channel, then force an RPC barrier.
    Thread.Sleep(SmokeTiming.StaleChannelCloseGrace);
    LobbyPreparation afterOldClose = resumedHostLobby.GetPreparation(code, hostToken);
    Require(!afterOldClose.DisconnectedPlayerIds.Contains(hostId),
        "Closing the old channel disconnected the replacement");
    Require(afterOldClose.MatchId == matchId, "Old channel close changed MatchId");
    Console.WriteLine("PASS: STALE CHANNEL CLOSE IGNORED");

    // Close all live match channels. After both closures, rejoin with another new channel.
    resumedGuestLobby.Disconnect();
    LobbyPreparation afterSecondGuestLoss = AwaitPreparation(resumedHostLobby, hostCredentials,
        state => state.DisconnectedPlayerIds.Contains(guestId));
    Require(!afterSecondGuestLoss.IsSuspended, "Game suspended with host connected");
    revision = afterSecondGuestLoss.Revision;
    resumedHostLobby.Disconnect();

    // A fresh, independent channel observes Game's suspended state without rejoining.
    // Poll rather than sleeping: the server may process Closed callbacks asynchronously.
    LobbyPreparation suspended = AwaitPreparation(observerLobby, hostCredentials,
        state => state.IsSuspended
            && state.DisconnectedPlayerIds.Contains(hostId)
            && state.DisconnectedPlayerIds.Contains(guestId));
    Require(suspended.IsInGame && suspended.MatchId == matchId,
        "Suspension must preserve the active match identifier");
    Require(suspended.Revision > revision,
        "Suspension did not advance the lobby revision");
    revision = suspended.Revision;
    Console.WriteLine("PASS: BOTH DISCONNECTED AND MATCH SUSPENDED");

    hostLobby.JoinLobby(code, hostToken); // Disconnected LobbyClient creates a fresh duplex channel.
    LobbyPreparation afterAllReturn = hostLobby.GetPreparation(code, hostToken);
    Require(afterAllReturn.MatchId == matchId, "Reconnection created a different match");
    Require(afterAllReturn.IsInGame && !afterAllReturn.IsSuspended,
        "Game did not resume Running after a participant returned");
    Require(afterAllReturn.DisconnectedPlayerIds.Contains(guestId),
        "Guest was unexpectedly reconnected when only host returned");
    Require(afterAllReturn.Revision > revision, "All-disconnected reconnect did not advance revision");
    Console.WriteLine("PASS: MATCH RECOVERED AFTER BOTH CHANNELS CLOSED");

    resumedGuestLobby.JoinLobby(code, guestToken);
    LobbyPreparation restored = resumedGuestLobby.GetPreparation(code, guestToken);
    Require(restored.DisconnectedPlayerIds.Count == 0, "Participants were not fully restored");
    Require(restored.MatchId == matchId && !restored.IsSuspended, "Final restoration changed match state");
    Console.WriteLine("PASS: BOTH PARTICIPANTS RESTORED");
    Console.WriteLine("PASS: Reconnection smoke");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {exception.Message}");
    return 1;
}
finally
{
    // Match leave is intentionally rejected during InGame: close transports, not membership.
    hostLobby.Disconnect();
    guestLobby.Disconnect();
    outsiderLobby.Disconnect();
    resumedHostLobby.Disconnect();
    resumedGuestLobby.Disconnect();
    observerLobby.Disconnect();
    LogoutQuietly(host);
    LogoutQuietly(guest);
    LogoutQuietly(outsider);
}

static ServiceProvider CreateClient()
{
    ServiceCollection services = new();
    services.AddVertigoClientNetworking();
    return services.BuildServiceProvider();
}

static string CreateGuest(ServiceProvider services)
{
    var result = services.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Guest authentication failed");
    }

    return result.Value.Token;
}

static LobbyPreparation AwaitPreparation(
    ILobbyClient lobby, LobbyCredentials credentials, Func<LobbyPreparation, bool> predicate)
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    LobbyPreparation state = lobby.GetPreparation(credentials.Code, credentials.Token);
    while (!predicate(state) && stopwatch.Elapsed < SmokeTiming.ObservationTimeout)
    {
        Thread.Sleep(SmokeTiming.PollInterval);
        state = lobby.GetPreparation(credentials.Code, credentials.Token);
    }

    Require(predicate(state),
        $"Timed out after {SmokeTiming.ObservationTimeout.TotalSeconds}s waiting for expected NetTCP state");
    return state;
}

static void RequireFault(Action operation, string code)
{
    bool rejected = false;
    try
    {
        operation();
    }
    catch (FaultException<ServiceFault> fault)
    {
        rejected = fault.Detail.ErrorCode == code;
    }

    Require(rejected, $"Expected business fault {code}");
}

static void LogoutQuietly(ServiceProvider services)
{
    try
    {
        services.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (CommunicationException)
    {
        // Best-effort cleanup after an intentional duplex disconnect.
    }
    catch (TimeoutException)
    {
        // Best-effort cleanup after an intentional duplex disconnect.
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

internal sealed record LobbyCredentials(string Code, string Token);

internal static class SmokeTiming
{
    internal static readonly TimeSpan ObservationTimeout = TimeSpan.FromSeconds(10);
    internal static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);
    internal static readonly TimeSpan StaleChannelCloseGrace = TimeSpan.FromMilliseconds(500);
}
```

## File: tests/Vertigo.Client.Reconnection.Smoke/Vertigo.Client.Reconnection.Smoke.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <ProjectReference Include="../../src/Client/Vertigo.Client.Core/Vertigo.Client.Core.csproj" />
  </ItemGroup>
</Project>
```

## File: tests/Vertigo.Client.TestAccounts.Setup/Program.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

const string HostUsernameKey = "VERTIGO_TEST_HOST_USER";
const string HostPasswordKey = "VERTIGO_TEST_HOST_PASSWORD";
const string InviteeUsernameKey = "VERTIGO_TEST_INVITEE_USER";
const string InviteePasswordKey = "VERTIGO_TEST_INVITEE_PASSWORD";

string? hostName = Environment.GetEnvironmentVariable(HostUsernameKey);
string? hostPassword = Environment.GetEnvironmentVariable(HostPasswordKey);
string? inviteeName = Environment.GetEnvironmentVariable(InviteeUsernameKey);
string? inviteePassword = Environment.GetEnvironmentVariable(InviteePasswordKey);

if (string.IsNullOrWhiteSpace(hostName) || string.IsNullOrWhiteSpace(hostPassword)
    || string.IsNullOrWhiteSpace(inviteeName) || string.IsNullOrWhiteSpace(inviteePassword))
{
    Console.Error.WriteLine("CONFIGURATION REQUIRED: set the four VERTIGO_TEST_* variables.");
    return 2;
}

if (string.Equals(hostName, inviteeName, StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine("FAIL: Test accounts must have different usernames.");
    return 2;
}

ServiceCollection registrations = new();
registrations.AddVertigoClientNetworking();
using ServiceProvider provider = registrations.BuildServiceProvider();
IAccountClient accounts = provider.GetRequiredService<IAccountClient>();
IAuthClient authentication = provider.GetRequiredService<IAuthClient>();

bool hostReady = PrepareAccount(accounts, authentication, hostName, hostPassword, "host");
bool inviteeReady = PrepareAccount(accounts, authentication, inviteeName, inviteePassword, "invitee");

if (!hostReady || !inviteeReady)
{
    Console.Error.WriteLine("FAIL: Registered test accounts are not ready.");
    return 1;
}

Console.WriteLine("PASS: Both registered test accounts are ready.");
return 0;

static bool PrepareAccount(IAccountClient accounts, IAuthClient authentication,
    string username, string password, string label)
{
    // The test identity never needs incoming email; use a unique, reserved example address.
    string email = $"vertigo-test-{Guid.NewGuid():N}@example.com";
    AccountRegistration registration = new()
    {
        Username = username,
        Email = email,
        Password = password
    };

    ClientResult<bool> register = accounts.RegisterAccount(registration);
    if (!register.IsSuccess && register.ErrorCode != ServiceErrorCodes.UsernameAlreadyRegistered)
    {
        Console.Error.WriteLine($"FAIL: {label} registration: {register.Failure} ({register.ErrorCode}).");
        return false;
    }

    ClientResult<UserSession> login = authentication.Login(new LoginCredentials
    {
        Username = username,
        Password = password
    });

    if (!login.IsSuccess || login.Value is null || login.Value.IsGuest)
    {
        Console.Error.WriteLine($"FAIL: {label} authentication: {login.Failure} ({login.ErrorCode}).");
        return false;
    }

    ClientResult<bool> logout = authentication.Logout(login.Value.Token);
    if (!logout.IsSuccess || !logout.Value)
    {
        Console.Error.WriteLine($"FAIL: {label} logout: {logout.Failure} ({logout.ErrorCode}).");
        return false;
    }

    Console.WriteLine($"PASS: {label} registered account verified.");
    return true;
}
```

## File: tests/Vertigo.Client.TestAccounts.Setup/Vertigo.Client.TestAccounts.Setup.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
  </ItemGroup>
</Project>
```

## File: tests/Vertigo.Client.UnitTests/Errors/ClientErrorTextTests.cs
```csharp
using System.ServiceModel;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Errors;

public sealed class ClientErrorTextTests
{
    [Fact]
    public void FromResult_Timeout_ReturnsLocalizedMessage()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ClientResult<bool> failed = ClientResult<bool>.TechnicalFailure(FailureKind.Timeout);

        string message = errors.FromResult(failed, "Error.Unknown");

        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.NotEqual("Error.Timeout", message);
    }

    [Fact]
    public void IsUnauthorized_BusinessFailure_ReturnsTrue()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ClientResult<bool> failed = ClientResult<bool>.BusinessFailure(
            ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);

        Assert.True(errors.IsUnauthorized(failed));
    }

    [Fact]
    public void IsUnauthorized_OtherFailure_ReturnsFalse()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ClientResult<bool> failed = ClientResult<bool>.BusinessFailure(
            ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);

        Assert.False(errors.IsUnauthorized(failed));
    }

    [Fact]
    public void FromException_ServerFault_ReturnsLocalizedMessage()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ServiceFault fault = new()
        {
            ErrorCode = ServiceErrorCodes.LobbyNotFound,
            MessageKey = ServiceMessageKeys.LobbyNotFound
        };
        FaultException<ServiceFault> exception = new(fault);

        string message = errors.FromException(exception, "Error.Unknown");

        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.NotEqual(fault.MessageKey, message);
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Lobbies/LobbyPreparationRevisionTests.cs
```csharp
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Lobbies;

public sealed class LobbyPreparationRevisionTests
{
    [Fact]
    public void TryAdvance_OutOfOrderUpdates_KeepsNewest()
    {
        LobbyPreparationRevision tracker = new();
        LobbyPreparation newer = new() { LobbyCode = "ABCDEF", Revision = 12 };
        LobbyPreparation older = new() { LobbyCode = "ABCDEF", Revision = 11 };

        Assert.True(tracker.TryAdvance(newer, "ABCDEF"));
        Assert.False(tracker.TryAdvance(older, "ABCDEF"));
        Assert.False(tracker.TryAdvance(newer, "ABCDEF"));
    }

    [Fact]
    public void TryAdvance_AfterLeavingRoom_DiscardsQueuedUpdate()
    {
        LobbyPreparationRevision tracker = new();
        LobbyPreparation queued = new() { LobbyCode = "ABCDEF", Revision = 5 };
        tracker.TryAdvance(new LobbyPreparation { LobbyCode = "ABCDEF", Revision = 4 }, "ABCDEF");
        tracker.Reset();

        Assert.False(tracker.TryAdvance(queued, string.Empty));
        Assert.True(tracker.TryAdvance(new LobbyPreparation { LobbyCode = "GHIJKL", Revision = 1 }, "GHIJKL"));
    }

    [Fact]
    public void TryAdvance_AnotherRoom_DoesNotAdvanceRevision()
    {
        LobbyPreparationRevision tracker = new();
        Assert.False(tracker.TryAdvance(new LobbyPreparation { LobbyCode = "OLD", Revision = 50 }, "NEW"));
        Assert.True(tracker.TryAdvance(new LobbyPreparation { LobbyCode = "NEW", Revision = 1 }, "NEW"));
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Localization/AccessLanguageSwitchTests.cs
```csharp
using System.Globalization;
using Vertigo.Client.UI.Utilities.Localization.Services;

namespace Vertigo.Client.UnitTests.Localization;

public sealed class AccessLanguageSwitchTests
{
    [Theory]
    [InlineData("es", "Bienvenidos", "Registrarse")]
    [InlineData("en", "Welcome", "Register")]
    public void Get_AccessLabels_ReturnsExpectedCulture(string cultureName, string title, string register)
    {
        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();
            Assert.Equal(title, localizer.Get("Access.Title"));
            Assert.Equal(register, localizer.Get("Access.Register"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Localization/AccessLocalizationTests.cs
```csharp
using System.Globalization;
using Vertigo.Client.UI.Utilities.Localization.Services;

namespace Vertigo.Client.UnitTests.Localization;

public sealed class AccessLocalizationTests
{
    [Theory]
    [InlineData("es-MX", "Iniciar sesión")]
    [InlineData("en-US", "Sign in")]
    public void Get_AccessLogin_ReturnsLocalizedText(string cultureName, string expected)
    {
        CultureInfo previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();

            string actual = localizer.Get("Access.Login");

            Assert.Equal(expected, actual);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Theory]
    [InlineData("es-MX")]
    [InlineData("en-US")]
    public void Get_AccessKeys_AreNotEmpty(string cultureName)
    {
        CultureInfo previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();
            string[] keys =
            [
                "Access.Title", "Access.Description", "Access.Username",
                "Access.Password", "Access.Login", "Access.Guest",
                "Access.ResumeSection", "Access.Token", "Access.Resume",
                "Access.Progress", "Access.Help", "Access.VisualCaption",
                "Access.SecurityNote"
            ];

            foreach (string key in keys)
            {
                string value = localizer.Get(key);
                Assert.False(string.IsNullOrWhiteSpace(value));
                Assert.NotEqual(localizer.Get("Error.Unknown"), value);
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Localization/HomeLocalizationTests.cs
```csharp
using System.Globalization;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Xunit;

namespace Vertigo.Client.UnitTests.Localization;

public sealed class HomeLocalizationTests
{
    [Theory]
    [InlineData("es-MX")]
    [InlineData("en-US")]
    public void Get_HomeKeys_AreLocalized(string cultureName)
    {
        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();
            string[] keys =
            [
                "Home.BrandSubtitle", "Home.Play", "Home.Social",
                "Home.Ranking", "Home.Settings", "Home.Profile",
                "Home.Notifications", "Home.Logout", "Home.Create",
                "Home.Join", "Home.Glossary", "Home.GuestAccount",
                "Home.RegisteredAccount", "Home.NotificationsHelp"
            ];

            foreach (string key in keys)
            {
                string actual = localizer.Get(key);
                Assert.False(string.IsNullOrWhiteSpace(actual));
                Assert.NotEqual(key, actual);
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Navigation/NavigatorTests.cs
```csharp
using Vertigo.Client.Core.Navigation;

namespace Vertigo.Client.UnitTests.Navigation;

public sealed class NavigatorTests
{
    [Fact]
    public void NavigateTo_DifferentPage_RaisesOneEvent()
    {
        Navigator navigator = new();
        List<NavigationPage> notifications = new();
        navigator.PageChanged += (_, page) => notifications.Add(page);

        navigator.NavigateTo(NavigationPage.Home);

        Assert.Equal(NavigationPage.Home, navigator.CurrentPage);
        Assert.Equal(new[] { NavigationPage.Home }, notifications);
    }

    [Fact]
    public void NavigateTo_SamePage_DoesNotRepeatEvent()
    {
        Navigator navigator = new();
        int notificationCount = 0;
        navigator.PageChanged += (_, _) => notificationCount++;

        navigator.NavigateTo(NavigationPage.Home);
        navigator.NavigateTo(NavigationPage.Home);

        Assert.Equal(1, notificationCount);
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Sessions/PlayerSessionTests.cs
```csharp
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UnitTests.Sessions;

public sealed class PlayerSessionTests
{
    [Fact]
    public void SetSession_ValidSession_AuthenticatesPlayer()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot snapshot = CreateSnapshot("token-a");

        playerSession.SetSession(snapshot);

        Assert.True(playerSession.IsAuthenticated);
        Assert.Same(snapshot, playerSession.GetActiveSession());
    }

    [Fact]
    public void SetSession_ExpiredSession_RejectsInvalidData()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot expired = CreateSnapshot("token-a", DateTime.UtcNow.AddMinutes(-1));

        Assert.Throws<ArgumentException>(() => playerSession.SetSession(expired));
        Assert.False(playerSession.IsAuthenticated);
    }

    [Fact]
    public void Clear_OlderSnapshot_DoesNotCloseNewSession()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot oldSession = CreateSnapshot("token-a");
        SessionSnapshot newSession = CreateSnapshot("token-b");
        playerSession.SetSession(oldSession);
        playerSession.SetSession(newSession);

        playerSession.Clear(oldSession);

        Assert.Same(newSession, playerSession.GetActiveSession());
    }

    [Fact]
    public void Clear_CurrentSnapshot_EndsSession()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot snapshot = CreateSnapshot("token-a");
        playerSession.SetSession(snapshot);

        playerSession.Clear(snapshot);

        Assert.False(playerSession.IsAuthenticated);
    }

    internal static SessionSnapshot CreateSnapshot(string token, DateTime? expiresAt = null)
    {
        return new SessionSnapshot(Guid.NewGuid(), "test_player", token,
            expiresAt ?? DateTime.UtcNow.AddMinutes(30), false);
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Sessions/SessionCoordinatorTests.cs
```csharp
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Sessions;

public sealed class SessionCoordinatorTests
{
    [Fact]
    public void Login_Failed_LeavesPreviousSessionUntouched()
    {
        PlayerSession state = new(TimeProvider.System);
        SessionSnapshot previous = PlayerSessionTests.CreateSnapshot("old");
        state.SetSession(previous);
        FakeAuthClient client = new() { LoginResult = ClientResult<UserSession>.BusinessFailure("INVALID_CREDENTIALS", "ErrorInvalidCredentials") };
        SessionCoordinator coordinator = new(client, state);

        ClientResult<UserSession> result = coordinator.Login("wrong", "wrong");

        Assert.False(result.IsSuccess);
        Assert.Same(previous, state.GetActiveSession());
    }

    [Fact]
    public void Logout_Failed_KeepsSession()
    {
        PlayerSession state = new(TimeProvider.System);
        SessionSnapshot active = PlayerSessionTests.CreateSnapshot("current");
        state.SetSession(active);
        FakeAuthClient client = new() { LogoutResult = ClientResult<bool>.TechnicalFailure(FailureKind.ServerUnreachable) };
        SessionCoordinator coordinator = new(client, state);

        ClientResult<bool> result = coordinator.Logout();

        Assert.False(result.IsSuccess);
        Assert.Same(active, state.GetActiveSession());
    }

    [Fact]
    public async Task Logout_LateSuccess_DoesNotClearReplacementSession()
    {
        PlayerSession state = new(TimeProvider.System);
        SessionSnapshot oldSession = PlayerSessionTests.CreateSnapshot("old");
        SessionSnapshot newSession = PlayerSessionTests.CreateSnapshot("new");
        state.SetSession(oldSession);
        using ManualResetEventSlim started = new(false);
        using ManualResetEventSlim release = new(false);
        FakeAuthClient client = new()
        {
            OnLogout = _ =>
            {
                started.Set();
                if (!release.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new TimeoutException("Test synchronization timed out.");
                }
                return ClientResult<bool>.Success(true);
            }
        };
        SessionCoordinator coordinator = new(client, state);

        Task<ClientResult<bool>> pending = Task.Run(coordinator.Logout);
        Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
        try
        {
            state.SetSession(newSession);
        }
        finally
        {
            release.Set();
        }
        ClientResult<bool> result = await pending;

        Assert.True(result.IsSuccess);
        Assert.Same(newSession, state.GetActiveSession());
    }

    private sealed class FakeAuthClient : IAuthClient
    {
        public ClientResult<UserSession> LoginResult { get; set; } = ClientResult<UserSession>.TechnicalFailure(FailureKind.Unexpected);
        public ClientResult<bool> LogoutResult { get; set; } = ClientResult<bool>.Success(true);
        public Func<string, ClientResult<bool>>? OnLogout { get; set; }

        public ClientResult<UserSession> Login(LoginCredentials request)
        {
            return LoginResult;
        }

        public ClientResult<UserSession> ResumeSession(string sessionToken)
        {
            return LoginResult;
        }

        public ClientResult<UserSession> CreateGuestSession()
        {
            return LoginResult;
        }

        public ClientResult<bool> Logout(string sessionToken)
        {
            return OnLogout?.Invoke(sessionToken) ?? LogoutResult;
        }
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Sessions/SessionGuardTests.cs
```csharp
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Services;
using Vertigo.Client.UnitTests.Sessions;

namespace Vertigo.Client.UnitTests.Sessions;

public sealed class SessionGuardTests
{
    [Fact]
    public void Invalidate_CurrentSession_ClosesChannelsAndNavigatesToAccess()
    {
        PlayerSession sessions = new(TimeProvider.System);
        SessionSnapshot current = PlayerSessionTests.CreateSnapshot("token-1");
        sessions.SetSession(current);
        Navigator navigator = new();
        navigator.NavigateTo(NavigationPage.Home);
        FakeTransportCleaner transport = new();
        SessionGuard guard = new(sessions, navigator, transport);

        guard.Invalidate(current);

        Assert.False(sessions.IsAuthenticated);
        Assert.Equal(NavigationPage.Access, navigator.CurrentPage);
        Assert.Equal(1, transport.DisconnectCount);
    }

    [Fact]
    public void Invalidate_StaleResponse_DoesNotCloseNewSessionOrTransport()
    {
        PlayerSession sessions = new(TimeProvider.System);
        SessionSnapshot previous = PlayerSessionTests.CreateSnapshot("token-1");
        SessionSnapshot current = PlayerSessionTests.CreateSnapshot("token-2");
        sessions.SetSession(previous);
        sessions.SetSession(current);
        Navigator navigator = new();
        navigator.NavigateTo(NavigationPage.Home);
        FakeTransportCleaner transport = new();
        SessionGuard guard = new(sessions, navigator, transport);

        guard.Invalidate(previous);

        Assert.Same(current, sessions.GetActiveSession());
        Assert.Equal(NavigationPage.Home, navigator.CurrentPage);
        Assert.Equal(0, transport.DisconnectCount);
    }

    [Fact]
    public void HasActiveSession_WithoutSession_ReturnsFalseAndNavigatesToAccess()
    {
        PlayerSession sessions = new(TimeProvider.System);
        Navigator navigator = new();
        navigator.NavigateTo(NavigationPage.Home);
        FakeTransportCleaner transport = new();
        SessionGuard guard = new(sessions, navigator, transport);

        bool active = guard.HasActiveSession();

        Assert.False(active);
        Assert.Equal(NavigationPage.Access, navigator.CurrentPage);
        Assert.Equal(1, transport.DisconnectCount);
    }

    private sealed class FakeTransportCleaner : ISessionTransportCleaner
    {
        public int DisconnectCount { get; private set; }

        public void DisconnectAll()
        {
            DisconnectCount++;
        }
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Settings/JsonUserSettingsStoreTests.cs
```csharp
using System;
using System.IO;
using Vertigo.Client.UI.Utilities.Settings.Models;
using Vertigo.Client.UI.Utilities.Settings.Services;
using Xunit;

namespace Vertigo.Client.UnitTests.Settings;

public sealed class JsonUserSettingsStoreTests
{
    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void TrySave_ValidLanguage_Persists(string language)
    {
        string folder = Path.Combine(Path.GetTempPath(), "vertigo-settings-" + Guid.NewGuid().ToString("N"));
        try
        {
            JsonUserSettingsStore store = new(Path.Combine(folder, "settings.json"));
            Assert.True(store.TrySave(new UserSettings { Language = language }));
            Assert.Equal(language, store.Load().Language);
        }
        finally
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }
    }

    [Fact]
    public void Load_InvalidJson_ReturnsSpanish()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{invalid json");
            JsonUserSettingsStore store = new(path);
            Assert.Equal("es", store.Load().Language);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void TrySave_UnsupportedLanguage_DoesNotCreateFile()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "settings.json");
        JsonUserSettingsStore store = new(path);
        Assert.False(store.TrySave(new UserSettings { Language = "fr" }));
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Load_MissingFile_ReturnsSpanish()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "settings.json");
        JsonUserSettingsStore store = new(path);
        Assert.Equal("es", store.Load().Language);
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Social/LobbyInvitationsViewModelTests.cs
```csharp
using System.ServiceModel;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.Utilities.Sessions.Services;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Social;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Social;

public sealed class LobbyInvitationsViewModelTests
{
    [Fact]
    public async Task AcceptInvitation_Valid_RemovesOnlyAcceptedInvitation()
    {
        TestContext test = Create();
        string accepted = "AAAAAA";
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = accepted });
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "BBBBBB" });

        bool result = await test.Invitations.AcceptInvitationAsync(accepted);

        Assert.True(result);
        Assert.Single(test.Invitations.PendingInvitations);
        Assert.Equal("BBBBBB", test.Invitations.PendingInvitations[0].LobbyCode);
        Assert.Equal(NavigationPage.Lobby, test.Navigator.CurrentPage);
    }

    [Fact]
    public async Task AcceptInvitation_ServerRejects_StaysInSocialAndKeepsInvitations()
    {
        TestContext test = Create();
        test.Navigator.NavigateTo(NavigationPage.Social);
        test.LobbyClient.JoinFailure = new FaultException<ServiceFault>(
            new ServiceFault { ErrorCode = ServiceErrorCodes.LobbyNotFound, MessageKey = ServiceMessageKeys.LobbyNotFound },
            new FaultReason("Not found"));
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "AAAAAA" });
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "BBBBBB" });

        bool result = await test.Invitations.AcceptInvitationAsync("AAAAAA");

        Assert.False(result);
        Assert.Equal(2, test.Invitations.PendingInvitations.Count);
        Assert.Equal("No se encontró la sala.", test.Invitations.ErrorMessage);
        Assert.Equal(NavigationPage.Social, test.Navigator.CurrentPage);
    }

    [Fact]
    public async Task AcceptInvitation_StaleResponse_DoesNotNavigateOrRemove()
    {
        TestContext test = Create();
        test.Navigator.NavigateTo(NavigationPage.Social);
        test.Invitations.PendingInvitations.Add(new LobbyInvitationEntry { LobbyCode = "AAAAAA" });
        using ManualResetEventSlim started = new(false);
        using ManualResetEventSlim release = new(false);
        test.LobbyClient.OnJoin = _ =>
        {
            started.Set();
            if (!release.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException("Synchronization timeout");
            }
            return new LobbyAdmission { LobbyCode = "AAAAAA", HostPlayerId = Guid.NewGuid() };
        };

        Task<bool> pending = test.Invitations.AcceptInvitationAsync("AAAAAA");
        Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
        try
        {
            test.PlayerSession.SetSession(new SessionSnapshot(
                Guid.NewGuid(), "replacement", "replacement-token", DateTime.UtcNow.AddHours(1), false));
        }
        finally
        {
            release.Set();
        }
        bool result = await pending;

        Assert.False(result);
        Assert.Single(test.Invitations.PendingInvitations);
        Assert.Equal(NavigationPage.Social, test.Navigator.CurrentPage);
    }

    private static TestContext Create()
    {
        PlayerSession sessions = new(TimeProvider.System);
        sessions.SetSession(new SessionSnapshot(Guid.NewGuid(), "registered", "test-token", DateTime.UtcNow.AddHours(1), false));
        Navigator navigator = new();
        ITextLocalizer text = new FakeLocalizer();
        IClientErrorText errors = new ClientErrorText(text);
        SessionGuard guard = new(sessions, navigator, new FakeTransportCleaner());
        UiServices uiServices = new(errors, text, guard);
        UiNavigationContext ui = new(navigator, uiServices);
        FakeLobbyClient lobbyClient = new();
        LobbyViewModel lobby = new(lobbyClient, new FakeChatClient(), new LobbyContext(sessions, ui));
        SocialOperation operation = new(sessions, guard, errors);
        LobbyInvitationsViewModel invitations = new(new FakeInvitationClient(), operation, new LobbyInvitationContext(lobby, text));
        return new TestContext(sessions, navigator, lobbyClient, invitations);
    }

    private sealed record TestContext(PlayerSession PlayerSession, Navigator Navigator,
        FakeLobbyClient LobbyClient, LobbyInvitationsViewModel Invitations);

    private sealed class FakeLocalizer : ITextLocalizer
    {
        public string Get(string key)
        {
            return key switch
            {
                "ROOM_NOT_FOUND" => "No se encontró la sala.",
                "Lobby.JoinFailed" => "No se pudo entrar a la sala.",
                _ => key
            };
        }
    }

    private sealed class FakeTransportCleaner : ISessionTransportCleaner
    {
        public void DisconnectAll() { }
    }

    private sealed class FakeChatClient : IChatClient
    {
        public ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken)
        {
            return ClientResult<bool>.Success(true);
        }
    }

    private sealed class FakeLobbyClient : ILobbyClient
    {
        public Exception? JoinFailure { get; set; }
        public Func<string, LobbyAdmission>? OnJoin { get; set; }
        public event EventHandler<LobbyMember>? PlayerJoined;
        public event EventHandler<Guid>? PlayerLeft;
        public event EventHandler<LobbyChatMessage>? MessageReceived;
        public event EventHandler<LobbyPreparation>? PreparationChanged;

        public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
        {
            return "AAAAAA";
        }

        public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
        {
            if (JoinFailure is not null)
            {
                throw JoinFailure;
            }
            return OnJoin?.Invoke(lobbyCode) ?? new LobbyAdmission { LobbyCode = lobbyCode, HostPlayerId = Guid.NewGuid() };
        }

        public LobbyPreparation GetPreparation(string lobbyCode, string sessionToken)
        {
            return new LobbyPreparation { LobbyCode = lobbyCode };
        }

        public LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken)
        {
            return GetPreparation(lobbyCode, sessionToken);
        }

        public LobbyPreparation StartMatch(string lobbyCode, string sessionToken)
        {
            return GetPreparation(lobbyCode, sessionToken);
        }

        public bool LeaveLobby(string lobbyCode, string sessionToken)
        {
            return true;
        }

        public List<LobbySummary> GetPublicLobbies()
        {
            return new();
        }

        public void ReplayPendingEvents() { }
        public void Disconnect() { }
        public void Dispose() { }
    }

    private sealed class FakeInvitationClient : ILobbyInvitationClient
    {
        public ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken)
        {
            return ClientResult<List<FriendEntry>>.Success(new());
        }

        public ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
        {
            return ClientResult<List<Guid>>.Success(new());
        }

        public ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken)
        {
            return ClientResult<List<LobbyInvitationEntry>>.Success(new());
        }

        public ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken)
        {
            return ClientResult<bool>.Success(true);
        }
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Validation/RegistrationRulesTests.cs
```csharp
using Vertigo.Client.UI.Utilities.Validation;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class RegistrationRulesTests
{
    [Theory]
    [InlineData("Ab3", FieldValidationError.None)]
    [InlineData("Niño2026", FieldValidationError.None)]
    [InlineData("A", FieldValidationError.TooShort)]
    [InlineData("", FieldValidationError.Required)]
    [InlineData("Juan Perez", FieldValidationError.InvalidFormat)]
    [InlineData("Juan_23", FieldValidationError.InvalidFormat)]
    [InlineData("Juan!", FieldValidationError.InvalidFormat)]
    [InlineData("12345", FieldValidationError.InvalidFormat)]
    [InlineData("Ñ123", FieldValidationError.InvalidFormat)]
    [InlineData("a123", FieldValidationError.None)]
    [InlineData("A123", FieldValidationError.None)]
    [InlineData("a<script>", FieldValidationError.InvalidFormat)]
    public void ValidateUsername_Formats_ReturnsExpected(string value, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidateUsername(value));
    }

    [Fact]
    public void ValidateUsername_Boundaries_EnforcesServerMaximum()
    {
        Assert.Equal(FieldValidationError.None, RegistrationRules.ValidateUsername(new string('A', 20)));
        Assert.Equal(FieldValidationError.TooLong, RegistrationRules.ValidateUsername(new string('A', 21)));
    }

    [Theory]
    [InlineData("user@example.com", FieldValidationError.None)]
    [InlineData("user+tag@example.com", FieldValidationError.None)]
    [InlineData("user@example", FieldValidationError.InvalidFormat)]
    [InlineData("userexample.com", FieldValidationError.InvalidFormat)]
    [InlineData("user@@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user@.com", FieldValidationError.InvalidFormat)]
    [InlineData("user@domain..com", FieldValidationError.InvalidFormat)]
    [InlineData("user @example.com", FieldValidationError.InvalidFormat)]
    [InlineData("", FieldValidationError.Required)]
    [InlineData("user'<script>@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user;drop@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user\\name@example.com", FieldValidationError.InvalidFormat)]
    public void ValidateEmail_Formats_ReturnsExpected(string value, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidateEmail(value));
    }

    [Fact]
    public void ValidateEmail_OverServerLimit_ReturnsTooLong()
    {
        string suffix = "@example.com";
        string maximum = new string('a', 60 - suffix.Length) + suffix;
        Assert.Equal(FieldValidationError.None, RegistrationRules.ValidateEmail(maximum));
        Assert.Equal(FieldValidationError.TooLong, RegistrationRules.ValidateEmail("a" + maximum));
    }

    [Theory]
    [InlineData("Clave2026!", FieldValidationError.None)]
    [InlineData("Clave2026#", FieldValidationError.None)]
    [InlineData("Clave2026ñ", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("clave2026!", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Claveabc!", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave1!", FieldValidationError.TooShort)]
    [InlineData("", FieldValidationError.Required)]
    [InlineData("Clave2026<", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave2026;", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave2026'", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave2026!\\", FieldValidationError.PasswordRequirementsNotMet)]
    public void ValidatePassword_Requirements_ReturnsExpected(string value, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidatePassword(value));
    }

    [Fact]
    public void ValidatePassword_Boundaries_EnforcesMaximum()
    {
        string validMaximum = "A1!" + new string('b', 17);
        Assert.Equal(FieldValidationError.None, RegistrationRules.ValidatePassword(validMaximum));
        Assert.Equal(FieldValidationError.TooLong, RegistrationRules.ValidatePassword(validMaximum + "b"));
    }

    [Theory]
    [InlineData("Clave2026!", "Clave2026!", FieldValidationError.None)]
    [InlineData("Clave2026!", "clave2026!", FieldValidationError.PasswordsDoNotMatch)]
    [InlineData("Clave2026!", "", FieldValidationError.Required)]
    public void ValidateConfirmation_Comparisons_ReturnsExpected(
        string password, string confirmation, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidateConfirmation(password, confirmation));
    }
}
```

## File: tests/Vertigo.Client.UnitTests/Vertigo.Client.UnitTests.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
  </ItemGroup>
  <ItemGroup>
    <Using Include="Xunit" />
    <ProjectReference Include="../../src/Client/Vertigo.Client.Core/Vertigo.Client.Core.csproj" />
    <ProjectReference Include="../../src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <ProjectReference Include="../../src/Client/Vertigo.Client.UI/Vertigo.Client.UI.csproj" />
  </ItemGroup>
</Project>
```

## File: tests/Vertigo.Server.Game.Tests/Matches/InMemoryMatchSessionStarterTests.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class InMemoryMatchSessionStarterTests
{
    [Fact]
    public void Start_ValidRequest_CreatesMatchingSession()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        MatchSessionSnapshot result = starter.Start(request);

        Assert.NotEqual(Guid.Empty, result.MatchId);
        Assert.Equal(request.LobbyInstanceId, result.LobbyInstanceId);
        Assert.Equal(request.PlayerIds, result.PlayerIds);
    }

    [Fact]
    public void Start_RepeatedRequest_ReturnsSameSession()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        MatchSessionSnapshot first = starter.Start(request);
        MatchSessionSnapshot second = starter.Start(request);

        Assert.Same(first, second);
    }

    [Fact]
    public void Start_ConcurrentRequests_CreatesOnlyOneMatch()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        Guid[] identifiers = new Guid[32];

        Parallel.For(0, identifiers.Length, index =>
        {
            identifiers[index] = starter.Start(request).MatchId;
        });

        Assert.Single(identifiers.Distinct());
    }

    [Fact]
    public void Start_DifferentParticipantsForSameLobby_RejectsConflict()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        starter.Start(request);
        MatchStartRequest changed = request with
        {
            PlayerIds = new[] { request.HostPlayerId, Guid.NewGuid() }
        };

        Assert.Throws<InvalidOperationException>(() => starter.Start(changed));
    }

    [Fact]
    public void Start_AnotherLobbyInstanceWithSameCode_CreatesNewMatch()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        Guid first = starter.Start(request).MatchId;
        Guid second = starter.Start(request with { LobbyInstanceId = Guid.NewGuid() }).MatchId;

        Assert.NotEqual(first, second);
    }

    private static MatchStartRequest CreateRequest()
    {
        Guid host = Guid.NewGuid();
        return new MatchStartRequest
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABCDEF",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };
    }
}
```

## File: tests/Vertigo.Server.Game.Tests/Matches/MatchLifecycleRegistrationTests.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.DependencyInjection;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchLifecycleRegistrationTests
{
    [Fact]
    public void AddMatchLifecycleServices_ResolvedInterfaces_ShareGameRegistry()
    {
        ServiceCollection services = new();
        services.AddSingleton(TimeProvider.System);
        services.AddMatchLifecycleServices();
        using ServiceProvider provider = services.BuildServiceProvider();
        IMatchSessionStarter starter = provider.GetRequiredService<IMatchSessionStarter>();
        IMatchLifecycle lifecycle = provider.GetRequiredService<IMatchLifecycle>();
        Guid host = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABCDEF",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };

        MatchSessionSnapshot match = starter.Start(request);
        bool finished = lifecycle.TryFinish(match.MatchId);

        Assert.True(finished);
        Assert.Equal(MatchLifecycleStatus.Finished, lifecycle.Get(match.MatchId)?.Status);
        Assert.Equal(2, lifecycle.Get(match.MatchId)?.Revision);
        Assert.Same(starter, lifecycle);
    }
}
```

## File: tests/Vertigo.Server.Game.Tests/Matches/MatchLifecycleTests.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchLifecycleTests
{
    [Fact]
    public void Start_ValidRequest_EntersRunningWithRevisionOne()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchSessionSnapshot session = starter.Start(CreateRequest());

        MatchLifecycleSnapshot? state = starter.Get(session.MatchId);

        Assert.NotNull(state);
        Assert.Equal(MatchLifecycleStatus.Running, state.Status);
        Assert.Equal(1, state.Revision);
        Assert.Equal(session.LobbyInstanceId, state.LobbyInstanceId);
    }

    [Fact]
    public void TryFinish_RunningMatch_ChangesToFinished()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;

        bool finished = starter.TryFinish(matchId);

        Assert.True(finished);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(matchId)?.Status);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void TryCancel_RunningMatch_ChangesToCancelled()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;

        bool cancelled = starter.TryCancel(matchId);

        Assert.True(cancelled);
        Assert.Equal(MatchLifecycleStatus.Cancelled, starter.Get(matchId)?.Status);
    }

    [Fact]
    public void TryFinish_FinishedMatch_LeavesRevisionUnchanged()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        starter.TryFinish(matchId);

        bool repeated = starter.TryFinish(matchId);

        Assert.False(repeated);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void TryCancel_FinishedMatch_DoesNotChangeTerminalStatus()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        starter.TryFinish(matchId);

        bool cancelled = starter.TryCancel(matchId);

        Assert.False(cancelled);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(matchId)?.Status);
    }

    [Fact]
    public void TryFinish_UnknownMatch_ReturnsFalse()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);

        bool result = starter.TryFinish(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public void TryFinish_ConcurrentCalls_OnlyOneSucceeds()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        int succeeded = 0;

        Parallel.For(0, 64, _ =>
        {
            if (starter.TryFinish(matchId))
            {
                Interlocked.Increment(ref succeeded);
            }
        });

        Assert.Equal(1, succeeded);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void PurgeTerminated_RunningMatch_KeepsSession()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        clock.Advance(TimeSpan.FromDays(30));

        int removed = starter.PurgeTerminated(TimeSpan.Zero);

        Assert.Equal(0, removed);
        Assert.NotNull(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_BeforeRetention_KeepsFinishedSession()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        starter.TryFinish(matchId);
        clock.Advance(TimeSpan.FromMinutes(9));

        int removed = starter.PurgeTerminated(TimeSpan.FromMinutes(10));

        Assert.Equal(0, removed);
        Assert.NotNull(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_AtRetention_RemovesFinishedSession()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        MatchStartRequest request = CreateRequest();
        Guid matchId = starter.Start(request).MatchId;
        starter.TryFinish(matchId);
        clock.Advance(TimeSpan.FromMinutes(10));

        int removed = starter.PurgeTerminated(TimeSpan.FromMinutes(10));

        Assert.Equal(1, removed);
        Assert.Null(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_NegativeRetention_ThrowsArgumentOutOfRangeException()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);

        Assert.Throws<ArgumentOutOfRangeException>(() => starter.PurgeTerminated(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void Start_RepeatedAfterFinish_ReturnsOriginalMatch()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        MatchSessionSnapshot first = starter.Start(request);
        starter.TryFinish(first.MatchId);

        MatchSessionSnapshot second = starter.Start(request);

        Assert.Same(first, second);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(second.MatchId)?.Status);
    }

    [Fact]
    public void TryCancel_ConcurrentWithFinish_ChoosesOneTerminalState()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        bool[] results = new bool[2];

        Parallel.Invoke(
            () => results[0] = starter.TryFinish(matchId),
            () => results[1] = starter.TryCancel(matchId));

        Assert.Single(results, result => result);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void PurgeTerminated_MixedSessions_RemovesOnlyEligibleAndKeepsLobbyIndexConsistent()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        MatchStartRequest eligibleRequest = CreateRequest();
        MatchStartRequest recentRequest = CreateRequest();
        MatchStartRequest runningRequest = CreateRequest();
        MatchSessionSnapshot eligible = starter.Start(eligibleRequest);
        Assert.True(starter.TryFinish(eligible.MatchId));

        clock.Advance(TimeSpan.FromMinutes(7));
        MatchSessionSnapshot recent = starter.Start(recentRequest);
        Assert.True(starter.TryCancel(recent.MatchId));
        MatchSessionSnapshot running = starter.Start(runningRequest);
        clock.Advance(TimeSpan.FromMinutes(3));

        int removed = starter.PurgeTerminated(TimeSpan.FromMinutes(10));

        Assert.Equal(1, removed);
        Assert.Null(starter.Get(eligible.MatchId));
        Assert.Equal(MatchLifecycleStatus.Cancelled, starter.Get(recent.MatchId)?.Status);
        Assert.Equal(MatchLifecycleStatus.Running, starter.Get(running.MatchId)?.Status);
        Assert.Same(recent, starter.Start(recentRequest));
        Assert.Same(running, starter.Start(runningRequest));
        MatchSessionSnapshot recreated = starter.Start(eligibleRequest);
        Assert.NotEqual(eligible.MatchId, recreated.MatchId);
        Assert.Equal(MatchLifecycleStatus.Running, starter.Get(recreated.MatchId)?.Status);
    }

    [Fact]
    public void PurgeTerminated_ExtremePositiveRetention_DoesNotOverflowOrPurge()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        Assert.True(starter.TryFinish(matchId));

        int removed = starter.PurgeTerminated(TimeSpan.MaxValue);

        Assert.Equal(0, removed);
        Assert.NotNull(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_RetentionBeyondRepresentablePast_KeepsTerminalAtMinimumDate()
    {
        FakeClock clock = new();
        clock.Set(DateTimeOffset.MinValue);
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        Assert.True(starter.TryFinish(matchId));

        int removed = starter.PurgeTerminated(TimeSpan.FromTicks(1));

        Assert.Equal(0, removed);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(matchId)?.Status);
    }

    [Fact]
    public void PurgeTerminated_AtMinimumDateWithZeroRetention_RemovesTerminal()
    {
        FakeClock clock = new();
        clock.Set(DateTimeOffset.MinValue);
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        Assert.True(starter.TryFinish(matchId));

        int removed = starter.PurgeTerminated(TimeSpan.Zero);

        Assert.Equal(1, removed);
        Assert.Null(starter.Get(matchId));
    }

    private static MatchStartRequest CreateRequest()
    {
        Guid host = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABCDEF",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };

        return request;
    }

    private sealed class FakeClock : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 11, 16, 0, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow()
        {
            return _now;
        }

        public void Set(DateTimeOffset now)
        {
            _now = now;
        }

        public void Advance(TimeSpan duration)
        {
            _now = _now.Add(duration);
        }
    }
}
```

## File: tests/Vertigo.Server.Game.Tests/Matches/MatchPresenceTests.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchPresenceTests
{
    [Fact]
    public void TryDisconnect_OnePlayer_MatchRemainsRunning()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        Assert.True(registry.TryDisconnect(match.MatchId, first));
        Assert.Equal(MatchLifecycleStatus.Running, registry.Get(match.MatchId)?.Status);
        Assert.Equal(new[] { first }, registry.GetPresence(match.MatchId)?.DisconnectedPlayerIds);
    }

    [Fact]
    public void TryDisconnect_AllPlayers_SuspendsMatch()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        registry.TryDisconnect(match.MatchId, first);
        registry.TryDisconnect(match.MatchId, second);

        Assert.Equal(MatchLifecycleStatus.Suspended, registry.Get(match.MatchId)?.Status);
        Assert.Equal(3, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryReconnect_SuspendedMatch_RestoresRunning()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryDisconnect(match.MatchId, second);

        MatchReconnectResult restored = registry.TryReconnect(match.MatchId, first);

        Assert.Equal(MatchReconnectResult.Reconnected, restored);
        Assert.Equal(MatchLifecycleStatus.Running, registry.Get(match.MatchId)?.Status);
        Assert.Equal(match.MatchId, registry.GetPresence(match.MatchId)?.MatchId);
    }

    [Fact]
    public void TryReconnect_DuplicateCall_PreservesRevision()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryReconnect(match.MatchId, first);
        long revision = registry.Get(match.MatchId)!.Revision;

        Assert.Equal(MatchReconnectResult.AlreadyConnected, registry.TryReconnect(match.MatchId, first));
        Assert.Equal(revision, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryDisconnect_UnknownParticipant_IsRejected()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        Assert.False(registry.TryDisconnect(match.MatchId, Guid.NewGuid()));
        Assert.Equal(1, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryReconnect_FinishedMatch_IsRejected()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryFinish(match.MatchId);
        long revision = registry.Get(match.MatchId)!.Revision;

        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(match.MatchId, first));
        Assert.Equal(revision, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryFinish_SuspendedMatch_IsAllowed()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryDisconnect(match.MatchId, second);

        Assert.True(registry.TryFinish(match.MatchId));
        Assert.Equal(MatchLifecycleStatus.Finished, registry.Get(match.MatchId)?.Status);
    }

    [Fact]
    public void TryReconnect_UnknownMatchOrPlayer_ReturnsRejected()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(Guid.NewGuid(), first));
        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(match.MatchId, Guid.NewGuid()));
        Assert.Equal(1, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public async Task TryReconnect_ConcurrentFinish_DoesNotReviveTerminalMatch()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        using Barrier barrier = new(2);

        Task<MatchReconnectResult> reconnect = Task.Run(() =>
        {
            barrier.SignalAndWait();
            return registry.TryReconnect(match.MatchId, first);
        });
        Task<bool> finish = Task.Run(() =>
        {
            barrier.SignalAndWait();
            return registry.TryFinish(match.MatchId);
        });
        await Task.WhenAll(reconnect, finish);

        Assert.True(await finish);
        Assert.Equal(MatchLifecycleStatus.Finished, registry.Get(match.MatchId)?.Status);
        Assert.Contains(await reconnect, new[] { MatchReconnectResult.Reconnected, MatchReconnectResult.Rejected });
        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(match.MatchId, first));
    }

    private static (MatchSessionSnapshot Match, Guid First, Guid Second) Start(InMemoryMatchSessionStarter registry)
    {
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC123",
            HostPlayerId = first,
            PlayerIds = new[] { first, second }
        };
        return (registry.Start(request), first, second);
    }
}
```

## File: tests/Vertigo.Server.Game.Tests/UnitTest1.cs
```csharp
namespace Vertigo.Server.Game.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
    }
}
```

## File: tests/Vertigo.Server.Matchmaking.Tests/Authentication/GuestSessionTests.cs
```csharp
using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Services;

namespace Vertigo.Server.Matchmaking.Tests.Authentication;

public sealed class GuestSessionTests
{
    [Fact]
    public void Find_AtExactExpiration_ReturnsNull()
    {
        ControlledTimeProvider clock = new(new DateTimeOffset(2026, 11, 16, 12, 0, 0, TimeSpan.Zero));
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string hash = tokens.HashSessionToken(registry.Create().Token);
        Assert.NotNull(registry.Find(hash));

        clock.Advance(TimeSpan.FromHours(12));

        Assert.Null(registry.Find(hash));
    }

    [Fact]
    public void Revoke_RemovesGuestSession()
    {
        ControlledTimeProvider clock = new(DateTimeOffset.UtcNow);
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string hash = tokens.HashSessionToken(registry.Create().Token);

        Assert.True(registry.Revoke(hash));
        Assert.Null(registry.Find(hash));
    }

    [Fact]
    public void RequireRegistered_GuestIsRejected()
    {
        ControlledTimeProvider clock = new(DateTimeOffset.UtcNow);
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string raw = registry.Create().Token;
        SessionIdentityResolver resolver = new(new FakeSessionTokenRepository(), tokens, registry);

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => resolver.RequireRegistered(raw));

        Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
    }

    [Fact]
    public void Require_RevokedGuestIsRejected()
    {
        ControlledTimeProvider clock = new(DateTimeOffset.UtcNow);
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string raw = registry.Create().Token;
        registry.Revoke(tokens.HashSessionToken(raw));
        SessionIdentityResolver resolver = new(new FakeSessionTokenRepository(), tokens, registry);

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => resolver.Require(raw));

        Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
    }

    private sealed class ControlledTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public ControlledTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }

        public void Advance(TimeSpan amount)
        {
            _utcNow += amount;
        }
    }

    private sealed class FakeTokenGenerator : ITokenGenerator
    {
        public string GenerateSessionToken()
        {
            return new string('A', 64);
        }

        public string GenerateVerificationCode()
        {
            return "123456";
        }

        public string HashSessionToken(string sessionToken)
        {
            return sessionToken;
        }
    }

    private sealed class FakeSessionTokenRepository : ISessionTokenRepository
    {
        public bool CreateSessionToken(SessionToken token)
        {
            return false;
        }

        public SessionToken? GetActiveSessionToken(string tokenHash)
        {
            return null;
        }

        public bool RevokeSessionToken(string tokenHash)
        {
            return false;
        }

        public int RevokePlayerSessions(Guid playerId)
        {
            return 0;
        }
    }
}
```

## File: tests/Vertigo.Server.Matchmaking.Tests/Lobbies/LobbyDisconnectionTests.cs
```csharp
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyDisconnectionTests
{
    [Fact]
    public void DisconnectSession_InGame_PreservesParticipantAndHost()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "host-channel");
        Add(lobby, other, "other-channel");
        lobby.StartMatch(Guid.NewGuid());
        long revision = lobby.Revision;

        List<Guid> disconnected = lobby.DisconnectSession("host-channel");

        Assert.Equal(new[] { host }, disconnected);
        Assert.Equal(host, lobby.HostPlayerId);
        Assert.True(lobby.Contains(host));
        Assert.True(lobby.Contains(other));
        Assert.Equal(revision + 1, lobby.Revision);
        Assert.Contains(host, lobby.ToPreparation().DisconnectedPlayerIds);
        Assert.False(lobby.ToPreparation().IsSuspended);
    }

    [Fact]
    public void DisconnectSession_AllMembers_SetsSuspendedFlag()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "a");
        Add(lobby, other, "b");
        lobby.StartMatch(Guid.NewGuid());
        lobby.DisconnectSession("a");
        lobby.DisconnectSession("b");

        Assert.True(lobby.ToPreparation().IsSuspended);
        Assert.Equal(2, lobby.ToPreparation().DisconnectedPlayerIds.Count);
    }

    [Fact]
    public void AddOrReplace_LateCloseOfOldChannel_DoesNotDisconnectNewChannel()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "old-channel");
        Add(lobby, other, "other-channel");
        lobby.StartMatch(Guid.NewGuid());
        lobby.DisconnectSession("old-channel");
        Add(lobby, host, "new-channel");
        long revision = lobby.Revision;

        List<Guid> stale = lobby.DisconnectSession("old-channel");

        Assert.Empty(stale);
        Assert.Equal(revision, lobby.Revision);
        Assert.DoesNotContain(host, lobby.ToPreparation().DisconnectedPlayerIds);
    }

    [Fact]
    public void DisconnectSession_DuplicateNotification_DoesNotAdvanceRevision()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "channel");
        lobby.StartMatch(Guid.NewGuid());
        lobby.DisconnectSession("channel");
        long revision = lobby.Revision;

        Assert.Empty(lobby.DisconnectSession("channel"));
        Assert.Equal(revision, lobby.Revision);
    }

    [Fact]
    public void DisconnectSession_ReplacedChannel_DoesNotAlterExistingMembers()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "first");
        Add(lobby, other, "second");
        lobby.StartMatch(Guid.NewGuid());
        Add(lobby, host, "replacement");
        long revision = lobby.Revision;

        Assert.Empty(lobby.DisconnectSession("first"));
        Assert.Equal(revision, lobby.Revision);
        Assert.Equal(host, lobby.HostPlayerId);
        Assert.True(lobby.Contains(host));
        Assert.True(lobby.Contains(other));
    }

    private static LobbyState NewLobby(Guid host)
    {
        return new LobbyState(new LobbySettings("ABCDEF", host, true, 6, DateTimeOffset.UtcNow));
    }

    private static void Add(LobbyState lobby, Guid id, string channel)
    {
        LobbyMember member = new() { PlayerId = id, Username = channel };
        lobby.AddOrReplace(new LobbyConnection(member, new CallbackStub(), channel));
    }

    private sealed class CallbackStub : ILobbyCallback
    {
        public void PlayerJoined(LobbyMember member) { }
        public void PlayerLeft(Guid playerId) { }
        public void MessageReceived(LobbyChatMessage message) { }
        public void PreparationChanged(LobbyPreparation preparation) { }
    }
}
```

## File: tests/Vertigo.Server.Matchmaking.Tests/Lobbies/LobbyMatchActivationTests.cs
```csharp
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;
using Vertigo.Server.Matchmaking.Lobbies.Services;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyMatchActivationTests
{
    [Fact]
    public void Activate_ValidLobby_SetsMatchIdAndInGame()
    {
        LobbyState lobby = CreateReadyLobby();
        LobbyMatchActivation activation = new(new FakeStarter());
        long previousRevision = lobby.Revision;

        activation.Activate(lobby);

        Assert.True(lobby.ToPreparation().IsInGame);
        Assert.NotEqual(Guid.Empty, lobby.MatchId);
        Assert.True(lobby.Revision > previousRevision);
    }

    [Fact]
    public void Activate_StarterFails_KeepsWaitingAndRevision()
    {
        LobbyState lobby = CreateReadyLobby();
        LobbyMatchActivation activation = new(new FakeStarter(shouldThrow: true));
        long revision = lobby.Revision;

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));

        Assert.Equal(Guid.Empty, lobby.MatchId);
        Assert.False(lobby.ToPreparation().IsInGame);
        Assert.Equal(revision, lobby.Revision);
    }

    [Fact]
    public void Activate_DuplicateAttempt_DoesNotCreateAgain()
    {
        LobbyState lobby = CreateReadyLobby();
        FakeStarter starter = new();
        LobbyMatchActivation activation = new(starter);
        activation.Activate(lobby);

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));
        Assert.Equal(1, starter.Count);
    }

    [Fact]
    public void Activate_StarterCommitsThenThrows_RetryUsesSameMatchAndPreservesRevisionUntilSuccess()
    {
        LobbyState lobby = CreateReadyLobby();
        CommitThenThrowStarter starter = new();
        LobbyMatchActivation activation = new(starter);
        long before = lobby.Revision;

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));

        Assert.Equal(Guid.Empty, lobby.MatchId);
        Assert.False(lobby.ToPreparation().IsInGame);
        Assert.Equal(before, lobby.Revision);

        activation.Activate(lobby);

        Assert.Equal(starter.CreatedMatchId, lobby.MatchId);
        Assert.True(lobby.ToPreparation().IsInGame);
        Assert.Equal(before + 1, lobby.Revision);
        Assert.Equal(2, starter.CallCount);
        Assert.Equal(1, starter.CreatedCount);
    }

    private sealed class CommitThenThrowStarter : IMatchSessionStarter
    {
        private readonly Dictionary<Guid, MatchSessionSnapshot> _registered = new();
        private bool _throwAfterCommit = true;

        public int CallCount { get; private set; }
        public int CreatedCount => _registered.Count;
        public Guid CreatedMatchId => Assert.Single(_registered.Values).MatchId;

        public MatchSessionSnapshot Start(MatchStartRequest request)
        {
            CallCount++;
            if (!_registered.TryGetValue(request.LobbyInstanceId, out MatchSessionSnapshot? session))
            {
                session = new MatchSessionSnapshot(Guid.NewGuid(), request.LobbyInstanceId,
                    request.LobbyCode, request.HostPlayerId, request.PlayerIds, DateTime.UtcNow);
                _registered.Add(request.LobbyInstanceId, session);
            }

            if (_throwAfterCommit)
            {
                _throwAfterCommit = false;
                throw new InvalidOperationException("Simulated failure after registration.");
            }

            return session;
        }
    }

    [Fact]
    public void Activate_InconsistentGameResult_KeepsWaiting()
    {
        LobbyState lobby = CreateReadyLobby();
        LobbyMatchActivation activation = new(new FakeStarter(wrongLobby: true));

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));
        Assert.False(lobby.ToPreparation().IsInGame);
    }

    private static LobbyState CreateReadyLobby()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = new(new LobbySettings("ABCDEF", host, false, 4, DateTimeOffset.UtcNow));
        lobby.AddOrReplace(new LobbyConnection(new LobbyMember { PlayerId = host }, new StubCallback(), "a"));
        lobby.AddOrReplace(new LobbyConnection(new LobbyMember { PlayerId = other }, new StubCallback(), "b"));
        lobby.SetReady(host, true);
        lobby.SetReady(other, true);
        return lobby;
    }

    private sealed class FakeStarter : IMatchSessionStarter
    {
        private readonly bool _shouldThrow;
        private readonly bool _wrongLobby;
        public int Count { get; private set; }

        public FakeStarter(bool shouldThrow = false, bool wrongLobby = false)
        {
            _shouldThrow = shouldThrow;
            _wrongLobby = wrongLobby;
        }

        public MatchSessionSnapshot Start(MatchStartRequest request)
        {
            Count++;
            if (_shouldThrow)
            {
                throw new InvalidOperationException("Simulated game failure.");
            }

            return new MatchSessionSnapshot(Guid.NewGuid(), request.LobbyInstanceId,
                _wrongLobby ? "WRONG" : request.LobbyCode,
                request.HostPlayerId, request.PlayerIds, DateTime.UtcNow);
        }
    }

    private sealed class StubCallback : ILobbyCallback
    {
        public void PlayerJoined(LobbyMember member) { }
        public void PlayerLeft(Guid playerId) { }
        public void MessageReceived(LobbyChatMessage message) { }
        public void PreparationChanged(LobbyPreparation preparation) { }
    }
}
```

## File: tests/Vertigo.Server.Matchmaking.Tests/Lobbies/LobbyStateTests.cs
```csharp
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyStateTests
{
    [Fact]
    public void Remove_Host_TransfersHostToLongestPresentPlayer()
    {
        Guid host = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        Guid third = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "a");
        Add(lobby, second, "b");
        Add(lobby, third, "c");

        Assert.True(lobby.Remove(host));

        Assert.Equal(second, lobby.HostPlayerId);
        Assert.False(lobby.Contains(host));
        Assert.True(lobby.Contains(third));
    }

    [Fact]
    public void RemoveSession_RemovesOnlyMembersOfThatChannel()
    {
        Guid host = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        Guid third = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "first");
        Add(lobby, second, "second");
        Add(lobby, third, "third");

        List<Guid> removed = lobby.RemoveSession("second");

        Assert.Equal(new[] { second }, removed);
        Assert.True(lobby.Contains(host));
        Assert.True(lobby.Contains(third));
    }

    [Fact]
    public void CancelInvitation_LeavesOtherInvitationsIntact()
    {
        LobbyState lobby = CreateLobby(Guid.NewGuid());
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        lobby.Invite(first, Guid.NewGuid());
        lobby.Invite(second, Guid.NewGuid());

        Assert.True(lobby.CancelInvitation(first));

        Assert.False(lobby.IsInvited(first));
        Assert.True(lobby.IsInvited(second));
    }

    [Fact]
    public void SetReady_NonMember_ReturnsFalse()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");

        Assert.False(lobby.SetReady(Guid.NewGuid(), true));
        Assert.True(lobby.SetReady(host, true));
        Assert.Contains(host, lobby.ToPreparation().ReadyPlayerIds);
    }

    [Fact]
    public void CanStartMatch_RequiresTwoPlayersAllReadyAndHost()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");
        Assert.False(lobby.CanStartMatch(host));
        Add(lobby, other, "other");
        lobby.SetReady(host, true);
        Assert.False(lobby.CanStartMatch(host));
        lobby.SetReady(other, true);

        Assert.True(lobby.CanStartMatch(host));
        Assert.False(lobby.CanStartMatch(other));
        lobby.StartMatch(Guid.NewGuid());
        Assert.False(lobby.CanStartMatch(host));
        Assert.True(lobby.ToPreparation().IsInGame);
    }

    [Fact]
    public void Join_ResetsReadyVotes()
    {
        Guid host = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        Guid third = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "a");
        Add(lobby, second, "b");
        lobby.SetReady(host, true);
        lobby.SetReady(second, true);

        Add(lobby, third, "c");

        Assert.Empty(lobby.ToPreparation().ReadyPlayerIds);
    }

    [Fact]
    public void Remove_ReadyPlayer_ExcludesDepartedPlayerFromPreparation()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");
        lobby.SetReady(host, true);

        lobby.Remove(host);

        Assert.Empty(lobby.ToPreparation().ReadyPlayerIds);
    }

    [Fact]
    public void ToPreparation_ConsecutiveChanges_RevisionIncreases()
    {
        Guid host = Guid.NewGuid();
        Guid visitor = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        long initial = lobby.ToPreparation().Revision;
        Add(lobby, host, "first");
        long entered = lobby.ToPreparation().Revision;
        Add(lobby, visitor, "second");
        long joined = lobby.ToPreparation().Revision;
        lobby.SetReady(host, true);
        long ready = lobby.ToPreparation().Revision;
        lobby.Remove(host);
        long transferred = lobby.ToPreparation().Revision;

        Assert.True(initial < entered);
        Assert.True(entered < joined);
        Assert.True(joined < ready);
        Assert.True(ready < transferred);
    }

    [Fact]
    public void SetReady_SameValue_DoesNotAdvanceRevision()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");
        lobby.SetReady(host, true);
        long revision = lobby.Revision;

        Assert.True(lobby.SetReady(host, true));
        Assert.Equal(revision, lobby.Revision);
    }

    private static LobbyState CreateLobby(Guid host)
    {
        return new LobbyState(new LobbySettings("ABCDEF", host, true, 6, DateTimeOffset.UtcNow));
    }

    private static void Add(LobbyState lobby, Guid playerId, string channel)
    {
        lobby.AddOrReplace(new LobbyConnection(
            new LobbyMember { PlayerId = playerId, Username = channel }, new CallbackStub(), channel));
    }

    private sealed class CallbackStub : ILobbyCallback
    {
        public void PlayerJoined(LobbyMember player) { }
        public void PlayerLeft(Guid playerId) { }
        public void MessageReceived(LobbyChatMessage message) { }
        public void PreparationChanged(LobbyPreparation preparation) { }
    }
}
```

## File: tests/Vertigo.Server.Matchmaking.Tests/Vertigo.Server.Matchmaking.Tests.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
  </ItemGroup>
  <ItemGroup>
    <Using Include="Xunit" />
    <ProjectReference Include="../../src/Server/Vertigo.Server.Matchmaking/Vertigo.Server.Matchmaking.csproj" />
  </ItemGroup>
</Project>
```

## File: .gitignore
```
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.
##
## Get latest from `dotnet new gitignore`

# dotenv files
.env

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# User-specific files (MonoDevelop/Xamarin Studio)
*.userprefs

# Mono auto generated files
mono_crash.*

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio 2015/2017 cache/options directory
.vs/
# Uncomment if you have tasks that create the project's static files in wwwroot
#wwwroot/

# Visual Studio 2017 auto generated files
Generated\ Files/

# MSTest test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# NUnit
*.VisualState.xml
TestResult.xml
nunit-*.xml

# Build Results of an ATL Project
[Dd]ebugPS/
[Rr]eleasePS/
dlldata.c

# Benchmark Results
BenchmarkDotNet.Artifacts/

# .NET
project.lock.json
project.fragment.lock.json
artifacts/

# Tye
.tye/

# ASP.NET Scaffolding
ScaffoldingReadMe.txt

# StyleCop
StyleCopReport.xml

# Files built by Visual Studio
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
# but not Directory.Build.rsp, as it configures directory-level build defaults
!Directory.Build.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.tlog
*.vspscc
*.vssscc
.builds
*.pidb
*.svclog
*.scc

# Chutzpah Test files
_Chutzpah*

# Visual C++ cache files
ipch/
*.aps
*.ncb
*.opendb
*.opensdf
*.sdf
*.cachefile
*.VC.db
*.VC.VC.opendb

# Visual Studio profiler
*.psess
*.vsp
*.vspx
*.sap

# Visual Studio Trace Files
*.e2e
# but not directories ending in .e2e
!*.e2e/

# TFS 2012 Local Workspace
$tf/

# Guidance Automation Toolkit
*.gpState

# ReSharper is a .NET coding add-in
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user

# TeamCity is a build add-in
_TeamCity*

# DotCover is a Code Coverage Tool
*.dotCover

# AxoCover is a Code Coverage Tool
.axoCover/*
!.axoCover/settings.json

# Coverlet is a free, cross platform Code Coverage Tool
coverage*.json
coverage*.xml
coverage*.info

# Visual Studio code coverage results
*.coverage
*.coveragexml

# NCrunch
_NCrunch_*
.*crunch*.local.xml
nCrunchTemp_*

# MightyMoose
*.mm.*
AutoTest.Net/

# Web workbench (sass)
.sass-cache/

# Installshield output folder
[Ee]xpress/

# DocProject is a documentation generator add-in
DocProject/buildhelp/
DocProject/Help/*.HxT
DocProject/Help/*.HxC
DocProject/Help/*.hhc
DocProject/Help/*.hhk
DocProject/Help/*.hhp
DocProject/Help/Html2
DocProject/Help/html

# Click-Once directory
publish/

# Publish Web Output
*.[Pp]ublish.xml
*.azurePubxml
# Note: Comment the next line if you want to checkin your web deploy settings,
# but database connection strings (with potential passwords) will be unencrypted
*.pubxml
*.publishproj

# Microsoft Azure Web App publish settings. Comment the next line if you want to
# checkin your Azure Web App publish settings, but sensitive information contained
# in these scripts will be unencrypted
PublishScripts/

# NuGet Packages
*.nupkg
# NuGet Symbol Packages
*.snupkg
# The packages folder can be ignored because of Package Restore
**/[Pp]ackages/*
# except build/, which is used as an MSBuild target.
!**/[Pp]ackages/build/
# Uncomment if necessary however generally it will be regenerated when needed
#!**/[Pp]ackages/repositories.config
# NuGet v3's project.json files produces more ignorable files
*.nuget.props
*.nuget.targets

# Microsoft Azure Build Output
csx/
*.build.csdef

# Microsoft Azure Emulator
ecf/
rcf/

# Windows Store app package directories and files
AppPackages/
BundleArtifacts/
Package.StoreAssociation.xml
_pkginfo.txt
*.appx
*.appxbundle
*.appxupload

# Visual Studio cache files
# files ending in .cache can be ignored
*.[Cc]ache
# but keep track of directories ending in .cache
!?*.[Cc]ache/

# Others
ClientBin/
~$*
*~
*.dbmdl
*.dbproj.schemaview
*.jfm
*.pfx
*.publishsettings
orleans.codegen.cs

# Including strong name files can present a security risk
# (https://github.com/github/gitignore/pull/2483#issue-259490424)
#*.snk

# Since there are multiple workflows, uncomment next line to ignore bower_components
# (https://github.com/github/gitignore/pull/1529#issuecomment-104372622)
#bower_components/

# RIA/Silverlight projects
Generated_Code/

# Backup & report files from converting an old project file
# to a newer Visual Studio version. Backup files are not needed,
# because we have git ;-)
_UpgradeReport_Files/
Backup*/
UpgradeLog*.XML
UpgradeLog*.htm
ServiceFabricBackup/
*.rptproj.bak

# SQL Server files
*.mdf
*.ldf
*.ndf

# Business Intelligence projects
*.rdl.data
*.bim.layout
*.bim_*.settings
*.rptproj.rsuser
*- [Bb]ackup.rdl
*- [Bb]ackup ([0-9]).rdl
*- [Bb]ackup ([0-9][0-9]).rdl

# Microsoft Fakes
FakesAssemblies/

# GhostDoc plugin setting file
*.GhostDoc.xml

# Node.js Tools for Visual Studio
.ntvs_analysis.dat
node_modules/

# Visual Studio 6 build log
*.plg

# Visual Studio 6 workspace options file
*.opt

# Visual Studio 6 auto-generated workspace file (contains which files were open etc.)
*.vbw

# Visual Studio 6 auto-generated project file (contains which files were open etc.)
*.vbp

# Visual Studio 6 workspace and project file (working project files containing files to include in project)
*.dsw
*.dsp

# Visual Studio 6 technical files
*.ncb
*.aps

# Visual Studio LightSwitch build output
**/*.HTMLClient/GeneratedArtifacts
**/*.DesktopClient/GeneratedArtifacts
**/*.DesktopClient/ModelManifest.xml
**/*.Server/GeneratedArtifacts
**/*.Server/ModelManifest.xml
_Pvt_Extensions

# Paket dependency manager
.paket/paket.exe
paket-files/

# FAKE - F# Make
.fake/

# CodeRush personal settings
.cr/personal

# Python Tools for Visual Studio (PTVS)
__pycache__/
*.pyc

# Cake - Uncomment if you are using it
# tools/**
# !tools/packages.config

# Tabs Studio
*.tss

# Telerik's JustMock configuration file
*.jmconfig

# BizTalk build output
*.btp.cs
*.btm.cs
*.odx.cs
*.xsd.cs

# OpenCover UI analysis results
OpenCover/

# Azure Stream Analytics local run output
ASALocalRun/

# MSBuild Binary and Structured Log
*.binlog

# NVidia Nsight GPU debugger configuration file
*.nvuser

# MFractors (Xamarin productivity tool) working folder
.mfractor/

# Local History for Visual Studio
.localhistory/

# Visual Studio History (VSHistory) files
.vshistory/

# BeatPulse healthcheck temp database
healthchecksdb

# Backup folder for Package Reference Convert tool in Visual Studio 2017
MigrationBackup/

# Ionide (cross platform F# VS Code tools) working folder
.ionide/

# Fody - auto-generated XML schema
FodyWeavers.xsd

# VS Code files for those working on multiple tools
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
*.code-workspace

# Official VS Code C# Dev Kit Extension exclusion
*.lscache

# Local History for Visual Studio Code
.history/

# Windows Installer files from build outputs
*.cab
*.msi
*.msix
*.msm
*.msp

# JetBrains Rider
*.sln.iml
.idea/

##
## Visual studio for Mac
##


# globs
Makefile.in
*.userprefs
*.usertasks
config.make
config.status
aclocal.m4
install-sh
autom4te.cache/
*.tar.gz
tarballs/
test-results/

# content below from: https://github.com/github/gitignore/blob/main/Global/macOS.gitignore
# General
.DS_Store
.AppleDouble
.LSOverride

# Icon must end with two \r
Icon


# Thumbnails
._*

# Files that might appear in the root of a volume
.DocumentRevisions-V100
.fseventsd
.Spotlight-V100
.TemporaryItems
.Trashes
.VolumeIcon.icns
.com.apple.timemachine.donotpresent

# Directories potentially created on remote AFP share
.AppleDB
.AppleDesktop
Network Trash Folder
Temporary Items
.apdisk

# content below from: https://github.com/github/gitignore/blob/main/Global/Windows.gitignore
# Windows thumbnail cache files
Thumbs.db
ehthumbs.db
ehthumbs_vista.db

# Dump file
*.stackdump

# Folder config file
[Dd]esktop.ini

# Recycle Bin used on file shares
$RECYCLE.BIN/

# Windows Installer files
*.cab
*.msi
*.msix
*.msm
*.msp

# Windows shortcuts
*.lnk

# Vim temporary swap files
*.swp
```

## File: src/Client/Vertigo.Client.Core/Navigation/NavigationPage.cs
```csharp
namespace Vertigo.Client.Core.Navigation;

public enum NavigationPage
{
    Access,
    Home,
    Play,
    Lobby,
    Social,
    Settings,
    Profile
}
```

## File: src/Client/Vertigo.Client.Networking/Callbacks/LobbyCallbackHandler.cs
```csharp
#nullable enable

using System;
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
public sealed class LobbyCallbackHandler : ILobbyCallback
{
    public event EventHandler<LobbyMember>? PlayerJoined;

    public event EventHandler<Guid>? PlayerLeft;

    public event EventHandler<LobbyChatMessage>? MessageReceived;

    public event EventHandler<LobbyPreparation>? PreparationChanged;

    void ILobbyCallback.PlayerJoined(LobbyMember player)
    {
        PlayerJoined?.Invoke(this, player);
    }

    void ILobbyCallback.PlayerLeft(Guid playerId)
    {
        PlayerLeft?.Invoke(this, playerId);
    }

    void ILobbyCallback.PreparationChanged(LobbyPreparation preparation)
    {
        PreparationChanged?.Invoke(this, preparation);
    }

    void ILobbyCallback.MessageReceived(LobbyChatMessage message)
    {
        MessageReceived?.Invoke(this, message);
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Callbacks/PendingLobbyEvent.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

public sealed record PendingLobbyEvent(LobbyMember? Joined, Guid LeftPlayerId, LobbyChatMessage? Chat, LobbyPreparation? Preparation = null);
```

## File: src/Client/Vertigo.Client.Networking/Interfaces/ILobbyClient.cs
```csharp
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyClient : IDisposable
{
    event EventHandler<LobbyMember>? PlayerJoined;

    event EventHandler<Guid>? PlayerLeft;

    event EventHandler<LobbyChatMessage>? MessageReceived;

    event EventHandler<LobbyPreparation>? PreparationChanged;

    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    bool LeaveLobby(string lobbyCode, string sessionToken);

    List<LobbySummary> GetPublicLobbies();

    LobbyPreparation GetPreparation(string lobbyCode, string sessionToken);

    LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken);

    LobbyPreparation StartMatch(string lobbyCode, string sessionToken);

    void ReplayPendingEvents();

    void Disconnect();
}
```

## File: src/Client/Vertigo.Client.Networking/Proxies/LobbyClient.cs
```csharp
using System.ServiceModel;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class LobbyClient : ILobbyClient
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    private LobbyCallbackHandler? _callbackHandler;
    private DuplexChannelFactory<ILobbyService>? _channelFactory;
    private readonly object _syncRoot = new();
    private readonly object _eventLock = new();
    private readonly Queue<PendingLobbyEvent> _pendingEvents = new();
    private EventHandler<LobbyMember>? _playerJoined;
    private EventHandler<Guid>? _playerLeft;
    private EventHandler<LobbyChatMessage>? _messageReceived;
    private EventHandler<LobbyPreparation>? _preparationChanged;
    private ILobbyService? _channel;
    private bool _isDisposed;

    public LobbyClient()
    {
    }

    public event EventHandler<LobbyMember>? PlayerJoined
    {
        add
        {
            lock (_eventLock)
            {
                _playerJoined += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _playerJoined -= value;
            }
        }
    }

    public event EventHandler<Guid>? PlayerLeft
    {
        add
        {
            lock (_eventLock)
            {
                _playerLeft += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _playerLeft -= value;
            }
        }
    }
    
    public event EventHandler<LobbyChatMessage>? MessageReceived
    {
        add
        {
            lock (_eventLock)
            {
                _messageReceived += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _messageReceived -= value;
            }
        }
    }

    public event EventHandler<LobbyPreparation>? PreparationChanged
    {
        add
        {
            lock (_eventLock)
            {
                _preparationChanged += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _preparationChanged -= value;
            }
        }
    }

    public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
    {
        ClearPendingEvents();
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.CreateLobby(isPublic, maxPlayers, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
    {
        ClearPendingEvents();
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.JoinLobby(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public bool LeaveLobby(string lobbyCode, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.LeaveLobby(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public List<LobbySummary> GetPublicLobbies()
    {
        ILobbyService channel = GetOrCreateChannel();
        List<LobbySummary> lobbies;

        try
        {
            lobbies = channel.GetPublicLobbies();
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }

        return lobbies;
    }

    public LobbyPreparation GetPreparation(string lobbyCode, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.GetPreparation(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.SetReady(lobbyCode, isReady, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public LobbyPreparation StartMatch(string lobbyCode, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.StartMatch(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public void ReplayPendingEvents()
    {
        lock (_eventLock)
        {
            while (_pendingEvents.Count > 0)
            {
                PendingLobbyEvent pending = _pendingEvents.Dequeue();

                if (pending.Joined is not null)
                {
                    _playerJoined?.Invoke(this, pending.Joined);
                }
                else if (pending.Preparation is not null)
                {
                    _preparationChanged?.Invoke(this, pending.Preparation);
                }
                else if (pending.Chat is not null)
                {
                    _messageReceived?.Invoke(this, pending.Chat);
                }
                else
                {
                    _playerLeft?.Invoke(this, pending.LeftPlayerId);
                }
            }
        }
    }

    public void Disconnect()
    {
        ILobbyService? oldChannel;
        DuplexChannelFactory<ILobbyService>? oldFactory;
        lock (_syncRoot)
        {
            oldChannel = _channel;
            oldFactory = _channelFactory;
            _channel = null;
            _channelFactory = null;
            _callbackHandler = null;
        }
        (oldChannel as ICommunicationObject)?.Abort();
        oldFactory?.Abort();
        ClearPendingEvents();
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
        }
        Disconnect();
    }

    private static bool IsTransportFailure(Exception exception)
    {
        bool isCommunicationFailure = exception is CommunicationException and not FaultException;
        bool isTimeout = exception is TimeoutException;

        return isCommunicationFailure || isTimeout;
    }

    private ILobbyService GetOrCreateChannel()
    {
        lock (_syncRoot)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_channel is not null && !IsUnusable((ICommunicationObject)_channel))
            {
                return _channel;
            }
            _channelFactory?.Abort();
            LobbyCallbackHandler handler = new();
            _callbackHandler = handler;
            handler.PlayerJoined += (sender, player) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnPlayerJoined(sender, player);
                }
            };
            handler.PlayerLeft += (sender, playerId) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnPlayerLeft(sender, playerId);
                }
            };
            handler.PreparationChanged += (sender, preparation) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnPreparationChanged(sender, preparation);
                }
            };
            handler.MessageReceived += (sender, message) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnChatReceived(sender, message);
                }
            };
            NetTcpBinding binding = new(SecurityMode.None)
            {
                OpenTimeout = OperationTimeout,
                SendTimeout = OperationTimeout
            };
            _channelFactory = new DuplexChannelFactory<ILobbyService>(
                new InstanceContext(handler), binding,
                new EndpointAddress(ServiceEndpoints.LobbyServiceAddress));
            _channel = _channelFactory.CreateChannel();
            return _channel;
        }
    }

    private void AbortChannel(ILobbyService channel)
    {
        ((ICommunicationObject)channel).Abort();

        lock (_syncRoot)
        {
            if (ReferenceEquals(_channel, channel))
            {
                _channel = null;
            }
        }
    }

    private static bool IsUnusable(ICommunicationObject communicationObject)
    {
        bool isUnusable = communicationObject.State is CommunicationState.Faulted
            or CommunicationState.Closing
            or CommunicationState.Closed;

        return isUnusable;
    }

    private static void CloseSafely(ICommunicationObject communicationObject)
    {
        try
        {
            communicationObject.Close();
        }
        catch (CommunicationException)
        {
            communicationObject.Abort();
        }
        catch (TimeoutException)
        {
            communicationObject.Abort();
        }
    }

    private void ClearPendingEvents()
    {
        lock (_eventLock)
        {
            _pendingEvents.Clear();
        }
    }

    // Events are raised inside the lock so replayed and live events keep the server's order;
    // subscribers must only marshal to the UI thread (Dispatcher.Post) and return.
    private void OnPlayerJoined(object? sender, LobbyMember player)
    {
        lock (_eventLock)
        {
            if (_playerJoined is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(player, Guid.Empty, null));
            }
            else
            {
                _playerJoined.Invoke(this, player);
            }
        }
    }

    private void OnPlayerLeft(object? sender, Guid playerId)
    {
        lock (_eventLock)
        {
            if (_playerLeft is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, playerId, null));
            }
            else
            {
                _playerLeft.Invoke(this, playerId);
            }
        }
    }
    private void OnPreparationChanged(object? sender, LobbyPreparation preparation)
    {
        lock (_eventLock)
        {
            if (_preparationChanged is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, Guid.Empty, null, preparation));
            }
            else
            {
                _preparationChanged.Invoke(this, preparation);
            }
        }
    }

    private void OnChatReceived(object? sender, LobbyChatMessage message)
    {
        lock (_eventLock)
        {
            if (_messageReceived is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, Guid.Empty, message));
            }
            else
            {
                _messageReceived.Invoke(this, message);
            }
        }
    }
}
```

## File: src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Client.Core\Vertigo.Client.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="System.ServiceModel.NetTcp" Version="10.1.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.1" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Dependencies/MainContext.cs
```csharp
namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MainContext(HomeViewModel Home, MainSocialContext SocialContext,
    MainScreensContext Screens)
{
    public LobbyViewModel Lobby => Screens.Lobby;
    public SettingsViewModel Settings => Screens.Settings;
    public ProfileViewModel Profile => Screens.Profile;
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Social/FriendsViewModel.cs
```csharp
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
    private Guid _selectedSearchPlayerId;
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
    public Guid SelectedSearchPlayerId
    {
        get => _selectedSearchPlayerId;
        set => SetProperty(ref _selectedSearchPlayerId, value);
    }

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
        if (IsBusy || SelectedSearchPlayerId == Guid.Empty) return;
        SessionSnapshot? session = Operation.GetRegisteredSession();
        if (session is null)
        {
            return;
        }
        string token = session.Token;
        Guid targetPlayerId = SelectedSearchPlayerId;
        IsBusy = true;
        try
        {
            ClientResult<PlayerRelation> result = await Task.Run(() => _client.SendFriendRequest(targetPlayerId, token));
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
        SelectedPlayerId = Guid.Empty;
        SelectedSearchPlayerId = Guid.Empty;
        PendingSenderId = Guid.Empty;
        SearchTerm = string.Empty; ClearError();
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/Social/LobbyInvitationsViewModel.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.ViewModels.Dependencies;
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
    private readonly Vertigo.Client.UI.ViewModels.LobbyViewModel _lobby;
    private string _lobbyCode = string.Empty;
    private string _declinedCode = string.Empty;
    private string _acceptedCode = string.Empty;
    private Guid _targetPlayerId;

    public LobbyInvitationsViewModel(ILobbyInvitationClient client, SocialOperation operation,
        LobbyInvitationContext context) : base(operation, context.Text)
    {
        _client = client;
        _lobby = context.Lobby;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        GetInvitableCommand = new AsyncRelayCommand(GetInvitableAsync);
        InviteCommand = new AsyncRelayCommand(InviteAsync);
        DeclineCommand = new AsyncRelayCommand(DeclineAsync);
        AcceptCommand = new AsyncRelayCommand(AcceptAsync);
    }

    public ObservableCollection<LobbyInvitationEntry> PendingInvitations { get; } = new();
    public ObservableCollection<FriendEntry> InvitableFriends { get; } = new();
    public string LobbyCode { get => _lobbyCode; set => SetProperty(ref _lobbyCode, value); }
    public string DeclinedCode { get => _declinedCode; set => SetProperty(ref _declinedCode, value); }
    public string AcceptedCode { get => _acceptedCode; set => SetProperty(ref _acceptedCode, value); }
    public Guid TargetPlayerId { get => _targetPlayerId; set => SetProperty(ref _targetPlayerId, value); }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand GetInvitableCommand { get; }
    public IAsyncRelayCommand InviteCommand { get; }
    public IAsyncRelayCommand DeclineCommand { get; }
    public IAsyncRelayCommand AcceptCommand { get; }

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

    // Joining the lobby is the server-side acceptance of an invitation.
    // The server checks the invitation, room capacity and session before admitting the player.
    public async Task<bool> AcceptInvitationAsync(string code)
    {
        bool accepted = false;
        SessionSnapshot? session = null;
        string normalizedCode = code?.Trim() ?? string.Empty;

        if (!IsBusy && !string.IsNullOrWhiteSpace(normalizedCode))
        {
            session = Operation.GetRegisteredSession();
            if (session is not null)
            {
                bool isPending = PendingInvitations.Any(invitation =>
                    string.Equals(invitation.LobbyCode, normalizedCode, StringComparison.OrdinalIgnoreCase));

                if (!isPending)
                {
                    ErrorMessage = Text.Get("Social.InvitationUnavailable");
                }
                else
                {
                    IsBusy = true;
                    ClearError();
                    try
                    {
                        if (Operation.IsCurrent(session))
                        {
                            bool joined = await _lobby.JoinAsync(normalizedCode);
                            if (Operation.IsCurrent(session))
                            {
                                if (joined)
                                {
                                    LobbyInvitationEntry? acceptedInvitation = PendingInvitations.FirstOrDefault(invitation =>
                                        string.Equals(invitation.LobbyCode, normalizedCode, StringComparison.OrdinalIgnoreCase));
                                    if (acceptedInvitation is not null)
                                    {
                                        PendingInvitations.Remove(acceptedInvitation);
                                    }
                                    accepted = true;
                                }
                                else
                                {
                                    ErrorMessage = _lobby.ErrorMessage;
                                }
                            }
                        }
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
        }

        return accepted;
    }

    private async Task AcceptAsync()
    {
        await AcceptInvitationAsync(AcceptedCode);
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
        LobbyCode = string.Empty; DeclinedCode = string.Empty; AcceptedCode = string.Empty;
        TargetPlayerId = Guid.Empty; ClearError();
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/HomeViewModel.cs
```csharp
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels;

public sealed class HomeViewModel : ObservableObject, INavigationAware
{
    private readonly ISessionCoordinator _sessions;
    private readonly PlayerSession _playerSession;
    private readonly INavigator _navigation;
    private readonly LobbyViewModel _lobby;
    private readonly IClientErrorText _errors;
    private readonly ISessionGuard _coordinator;
    private readonly ITextLocalizer _text;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _isNotificationsOpen;

    public HomeViewModel(HomeContext context, LobbyViewModel lobby, PlayViewModel play)
    {
        _sessions = context.Sessions;
        _playerSession = context.PlayerSession;
        _navigation = context.Ui.Navigation;
        _lobby = lobby;
        Play = play;
        _errors = context.Ui.Services.Errors;
        _coordinator = context.Ui.Services.SessionGuard;
        _text = context.Ui.Services.Text;
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
        OpenSocialCommand = new RelayCommand(OpenSocial);
        OpenPlayCommand = new RelayCommand(OpenPlay);
        ToggleNotificationsCommand = new RelayCommand(ToggleNotifications);
        OpenProfileCommand = new RelayCommand(OpenProfile);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
    }

    public PlayViewModel Play { get; }
    public IRelayCommand OpenPlayCommand { get; }

    public string Username => _playerSession.GetActiveSession()?.Username ?? string.Empty;
    public bool IsGuest => _playerSession.GetActiveSession()?.IsGuest ?? false;

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

    public IAsyncRelayCommand LogoutCommand { get; }
    public IRelayCommand OpenSocialCommand { get; }
    public IRelayCommand ToggleNotificationsCommand { get; }
    public IRelayCommand OpenProfileCommand { get; }
    public IRelayCommand OpenSettingsCommand { get; }

    public bool IsNotificationsOpen
    {
        get => _isNotificationsOpen;
        private set => SetProperty(ref _isNotificationsOpen, value);
    }

    public string PlayerTag => string.IsNullOrEmpty(Username) ? string.Empty : "@" + Username;
    public string AccountKindText => _text.Get(IsGuest ? "Home.GuestAccount" : "Home.RegisteredAccount");
    public string BrandSubtitle => _text.Get("Home.BrandSubtitle");
    public string PlayText => _text.Get("Home.Play");
    public string SocialText => _text.Get("Home.Social");
    public string RankingText => _text.Get("Home.Ranking");
    public string SettingsText => _text.Get("Home.Settings");
    public string ProfileText => _text.Get("Home.Profile");
    public string NotificationsText => _text.Get("Home.Notifications");
    public string NoNotificationsText => _text.Get("Home.NotificationsHelp");
    public string OpenSocialNotificationsText => _text.Get("Home.OpenSocialNotifications");
    public string GlossaryText => _text.Get("Home.Glossary");
    public string LogoutText => _text.Get("Home.Logout");
    public string CloseText => _text.Get("Home.Close");
    public string NotAvailableText => _text.Get("Home.NotAvailable");
    public string VersionText => "v0.1.0";


    public void OnNavigatedTo()
    {
        ErrorMessage = string.Empty;
        IsNotificationsOpen = false;
        if (_coordinator.HasActiveSession())
        {
            RefreshIdentity();
            NotifyLanguageChanged();
        }
    }

    public void OnNavigatedFrom()
    {
        ErrorMessage = string.Empty;
        IsNotificationsOpen = false;
    }

    public void RefreshIdentity()
    {
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(IsGuest));
        OnPropertyChanged(nameof(PlayerTag));
        OnPropertyChanged(nameof(AccountKindText));
    }

    private void OpenPlay()
    {
        if (!IsBusy && _coordinator.HasActiveSession())
        {
            _navigation.NavigateTo(NavigationPage.Play);
        }
    }

    private void ToggleNotifications()
    {
        IsNotificationsOpen = !IsNotificationsOpen;
    }

    private void OpenProfile()
    {
        if (!IsBusy && _coordinator.HasActiveSession())
        {
            _navigation.NavigateTo(NavigationPage.Profile);
        }
    }

    private void OpenSettings()
    {
        if (!IsBusy && _coordinator.HasActiveSession())
        {
            _navigation.NavigateTo(NavigationPage.Settings);
        }
    }

    private void NotifyLanguageChanged()
    {
        OnPropertyChanged(nameof(AccountKindText));
        OnPropertyChanged(nameof(BrandSubtitle));
        OnPropertyChanged(nameof(PlayText));
        OnPropertyChanged(nameof(SocialText));
        OnPropertyChanged(nameof(RankingText));
        OnPropertyChanged(nameof(SettingsText));
        OnPropertyChanged(nameof(ProfileText));
        OnPropertyChanged(nameof(NotificationsText));
        OnPropertyChanged(nameof(NoNotificationsText));
        OnPropertyChanged(nameof(OpenSocialNotificationsText));
        OnPropertyChanged(nameof(LogoutText));
        OnPropertyChanged(nameof(GlossaryText));
        OnPropertyChanged(nameof(CloseText));
        OnPropertyChanged(nameof(NotAvailableText));
    }

    private void OpenSocial()
    {
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
            return;
        }
        if (session.IsGuest)
        {
            ErrorMessage = _text.Get("Social.RegisteredOnly");
            return;
        }
        _navigation.NavigateTo(NavigationPage.Social);
    }

    private async Task LogoutAsync()
    {
        if (IsBusy || !string.IsNullOrEmpty(_lobby.LobbyCode)) return;
        if (!_coordinator.HasActiveSession()) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            SessionSnapshot? session = _playerSession.GetActiveSession();
            ClientResult<bool> result = await Task.Run(_sessions.Logout);
            if (_errors.IsUnauthorized(result)) { _coordinator.Invalidate(session); return; }
            if (result.IsSuccess && result.Value) _coordinator.Invalidate(session);
            else ErrorMessage = _errors.FromResult(result, "Home.LogoutFailed");
        }
        catch (Exception exception) { ErrorMessage = _errors.FromException(exception, "Home.LogoutFailed"); }
        finally { IsBusy = false; }
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/LobbyViewModel.cs
```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Vertigo.Client.UI.ViewModels.Presentation;
using System.ServiceModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels;

public sealed class LobbyViewModel : ObservableObject, IDisposable, INavigationAware
{
    private readonly ILobbyClient _lobbyClient;
    private readonly IChatClient _chatClient;
    private readonly PlayerSession _playerSession;
    private readonly INavigator _navigation;
    private readonly IClientErrorText _errors;
    private readonly ISessionGuard _coordinator;
    private readonly ITextLocalizer _text;
    private string _lobbyCode = string.Empty;
    private string _messageText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private Guid _hostPlayerId;
    private bool _isMatchStarted;
    private bool _disposed;
    private long _lobbyGeneration;
    private readonly LobbyPreparationRevision _preparationRevision = new();

    public LobbyViewModel(ILobbyClient lobbyClient, IChatClient chatClient, LobbyContext context)
    {
        _lobbyClient = lobbyClient;
        _chatClient = chatClient;
        _playerSession = context.PlayerSession;
        _navigation = context.Ui.Navigation;
        _errors = context.Ui.Services.Errors;
        _coordinator = context.Ui.Services.SessionGuard;
        _text = context.Ui.Services.Text;
        Members.CollectionChanged += (_, _) => RefreshMemberRows();
        _lobbyClient.PlayerJoined += OnPlayerJoined;
        _lobbyClient.PlayerLeft += OnPlayerLeft;
        _lobbyClient.MessageReceived += OnMessageReceived;
        _lobbyClient.PreparationChanged += OnPreparationChanged;
        SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);
        LeaveCommand = new AsyncRelayCommand(LeaveAsync);
        ToggleReadyCommand = new AsyncRelayCommand(ToggleReadyAsync);
        StartMatchCommand = new AsyncRelayCommand(StartMatchAsync);
    }

    public ObservableCollection<LobbyMember> Members { get; } = new();
    public ObservableCollection<LobbyMemberRow> MemberRows { get; } = new();
    public ObservableCollection<LobbyChatMessage> Messages { get; } = new();
    public ObservableCollection<Guid> ReadyPlayerIds { get; } = new();

    public Guid HostPlayerId
    {
        get => _hostPlayerId;
        private set => SetProperty(ref _hostPlayerId, value);
    }

    public bool IsMatchStarted
    {
        get => _isMatchStarted;
        private set
        {
            if (SetProperty(ref _isMatchStarted, value))
            {
                OnPropertyChanged(nameof(MatchStatusText));
                OnPropertyChanged(nameof(CanPrepare));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string HostUsername => Members.FirstOrDefault(m => m.PlayerId == HostPlayerId)?.Username ?? string.Empty;
    public bool IsHost => _playerSession.GetActiveSession()?.PlayerId == HostPlayerId;
    public bool IsReady => _playerSession.GetActiveSession() is { } active && ReadyPlayerIds.Contains(active.PlayerId);

    public string LobbyCode
    {
        get => _lobbyCode;
        private set => SetProperty(ref _lobbyCode, value);
    }

    public string MessageText
    {
        get => _messageText;
        set => SetProperty(ref _messageText, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(CanPrepare));
            }
        }
    }

    public bool CanPrepare => !IsBusy && !IsMatchStarted;

    public string RoomTitleText => _text.Get("Room.Title");
    public string CodeTitleText => _text.Get("Room.Code");
    public string LeaveText => _text.Get("Room.Leave");
    public string MembersTitleText => _text.Get("Room.Members");
    public string MembersDescriptionText => _text.Get("Room.MembersDescription");
    public string PreparationTitleText => _text.Get("Room.Preparation");
    public string PreparationHintText => _text.Get("Room.PreparationHint");
    public string HostLabelText => _text.Get("Room.Host");
    public string MatchStatusText => _text.Get(IsMatchStarted ? "Room.InGame" : "Room.Waiting");
    public string ReadyActionText => _text.Get(IsReady ? "Room.NotReadyAction" : "Room.ReadyAction");
    public string StartText => _text.Get("Room.Start");
    public string ChatTitleText => _text.Get("Room.Chat");
    public string MessagePlaceholderText => _text.Get("Room.MessagePlaceholder");
    public string SendText => _text.Get("Room.Send");
    public string ErrorHeadingText => _text.Get("Room.ErrorHeading");

    public IAsyncRelayCommand SendMessageCommand { get; }
    public IAsyncRelayCommand LeaveCommand { get; }
    public IAsyncRelayCommand ToggleReadyCommand { get; }
    public IAsyncRelayCommand StartMatchCommand { get; }

    public async Task<bool> CreateAsync(bool isPublic, int maxPlayers)
    {
        if (IsBusy || !string.IsNullOrEmpty(LobbyCode)) return false;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return false; }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            string code = await Task.Run(() => _lobbyClient.CreateLobby(isPublic, maxPlayers, session.Token));
            if (!IsCurrentSession(session))
            {
                return false;
            }
            Members.Clear();
            Messages.Clear();
            Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                IsGuest = session.IsGuest });
            LobbyCode = code;
            HostPlayerId = session.PlayerId;
            ReadyPlayerIds.Clear();
            _preparationRevision.Reset();
            IsMatchStarted = false;
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
            await RefreshPreparationAsync(session);
            return true;
        }
        catch (Exception exception)
        {
            return HandleServiceFailure(exception, session, "Lobby.CreateFailed");
        }
        finally { IsBusy = false; }
    }

    public async Task<bool> JoinAsync(string code)
    {
        if (IsBusy || !string.IsNullOrEmpty(LobbyCode)) return false;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return false; }
        if (string.IsNullOrWhiteSpace(code)) return Fail("Lobby.CodeRequired");

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            LobbyAdmission admission = await Task.Run(() => _lobbyClient.JoinLobby(code.Trim(), session.Token));
            if (!IsCurrentSession(session))
            {
                return false;
            }
            Members.Clear();
            Messages.Clear();
            foreach (LobbyMember member in admission.ExistingMembers) Members.Add(member);
            Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                IsGuest = session.IsGuest });
            LobbyCode = admission.LobbyCode;
            HostPlayerId = admission.HostPlayerId;
            ReadyPlayerIds.Clear();
            _preparationRevision.Reset();
            IsMatchStarted = false;
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
            await RefreshPreparationAsync(session);
            return true;
        }
        catch (Exception exception)
        {
            return HandleServiceFailure(exception, session, "Lobby.JoinFailed");
        }
        finally { IsBusy = false; }
    }

    private async Task RefreshPreparationAsync(SessionSnapshot session)
    {
        string code = LobbyCode;
        try
        {
            LobbyPreparation preparation = await Task.Run(() => _lobbyClient.GetPreparation(code, session.Token));
            if (IsCurrentSession(session) && string.Equals(LobbyCode, code, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        }
        catch (Exception exception)
        {
            // Creation/join already succeeded; do not report a failed admission.
            if (_errors.IsUnauthorized(exception))
            {
                _coordinator.Invalidate(session);
            }
        }
    }

    private async Task ToggleReadyAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(LobbyCode) || IsMatchStarted)
        {
            return;
        }
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
            return;
        }
        IsBusy = true;
        string code = LobbyCode;
        bool ready = !IsReady;
        try
        {
            LobbyPreparation preparation = await Task.Run(() => _lobbyClient.SetReady(code, ready, session.Token));
            if (IsCurrentSession(session) && string.Equals(LobbyCode, code, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        }
        catch (Exception exception)
        {
            HandleServiceFailure(exception, session, "Lobby.ReadyFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task StartMatchAsync()
    {
        if (IsBusy || !IsHost || IsMatchStarted || string.IsNullOrEmpty(LobbyCode))
        {
            return;
        }
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
            return;
        }
        IsBusy = true;
        string code = LobbyCode;
        try
        {
            LobbyPreparation preparation = await Task.Run(() => _lobbyClient.StartMatch(code, session.Token));
            if (IsCurrentSession(session) && string.Equals(LobbyCode, code, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        }
        catch (Exception exception)
        {
            HandleServiceFailure(exception, session, "Lobby.StartFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPreparationChanged(object? sender, LobbyPreparation preparation)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        long eventGeneration = _lobbyGeneration;
        Dispatcher.UIThread.Post(() =>
        {
            if (!_disposed && eventSession is not null && IsCurrentSession(eventSession)
                && eventGeneration == _lobbyGeneration
                && string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        });
    }

    private void ApplyPreparation(LobbyPreparation preparation)
    {
        if (!_preparationRevision.TryAdvance(preparation, LobbyCode))
        {
            return;
        }
        HostPlayerId = preparation.HostPlayerId;
        IsMatchStarted = preparation.IsInGame;
        ReadyPlayerIds.Clear();
        foreach (Guid playerId in preparation.ReadyPlayerIds)
        {
            ReadyPlayerIds.Add(playerId);
        }
        RefreshMemberRows();
        OnPropertyChanged(nameof(IsHost));
        OnPropertyChanged(nameof(IsReady));
        OnPropertyChanged(nameof(ReadyActionText));
    }

    private async Task SendMessageAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(MessageText) || string.IsNullOrEmpty(LobbyCode)) return;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return; }
        string text = MessageText.Trim();
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var result = await Task.Run(() => _chatClient.SendMessage(LobbyCode, text, session.Token));
            if (_errors.IsUnauthorized(result)) { _coordinator.Invalidate(session); return; }
            if (!IsCurrentSession(session))
            {
                return;
            }
            if (result.IsSuccess && result.Value) MessageText = string.Empty;
            else ErrorMessage = _errors.FromResult(result, "Lobby.MessageFailed");
        }
        catch (Exception exception) { HandleServiceFailure(exception, session, "Lobby.MessageFailed"); }
        finally { IsBusy = false; }
    }

    private async Task LeaveAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(LobbyCode)) return;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return; }
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            bool left = await Task.Run(() => _lobbyClient.LeaveLobby(LobbyCode, session.Token));
            if (!IsCurrentSession(session))
            {
                return;
            }
            if (!left) { Fail("Lobby.LeaveFailed"); return; }
            _lobbyClient.Disconnect();
            ResetState();
            _navigation.NavigateTo(NavigationPage.Home);
        }
        catch (Exception exception)
        {
            HandleServiceFailure(exception, session, "Lobby.LeaveFailed");
        }
        finally { IsBusy = false; }
    }

    private void OnPlayerJoined(object? sender, LobbyMember member)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !ReferenceEquals(_playerSession.GetActiveSession(), eventSession)
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (_disposed || string.IsNullOrEmpty(LobbyCode)) return;
            if (Members.All(existing => existing.PlayerId != member.PlayerId)) Members.Add(member);
        });
    }

    private void OnPlayerLeft(object? sender, Guid playerId)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !ReferenceEquals(_playerSession.GetActiveSession(), eventSession)
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (_disposed || string.IsNullOrEmpty(LobbyCode)) return;
            LobbyMember? member = Members.FirstOrDefault(existing => existing.PlayerId == playerId);
            if (member is not null) Members.Remove(member);
        });
    }

    private void OnMessageReceived(object? sender, LobbyChatMessage message)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !ReferenceEquals(_playerSession.GetActiveSession(), eventSession)
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (!_disposed && string.Equals(LobbyCode, message.LobbyCode, StringComparison.OrdinalIgnoreCase))
                Messages.Add(message);
        });
    }

    public void OnNavigatedTo()
    {
        RefreshRoomTexts();
        if (!_coordinator.HasActiveSession()) return;
        ErrorMessage = string.Empty;
    }

    public void OnNavigatedFrom()
    {
        // Keep the lobby connection only when moving back from a transient view.
        if (_navigation.CurrentPage == NavigationPage.Access) ResetState();
    }

    public void ResetState()
    {
        _lobbyGeneration++;
        _preparationRevision.Reset();
        LobbyCode = string.Empty;
        MessageText = string.Empty;
        ErrorMessage = string.Empty;
        Members.Clear();
        Messages.Clear();
        ReadyPlayerIds.Clear();
        HostPlayerId = Guid.Empty;
        IsMatchStarted = false;
        RefreshMemberRows();
        OnPropertyChanged(nameof(IsHost));
        OnPropertyChanged(nameof(IsReady));
    }

    private bool IsCurrentSession(SessionSnapshot previous)
    {
        SessionSnapshot? current = _playerSession.GetActiveSession();
        return ReferenceEquals(current, previous);
    }

    private bool HandleServiceFailure(Exception exception, SessionSnapshot session, string fallbackKey)
    {
        if (_errors.IsUnauthorized(exception) || !_coordinator.HasActiveSession())
        {
            _coordinator.Invalidate(session);
            return false;
        }

        ErrorMessage = _errors.FromException(exception, fallbackKey);
        return false;
    }

    private bool Fail(string key)
    {
        ErrorMessage = _text.Get(key);
        return false;
    }

    private void RefreshMemberRows()
    {
        MemberRows.Clear();
        foreach (LobbyMember member in Members)
        {
            bool host = member.PlayerId == HostPlayerId;
            bool ready = ReadyPlayerIds.Contains(member.PlayerId);
            MemberRows.Add(new LobbyMemberRow
            {
                Username = member.Username,
                RoleText = _text.Get(host ? "Room.Host" : member.IsGuest ? "Room.Guest" : "Room.Player"),
                ReadyText = _text.Get(ready ? "Room.Ready" : "Room.NotReady"),
                IsReady = ready
            });
        }
        OnPropertyChanged(nameof(HostUsername));
    }

    private void RefreshRoomTexts()
    {
        OnPropertyChanged(nameof(RoomTitleText));
        OnPropertyChanged(nameof(CodeTitleText));
        OnPropertyChanged(nameof(LeaveText));
        OnPropertyChanged(nameof(MembersTitleText));
        OnPropertyChanged(nameof(MembersDescriptionText));
        OnPropertyChanged(nameof(PreparationTitleText));
        OnPropertyChanged(nameof(PreparationHintText));
        OnPropertyChanged(nameof(HostLabelText));
        OnPropertyChanged(nameof(MatchStatusText));
        OnPropertyChanged(nameof(ReadyActionText));
        OnPropertyChanged(nameof(StartText));
        OnPropertyChanged(nameof(ChatTitleText));
        OnPropertyChanged(nameof(MessagePlaceholderText));
        OnPropertyChanged(nameof(SendText));
        OnPropertyChanged(nameof(ErrorHeadingText));
        RefreshMemberRows();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lobbyClient.PlayerJoined -= OnPlayerJoined;
        _lobbyClient.PlayerLeft -= OnPlayerLeft;
        _lobbyClient.MessageReceived -= OnMessageReceived;
        _lobbyClient.PreparationChanged -= OnPreparationChanged;
    }
}
```

## File: src/Client/Vertigo.Client.UI/ViewModels/SocialViewModel.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.ViewModels.Social;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed class SocialViewModel : ObservableObject, INavigationAware, IDisposable
{
    private readonly ITextLocalizer _text;
    private FriendEntry? _selectedFriend;
    private PlayerSearchEntry? _selectedSearch;
    private FriendshipInvitationEntry? _selectedRequest;
    private LobbyInvitationEntry? _selectedInvitation;
    private BlockedPlayerSummary? _selectedBlocked;
    private readonly IFriendshipNotificationClient _notifications;
    private readonly PlayerSession _session;
    private readonly INavigator _navigation;
    private readonly ISessionGuard _guard;
    private readonly IClientErrorText _errors;
    private bool _disposed;
    private bool _active;
    private bool _subscribed;
    private bool _isRefreshing;
    private int _generation;
    private SessionSnapshot? _subscribedSession;
    private string _errorMessage = string.Empty;

    public SocialViewModel(FriendsViewModel friends, BlockedPlayersViewModel blocked,
        SocialViewModelContext context)
    {
        Friends = friends;
        Blocked = blocked;
        Invitations = context.Invitations;
        _text = context.NavigationContext.Services.Text;
        _notifications = context.Notifications;
        _session = context.NavigationContext.Session;
        _navigation = context.NavigationContext.Navigation;
        _guard = context.NavigationContext.Services.SessionGuard;
        _errors = context.NavigationContext.Services.Errors;
        BackCommand = new RelayCommand(() => _navigation.NavigateTo(NavigationPage.Home));
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        _notifications.FriendRequestReceived += OnFriendRequest;
        _notifications.FriendshipEstablished += OnFriendship;
        _notifications.LobbyInvitationReceived += OnInvitation;
        _notifications.LobbyInvitationWithdrawn += OnInvitationWithdrawn;
    }

    public string HeadingText => _text.Get("SocialUi.Title");
    public string BackText => _text.Get("Navigation.Back");
    public string RefreshText => _text.Get("SocialUi.Refresh");
    public string FriendsTabText => _text.Get("SocialUi.Friends");
    public string RequestsTabText => _text.Get("SocialUi.Requests");
    public string InvitationsTabText => _text.Get("SocialUi.Invitations");
    public string BlockedTabText => _text.Get("SocialUi.Blocked");
    public string SearchText => _text.Get("SocialUi.Search");
    public string SearchResultsText => _text.Get("SocialUi.SearchResults");
    public string RemoveFriendText => _text.Get("SocialUi.RemoveFriend");
    public string SendRequestText => _text.Get("SocialUi.SendRequest");
    public string AcceptText => _text.Get("SocialUi.Accept");
    public string RejectText => _text.Get("SocialUi.Reject");
    public string UnblockText => _text.Get("SocialUi.Unblock");

    public FriendEntry? SelectedFriend
    {
        get => _selectedFriend;
        set
        {
            if (SetProperty(ref _selectedFriend, value))
            {
                Friends.SelectedPlayerId = value?.PlayerId ?? Guid.Empty;
            }
        }
    }

    public PlayerSearchEntry? SelectedSearch
    {
        get => _selectedSearch;
        set
        {
            if (SetProperty(ref _selectedSearch, value))
            {
                Friends.SelectedSearchPlayerId = value?.PlayerId ?? Guid.Empty;
            }
        }
    }

    public FriendshipInvitationEntry? SelectedRequest
    {
        get => _selectedRequest;
        set
        {
            if (SetProperty(ref _selectedRequest, value))
            {
                Friends.PendingSenderId = value?.SenderId ?? Guid.Empty;
            }
        }
    }

    public LobbyInvitationEntry? SelectedInvitation
    {
        get => _selectedInvitation;
        set
        {
            if (SetProperty(ref _selectedInvitation, value))
            {
                string code = value?.LobbyCode ?? string.Empty;
                Invitations.AcceptedCode = code;
                Invitations.DeclinedCode = code;
            }
        }
    }

    public BlockedPlayerSummary? SelectedBlocked
    {
        get => _selectedBlocked;
        set
        {
            if (SetProperty(ref _selectedBlocked, value))
            {
                Blocked.SelectedPlayerId = value?.PlayerId ?? Guid.Empty;
            }
        }
    }

    private void RefreshViewText()
    {
        OnPropertyChanged(nameof(HeadingText));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(RefreshText));
        OnPropertyChanged(nameof(FriendsTabText));
        OnPropertyChanged(nameof(RequestsTabText));
        OnPropertyChanged(nameof(InvitationsTabText));
        OnPropertyChanged(nameof(BlockedTabText));
        OnPropertyChanged(nameof(SearchText));
        OnPropertyChanged(nameof(SearchResultsText));
        OnPropertyChanged(nameof(RemoveFriendText));
        OnPropertyChanged(nameof(SendRequestText));
        OnPropertyChanged(nameof(AcceptText));
        OnPropertyChanged(nameof(RejectText));
        OnPropertyChanged(nameof(UnblockText));
    }

    public FriendsViewModel Friends { get; }
    public BlockedPlayersViewModel Blocked { get; }
    public LobbyInvitationsViewModel Invitations { get; }
    public string ErrorMessage { get => _errorMessage; private set => SetProperty(ref _errorMessage, value); }
    public IRelayCommand BackCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    public void OnNavigatedTo()
    {
        RefreshViewText();
        _active = true;
        _ = RefreshAsync(); // Every exception is handled inside RefreshAsync.
    }

    public void OnNavigatedFrom()
    {
        SelectedFriend = null;
        SelectedSearch = null;
        SelectedRequest = null;
        SelectedInvitation = null;
        SelectedBlocked = null;
        _active = false;
        _generation++;
        _isRefreshing = false;
        _subscribed = false;
        _subscribedSession = null;
        _notifications.Disconnect();
    }

    public async Task RefreshAsync()
    {
        if (_disposed || !_active || _isRefreshing)
        {
            return;
        }
        _isRefreshing = true;
        int generation = _generation;
        try
        {
            SessionSnapshot? session = _session.GetActiveSession();
            if (session is null)
            {
                _guard.Invalidate();
                return;
            }
            if (session.IsGuest)
            {
                _navigation.NavigateTo(NavigationPage.Home);
                return;
            }
            if (!_subscribed || !ReferenceEquals(session, _subscribedSession))
            {
                try
                {
                    bool accepted = await Task.Run(() => _notifications.Subscribe(session.Token));
                    if (!IsActive(session, generation))
                    {
                        return;
                    }
                    if (!accepted)
                    {
                        ErrorMessage = _errors.FromException(new InvalidOperationException(),
                            "Social.NotificationsFailed");
                        return;
                    }
                    _subscribed = true;
                    _subscribedSession = session;
                }
                catch (Exception exception)
                {
                    if (!IsActive(session, generation))
                    {
                        return;
                    }
                    if (_errors.IsUnauthorized(exception))
                    {
                        _guard.Invalidate(session);
                        return;
                    }
                    ErrorMessage = _errors.FromException(exception, "Social.NotificationsFailed");
                    return;
                }
            }
            if (!IsActive(session, generation))
            {
                return;
            }
            await Friends.RefreshAsync();
            if (!IsActive(session, generation))
            {
                return;
            }
            await Blocked.RefreshAsync();
            if (!IsActive(session, generation))
            {
                return;
            }
            await Invitations.RefreshAsync();
        }
        finally
        {
            if (generation == _generation)
            {
                _isRefreshing = false;
            }
        }
    }

    private bool IsActive(SessionSnapshot session, int generation)
    {
        return !_disposed && _active && generation == _generation
            && ReferenceEquals(_session.GetActiveSession(), session);
    }

    private void OnFriendRequest(object? sender, FriendshipInvitationEntry entry)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            if (Friends.PendingRequests.All(item => item.SenderId != entry.SenderId)) Friends.PendingRequests.Add(entry);
        });
    }

    private void OnFriendship(object? sender, FriendEntry friend)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            if (Friends.Friends.All(item => item.PlayerId != friend.PlayerId)) Friends.Friends.Add(friend);
        });
    }

    private void OnInvitation(object? sender, LobbyInvitationEntry invitation)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            if (Invitations.PendingInvitations.All(item => item.LobbyCode != invitation.LobbyCode))
                Invitations.PendingInvitations.Add(invitation);
        });
    }

    private void OnInvitationWithdrawn(object? sender, string code)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            LobbyInvitationEntry? item = Invitations.PendingInvitations.FirstOrDefault(
                invitation => invitation.LobbyCode == code);
            if (item is not null) Invitations.PendingInvitations.Remove(item);
        });
    }

    public void Reset()
    {
        _active = false;
        _generation++;
        _isRefreshing = false;
        _subscribed = false;
        _subscribedSession = null;
        _notifications.Disconnect();
        ErrorMessage = string.Empty;
        Friends.Reset(); Blocked.Reset(); Invitations.Reset();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _notifications.FriendRequestReceived -= OnFriendRequest;
        _notifications.FriendshipEstablished -= OnFriendship;
        _notifications.LobbyInvitationReceived -= OnInvitation;
        _notifications.LobbyInvitationWithdrawn -= OnInvitationWithdrawn;
    }
}
```

## File: src/Client/Vertigo.Client.UI/Vertigo.Client.UI.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ApplicationManifest>app.manifest</ApplicationManifest>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <Folder Include="Models\" />
    <AvaloniaResource Include="Assets\**" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.1.6" />
    <PackageReference Include="Avalonia.Desktop" Version="12.1.6" />
    <PackageReference Include="Avalonia.Themes.Fluent" Version="12.1.6" />
    <PackageReference Include="Avalonia.Fonts.Inter" Version="12.1.6" />
    <PackageReference Include="AvaloniaUI.DiagnosticsSupport" Version="2.2.3">
      <IncludeAssets Condition="'$(Configuration)' != 'Debug'">None</IncludeAssets>
      <PrivateAssets Condition="'$(Configuration)' != 'Debug'">All</PrivateAssets>
    </PackageReference>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" />
  </ItemGroup>
</Project>
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/BlockedPlayerRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class BlockedPlayerRepository : IBlockedPlayerRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public BlockedPlayerRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public List<BlockedPlayer> GetBlockedPlayers(Guid playerId)
    {
        List<BlockedPlayer> blockedPlayers = _context.BlockedPlayers
            .AsNoTracking()
            .Include(b => b.PlayerBlocked)
            .Where(b => b.PlayerBlockerId == playerId)
            .OrderBy(b => b.PlayerBlocked.Username)
            .ToList();

        return blockedPlayers;
    }

    public bool IsPlayerBlocked(Guid playerId, Guid otherPlayerId)
    {
        bool isBlocked = _context.BlockedPlayers
            .Any(b => (b.PlayerBlockerId == playerId && b.PlayerBlockedId == otherPlayerId)
                      || (b.PlayerBlockerId == otherPlayerId && b.PlayerBlockedId == playerId));

        return isBlocked;
    }

    public bool BlockPlayer(Guid playerId, Guid otherPlayerId)
    {
        BlockedPlayer blockedPlayer = new BlockedPlayer
        {
            PlayerBlockerId = playerId,
            PlayerBlockedId = otherPlayerId
        };

        using IDbContextTransaction transaction = _context.Database.BeginTransaction();

        _context.Friendships
            .Where(friendship => (friendship.PlayerRequesterId == playerId
                                  && friendship.PlayerAddresseeId == otherPlayerId)
                                 || (friendship.PlayerRequesterId == otherPlayerId
                                     && friendship.PlayerAddresseeId == playerId))
            .ExecuteDelete();

        _context.BlockedPlayers.Add(blockedPlayer);
        int affectedRows = _context.SaveChanges();
        transaction.Commit();

        return affectedRows == ExpectedAffectedRows;
    }

    public bool UnblockPlayer(Guid playerId, Guid otherPlayerId)
    {
        int affectedRows = _context.BlockedPlayers
            .Where(b => b.PlayerBlockerId == playerId && b.PlayerBlockedId == otherPlayerId)
            .ExecuteDelete();

        return affectedRows == ExpectedAffectedRows;
    }

    public List<BlockedPlayer> GetBlocksBetween(Guid playerId, List<Guid> otherPlayerIds)
    {
        return _context.BlockedPlayers.AsNoTracking()
            .Where(block => (block.PlayerBlockerId == playerId && otherPlayerIds.Contains(block.PlayerBlockedId))
                            || (block.PlayerBlockedId == playerId && otherPlayerIds.Contains(block.PlayerBlockerId)))
            .ToList();
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/FriendshipRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class FriendshipRepository : IFriendshipRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public FriendshipRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public Friendship? GetFriendshipBetween(Guid playerId, Guid otherPlayerId)
    {
        Friendship? friendship = _context.Friendships
            .AsNoTracking()
            .FirstOrDefault(f => (f.PlayerRequesterId == playerId && f.PlayerAddresseeId == otherPlayerId)
                                 || (f.PlayerRequesterId == otherPlayerId && f.PlayerAddresseeId == playerId));

        return friendship;
    }

    public List<Friendship> GetFriendshipsBetween(Guid playerId, List<Guid> otherPlayerIds)
    {
        List<Friendship> friendships = _context.Friendships
            .AsNoTracking()
            .Where(f => (f.PlayerRequesterId == playerId && otherPlayerIds.Contains(f.PlayerAddresseeId))
                        || (f.PlayerAddresseeId == playerId && otherPlayerIds.Contains(f.PlayerRequesterId)))
            .ToList();

        return friendships;
    }

    public List<Friendship> GetAcceptedFriendships(Guid playerId)
    {
        List<Friendship> friendships = _context.Friendships
            .AsNoTracking()
            .Include(f => f.PlayerRequester)
            .Include(f => f.PlayerAddressee)
            .Where(f => (f.PlayerRequesterId == playerId || f.PlayerAddresseeId == playerId)
                        && f.Status == FriendshipStatus.Accepted)
            .ToList();

        return friendships;
    }

    public List<Friendship> GetPendingRequestsReceivedBy(Guid addresseeId)
    {
        List<Friendship> friendships = _context.Friendships
            .AsNoTracking()
            .Include(f => f.PlayerRequester)
            .Where(f => f.PlayerAddresseeId == addresseeId && f.Status == FriendshipStatus.Pending)
            .OrderBy(f => f.PlayerRequester.Username)
            .ToList();

        return friendships;
    }

    public bool CreatePendingFriendship(Guid requesterId, Guid addresseeId)
    {
        Friendship friendship = new Friendship
        {
            PlayerRequesterId = requesterId,
            PlayerAddresseeId = addresseeId,
            Status = FriendshipStatus.Pending
        };

        _context.Friendships.Add(friendship);
        int affectedRows = _context.SaveChanges();

        return affectedRows == ExpectedAffectedRows;
    }

    public bool AcceptPendingFriendship(Guid requesterId, Guid addresseeId)
    {
        int affectedRows = _context.Friendships
            .Where(f => f.PlayerRequesterId == requesterId
                        && f.PlayerAddresseeId == addresseeId
                        && f.Status == FriendshipStatus.Pending)
            .ExecuteUpdate(setters => setters.SetProperty(f => f.Status, FriendshipStatus.Accepted));

        return affectedRows == ExpectedAffectedRows;
    }

    public bool DeletePendingFriendship(Guid requesterId, Guid addresseeId)
    {
        int affectedRows = _context.Friendships
            .Where(f => f.PlayerRequesterId == requesterId
                        && f.PlayerAddresseeId == addresseeId
                        && f.Status == FriendshipStatus.Pending)
            .ExecuteDelete();

        return affectedRows == ExpectedAffectedRows;
    }
    public bool DeleteFriendshipBetween(Guid playerId, Guid otherPlayerId)
    {
        int affectedRows = _context.Friendships
            .Where(friendship => (friendship.PlayerRequesterId == playerId
                                  && friendship.PlayerAddresseeId == otherPlayerId)
                                 || (friendship.PlayerRequesterId == otherPlayerId
                                     && friendship.PlayerAddresseeId == playerId))
            .ExecuteDelete();

        return affectedRows == ExpectedAffectedRows;
    }

}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/PlayerRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class PlayerRepository : IPlayerRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public PlayerRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public Player? GetPlayerById(Guid playerId)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.PlayerId == playerId);

        return player;
    }

    public Player? GetPlayerByUsername(string username)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.Username == username);

        return player;
    }

    public Player? GetPlayerByEmail(string email)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.Email == email);

        return player;
    }

    public bool UsernameExists(string username)
    {
        bool exists = _context.Players.Any(p => p.Username == username);

        return exists;
    }

    public bool EmailExists(string email)
    {
        bool exists = _context.Players.Any(p => p.Email == email);

        return exists;
    }

    public bool CreatePlayer(Player player)
    {
        _context.Players.Add(player);
        int affectedRows = _context.SaveChanges();

        return affectedRows >= ExpectedAffectedRows;
    }

    public bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt)
    {
        int affectedRows = _context.Players
            .Where(p => p.PlayerId == playerId)
            .ExecuteUpdate(setters => setters.SetProperty(p => p.LastLoginAt, lastLoginAt));

        return affectedRows == ExpectedAffectedRows;
    }

    public List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults)
    {
        return _context.Players.AsNoTracking()
            .Where(player => player.PlayerId != excludedPlayerId && player.Username.Contains(searchTerm))
            .OrderBy(player => player.Username)
            .Take(maximumResults)
            .ToList();
    }

    public bool UpdatePassword(Guid playerId, string passwordHash, string passwordSalt)
    {
        int affectedRows = _context.Players
            .Where(player => player.PlayerId == playerId)
            .ExecuteUpdate(setters => setters
                .SetProperty(player => player.PasswordHash, passwordHash)
                .SetProperty(player => player.PasswordSalt, passwordSalt));

        return affectedRows == ExpectedAffectedRows;
    }
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Implementations/SessionTokenRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class SessionTokenRepository : ISessionTokenRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public SessionTokenRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public bool CreateSessionToken(SessionToken token)
    {
        _context.SessionTokens.Add(token);
        int affectedRows = _context.SaveChanges();

        return affectedRows == ExpectedAffectedRows;
    }

    public SessionToken? GetActiveSessionToken(string tokenHash)
    {
        DateTime currentTime = DateTime.UtcNow;
        SessionToken? token = _context.SessionTokens
            .AsNoTracking()
            .Include(session => session.Player)
            .FirstOrDefault(session => session.TokenValue == tokenHash
                                       && !session.IsRevoked
                                       && session.ExpiresAt > currentTime);

        return token;
    }

    public bool RevokeSessionToken(string tokenHash)
    {
        int affectedRows = _context.SessionTokens
            .Where(session => session.TokenValue == tokenHash && !session.IsRevoked)
            .ExecuteUpdate(setters => setters.SetProperty(session => session.IsRevoked, true));

        return affectedRows == ExpectedAffectedRows;
    }

    public int RevokePlayerSessions(Guid playerId)
    {
        int affectedRows = _context.SessionTokens
            .Where(session => session.PlayerId == playerId && !session.IsRevoked)
            .ExecuteUpdate(setters => setters.SetProperty(session => session.IsRevoked, true));

        return affectedRows;
    }

}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/IBlockedPlayerRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IBlockedPlayerRepository
{
    List<BlockedPlayer> GetBlocksBetween(Guid playerId, List<Guid> otherPlayerIds);
    List<BlockedPlayer> GetBlockedPlayers(Guid playerId);
    bool IsPlayerBlocked(Guid playerId, Guid otherPlayerId);
    bool BlockPlayer(Guid playerId, Guid otherPlayerId);
    bool UnblockPlayer(Guid playerId, Guid otherPlayerId);
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/IFriendshipRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IFriendshipRepository
{
    Friendship? GetFriendshipBetween(Guid playerId, Guid otherPlayerId);
    List<Friendship> GetFriendshipsBetween(Guid playerId, List<Guid> otherPlayerIds);
    List<Friendship> GetAcceptedFriendships(Guid playerId);
    List<Friendship> GetPendingRequestsReceivedBy(Guid addresseeId);
    bool CreatePendingFriendship(Guid requesterId, Guid addresseeId);
    bool AcceptPendingFriendship(Guid requesterId, Guid addresseeId);
    bool DeleteFriendshipBetween(Guid playerId, Guid otherPlayerId);
    bool DeletePendingFriendship(Guid requesterId, Guid addresseeId);
}
```

## File: src/Server/Vertigo.Server.DataAccess/Repositories/Interfaces/IPlayerRepository.cs
```csharp
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IPlayerRepository
{
    Player? GetPlayerById(Guid playerId);
    Player? GetPlayerByUsername(string username);
    Player? GetPlayerByEmail(string email);
    bool UsernameExists(string username);
    bool EmailExists(string email);
    bool CreatePlayer(Player player);
    List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults);
    bool UpdatePassword(Guid playerId, string passwordHash, string passwordSalt);
    bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt);
}
```

## File: src/Server/Vertigo.Server.Game/Matches/Implementations/InMemoryMatchSessionStarter.cs
```csharp
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Models;

namespace Vertigo.Server.Game.Matches.Implementations;

// One synchronized in-memory registry for idempotent starts and terminal transitions.
// Deliberately no automatic cancellation when lobby connections disappear (Phase 7B).
public sealed class InMemoryMatchSessionStarter : IMatchSessionStarter, IMatchLifecycle, IMatchPresence
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<Guid, MatchRecord> _matchesByLobby = new();
    private readonly Dictionary<Guid, MatchRecord> _matchesById = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryMatchSessionStarter(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public MatchSessionSnapshot Start(MatchStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        Validate(request);

        MatchSessionSnapshot result;
        lock (_syncRoot)
        {
            if (_matchesByLobby.TryGetValue(request.LobbyInstanceId, out MatchRecord? existing))
            {
                EnsureSameRequest(existing.Session, request);
                result = existing.Session;
            }
            else
            {
                MatchRecord created = CreateRecord(request);
                _matchesByLobby.Add(request.LobbyInstanceId, created);
                _matchesById.Add(created.Session.MatchId, created);
                result = created.Session;
            }
        }

        return result;
    }

    public MatchLifecycleSnapshot? Get(Guid matchId)
    {
        MatchLifecycleSnapshot? result = null;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.Lifecycle;
            }
        }

        return result;
    }

    public bool TryFinish(Guid matchId)
    {
        bool result = TryTransition(matchId, MatchLifecycleStatus.Finished);

        return result;
    }

    public bool TryCancel(Guid matchId)
    {
        bool result = TryTransition(matchId, MatchLifecycleStatus.Cancelled);

        return result;
    }

    public MatchPresenceSnapshot? GetPresence(Guid matchId)
    {
        MatchPresenceSnapshot? result = null;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.ToPresence();
            }
        }

        return result;
    }

    public bool TryDisconnect(Guid matchId, Guid playerId)
    {
        return TrySetConnected(matchId, playerId, false);
    }

    public MatchReconnectResult TryReconnect(Guid matchId, Guid playerId)
    {
        MatchReconnectResult result = MatchReconnectResult.Rejected;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.TryReconnect(playerId, _timeProvider.GetUtcNow());
            }
        }

        return result;
    }

    private bool TrySetConnected(Guid matchId, Guid playerId, bool isConnected)
    {
        bool changed = false;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                changed = record.TrySetConnected(playerId, isConnected, _timeProvider.GetUtcNow());
            }
        }

        return changed;
    }

    public int PurgeTerminated(TimeSpan retention)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(retention.Ticks);
        int removed = 0;
        DateTimeOffset now = _timeProvider.GetUtcNow();
        TimeSpan maximumSubtractable = now - DateTimeOffset.MinValue;
        if (retention <= maximumSubtractable)
        {
            DateTimeOffset cutoff = now - retention;
            lock (_syncRoot)
            {
                Guid[] expiredIds = _matchesById.Values
                    .Where(record => record.IsTerminal && record.Lifecycle.UpdatedAtUtc <= cutoff)
                    .Select(record => record.Session.MatchId)
                    .ToArray();
                foreach (Guid matchId in expiredIds)
                {
                    MatchRecord record = _matchesById[matchId];
                    _matchesById.Remove(matchId);
                    _matchesByLobby.Remove(record.Session.LobbyInstanceId);
                    removed++;
                }
            }
        }

        return removed;
    }

    private bool TryTransition(Guid matchId, MatchLifecycleStatus next)
    {
        bool changed = false;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                changed = record.TryTransition(next, _timeProvider.GetUtcNow());
            }
        }

        return changed;
    }

    private MatchRecord CreateRecord(MatchStartRequest request)
    {
        Guid[] players = request.PlayerIds.ToArray();
        DateTimeOffset now = _timeProvider.GetUtcNow();
        MatchSessionSnapshot session = new(
            Guid.NewGuid(), request.LobbyInstanceId, request.LobbyCode,
            request.HostPlayerId, Array.AsReadOnly(players), now.UtcDateTime);
        MatchRecord record = new(session, now);

        return record;
    }

    private static void Validate(MatchStartRequest request)
    {
        if (request.LobbyInstanceId == Guid.Empty || string.IsNullOrWhiteSpace(request.LobbyCode)
            || request.HostPlayerId == Guid.Empty || request.PlayerIds is null
            || request.PlayerIds.Count < 2 || request.PlayerIds.Distinct().Count() != request.PlayerIds.Count
            || !request.PlayerIds.Contains(request.HostPlayerId) || request.PlayerIds.Any(id => id == Guid.Empty))
        {
            throw new ArgumentException("Invalid match initialization data.", nameof(request));
        }
    }

    private static void EnsureSameRequest(MatchSessionSnapshot existing, MatchStartRequest request)
    {
        if (existing.LobbyCode != request.LobbyCode || existing.InitialHostPlayerId != request.HostPlayerId
            || !existing.PlayerIds.SequenceEqual(request.PlayerIds))
        {
            throw new InvalidOperationException("A match already exists for a different lobby snapshot.");
        }
    }
}
```

## File: src/Server/Vertigo.Server.Game/Vertigo.Server.Game.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.Host/Vertigo.Server.Host.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.PlayerManagement\Vertigo.Server.PlayerManagement.csproj" />
    <ProjectReference Include="..\Vertigo.Server.Matchmaking\Vertigo.Server.Matchmaking.csproj" />
    <ProjectReference Include="..\Vertigo.Server.Game\Vertigo.Server.Game.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="CoreWCF.NetTcp" Version="1.9.1" />
    <PackageReference Include="CoreWCF.Primitives" Version="1.9.1" />
  </ItemGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.Matchmaking/Vertigo.Server.Matchmaking.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
    <ProjectReference Include="..\Vertigo.Server.PlayerManagement\Vertigo.Server.PlayerManagement.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="CoreWCF.Primitives" Version="1.9.1" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Server/Vertigo.Server.PlayerManagement/Authentication/Services/AuthService.cs
```csharp
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public class AuthService
{
    private const int SessionDurationDays = 7;

    private readonly IPlayerRepository _playerRepository;
    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IGuestSessionRegistry _guests;
    private readonly TimeProvider _timeProvider;

    public AuthService(AuthenticationDependencies dependencies, IGuestSessionRegistry guests, TimeProvider timeProvider)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _tokenGenerator = dependencies.TokenGenerator;
        _guests = guests;
        _timeProvider = timeProvider;
    }

    public AuthenticatedSession? Login(string username, string password)
    {
        Player? player = _playerRepository.GetPlayerByUsername(username);
        if (player is null || !_passwordHasher.VerifyPassword(
                password, player.PasswordHash, player.PasswordSalt))
        {
            return null;
        }

        string sessionToken = _tokenGenerator.GenerateSessionToken();
        DateTime expiresAt = _timeProvider.GetUtcNow().UtcDateTime.AddDays(SessionDurationDays);
        SessionToken token = new SessionToken
        {
            PlayerId = player.PlayerId,
            TokenValue = _tokenGenerator.HashSessionToken(sessionToken),
            ExpiresAt = expiresAt
        };

        bool isCreated = _sessionTokenRepository.CreateSessionToken(token);
        if (!isCreated)
        {
            throw new InvalidOperationException("The session could not be persisted.");
        }

        return new AuthenticatedSession
        {
            PlayerId = player.PlayerId,
            Username = player.Username,
            Token = sessionToken,
            ExpiresAt = expiresAt
        };
    }

    public AuthenticatedSession? ResumeSession(string sessionToken)
    {
        AuthenticatedSession? session = null;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
            session = _guests.GetSession(tokenHash);
            if (session is null)
            {
                SessionToken? storedToken = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
                if (storedToken is not null)
                {
                    session = new AuthenticatedSession
                    {
                        PlayerId = storedToken.PlayerId,
                        Username = storedToken.Player.Username,
                        Token = sessionToken,
                        ExpiresAt = storedToken.ExpiresAt
                    };
                }
            }
        }

        return session;
    }

    public bool Logout(string sessionToken)
    {
        bool isRevoked = false;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
            isRevoked = _guests.Revoke(tokenHash) || _sessionTokenRepository.RevokeSessionToken(tokenHash);
        }

        return isRevoked;
    }

    public AuthenticatedSession CreateGuestSession()
    {
        return _guests.Create();
    }
}
```

## File: src/Server/Vertigo.Server.PlayerManagement/Vertigo.Server.PlayerManagement.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Vertigo.Contracts\Vertigo.Contracts.csproj" />
    <ProjectReference Include="..\Vertigo.Server.DataAccess\Vertigo.Server.DataAccess.csproj" />
    <ProjectReference Include="..\Vertigo.Server.CrossCutting\Vertigo.Server.CrossCutting.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="CoreWCF.Primitives" Version="1.9.1" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## File: src/Shared/Vertigo.Contracts/Callbacks/ILobbyCallback.cs
```csharp
#nullable enable

using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface ILobbyCallback
{
    [OperationContract(IsOneWay = true)]
    void PlayerJoined(LobbyMember player);

    [OperationContract(IsOneWay = true)]
    void PlayerLeft(Guid playerId);

    [OperationContract(IsOneWay = true)]
    void MessageReceived(LobbyChatMessage message);

    [OperationContract(IsOneWay = true)]
    void PreparationChanged(LobbyPreparation preparation);
}
```

## File: src/Shared/Vertigo.Contracts/Faults/ServiceErrorCodes.cs
```csharp
namespace Vertigo.Contracts.Faults;

public static class ServiceErrorCodes
{
    public const string UsernameAlreadyRegistered = "USERNAME_ALREADY_REGISTERED";
    public const string EmailAlreadyRegistered = "EMAIL_ALREADY_REGISTERED";
    public const string InvalidRegistrationData = "INVALID_REGISTRATION_DATA";
    public const string LobbyNotFound = "ROOM_NOT_FOUND";
    public const string LobbyFull = "ROOM_FULL";
    public const string LobbyAlreadyStarted = "MSG-ROOM-009";
    public const string InvalidLobbyCapacity = "INVALID_ROOM_CAPACITY";
    public const string InvalidData = "INVALID_DATA";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string InvalidCode = "INVALID_CODE";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string SessionAlreadyActive = "SESSION_ALREADY_ACTIVE";
    public const string FriendRequestAlreadySent = "FRIEND_REQUEST_ALREADY_SENT";
    public const string AlreadyFriends = "ALREADY_FRIENDS";
    public const string FriendshipBlocked = "FRIENDSHIP_BLOCKED";
    public const string FriendRequestNotFound = "FRIEND_REQUEST_NOT_FOUND";
    public const string LobbyInviteNotAllowed = "LOBBY_INVITE_NOT_ALLOWED";
    
    public const string LobbyNotMember = "LOBBY_NOT_MEMBER";
    public const string LobbyNotHost = "LOBBY_NOT_HOST";
    public const string LobbyNotReady = "LOBBY_NOT_READY";
    public const string LobbyAlreadyInGame = "LOBBY_ALREADY_IN_GAME";

    public const string Unauthorized = "UNAUTHORIZED";
    public const string LobbyChatNotAllowed = "LOBBY_CHAT_NOT_ALLOWED";
}
```

## File: src/Shared/Vertigo.Contracts/Faults/ServiceMessageKeys.cs
```csharp
namespace Vertigo.Contracts.Faults;

public static class ServiceMessageKeys
{
    public const string UsernameAlreadyRegistered = "RegisterAccount.UsernameAlreadyRegistered";
    public const string EmailAlreadyRegistered = "RegisterAccount.EmailAlreadyRegistered";
    public const string InvalidRegistrationData = "RegisterAccount.ValidationSummary";
    public const string LobbyNotFound = "ROOM_NOT_FOUND";
    public const string LobbyFull = "ROOM_FULL";    
    public const string LobbyAlreadyStarted = "MSG-ROOM-009";
    public const string InvalidLobbyCapacity = "MSG-ROOM-011";
    public const string InvalidData = "ErrorInvalidData";
    public const string UserNotFound = "ErrorUserNotFound";
    public const string InvalidCode = "ErrorInvalidCode";
    public const string InvalidCredentials = "ErrorInvalidCredentials";
    public const string SessionAlreadyActive = "AccessMenu.SessionAlreadyActive";
    public const string FriendRequestAlreadySent = "Friends.RequestSent";
    public const string AlreadyFriends = "Friends.AlreadyFriends";
    public const string FriendshipBlocked = "Friends.RequestBlocked";
    public const string FriendRequestNotFound = "Notifications.FriendRequestNotFound";
    public const string LobbyInviteNotAllowed = "InviteFriends.NotAllowed";
    
    public const string LobbyNotMember = "Lobby.NotMember";
    public const string LobbyNotHost = "Lobby.NotHost";
    public const string LobbyNotReady = "Lobby.NotReady";
    public const string LobbyAlreadyInGame = "Lobby.AlreadyInGame";

    public const string Unauthorized = "Authentication.Unauthorized";
    public const string LobbyChatNotAllowed = "Lobby.ChatNotAllowed";
}
```

## File: src/Shared/Vertigo.Contracts/Interfaces/ILobbyService.cs
```csharp
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract(CallbackContract = typeof(ILobbyCallback), SessionMode = SessionMode.Required)]
public interface ILobbyService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool LeaveLobby(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<LobbySummary> GetPublicLobbies();

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyPreparation GetPreparation(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyPreparation StartMatch(string lobbyCode, string sessionToken);
}
```

## File: src/Shared/Vertigo.Contracts/Models/LobbyPreparation.cs
```csharp
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class LobbyPreparation
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public long Revision { get; set; }

    [DataMember]
    public Guid HostPlayerId { get; set; }

    [DataMember]
    public bool IsInGame { get; set; }

    [DataMember]
    public Guid MatchId { get; set; }

    [DataMember]
    public List<Guid> ReadyPlayerIds { get; set; } = new();

    [DataMember]
    public List<Guid> DisconnectedPlayerIds { get; set; } = new();

    [DataMember]
    public bool IsSuspended { get; set; }
}
```

## File: tests/Vertigo.Server.Game.Tests/Vertigo.Server.Game.Tests.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="11.0.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Server\Vertigo.Server.Game\Vertigo.Server.Game.csproj" />
  </ItemGroup>

</Project>
```

## File: src/Client/Vertigo.Client.UI/ViewModels/AccessViewModel.cs
```csharp
using Vertigo.Client.UI.Utilities.Settings.Interfaces;
using Vertigo.Client.UI.Utilities.Settings.Models;
using Vertigo.Client.UI.Utilities.Settings.Services;
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
    private readonly IUserSettingsStore _preferences;
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

    public AccessViewModel(AccessContext context, IAccountClient accounts, IUserSettingsStore preferences)
    {
        _sessions = context.Sessions;
        _accounts = accounts;
        _preferences = preferences;
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
            if (!_preferences.TrySave(new UserSettings { Language = targetLanguage }))
            {
                ErrorMessage = _text.Get("Settings.SaveFailed");
                return;
            }

            UiLanguage.Apply(targetLanguage);
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
        NotifyLocalizationChanged();
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
```

## File: src/Client/Vertigo.Client.UI/Views/MainWindow.axaml
```
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:views="using:Vertigo.Client.UI.Views"
        xmlns:vm="using:Vertigo.Client.UI.ViewModels"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="Vertigo.Client.UI.Views.MainWindow"
        x:DataType="vm:MainViewModel"
        x:CompileBindings="True"
        Icon="/Assets/avalonia-logo.ico"
        Title="Vertigo.Client.UI">

    <Grid>
        <ContentControl IsVisible="{Binding IsAccessPage}"
                        Content="{Binding AccessContent}">
            <ContentControl.ContentTemplate>
                <DataTemplate x:DataType="vm:AccessViewModel">
                    <views:AccessView/>
                </DataTemplate>
            </ContentControl.ContentTemplate>
        </ContentControl>
        <ContentControl Content="{Binding TemplatedContent}"/>
    </Grid>
</Window>
```

## File: src/Client/Vertigo.Client.UI/App.axaml
```
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:playviews="using:Vertigo.Client.UI.Views"
             xmlns:playvm="using:Vertigo.Client.UI.ViewModels"
             x:Class="Vertigo.Client.UI.App"
             xmlns:local="using:Vertigo.Client.UI"
             RequestedThemeVariant="Light"
             xmlns:homevm="using:Vertigo.Client.UI.ViewModels"
             xmlns:homeviews="using:Vertigo.Client.UI.Views">
             <!-- "Default" ThemeVariant follows system theme variant. "Dark" or "Light" are other available options. -->

    <Application.DataTemplates>
        <DataTemplate DataType="{x:Type playvm:PlayViewModel}">
            <playviews:PlayView />
        </DataTemplate>
        <DataTemplate DataType="{x:Type homevm:HomeViewModel}">
            <homeviews:HomeView />
        </DataTemplate>

        <DataTemplate DataType="{x:Type homevm:SocialViewModel}">
            <homeviews:SocialView/>
        </DataTemplate>

        <DataTemplate DataType="{x:Type homevm:SettingsViewModel}">
            <homeviews:SettingsView/>
        </DataTemplate>

        <DataTemplate DataType="{x:Type homevm:ProfileViewModel}">
            <homeviews:ProfileView/>
        </DataTemplate>

        <DataTemplate DataType="{x:Type homevm:LobbyViewModel}">
            <homeviews:LobbyView/>
        </DataTemplate>

        <local:ViewLocator />
    </Application.DataTemplates>

    <Application.Styles>
        <FluentTheme />
        <StyleInclude Source="avares://Vertigo.Client.UI/Themes/VertigoTheme.axaml"/>
    </Application.Styles>
</Application>
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/Services/LobbyService.cs
```csharp
using Vertigo.Contracts.Internal;
using System.Security.Cryptography;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.Matchmaking.Lobbies;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class LobbyService : ILobbyService, ILobbyDirectory
{
    private const string LobbyCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int LobbyCodeLength = 6;
    private const int MinimumMaxPlayers = 2;
    private const int MaximumMaxPlayers = 6;

    private readonly Dictionary<string, LobbyState> _lobbies = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lobbyLock = new();
    private readonly HashSet<string> _watchedSessions = new();
    private readonly TimeProvider _timeProvider;
    private readonly IFriendshipNotifier _notifier;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly LobbyGameOperations _gameOperations;

    public LobbyService(TimeProvider timeProvider, IFriendshipNotifier notifier, IServiceScopeFactory scopeFactory,
        LobbyGameOperations gameOperations)
    {
        _timeProvider = timeProvider;
        _notifier = notifier;
        _scopeFactory = scopeFactory;
        _gameOperations = gameOperations;
    }

    public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
    {
        if (maxPlayers < MinimumMaxPlayers || maxPlayers > MaximumMaxPlayers)
        {
            throw CreateFault(ServiceErrorCodes.InvalidLobbyCapacity, ServiceMessageKeys.InvalidLobbyCapacity);
        }

        SessionIdentity identity = ResolveIdentity(sessionToken);
        LobbyMember host = CreateMember(identity);
        LobbyConnection hostConnection = new(host, GetCurrentCallback(), WatchCurrentSession());
        DateTimeOffset createdAtUtc = _timeProvider.GetUtcNow();
        string lobbyCode;

        lock (_lobbyLock)
        {
            if (_lobbies.Values.Any(lobby => lobby.Contains(host.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
            }

            lobbyCode = GenerateUniqueLobbyCode();
            LobbySettings settings = new(lobbyCode, host.PlayerId, isPublic, maxPlayers, createdAtUtc);
            LobbyState lobby = new(settings);
            lobby.AddOrReplace(hostConnection);
            _lobbies.Add(lobbyCode, lobby);
        }

        return lobbyCode;
    }

    public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        LobbyMember player = CreateMember(identity);
        LobbyConnection connection = new(player, GetCurrentCallback(), WatchCurrentSession());
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        List<ILobbyCallback> callbacksToNotify = new();
        List<ILobbyCallback> preparationCallbacks = new();
        LobbyPreparation? preparation = null;
        LobbyAdmission response;

        lock (_lobbyLock)
        {
            if (!_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
            }

            if (lobby.Status != LobbyStatus.Waiting && !lobby.Contains(player.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
            }

            if (lobby.Contains(player.PlayerId))
            {
                if (lobby.Status == LobbyStatus.InGame)
                {
                    if (!_gameOperations.TryReconnect(lobby, player.PlayerId))
                    {
                        throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
                    }
                }
                lobby.CancelInvitation(player.PlayerId);
                lobby.AddOrReplace(connection);
                response = CreateJoinResponse(lobby, player.PlayerId);
            }
            else if (lobby.IsFull)
            {
                throw CreateFault(ServiceErrorCodes.LobbyFull, ServiceMessageKeys.LobbyFull);
            }
            else if (!lobby.IsPublic && !lobby.IsInvited(player.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
            }
            else if (_lobbies.Values.Any(otherLobby => otherLobby.Contains(player.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
            }
            else
            {
                response = CreateJoinResponse(lobby, player.PlayerId);
                callbacksToNotify = lobby.GetCallbacksExcept(player.PlayerId);
                lobby.CancelInvitation(player.PlayerId);
                lobby.AddOrReplace(connection);
            }
            preparationCallbacks = lobby.GetCallbacksExcept(Guid.Empty);
            preparation = lobby.ToPreparation();
        }

        Notify(callbacksToNotify, SendPlayerJoined, player);
        Notify(preparationCallbacks, SendPreparationChanged, preparation);

        return response;
    }

    // Leaving an active match is explicitly rejected without changing either module.
    // bool=false is used here because this method already returns bool.
    public bool LeaveLobby(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        Guid playerId = identity.PlayerId;
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        List<ILobbyCallback> callbacksToNotify = new();
        List<Guid> withdrawnInvitedIds = new();
        LobbyPreparation? preparation = null;
        bool wasRemoved = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby)
                && lobby.Status == LobbyStatus.Waiting && lobby.Remove(playerId))
            {
                wasRemoved = true;
                if (lobby.IsEmpty)
                {
                    withdrawnInvitedIds = lobby.GetInvitedPlayerIds();
                    _lobbies.Remove(lobby.LobbyCode);
                }
                else
                {
                    callbacksToNotify = lobby.GetCallbacksExcept(playerId);
                    preparation = lobby.ToPreparation();
                }
            }
        }

        Notify(callbacksToNotify, SendPlayerLeft, playerId);
        if (preparation is not null)
        {
            Notify(callbacksToNotify, SendPreparationChanged, preparation);
        }
        WithdrawInvitations(normalizedCode, withdrawnInvitedIds);
        return wasRemoved;
    }

    public LobbyPreparation GetPreparation(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            return lobby.ToPreparation();
        }
    }

    public LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        List<ILobbyCallback> callbacks;
        LobbyPreparation preparation;

        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.Waiting)
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyInGame, ServiceMessageKeys.LobbyAlreadyInGame);
            }
            lobby.SetReady(identity.PlayerId, isReady);
            preparation = lobby.ToPreparation();
            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        Notify(callbacks, SendPreparationChanged, preparation);
        return preparation;
    }

    public LobbyPreparation StartMatch(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        List<ILobbyCallback> callbacks;
        LobbyPreparation preparation;

        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.Waiting)
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyInGame, ServiceMessageKeys.LobbyAlreadyInGame);
            }
            if (lobby.HostPlayerId != identity.PlayerId)
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotHost, ServiceMessageKeys.LobbyNotHost);
            }
            if (!lobby.CanStartMatch(identity.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotReady, ServiceMessageKeys.LobbyNotReady);
            }

            // Entire transition is serialized with other lobby mutations.
            // No InGame state is committed if Game rejects initialization.
            _gameOperations.Activate(lobby);
            preparation = lobby.ToPreparation();
            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        Notify(callbacks, SendPreparationChanged, preparation);
        return preparation;
    }

    // Callers already hold the lobby lock.
    private LobbyState RequireLobbyMember(string lobbyCode, Guid playerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        if (!_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }
        if (!lobby.Contains(playerId))
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotMember, ServiceMessageKeys.LobbyNotMember);
        }
        return lobby;
    }

    public List<LobbySummary> GetPublicLobbies()
    {
        List<LobbySummary> publicLobbies;

        lock (_lobbyLock)
        {
            publicLobbies = _lobbies.Values
                .Where(lobby => lobby.IsPublic && lobby.Status == LobbyStatus.Waiting && !lobby.IsFull)
                .OrderBy(lobby => lobby.CreatedAtUtc)
                .Select(lobby => lobby.ToSummary())
                .ToList();
        }

        return publicLobbies;
    }

    public LobbyOverview? FindLobby(string lobbyCode)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        LobbyOverview? overview = null;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                overview = lobby.ToOverview();
            }
        }

        return overview;
    }

    public bool IsInAnyLobby(Guid playerId)
    {
        bool isInLobby;

        lock (_lobbyLock)
        {
            isInLobby = _lobbies.Values.Any(lobby => lobby.Contains(playerId));
        }

        return isInLobby;
    }

    public bool RegisterInvitation(string lobbyCode, Guid targetPlayerId, Guid senderPlayerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        bool isRegistered = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby)
                && lobby.Status == LobbyStatus.Waiting
                && !lobby.Contains(targetPlayerId))
            {
                isRegistered = lobby.Invite(targetPlayerId, senderPlayerId);
            }
        }

        return isRegistered;
    }

    public bool CancelInvitation(string lobbyCode, Guid targetPlayerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        bool isCancelled = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                isCancelled = lobby.CancelInvitation(targetPlayerId);
            }
        }

        return isCancelled;
    }

    public List<LobbyOverview> FindLobbiesInviting(Guid playerId)
    {
        List<LobbyOverview> overviews;

        lock (_lobbyLock)
        {
            overviews = _lobbies.Values
                .Where(lobby => lobby.Status == LobbyStatus.Waiting && lobby.IsInvited(playerId))
                .Select(lobby => lobby.ToOverview())
                .ToList();
        }

        return overviews;
    }

    // A closed or faulted channel means the client left or dropped: its players are removed from every lobby.
    public bool PublishChatMessage(LobbyChatMessage message)
    {
        List<ILobbyCallback> callbacks;
        lock (_lobbyLock)
        {
            if (!_lobbies.TryGetValue(message.LobbyCode, out LobbyState? lobby)
                || lobby.Status != LobbyStatus.Waiting
                || !lobby.Contains(message.SenderId))
            {
                return false;
            }

            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        Notify(callbacks, SendChatMessage, message);
        return true;
    }

    private SessionIdentity ResolveIdentity(string sessionToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISessionIdentityResolver resolver = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        return resolver.Require(sessionToken);
    }

    private static LobbyMember CreateMember(SessionIdentity identity)
    {
        return new LobbyMember
        {
            PlayerId = identity.PlayerId,
            Username = identity.Username,
            IsGuest = identity.IsGuest
        };
    }

    private static void SendPreparationChanged(ILobbyCallback callback, LobbyPreparation preparation)
    {
        callback.PreparationChanged(preparation);
    }

    private static void SendChatMessage(ILobbyCallback callback, LobbyChatMessage message)
    {
        callback.MessageReceived(message);
    }

    private string WatchCurrentSession()
    {
        IContextChannel channel = (IContextChannel)GetCurrentCallback();
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        bool isNewSession;

        lock (_lobbyLock)
        {
            isNewSession = _watchedSessions.Add(sessionId);
        }

        if (isNewSession)
        {
            channel.Closed += OnChannelClosed;
            channel.Faulted += OnChannelClosed;
        }

        return sessionId;
    }

    private void OnChannelClosed(object? sender, EventArgs eventArgs)
    {
        string sessionId = (sender as IContextChannel)?.SessionId ?? string.Empty;
        List<(List<ILobbyCallback> Callbacks, List<Guid> LeftIds, LobbyPreparation State)> notifications = new();
        List<(string LobbyCode, List<Guid> InvitedIds)> withdrawals = new();

        lock (_lobbyLock)
        {
            _watchedSessions.Remove(sessionId);

            foreach (LobbyState lobby in _lobbies.Values.ToList())
            {
                List<Guid> leftIds;
                if (lobby.Status == LobbyStatus.InGame)
                {
                    leftIds = lobby.DisconnectSession(sessionId);
                    foreach (Guid playerId in leftIds)
                    {
                        _gameOperations.Disconnect(lobby, playerId);
                    }
                }
                else
                {
                    leftIds = lobby.RemoveSession(sessionId);
                }

                if (lobby.IsEmpty)
                {
                    withdrawals.Add((lobby.LobbyCode, lobby.GetInvitedPlayerIds()));
                    _lobbies.Remove(lobby.LobbyCode);
                }
                else if (leftIds.Count > 0)
                {
                    notifications.Add((lobby.GetCallbacksExcept(Guid.Empty), leftIds, lobby.ToPreparation()));
                }
            }
        }

        foreach ((List<ILobbyCallback> callbacks, List<Guid> leftIds, LobbyPreparation state) in notifications)
        {
            // During a match, disconnected players retain their participant identity.
            // PlayerLeft is reserved for actual removals during the waiting phase.
            if (!state.IsInGame)
            {
                foreach (Guid leftId in leftIds)
                {
                    Notify(callbacks, SendPlayerLeft, leftId);
                }
            }
            Notify(callbacks, SendPreparationChanged, state);
        }

        foreach ((string lobbyCode, List<Guid> invitedIds) in withdrawals)
        {
            WithdrawInvitations(lobbyCode, invitedIds);
        }
    }

    private void WithdrawInvitations(string lobbyCode, List<Guid> invitedIds)
    {
        foreach (Guid invitedId in invitedIds)
        {
            _notifier.NotifyLobbyInvitationWithdrawn(invitedId, lobbyCode);
        }
    }

    private string GenerateUniqueLobbyCode()
    {
        string lobbyCode;

        do
        {
            lobbyCode = RandomNumberGenerator.GetString(LobbyCodeAlphabet, LobbyCodeLength);
        }
        while (_lobbies.ContainsKey(lobbyCode));

        return lobbyCode;
    }

    private static LobbyAdmission CreateJoinResponse(LobbyState lobby, Guid joiningPlayerId)
    {
        LobbyAdmission response = new()
        {
            LobbyCode = lobby.LobbyCode,
            HostPlayerId = lobby.HostPlayerId,
            IsPublic = lobby.IsPublic,
            MaxPlayers = lobby.MaxPlayers,
            ExistingMembers = lobby.GetMembersExcept(joiningPlayerId)
        };

        return response;
    }

    private static ILobbyCallback GetCurrentCallback()
    {
        ILobbyCallback callback = OperationContext.Current.GetCallbackChannel<ILobbyCallback>();

        return callback;
    }

    private static void SendPlayerJoined(ILobbyCallback callback, LobbyMember player)
    {
        callback.PlayerJoined(player);
    }

    private static void SendPlayerLeft(ILobbyCallback callback, Guid playerId)
    {
        callback.PlayerLeft(playerId);
    }

    private static void Notify<TArgument>(
        IReadOnlyList<ILobbyCallback> callbacks,
        Action<ILobbyCallback, TArgument> notification,
        TArgument argument)
    {
        foreach (ILobbyCallback callback in callbacks)
        {
            try
            {
                notification(callback, argument);
            }
            catch (CommunicationException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
            catch (TimeoutException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
            catch (ObjectDisposedException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
        }
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new()
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
```

## File: src/Server/Vertigo.Server.Matchmaking/Lobbies/LobbyState.cs
```csharp
#nullable enable

using Vertigo.Contracts.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed class LobbyState
{
    private readonly List<LobbyConnection> _connections = new();
    private readonly Dictionary<Guid, Guid> _invitations = new();
    private readonly HashSet<Guid> _readyPlayerIds = new();
    private readonly HashSet<Guid> _disconnectedPlayerIds = new();
    private long _revision;
    private Guid _matchId;

    public Guid LobbyInstanceId { get; } = Guid.NewGuid();

    public Guid MatchId => _matchId;

    public long Revision => _revision;

    public LobbyState(LobbySettings settings)
    {
        LobbyCode = settings.LobbyCode;
        HostPlayerId = settings.HostPlayerId;
        IsPublic = settings.IsPublic;
        MaxPlayers = settings.MaxPlayers;
        CreatedAtUtc = settings.CreatedAtUtc;
        Status = LobbyStatus.Waiting;
    }

    public string LobbyCode { get; }

    public Guid HostPlayerId { get; private set; }

    public bool IsPublic { get; }

    public int MaxPlayers { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public LobbyStatus Status { get; private set; }

    public bool IsEmpty => _connections.Count == 0;

    public bool IsFull => _connections.Count >= MaxPlayers;

    public bool Contains(Guid playerId)
    {
        bool contains = _connections.Exists(connection => connection.Member.PlayerId == playerId);

        return contains;
    }

    public void AddOrReplace(LobbyConnection connection)
    {
        int index = _connections.FindIndex(existing => existing.Member.PlayerId == connection.Member.PlayerId);
        if (index >= 0)
        {
            _connections[index] = connection;
            _disconnectedPlayerIds.Remove(connection.Member.PlayerId);
            _revision++;
        }
        else
        {
            _connections.Add(connection);
            _readyPlayerIds.Clear(); // Joining changes the preparation quorum.
            _revision++;
        }
    }

    public bool Remove(Guid playerId)
    {
        int removedCount = _connections.RemoveAll(connection => connection.Member.PlayerId == playerId);
        if (removedCount > 0)
        {
            _readyPlayerIds.Remove(playerId);
            _disconnectedPlayerIds.Remove(playerId);
            _revision++;
        }

        // The role passes to the participant who has been longest in the lobby (CU-014 FA-04).
        if (removedCount > 0 && playerId == HostPlayerId && _connections.Count > 0)
        {
            HostPlayerId = _connections[0].Member.PlayerId;
        }

        return removedCount > 0;
    }

    // In-game transport loss does not remove a player or transfer the host role.
    // A delayed Closed event from the old channel cannot disconnect a new channel.
    public List<Guid> DisconnectSession(string sessionId)
    {
        List<Guid> changed = new();
        foreach (LobbyConnection connection in _connections)
        {
            if (connection.SessionId == sessionId
                && _disconnectedPlayerIds.Add(connection.Member.PlayerId))
            {
                changed.Add(connection.Member.PlayerId);
                _revision++;
            }
        }

        return changed;
    }

    public List<Guid> RemoveSession(string sessionId)
    {
        List<Guid> removedIds = _connections
            .Where(connection => connection.SessionId == sessionId)
            .Select(connection => connection.Member.PlayerId)
            .ToList();

        foreach (Guid playerId in removedIds)
        {
            Remove(playerId);
        }

        return removedIds;
    }

    public bool Invite(Guid targetPlayerId, Guid senderPlayerId)
    {
        bool isInvited = _invitations.TryAdd(targetPlayerId, senderPlayerId);

        return isInvited;
    }

    public bool CancelInvitation(Guid targetPlayerId)
    {
        bool isCancelled = _invitations.Remove(targetPlayerId);

        return isCancelled;
    }

    public bool IsInvited(Guid playerId)
    {
        bool isInvited = _invitations.ContainsKey(playerId);

        return isInvited;
    }

    public List<Guid> GetInvitedPlayerIds()
    {
        List<Guid> invitedIds = _invitations.Keys.ToList();

        return invitedIds;
    }

    public LobbyOverview ToOverview()
    {
        LobbyOverview overview = new(
            LobbyCode,
            HostPlayerId,
            MaxPlayers,
            Status == LobbyStatus.Waiting,
            GetMembersExcept(Guid.Empty),
            new Dictionary<Guid, Guid>(_invitations));

        return overview;
    }

    public bool SetReady(Guid playerId, bool isReady)
    {
        if (Status != LobbyStatus.Waiting || !Contains(playerId))
        {
            return false;
        }

        bool changed = isReady ? _readyPlayerIds.Add(playerId) : _readyPlayerIds.Remove(playerId);
        if (changed)
        {
            _revision++;
        }

        return true;
    }

    public bool CanStartMatch(Guid requesterId)
    {
        return Status == LobbyStatus.Waiting
            && requesterId == HostPlayerId
            && _connections.Count >= 2
            && _connections.All(connection => _readyPlayerIds.Contains(connection.Member.PlayerId));
    }

    public LobbyPreparation ToPreparation()
    {
        return new LobbyPreparation
        {
            LobbyCode = LobbyCode,
            Revision = _revision,
            HostPlayerId = HostPlayerId,
            IsInGame = Status == LobbyStatus.InGame,
            IsSuspended = Status == LobbyStatus.InGame && _connections.Count > 0
                && _disconnectedPlayerIds.Count == _connections.Count,
            DisconnectedPlayerIds = _disconnectedPlayerIds.OrderBy(id => id).ToList(),
            MatchId = _matchId,
            ReadyPlayerIds = _readyPlayerIds.ToList()
        };
    }

    public void StartMatch(Guid matchId)
    {
        if (matchId == Guid.Empty)
        {
            throw new ArgumentException("A created match identifier is required.", nameof(matchId));
        }

        if (Status != LobbyStatus.Waiting || _matchId != Guid.Empty)
        {
            throw new InvalidOperationException("The lobby has already started a match.");
        }

        _matchId = matchId;
        Status = LobbyStatus.InGame;
        _revision++;
    }

    public List<LobbyMember> GetMembersExcept(Guid playerId)
    {
        List<LobbyMember> members = _connections
            .Where(connection => connection.Member.PlayerId != playerId)
            .Select(connection => connection.Member)
            .ToList();

        return members;
    }

    public List<ILobbyCallback> GetCallbacksExcept(Guid playerId)
    {
        List<ILobbyCallback> callbacks = _connections
            .Where(connection => connection.Member.PlayerId != playerId
                && !_disconnectedPlayerIds.Contains(connection.Member.PlayerId))
            .Select(connection => connection.Callback)
            .ToList();

        return callbacks;
    }

    public LobbySummary ToSummary()
    {
        LobbyConnection? hostConnection = _connections.Find(
            connection => connection.Member.PlayerId == HostPlayerId);

        LobbySummary summary = new()
        {
            LobbyCode = LobbyCode,
            HostUsername = hostConnection?.Member.Username ?? string.Empty,
            CurrentPlayers = _connections.Count,
            MaxPlayers = MaxPlayers
        };

        return summary;
    }
}
```

## File: Vertigo.Server.slnx
```
<Solution>
  <Folder Name="/src/" />
  <Folder Name="/src/Server/">
    <Project Path="src/Server/Vertigo.Server.CrossCutting/Vertigo.Server.CrossCutting.csproj" />
    <Project Path="src/Server/Vertigo.Server.DataAccess/Vertigo.Server.DataAccess.csproj" />
    <Project Path="src/Server/Vertigo.Server.Game/Vertigo.Server.Game.csproj" />
    <Project Path="src/Server/Vertigo.Server.Host/Vertigo.Server.Host.csproj" />
    <Project Path="src/Server/Vertigo.Server.Matchmaking/Vertigo.Server.Matchmaking.csproj" />
    <Project Path="src/Server/Vertigo.Server.PlayerManagement/Vertigo.Server.PlayerManagement.csproj" />
  </Folder>
  <Folder Name="/src/Shared/">
    <Project Path="src/Shared/Vertigo.Contracts/Vertigo.Contracts.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/Vertigo.Server.Game.Tests/Vertigo.Server.Game.Tests.csproj" />
  </Folder>
  <Folder Name="/tests/Phase5/">
    <Project Path="tests/Vertigo.Server.Matchmaking.Tests/Vertigo.Server.Matchmaking.Tests.csproj" />
  </Folder>
</Solution>
```

## File: src/Client/Vertigo.Client.UI/ViewModels/MainViewModel.cs
```csharp
using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UI.ViewModels;

public sealed class MainViewModel : ViewModelBase, IDisposable
{
    private readonly INavigator _navigation;
    private readonly PlayerSession _playerSession;
    private readonly HomeViewModel _home;
    private bool _disposed;
    private NavigationPage _previousPage;

    public MainViewModel(INavigator navigation, AccessViewModel access, MainContext context)
    {
        _navigation = navigation;
        _playerSession = context.SocialContext.PlayerSession;
        Access = access;
        Home = context.Home;
        Play = context.Home.Play;
        Lobby = context.Lobby;
        Social = context.SocialContext.Social;
        Settings = context.Settings;
        Profile = context.Profile;
        _home = context.Home;
        _previousPage = _navigation.CurrentPage;
        _navigation.PageChanged += OnPageChanged;
        if (_playerSession.IsAuthenticated) _navigation.NavigateTo(NavigationPage.Home);
        else Access.OnNavigatedTo();
    }

    public AccessViewModel Access { get; }
    public HomeViewModel Home { get; }
    public PlayViewModel Play { get; }
    public LobbyViewModel Lobby { get; }
    public SocialViewModel Social { get; }
    public SettingsViewModel Settings { get; }
    public ProfileViewModel Profile { get; }

    public NavigationPage CurrentPage => _navigation.CurrentPage;
    public bool IsAccessPage => CurrentPage == NavigationPage.Access;

    public AccessViewModel? AccessContent => IsAccessPage ? Access : null;

    public object? TemplatedContent => IsAccessPage ? null : CurrentViewModel;


    public object CurrentViewModel => CurrentPage switch
    {
        NavigationPage.Home => Home,
        NavigationPage.Play => Play,
        NavigationPage.Lobby => Lobby,
        NavigationPage.Social => Social,
        NavigationPage.Settings => Settings,
        NavigationPage.Profile => Profile,
        _ => Access
    };


    private void OnPageChanged(object? sender, NavigationPage page)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(() => OnPageChanged(sender, page));
            return;
        }
        if (_disposed) return;
        if (page != NavigationPage.Access && !_playerSession.IsAuthenticated)
        {
            _navigation.NavigateTo(NavigationPage.Access);
            return;
        }

        GetPage(_previousPage)?.OnNavigatedFrom();
        _previousPage = page;
        if (page == NavigationPage.Access)
        {
            Lobby.ResetState();
            Social.Reset();
        }
        GetPage(page)?.OnNavigatedTo();
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(IsAccessPage));
        OnPropertyChanged(nameof(AccessContent));
        OnPropertyChanged(nameof(TemplatedContent));
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    private INavigationAware? GetPage(NavigationPage page)
    {
        return page switch
        {
            NavigationPage.Access => Access,
            NavigationPage.Home => Home,
            NavigationPage.Play => Play,
            NavigationPage.Lobby => Lobby,
            NavigationPage.Social => Social,
            NavigationPage.Settings => Settings,
            NavigationPage.Profile => Profile,
            _ => null
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _navigation.PageChanged -= OnPageChanged;
    }
}
```

## File: src/Server/Vertigo.Server.Host/Program.cs
```csharp
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.DataAccess;
using CoreWCF;
using CoreWCF.Configuration;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.DependencyInjection;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.Matchmaking.Invitations.Dependencies;
using Vertigo.Server.Matchmaking.Invitations.Services;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Services;
using Vertigo.Server.PlayerManagement.Friendships.Dependencies;
using Vertigo.Server.PlayerManagement.Friendships.Services;
using Vertigo.Server.PlayerManagement.Registration.Services;
using Vertigo.Server.PlayerManagement.Blocking.Services;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.Matchmaking.Chat.Services;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseNetTcp(IPAddress.Loopback, ServiceEndpoints.TcpPort);
builder.Services.AddServiceModelServices();
builder.Services.AddVertigoDataAccess(builder.Configuration);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

builder.Services.AddSingleton<IGuestSessionRegistry, GuestSessionRegistry>();
builder.Services.AddScoped<ISessionIdentityResolver, SessionIdentityResolver>();
builder.Services.AddTransient<AccountServiceDependencies>();
builder.Services.AddTransient<AuthenticationDependencies>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddTransient<AccountService>();
builder.Services.AddTransient<BlockedPlayerService>();
builder.Services.AddSingleton<IFriendshipNotifier, FriendshipNotifier>();
builder.Services.AddTransient<FriendshipServiceDependencies>();
builder.Services.AddTransient<FriendshipService>();
builder.Services.AddSingleton<FriendshipNotificationService>();
builder.Services.AddMatchLifecycleServices();
builder.Services.AddSingleton<LobbyMatchActivation>();
builder.Services.AddSingleton<LobbyGameOperations>();
builder.Services.AddSingleton<LobbyService>();
builder.Services.AddSingleton<ILobbyDirectory>(services => services.GetRequiredService<LobbyService>());
builder.Services.AddTransient<LobbyInvitationServiceDependencies>();
builder.Services.AddTransient<LobbyInvitationService>();
builder.Services.AddTransient<ChatService>();

WebApplication app = builder.Build();
((IApplicationBuilder)app).UseServiceModel(ConfigureServices);
app.Run();

static void ConfigureServices(IServiceBuilder services)
{
    NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
    services.AddService<AccountService>()
        .AddServiceEndpoint<AccountService, IAccountService>(binding, ServiceEndpoints.AccountServiceAddress);
    services.AddService<AuthenticationService>()
        .AddServiceEndpoint<AuthenticationService, IAuthService>(binding, ServiceEndpoints.AuthServiceAddress);
    services.AddService<BlockedPlayerService>()
        .AddServiceEndpoint<BlockedPlayerService, IBlockedPlayerService>(
            binding, ServiceEndpoints.BlockedPlayerServiceAddress);
    services.AddService<FriendshipService>()
        .AddServiceEndpoint<FriendshipService, IFriendshipService>(binding, ServiceEndpoints.FriendshipServiceAddress);
    services.AddService<FriendshipNotificationService>()
        .AddServiceEndpoint<FriendshipNotificationService, IFriendshipNotificationService>(
            binding, ServiceEndpoints.FriendshipNotificationServiceAddress);
    services.AddService<ChatService>()
        .AddServiceEndpoint<ChatService, IChatService>(binding, ServiceEndpoints.ChatServiceAddress);
    services.AddService<LobbyService>()
        .AddServiceEndpoint<LobbyService, ILobbyService>(binding, ServiceEndpoints.LobbyServiceAddress);
    services.AddService<LobbyInvitationService>()
        .AddServiceEndpoint<LobbyInvitationService, ILobbyInvitationService>(
            binding, ServiceEndpoints.LobbyInvitationServiceAddress);
}
```

## File: src/Client/Vertigo.Client.UI/Resources/Localization/Strings.en.resx
```
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Error.Unknown" xml:space="preserve"><value>An error occurred. Please try again.</value></data>
  <data name="Error.Timeout" xml:space="preserve"><value>The server took too long to respond.</value></data>
  <data name="Error.Connection" xml:space="preserve"><value>Could not connect to the server.</value></data>
  <data name="Access.MissingCredentials" xml:space="preserve"><value>Enter a username and password.</value></data>
  <data name="Access.LoginFailed" xml:space="preserve"><value>Sign-in could not be completed.</value></data>
  <data name="Access.GuestFailed" xml:space="preserve"><value>Guest access could not be completed.</value></data>
  <data name="Access.MissingToken" xml:space="preserve"><value>A previous session token is required.</value></data>
  <data name="Access.ResumeFailed" xml:space="preserve"><value>The session could not be restored.</value></data>
  <data name="Home.LogoutFailed" xml:space="preserve"><value>The session could not be closed.</value></data>
  <data name="Lobby.SessionRequired" xml:space="preserve"><value>Your session has expired. Please sign in again.</value></data>
  <data name="Lobby.CodeRequired" xml:space="preserve"><value>Enter a lobby code.</value></data>
  <data name="Lobby.CreateFailed" xml:space="preserve"><value>The lobby could not be created.</value></data>
  <data name="Lobby.JoinFailed" xml:space="preserve"><value>Could not join the lobby.</value></data>
  <data name="Lobby.MessageFailed" xml:space="preserve"><value>The message could not be sent.</value></data>
  <data name="Lobby.LeaveFailed" xml:space="preserve"><value>The lobby could not be left.</value></data>
  <data name="Authentication.Unauthorized" xml:space="preserve"><value>Your session is no longer valid. Sign in again.</value></data>
  <data name="Lobby.ChatNotAllowed" xml:space="preserve"><value>You cannot send messages in this lobby.</value></data>
  <data name="RegisterAccount.UsernameAlreadyRegistered" xml:space="preserve"><value>Username already exists.</value></data>
  <data name="RegisterAccount.EmailAlreadyRegistered" xml:space="preserve"><value>Email already exists.</value></data>
  <data name="RegisterAccount.ValidationSummary" xml:space="preserve"><value>Check the registration details.</value></data>
  <data name="ROOM_NOT_FOUND" xml:space="preserve"><value>Lobby not found.</value></data>
  <data name="ROOM_FULL" xml:space="preserve"><value>The lobby is full.</value></data>
  <data name="MSG-ROOM-009" xml:space="preserve"><value>The match has already started.</value></data>
  <data name="MSG-ROOM-011" xml:space="preserve"><value>Invalid lobby capacity.</value></data>
  <data name="ErrorInvalidData" xml:space="preserve"><value>The supplied data is invalid.</value></data>
  <data name="ErrorUserNotFound" xml:space="preserve"><value>Player not found.</value></data>
  <data name="ErrorInvalidCode" xml:space="preserve"><value>The code is invalid.</value></data>
  <data name="ErrorInvalidCredentials" xml:space="preserve"><value>Incorrect username or password.</value></data>
  <data name="AccessMenu.SessionAlreadyActive" xml:space="preserve"><value>A session is already active.</value></data>
  <data name="Friends.RequestSent" xml:space="preserve"><value>The request has already been sent.</value></data>
  <data name="Friends.AlreadyFriends" xml:space="preserve"><value>You are already friends.</value></data>
  <data name="Friends.RequestBlocked" xml:space="preserve"><value>The request is blocked.</value></data>
  <data name="Notifications.FriendRequestNotFound" xml:space="preserve"><value>Friend request not found.</value></data>
  <data name="InviteFriends.NotAllowed" xml:space="preserve"><value>This invitation is not allowed.</value></data>
  <data name="Social.RegisteredOnly" xml:space="preserve"><value>Social features require a registered account.</value></data>
  <data name="Social.FriendsFailed" xml:space="preserve"><value>Could not load friendships.</value></data>
  <data name="Social.SearchLength" xml:space="preserve"><value>Enter at least three characters.</value></data>
  <data name="Social.SearchFailed" xml:space="preserve"><value>Could not search for players.</value></data>
  <data name="Social.RequestFailed" xml:space="preserve"><value>Could not complete the request.</value></data>
  <data name="Social.BlockFailed" xml:space="preserve"><value>Could not update blocked players.</value></data>
  <data name="Social.InvitationFailed" xml:space="preserve"><value>Could not manage invitations.</value></data>
  <data name="Social.NotificationsFailed" xml:space="preserve"><value>Could not connect to notifications.</value></data>
  <data name="Social.InvitationUnavailable" xml:space="preserve"><value>The invitation is no longer available. Refresh the list.</value></data>
  <data name="Lobby.NotMember" xml:space="preserve"><value>You are not a member of this lobby.</value></data>
  <data name="Lobby.NotHost" xml:space="preserve"><value>Only the host can start the match.</value></data>
  <data name="Lobby.NotReady" xml:space="preserve"><value>At least two players are required and all must be ready.</value></data>
  <data name="Lobby.AlreadyInGame" xml:space="preserve"><value>The match has already started.</value></data>
  <data name="Lobby.ReadyFailed" xml:space="preserve"><value>Could not change ready status.</value></data>
  <data name="Lobby.StartFailed" xml:space="preserve"><value>Could not start the match.</value></data>
  <data name="Access.BrandTagline" xml:space="preserve">
        <value>Building to new heights</value>
    </data>
  <data name="Access.Title" xml:space="preserve">
        <value>Welcome</value>
    </data>
  <data name="Access.Description" xml:space="preserve">
        <value>Build, collaborate, and reach new heights.</value>
    </data>
  <data name="Access.Username" xml:space="preserve">
        <value>Username</value>
    </data>
  <data name="Access.Password" xml:space="preserve">
        <value>Password</value>
    </data>
  <data name="Access.Login" xml:space="preserve">
        <value>Sign in</value>
    </data>
  <data name="Access.Guest" xml:space="preserve">
        <value>Continue as guest</value>
    </data>
  <data name="Access.ResumeSection" xml:space="preserve">
        <value>Resume a session</value>
    </data>
  <data name="Access.Token" xml:space="preserve">
        <value>Session token</value>
    </data>
  <data name="Access.Resume" xml:space="preserve">
        <value>Resume session</value>
    </data>
  <data name="Access.Progress" xml:space="preserve">
        <value>Processing request…</value>
    </data>
  <data name="Access.ErrorHeading" xml:space="preserve">
        <value>The action could not be completed</value>
    </data>
  <data name="Access.Help" xml:space="preserve">
        <value>Use Tab to navigate and Enter to confirm.</value>
    </data>
  <data name="Access.VisualCaption" xml:space="preserve">
        <value>A TOWER IS BUILT ONE FLOOR AT A TIME</value>
    </data>
  <data name="Access.SecurityNote" xml:space="preserve">
        <value>Do not share your session token.</value>
    </data>
  <data name="Access.NetworkNote" xml:space="preserve">
        <value>Communication with the server is secured.</value>
    </data>
  <data name="Access.Register" xml:space="preserve">
    <value>Register</value>
  </data>
  <data name="Access.RegisterHeading" xml:space="preserve">
    <value>Create an account</value>
  </data>
  <data name="Access.CreateAccount" xml:space="preserve">
    <value>Create account</value>
  </data>
  <data name="Access.Email" xml:space="preserve">
    <value>Email address</value>
  </data>
  <data name="Access.ConfirmPassword" xml:space="preserve">
    <value>Confirm password</value>
  </data>
  <data name="Access.Back" xml:space="preserve">
    <value>Back to sign in</value>
  </data>
  <data name="Access.ChangeLanguage" xml:space="preserve">
    <value>Switch language to Spanish</value>
  </data>
  <data name="Access.RegistrationInvalid" xml:space="preserve">
    <value>Complete the fields and check that the passwords match.</value>
  </data>
  <data name="Access.RegistrationFailed" xml:space="preserve">
    <value>The account could not be registered.</value>
  </data>
  <data name="Access.RegistrationSuccess" xml:space="preserve">
    <value>Account created. You can now sign in.</value>
  </data>
  <data name="Access.UsernameHint" xml:space="preserve">
    <value>3 to 20 letters or digits; include at least one A-Z letter, without spaces or symbols.</value>
  </data>
  <data name="Access.PasswordHint" xml:space="preserve">
    <value>8 to 20 characters, including an uppercase letter, digit and allowed symbol (!, #, etc.).</value>
  </data>
  <data name="Access.Validation.Username.Required" xml:space="preserve">
    <value>Enter a username.</value>
  </data>
  <data name="Access.Validation.Username.TooShort" xml:space="preserve">
    <value>Username must have at least 3 characters.</value>
  </data>
  <data name="Access.Validation.Username.TooLong" xml:space="preserve">
    <value>Username cannot exceed 20 characters.</value>
  </data>
  <data name="Access.Validation.Username.InvalidFormat" xml:space="preserve">
    <value>Use letters and digits with at least one A-Z letter; no spaces or symbols.</value>
  </data>
  <data name="Access.Validation.Email.Required" xml:space="preserve">
    <value>Enter an email address.</value>
  </data>
  <data name="Access.Validation.Email.TooLong" xml:space="preserve">
    <value>Email cannot exceed 60 characters.</value>
  </data>
  <data name="Access.Validation.Email.InvalidFormat" xml:space="preserve">
    <value>Enter an email with @ and a domain; before @ use letters, digits, dot, hyphen, underscore, + or %.</value>
  </data>
  <data name="Access.Validation.Password.Required" xml:space="preserve">
    <value>Enter a password.</value>
  </data>
  <data name="Access.Validation.Password.TooShort" xml:space="preserve">
    <value>Password must have at least 8 characters.</value>
  </data>
  <data name="Access.Validation.Password.TooLong" xml:space="preserve">
    <value>Password cannot exceed 20 characters.</value>
  </data>
  <data name="Access.Validation.Password.PasswordRequirementsNotMet" xml:space="preserve">
    <value>Include an uppercase letter, digit and allowed symbol (!@#$%&amp;*?_+-.); no spaces, quotes or &lt; or &gt;.</value>
  </data>
  <data name="Access.Validation.ConfirmPassword.Required" xml:space="preserve">
    <value>Confirm the password.</value>
  </data>
  <data name="Access.Validation.ConfirmPassword.PasswordsDoNotMatch" xml:space="preserve">
    <value>Passwords do not match.</value>
  </data>
  <data name="Home.BrandSubtitle" xml:space="preserve">
      <value>Building to New Heights</value>
  </data>
  <data name="Home.Play" xml:space="preserve">
      <value>PLAY</value>
  </data>
  <data name="Home.Social" xml:space="preserve">
      <value>SOCIAL</value>
  </data>
  <data name="Home.Ranking" xml:space="preserve">
      <value>LEADERBOARD</value>
  </data>
  <data name="Home.Settings" xml:space="preserve">
      <value>SETTINGS</value>
  </data>
  <data name="Home.Profile" xml:space="preserve">
      <value>Profile</value>
  </data>
  <data name="Home.GuestAccount" xml:space="preserve">
      <value>Guest account</value>
  </data>
  <data name="Home.RegisteredAccount" xml:space="preserve">
      <value>Registered account</value>
  </data>
  <data name="Home.Notifications" xml:space="preserve">
      <value>Notifications</value>
  </data>
  <data name="Home.NotificationsHelp" xml:space="preserve">
      <value>Check your requests and invitations in Social.</value>
  </data>
  <data name="Home.OpenSocialNotifications" xml:space="preserve">
      <value>Open Social</value>
  </data>
  <data name="Home.Logout" xml:space="preserve">
      <value>Log out</value>
  </data>
  <data name="Home.Create" xml:space="preserve">
      <value>Create lobby</value>
  </data>
  <data name="Home.Join" xml:space="preserve">
      <value>Join lobby</value>
  </data>
  <data name="Home.Code" xml:space="preserve">
      <value>Lobby code</value>
  </data>
  <data name="Home.Public" xml:space="preserve">
      <value>Public</value>
  </data>
  <data name="Home.MaxPlayers" xml:space="preserve">
      <value>Max players</value>
  </data>
  <data name="Home.Close" xml:space="preserve">
      <value>Close</value>
  </data>
  <data name="Home.Glossary" xml:space="preserve">
      <value>Glossary and game rules</value>
  </data>
  <data name="Home.NotAvailable" xml:space="preserve">
      <value>Available in a later phase</value>
  </data>
  <data name="Navigation.Back" xml:space="preserve">
    <value>Back</value>
  </data>
  <data name="Settings.Title" xml:space="preserve">
    <value>SETTINGS</value>
  </data>
  <data name="Settings.Language" xml:space="preserve">
    <value>Interface language</value>
  </data>
  <data name="Settings.Spanish" xml:space="preserve">
    <value>Spanish</value>
  </data>
  <data name="Settings.English" xml:space="preserve">
    <value>English</value>
  </data>
  <data name="Settings.Save" xml:space="preserve">
    <value>Save preferences</value>
  </data>
  <data name="Settings.SaveFailed" xml:space="preserve">
    <value>Could not save preferences on this device.</value>
  </data>
  <data name="Settings.LanguageInvalid" xml:space="preserve">
    <value>Select a supported language.</value>
  </data>
  <data name="Profile.Title" xml:space="preserve">
    <value>PROFILE</value>
  </data>
  <data name="Profile.Username" xml:space="preserve">
    <value>Player name</value>
  </data>
  <data name="Profile.Account" xml:space="preserve">
    <value>Account type</value>
  </data>
  <data name="SocialUi.Title" xml:space="preserve">
    <value>SOCIAL</value>
  </data>
  <data name="SocialUi.Refresh" xml:space="preserve">
    <value>Refresh</value>
  </data>
  <data name="SocialUi.Friends" xml:space="preserve">
    <value>Friends</value>
  </data>
  <data name="SocialUi.Requests" xml:space="preserve">
    <value>Requests</value>
  </data>
  <data name="SocialUi.Invitations" xml:space="preserve">
    <value>Invitations</value>
  </data>
  <data name="SocialUi.Blocked" xml:space="preserve">
    <value>Blocked</value>
  </data>
  <data name="SocialUi.Search" xml:space="preserve">
    <value>Search players</value>
  </data>
  <data name="SocialUi.SearchResults" xml:space="preserve">
    <value>Results</value>
  </data>
  <data name="SocialUi.RemoveFriend" xml:space="preserve">
    <value>Remove friend</value>
  </data>
  <data name="SocialUi.SendRequest" xml:space="preserve">
    <value>Send request</value>
  </data>
  <data name="SocialUi.Accept" xml:space="preserve">
    <value>Accept</value>
  </data>
  <data name="SocialUi.Reject" xml:space="preserve">
    <value>Reject</value>
  </data>
  <data name="SocialUi.Unblock" xml:space="preserve">
    <value>Unblock</value>
  </data>
  <data name="Play.Title" xml:space="preserve">
      <value>PLAY</value>
  </data>
  <data name="Play.Subtitle" xml:space="preserve">
      <value>Create your room or find a match.</value>
  </data>
  <data name="Play.CreateTitle" xml:space="preserve">
      <value>Create match</value>
  </data>
  <data name="Play.Public" xml:space="preserve">
      <value>Public room</value>
  </data>
  <data name="Play.MaxPlayers" xml:space="preserve">
      <value>Maximum players</value>
  </data>
  <data name="Play.Create" xml:space="preserve">
      <value>Create room</value>
  </data>
  <data name="Play.JoinTitle" xml:space="preserve">
      <value>Join with code</value>
  </data>
  <data name="Play.Code" xml:space="preserve">
      <value>Room code (6 characters)</value>
  </data>
  <data name="Play.Join" xml:space="preserve">
      <value>Join</value>
  </data>
  <data name="Play.BrowseTitle" xml:space="preserve">
      <value>Available public matches</value>
  </data>
  <data name="Play.Browse" xml:space="preserve">
      <value>Browse public matches</value>
  </data>
  <data name="Play.Refresh" xml:space="preserve">
      <value>Refresh list</value>
  </data>
  <data name="Play.Empty" xml:space="preserve">
      <value>No public rooms available.</value>
  </data>
  <data name="Play.Room" xml:space="preserve">
      <value>Room</value>
  </data>
  <data name="Play.Host" xml:space="preserve">
      <value>Host</value>
  </data>
  <data name="Play.Players" xml:space="preserve">
      <value>Players</value>
  </data>
  <data name="Play.InvalidCode" xml:space="preserve">
      <value>The code must contain six valid characters.</value>
  </data>
  <data name="Play.InvalidCapacity" xml:space="preserve">
      <value>Choose between 2 and 6 players.</value>
  </data>
  <data name="Play.SelectRoom" xml:space="preserve">
      <value>Select a room from the list.</value>
  </data>
  <data name="Play.NetworkError" xml:space="preserve">
      <value>The operation could not be completed. Check your connection.</value>
  </data>
  <data name="Room.Title" xml:space="preserve">
      <value>GAME ROOM</value>
  </data>
  <data name="Room.Code" xml:space="preserve">
      <value>Code</value>
  </data>
  <data name="Room.Leave" xml:space="preserve">
      <value>Leave room</value>
  </data>
  <data name="Room.Members" xml:space="preserve">
      <value>Participants</value>
  </data>
  <data name="Room.MembersDescription" xml:space="preserve">
      <value>Players in this room.</value>
  </data>
  <data name="Room.Preparation" xml:space="preserve">
      <value>Ready check</value>
  </data>
  <data name="Room.PreparationHint" xml:space="preserve">
      <value>All players must be ready to start.</value>
  </data>
  <data name="Room.Host" xml:space="preserve">
      <value>Host</value>
  </data>
  <data name="Room.Player" xml:space="preserve">
      <value>Player</value>
  </data>
  <data name="Room.Guest" xml:space="preserve">
      <value>Guest</value>
  </data>
  <data name="Room.Ready" xml:space="preserve">
      <value>Ready</value>
  </data>
  <data name="Room.NotReady" xml:space="preserve">
      <value>Not ready</value>
  </data>
  <data name="Room.InGame" xml:space="preserve">
      <value>Match in progress</value>
  </data>
  <data name="Room.Waiting" xml:space="preserve">
      <value>Waiting for players</value>
  </data>
  <data name="Room.ReadyAction" xml:space="preserve">
      <value>I'm ready</value>
  </data>
  <data name="Room.NotReadyAction" xml:space="preserve">
      <value>Not ready</value>
  </data>
  <data name="Room.Start" xml:space="preserve">
      <value>Start match</value>
  </data>
  <data name="Room.Chat" xml:space="preserve">
      <value>Room chat</value>
  </data>
  <data name="Room.MessagePlaceholder" xml:space="preserve">
      <value>Write a message...</value>
  </data>
  <data name="Room.Send" xml:space="preserve">
      <value>Send</value>
  </data>
  <data name="Room.ErrorHeading" xml:space="preserve">
      <value>The action could not be completed</value>
  </data>
  <data name="Match.Title" xml:space="preserve">
    <value>MATCH IN PROGRESS</value>
  </data>
  <data name="Match.Round" xml:space="preserve">
    <value>Round</value>
  </data>
  <data name="Match.Turn" xml:space="preserve">
    <value>Turn</value>
  </data>
  <data name="Match.Players" xml:space="preserve">
    <value>PLAYERS</value>
  </data>
  <data name="Match.Floors" xml:space="preserve">
    <value>Floors</value>
  </data>
  <data name="Match.Board" xml:space="preserve">
    <value>DISCARD PILES</value>
  </data>
  <data name="Match.BoardHint" xml:space="preserve">
    <value>Select a card from your hand and a compatible pile. Moves become available when Game is connected.</value>
  </data>
  <data name="Match.Hand" xml:space="preserve">
    <value>MY HAND</value>
  </data>
  <data name="Match.Wildcards" xml:space="preserve">
    <value>MASTER PLANS</value>
  </data>
  <data name="Match.Chat" xml:space="preserve">
    <value>MATCH CHAT</value>
  </data>
  <data name="Match.MessageHint" xml:space="preserve">
    <value>Message (max. 40 characters)</value>
  </data>
  <data name="Match.Send" xml:space="preserve">
    <value>Send</value>
  </data>
  <data name="Match.PlayCard" xml:space="preserve">
    <value>Play card</value>
  </data>
  <data name="Match.Exit" xml:space="preserve">
    <value>Exit</value>
  </data>
  <data name="Match.ExitConfirmTitle" xml:space="preserve">
    <value>Leave the match?</value>
  </data>
  <data name="Match.ExitConfirmBody" xml:space="preserve">
    <value>Leaving the match is not enabled yet. You can cancel this dialog.</value>
  </data>
  <data name="Match.Cancel" xml:space="preserve">
    <value>Cancel</value>
  </data>
  <data name="Match.Confirm" xml:space="preserve">
    <value>Confirm exit</value>
  </data>
</root>
```

## File: src/Client/Vertigo.Client.UI/Resources/Localization/Strings.resx
```
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Error.Unknown" xml:space="preserve"><value>Ocurrió un error. Inténtalo de nuevo.</value></data>
  <data name="Error.Timeout" xml:space="preserve"><value>El servidor tardó demasiado en responder.</value></data>
  <data name="Error.Connection" xml:space="preserve"><value>No se pudo conectar con el servidor.</value></data>
  <data name="Access.MissingCredentials" xml:space="preserve"><value>Ingresa usuario y contraseña.</value></data>
  <data name="Access.LoginFailed" xml:space="preserve"><value>No se pudo iniciar sesión.</value></data>
  <data name="Access.GuestFailed" xml:space="preserve"><value>No se pudo ingresar como invitado.</value></data>
  <data name="Access.MissingToken" xml:space="preserve"><value>Se requiere un token de sesión previo.</value></data>
  <data name="Access.ResumeFailed" xml:space="preserve"><value>No se pudo recuperar la sesión.</value></data>
  <data name="Home.LogoutFailed" xml:space="preserve"><value>No se pudo cerrar sesión.</value></data>
  <data name="Lobby.SessionRequired" xml:space="preserve"><value>Tu sesión caducó. Inicia sesión nuevamente.</value></data>
  <data name="Lobby.CodeRequired" xml:space="preserve"><value>Ingresa el código de sala.</value></data>
  <data name="Lobby.CreateFailed" xml:space="preserve"><value>No se pudo crear la sala.</value></data>
  <data name="Lobby.JoinFailed" xml:space="preserve"><value>No se pudo entrar a la sala.</value></data>
  <data name="Lobby.MessageFailed" xml:space="preserve"><value>No se pudo enviar el mensaje.</value></data>
  <data name="Lobby.LeaveFailed" xml:space="preserve"><value>No se pudo salir de la sala.</value></data>
  <data name="Authentication.Unauthorized" xml:space="preserve"><value>Tu sesión ya no es válida. Inicia sesión nuevamente.</value></data>
  <data name="Lobby.ChatNotAllowed" xml:space="preserve"><value>No puedes enviar mensajes en esta sala.</value></data>
  <data name="RegisterAccount.UsernameAlreadyRegistered" xml:space="preserve"><value>Ese nombre de usuario ya está registrado.</value></data>
  <data name="RegisterAccount.EmailAlreadyRegistered" xml:space="preserve"><value>Ese correo ya está registrado.</value></data>
  <data name="RegisterAccount.ValidationSummary" xml:space="preserve"><value>Revisa los datos de registro.</value></data>
  <data name="ROOM_NOT_FOUND" xml:space="preserve"><value>No se encontró la sala.</value></data>
  <data name="ROOM_FULL" xml:space="preserve"><value>La sala está llena.</value></data>
  <data name="MSG-ROOM-009" xml:space="preserve"><value>La partida de esa sala ya comenzó.</value></data>
  <data name="MSG-ROOM-011" xml:space="preserve"><value>La capacidad de la sala no es válida.</value></data>
  <data name="ErrorInvalidData" xml:space="preserve"><value>Los datos proporcionados no son válidos.</value></data>
  <data name="ErrorUserNotFound" xml:space="preserve"><value>No se encontró el jugador.</value></data>
  <data name="ErrorInvalidCode" xml:space="preserve"><value>El código no es válido.</value></data>
  <data name="ErrorInvalidCredentials" xml:space="preserve"><value>Usuario o contraseña incorrectos.</value></data>
  <data name="AccessMenu.SessionAlreadyActive" xml:space="preserve"><value>Ya existe una sesión activa.</value></data>
  <data name="Friends.RequestSent" xml:space="preserve"><value>La solicitud ya fue enviada.</value></data>
  <data name="Friends.AlreadyFriends" xml:space="preserve"><value>Ya son amigos.</value></data>
  <data name="Friends.RequestBlocked" xml:space="preserve"><value>La solicitud está bloqueada.</value></data>
  <data name="Notifications.FriendRequestNotFound" xml:space="preserve"><value>No se encontró la solicitud de amistad.</value></data>
  <data name="InviteFriends.NotAllowed" xml:space="preserve"><value>No está permitido invitar a este jugador.</value></data>
  <data name="Social.RegisteredOnly" xml:space="preserve"><value>Las funciones sociales requieren una cuenta registrada.</value></data>
  <data name="Social.FriendsFailed" xml:space="preserve"><value>No se pudieron cargar las amistades.</value></data>
  <data name="Social.SearchLength" xml:space="preserve"><value>Escribe al menos tres caracteres.</value></data>
  <data name="Social.SearchFailed" xml:space="preserve"><value>No se pudieron buscar jugadores.</value></data>
  <data name="Social.RequestFailed" xml:space="preserve"><value>No se pudo completar la solicitud.</value></data>
  <data name="Social.BlockFailed" xml:space="preserve"><value>No se pudo actualizar la lista de bloqueos.</value></data>
  <data name="Social.InvitationFailed" xml:space="preserve"><value>No se pudieron gestionar las invitaciones.</value></data>
  <data name="Social.NotificationsFailed" xml:space="preserve"><value>No se pudo conectar a las notificaciones.</value></data>
  <data name="Social.InvitationUnavailable" xml:space="preserve"><value>La invitación ya no está disponible. Actualiza la lista.</value></data>
  <data name="Lobby.NotMember" xml:space="preserve"><value>No perteneces a esta sala.</value></data>
  <data name="Lobby.NotHost" xml:space="preserve"><value>Solo el anfitrión puede iniciar la partida.</value></data>
  <data name="Lobby.NotReady" xml:space="preserve"><value>Se necesitan al menos dos jugadores y todos deben estar preparados.</value></data>
  <data name="Lobby.AlreadyInGame" xml:space="preserve"><value>La partida ya ha comenzado.</value></data>
  <data name="Lobby.ReadyFailed" xml:space="preserve"><value>No se pudo cambiar el estado de preparación.</value></data>
  <data name="Lobby.StartFailed" xml:space="preserve"><value>No se pudo iniciar la partida.</value></data>
  <data name="Access.BrandTagline" xml:space="preserve">
        <value>Construcción en altura</value>
    </data>
  <data name="Access.Title" xml:space="preserve">
        <value>Bienvenidos</value>
    </data>
  <data name="Access.Description" xml:space="preserve">
        <value>Construye, colabora y alcanza nuevas alturas.</value>
    </data>
  <data name="Access.Username" xml:space="preserve">
        <value>Usuario</value>
    </data>
  <data name="Access.Password" xml:space="preserve">
        <value>Contraseña</value>
    </data>
  <data name="Access.Login" xml:space="preserve">
        <value>Iniciar sesión</value>
    </data>
  <data name="Access.Guest" xml:space="preserve">
        <value>Continuar como invitado</value>
    </data>
  <data name="Access.ResumeSection" xml:space="preserve">
        <value>Recuperar una sesión</value>
    </data>
  <data name="Access.Token" xml:space="preserve">
        <value>Token de sesión</value>
    </data>
  <data name="Access.Resume" xml:space="preserve">
        <value>Recuperar sesión</value>
    </data>
  <data name="Access.Progress" xml:space="preserve">
        <value>Procesando solicitud…</value>
    </data>
  <data name="Access.ErrorHeading" xml:space="preserve">
        <value>No se pudo completar la acción</value>
    </data>
  <data name="Access.Help" xml:space="preserve">
        <value>Usa Tab para moverte y Enter para confirmar.</value>
    </data>
  <data name="Access.VisualCaption" xml:space="preserve">
        <value>UNA TORRE SE CONSTRUYE PISO A PISO</value>
    </data>
  <data name="Access.SecurityNote" xml:space="preserve">
        <value>No compartas tu token de sesión.</value>
    </data>
  <data name="Access.NetworkNote" xml:space="preserve">
        <value>La conexión con el servidor se realiza de forma segura.</value>
    </data>
  <data name="Access.Register" xml:space="preserve">
    <value>Registrarse</value>
  </data>
  <data name="Access.RegisterHeading" xml:space="preserve">
    <value>Crear una cuenta</value>
  </data>
  <data name="Access.CreateAccount" xml:space="preserve">
    <value>Crear cuenta</value>
  </data>
  <data name="Access.Email" xml:space="preserve">
    <value>Correo electrónico</value>
  </data>
  <data name="Access.ConfirmPassword" xml:space="preserve">
    <value>Confirmar contraseña</value>
  </data>
  <data name="Access.Back" xml:space="preserve">
    <value>Volver al inicio de sesión</value>
  </data>
  <data name="Access.ChangeLanguage" xml:space="preserve">
    <value>Cambiar idioma a inglés</value>
  </data>
  <data name="Access.RegistrationInvalid" xml:space="preserve">
    <value>Completa los datos y verifica que las contraseñas coincidan.</value>
  </data>
  <data name="Access.RegistrationFailed" xml:space="preserve">
    <value>No se pudo registrar la cuenta.</value>
  </data>
  <data name="Access.RegistrationSuccess" xml:space="preserve">
    <value>Cuenta creada. Ya puedes iniciar sesión.</value>
  </data>
  <data name="Access.UsernameHint" xml:space="preserve">
    <value>De 3 a 20 letras o números; incluye al menos una letra A-Z, sin espacios ni símbolos.</value>
  </data>
  <data name="Access.PasswordHint" xml:space="preserve">
    <value>De 8 a 20 caracteres; usa mayúscula, número y un símbolo permitido (!, #, etc.).</value>
  </data>
  <data name="Access.Validation.Username.Required" xml:space="preserve">
    <value>Escribe un nombre de usuario.</value>
  </data>
  <data name="Access.Validation.Username.TooShort" xml:space="preserve">
    <value>El usuario debe tener al menos 3 caracteres.</value>
  </data>
  <data name="Access.Validation.Username.TooLong" xml:space="preserve">
    <value>El usuario no puede superar 20 caracteres.</value>
  </data>
  <data name="Access.Validation.Username.InvalidFormat" xml:space="preserve">
    <value>Usa solo letras y números, con al menos una letra A-Z; sin espacios ni símbolos.</value>
  </data>
  <data name="Access.Validation.Email.Required" xml:space="preserve">
    <value>Escribe un correo electrónico.</value>
  </data>
  <data name="Access.Validation.Email.TooLong" xml:space="preserve">
    <value>El correo no puede superar 60 caracteres.</value>
  </data>
  <data name="Access.Validation.Email.InvalidFormat" xml:space="preserve">
    <value>Introduce un correo con @ y dominio; solo letras, números, punto, guion, guion bajo, + o % antes de @.</value>
  </data>
  <data name="Access.Validation.Password.Required" xml:space="preserve">
    <value>Escribe una contraseña.</value>
  </data>
  <data name="Access.Validation.Password.TooShort" xml:space="preserve">
    <value>La contraseña debe tener al menos 8 caracteres.</value>
  </data>
  <data name="Access.Validation.Password.TooLong" xml:space="preserve">
    <value>La contraseña no puede superar 20 caracteres.</value>
  </data>
  <data name="Access.Validation.Password.PasswordRequirementsNotMet" xml:space="preserve">
    <value>Incluye mayúscula, número y símbolo permitido (!@#$%&amp;*?_+-.); no uses espacios, comillas ni &lt; o &gt;.</value>
  </data>
  <data name="Access.Validation.ConfirmPassword.Required" xml:space="preserve">
    <value>Confirma la contraseña.</value>
  </data>
  <data name="Access.Validation.ConfirmPassword.PasswordsDoNotMatch" xml:space="preserve">
    <value>Las contraseñas no coinciden.</value>
  </data>
  <data name="Home.BrandSubtitle" xml:space="preserve">
      <value>Construcción en Altura</value>
  </data>
  <data name="Home.Play" xml:space="preserve">
      <value>JUGAR</value>
  </data>
  <data name="Home.Social" xml:space="preserve">
      <value>SOCIAL</value>
  </data>
  <data name="Home.Ranking" xml:space="preserve">
      <value>CLASIFICACIÓN</value>
  </data>
  <data name="Home.Settings" xml:space="preserve">
      <value>AJUSTES</value>
  </data>
  <data name="Home.Profile" xml:space="preserve">
      <value>Perfil</value>
  </data>
  <data name="Home.GuestAccount" xml:space="preserve">
      <value>Cuenta invitada</value>
  </data>
  <data name="Home.RegisteredAccount" xml:space="preserve">
      <value>Cuenta registrada</value>
  </data>
  <data name="Home.Notifications" xml:space="preserve">
      <value>Notificaciones</value>
  </data>
  <data name="Home.NotificationsHelp" xml:space="preserve">
      <value>Consulta tus solicitudes e invitaciones desde Social.</value>
  </data>
  <data name="Home.OpenSocialNotifications" xml:space="preserve">
      <value>Abrir Social</value>
  </data>
  <data name="Home.Logout" xml:space="preserve">
      <value>Cerrar sesión</value>
  </data>
  <data name="Home.Create" xml:space="preserve">
      <value>Crear sala</value>
  </data>
  <data name="Home.Join" xml:space="preserve">
      <value>Unirse a sala</value>
  </data>
  <data name="Home.Code" xml:space="preserve">
      <value>Código de sala</value>
  </data>
  <data name="Home.Public" xml:space="preserve">
      <value>Pública</value>
  </data>
  <data name="Home.MaxPlayers" xml:space="preserve">
      <value>Jugadores máximos</value>
  </data>
  <data name="Home.Close" xml:space="preserve">
      <value>Cerrar</value>
  </data>
  <data name="Home.Glossary" xml:space="preserve">
      <value>Glosario y reglas del juego</value>
  </data>
  <data name="Home.NotAvailable" xml:space="preserve">
      <value>Disponible en una fase posterior</value>
  </data>
  <data name="Navigation.Back" xml:space="preserve">
    <value>Regresar</value>
  </data>
  <data name="Settings.Title" xml:space="preserve">
    <value>AJUSTES</value>
  </data>
  <data name="Settings.Language" xml:space="preserve">
    <value>Idioma de la interfaz</value>
  </data>
  <data name="Settings.Spanish" xml:space="preserve">
    <value>Español</value>
  </data>
  <data name="Settings.English" xml:space="preserve">
    <value>Inglés</value>
  </data>
  <data name="Settings.Save" xml:space="preserve">
    <value>Guardar preferencias</value>
  </data>
  <data name="Settings.SaveFailed" xml:space="preserve">
    <value>No se pudieron guardar las preferencias en este equipo.</value>
  </data>
  <data name="Settings.LanguageInvalid" xml:space="preserve">
    <value>Selecciona un idioma disponible.</value>
  </data>
  <data name="Profile.Title" xml:space="preserve">
    <value>PERFIL</value>
  </data>
  <data name="Profile.Username" xml:space="preserve">
    <value>Nombre de jugador</value>
  </data>
  <data name="Profile.Account" xml:space="preserve">
    <value>Tipo de cuenta</value>
  </data>
  <data name="SocialUi.Title" xml:space="preserve">
    <value>SOCIAL</value>
  </data>
  <data name="SocialUi.Refresh" xml:space="preserve">
    <value>Actualizar</value>
  </data>
  <data name="SocialUi.Friends" xml:space="preserve">
    <value>Amigos</value>
  </data>
  <data name="SocialUi.Requests" xml:space="preserve">
    <value>Solicitudes</value>
  </data>
  <data name="SocialUi.Invitations" xml:space="preserve">
    <value>Invitaciones</value>
  </data>
  <data name="SocialUi.Blocked" xml:space="preserve">
    <value>Bloqueados</value>
  </data>
  <data name="SocialUi.Search" xml:space="preserve">
    <value>Buscar jugadores</value>
  </data>
  <data name="SocialUi.SearchResults" xml:space="preserve">
    <value>Resultados</value>
  </data>
  <data name="SocialUi.RemoveFriend" xml:space="preserve">
    <value>Eliminar amigo</value>
  </data>
  <data name="SocialUi.SendRequest" xml:space="preserve">
    <value>Enviar solicitud</value>
  </data>
  <data name="SocialUi.Accept" xml:space="preserve">
    <value>Aceptar</value>
  </data>
  <data name="SocialUi.Reject" xml:space="preserve">
    <value>Rechazar</value>
  </data>
  <data name="SocialUi.Unblock" xml:space="preserve">
    <value>Desbloquear</value>
  </data>
  <data name="Play.Title" xml:space="preserve">
      <value>JUGAR</value>
  </data>
  <data name="Play.Subtitle" xml:space="preserve">
      <value>Construye tu sala o encuentra una partida.</value>
  </data>
  <data name="Play.CreateTitle" xml:space="preserve">
      <value>Crear partida</value>
  </data>
  <data name="Play.Public" xml:space="preserve">
      <value>Sala pública</value>
  </data>
  <data name="Play.MaxPlayers" xml:space="preserve">
      <value>Máximo de jugadores</value>
  </data>
  <data name="Play.Create" xml:space="preserve">
      <value>Crear sala</value>
  </data>
  <data name="Play.JoinTitle" xml:space="preserve">
      <value>Unirse con código</value>
  </data>
  <data name="Play.Code" xml:space="preserve">
      <value>Código de sala (6 caracteres)</value>
  </data>
  <data name="Play.Join" xml:space="preserve">
      <value>Unirse</value>
  </data>
  <data name="Play.BrowseTitle" xml:space="preserve">
      <value>Partidas públicas disponibles</value>
  </data>
  <data name="Play.Browse" xml:space="preserve">
      <value>Buscar partidas públicas</value>
  </data>
  <data name="Play.Refresh" xml:space="preserve">
      <value>Actualizar lista</value>
  </data>
  <data name="Play.Empty" xml:space="preserve">
      <value>No hay salas públicas disponibles.</value>
  </data>
  <data name="Play.Room" xml:space="preserve">
      <value>Sala</value>
  </data>
  <data name="Play.Host" xml:space="preserve">
      <value>Anfitrión</value>
  </data>
  <data name="Play.Players" xml:space="preserve">
      <value>Jugadores</value>
  </data>
  <data name="Play.InvalidCode" xml:space="preserve">
      <value>El código debe tener seis caracteres válidos.</value>
  </data>
  <data name="Play.InvalidCapacity" xml:space="preserve">
      <value>Selecciona entre 2 y 6 jugadores.</value>
  </data>
  <data name="Play.SelectRoom" xml:space="preserve">
      <value>Selecciona una sala de la lista.</value>
  </data>
  <data name="Play.NetworkError" xml:space="preserve">
      <value>No se pudo completar la operación. Comprueba la conexión.</value>
  </data>
  <data name="Room.Title" xml:space="preserve">
      <value>SALA DE PARTIDA</value>
  </data>
  <data name="Room.Code" xml:space="preserve">
      <value>Código</value>
  </data>
  <data name="Room.Leave" xml:space="preserve">
      <value>Salir de la sala</value>
  </data>
  <data name="Room.Members" xml:space="preserve">
      <value>Participantes</value>
  </data>
  <data name="Room.MembersDescription" xml:space="preserve">
      <value>Jugadores conectados a esta sala.</value>
  </data>
  <data name="Room.Preparation" xml:space="preserve">
      <value>Preparación</value>
  </data>
  <data name="Room.PreparationHint" xml:space="preserve">
      <value>Todos los jugadores deben estar listos para comenzar.</value>
  </data>
  <data name="Room.Host" xml:space="preserve">
      <value>Anfitrión</value>
  </data>
  <data name="Room.Player" xml:space="preserve">
      <value>Jugador</value>
  </data>
  <data name="Room.Guest" xml:space="preserve">
      <value>Invitado</value>
  </data>
  <data name="Room.Ready" xml:space="preserve">
      <value>Listo</value>
  </data>
  <data name="Room.NotReady" xml:space="preserve">
      <value>No listo</value>
  </data>
  <data name="Room.InGame" xml:space="preserve">
      <value>Partida iniciada</value>
  </data>
  <data name="Room.Waiting" xml:space="preserve">
      <value>Esperando jugadores</value>
  </data>
  <data name="Room.ReadyAction" xml:space="preserve">
      <value>Estoy listo</value>
  </data>
  <data name="Room.NotReadyAction" xml:space="preserve">
      <value>Dejar de estar listo</value>
  </data>
  <data name="Room.Start" xml:space="preserve">
      <value>Iniciar partida</value>
  </data>
  <data name="Room.Chat" xml:space="preserve">
      <value>Chat de la sala</value>
  </data>
  <data name="Room.MessagePlaceholder" xml:space="preserve">
      <value>Escribe un mensaje...</value>
  </data>
  <data name="Room.Send" xml:space="preserve">
      <value>Enviar</value>
  </data>
  <data name="Room.ErrorHeading" xml:space="preserve">
      <value>No se pudo completar la acción</value>
  </data>
  <data name="Match.Title" xml:space="preserve">
    <value>PARTIDA EN CURSO</value>
  </data>
  <data name="Match.Round" xml:space="preserve">
    <value>Ronda</value>
  </data>
  <data name="Match.Turn" xml:space="preserve">
    <value>Turno</value>
  </data>
  <data name="Match.Players" xml:space="preserve">
    <value>JUGADORES</value>
  </data>
  <data name="Match.Floors" xml:space="preserve">
    <value>Pisos</value>
  </data>
  <data name="Match.Board" xml:space="preserve">
    <value>PILAS DE DESCARTE</value>
  </data>
  <data name="Match.BoardHint" xml:space="preserve">
    <value>Selecciona una carta de tu mano y una pila compatible. Las jugadas se activarán al conectar Game.</value>
  </data>
  <data name="Match.Hand" xml:space="preserve">
    <value>MI MANO</value>
  </data>
  <data name="Match.Wildcards" xml:space="preserve">
    <value>PLANOS MAESTROS</value>
  </data>
  <data name="Match.Chat" xml:space="preserve">
    <value>CHAT DE PARTIDA</value>
  </data>
  <data name="Match.MessageHint" xml:space="preserve">
    <value>Mensaje (máx. 40 caracteres)</value>
  </data>
  <data name="Match.Send" xml:space="preserve">
    <value>Enviar</value>
  </data>
  <data name="Match.PlayCard" xml:space="preserve">
    <value>Colocar carta</value>
  </data>
  <data name="Match.Exit" xml:space="preserve">
    <value>Salir</value>
  </data>
  <data name="Match.ExitConfirmTitle" xml:space="preserve">
    <value>¿Abandonar la partida?</value>
  </data>
  <data name="Match.ExitConfirmBody" xml:space="preserve">
    <value>La salida definitiva todavía no está habilitada. Podrás cancelar este diálogo.</value>
  </data>
  <data name="Match.Cancel" xml:space="preserve">
    <value>Cancelar</value>
  </data>
  <data name="Match.Confirm" xml:space="preserve">
    <value>Confirmar salida</value>
  </data>
</root>
```

## File: src/Client/Vertigo.Client.UI/App.axaml.cs
```csharp
using Vertigo.Client.UI.Utilities.Settings.Interfaces;
using Vertigo.Client.UI.Utilities.Settings.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.Views;
using Vertigo.Client.UI.ViewModels.Social;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Services;

namespace Vertigo.Client.UI;

public partial class App : Application
{
    private ServiceProvider? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServiceCollection registrations = new();
            registrations.AddVertigoClientNetworking();
            registrations.AddSingleton<ITextLocalizer, TextLocalizer>();
            registrations.AddSingleton<IClientErrorText, ClientErrorText>();
            registrations.AddSingleton<ISessionGuard, SessionGuard>();
            registrations.AddTransient<UiServices>();
            registrations.AddTransient<UiNavigationContext>();
            registrations.AddTransient<AccessContext>();
            registrations.AddTransient<LobbyContext>();
            registrations.AddTransient<HomeContext>();
            registrations.AddSingleton<IUserSettingsStore, JsonUserSettingsStore>();
            registrations.AddSingleton<SettingsViewModel>();
            registrations.AddSingleton<ProfileViewModel>();
            registrations.AddTransient<MainScreensContext>();
            registrations.AddTransient<MainContext>();
            registrations.AddSingleton<AccessViewModel>();
            registrations.AddSingleton<PlayViewModel>();
            registrations.AddSingleton<HomeViewModel>();
            registrations.AddSingleton<LobbyViewModel>();
            registrations.AddSingleton<SocialOperation>();
            registrations.AddSingleton<FriendsViewModel>();
            registrations.AddSingleton<BlockedPlayersViewModel>();
            registrations.AddTransient<LobbyInvitationContext>();
            registrations.AddSingleton<LobbyInvitationsViewModel>();
            registrations.AddTransient<SocialNavigationContext>();
            registrations.AddTransient<SocialViewModelContext>();
            registrations.AddTransient<MainSocialContext>();
            registrations.AddSingleton<SocialViewModel>();
            registrations.AddSingleton<MainViewModel>();
            _services = registrations.BuildServiceProvider();
            UiLanguage.Apply(_services.GetRequiredService<IUserSettingsStore>().Load().Language);
            desktop.MainWindow = new MainWindow
            {
                DataContext = _services.GetRequiredService<MainViewModel>()
            };
            desktop.Exit += OnDesktopExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs eventArgs)
    {
        _services?.Dispose();
        _services = null;
    }
}
```

## File: Vertigo.Client.slnx
```
<Solution>
  <Folder Name="/src/" />
  <Folder Name="/src/Client/">
    <Project Path="src/Client/Vertigo.Client.Core/Vertigo.Client.Core.csproj" />
    <Project Path="src/Client/Vertigo.Client.Networking/Vertigo.Client.Networking.csproj" />
    <Project Path="src/Client/Vertigo.Client.UI/Vertigo.Client.UI.csproj" />
  </Folder>
  <Folder Name="/src/Shared/">
    <Project Path="src/Shared/Vertigo.Contracts/Vertigo.Contracts.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/Vertigo.Client.Networking.Smoke/Vertigo.Client.Networking.Smoke.csproj" />
    <Project Path="tests/Vertigo.Client.Multiplayer.Smoke/Vertigo.Client.Multiplayer.Smoke.csproj" />
    <Project Path="tests/Vertigo.Client.LobbyPreparation.Smoke/Vertigo.Client.LobbyPreparation.Smoke.csproj" />
    <Project Path="tests/Vertigo.Client.PrivateInvitations.Smoke/Vertigo.Client.PrivateInvitations.Smoke.csproj" />
    <Project Path="tests/Vertigo.Client.UnitTests/Vertigo.Client.UnitTests.csproj" />
    <Project Path="tests/Vertigo.Server.Matchmaking.Tests/Vertigo.Server.Matchmaking.Tests.csproj" />
    <Project Path="tests/Vertigo.Client.TestAccounts.Setup/Vertigo.Client.TestAccounts.Setup.csproj" />
    <Project Path="tests/Vertigo.Client.Reconnection.Smoke/Vertigo.Client.Reconnection.Smoke.csproj" />
  </Folder>
</Solution>
```
