using Vertigo.Client.UI.Utilities.Settings.Models;

namespace Vertigo.Client.UI.Utilities.Settings.Interfaces;

public interface IUserSettingsStore
{
    UserSettings Load();
    bool TrySave(UserSettings settings);
}
