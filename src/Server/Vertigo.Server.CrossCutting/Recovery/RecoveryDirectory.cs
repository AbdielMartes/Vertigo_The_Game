using System.Text.Json;

namespace Vertigo.Server.CrossCutting.Recovery;

public sealed class RecoveryDirectory : IDisposable
{
    private readonly FileStream _lease;

    public RecoveryDirectory(string path)
    {
        Root = Path.GetFullPath(path);
        if (OperatingSystem.IsWindows())
        {
            Directory.CreateDirectory(Root);
        }
        else
        {
            Directory.CreateDirectory(Root, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }

        _lease = new FileStream(Path.Combine(Root, "host.lock"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    }

    public string Root { get; }

    public static void Write<T>(string path, T value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string temporaryPath = path + ".pending";
        FileStreamOptions options = new()
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Share = FileShare.None
        };
        if (!OperatingSystem.IsWindows())
        {
            options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
        }

        using (FileStream stream = new(temporaryPath, options))
        {
            JsonSerializer.Serialize(stream, value);
            stream.Flush(flushToDisk: true);
        }

        File.Move(temporaryPath, path);
    }

    public static T Read<T>(string path) where T : class
    {
        using FileStream stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream) ?? throw new InvalidDataException("Missing recovery record.");
    }

    public void Dispose()
    {
        _lease.Dispose();
    }
}
