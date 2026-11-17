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
