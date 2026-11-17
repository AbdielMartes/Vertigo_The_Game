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
