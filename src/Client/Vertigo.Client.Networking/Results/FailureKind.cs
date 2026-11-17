namespace Vertigo.Client.Networking.Results;

public enum FailureKind
{
    None,
    Business,
    ServerUnreachable,
    Timeout,
    Unexpected
}
