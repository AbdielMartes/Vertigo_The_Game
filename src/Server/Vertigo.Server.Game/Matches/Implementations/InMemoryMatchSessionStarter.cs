using Vertigo.Contracts.Internal;

namespace Vertigo.Server.Game.Matches.Implementations;

// Single-process implementation; Game owns the created match sessions.
public sealed class InMemoryMatchSessionStarter : IMatchSessionStarter
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<Guid, MatchSessionSnapshot> _matches = new();
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
            if (_matches.TryGetValue(request.LobbyInstanceId, out MatchSessionSnapshot? existing))
            {
                EnsureSameRequest(existing, request);
                result = existing;
            }
            else
            {
                Guid[] players = request.PlayerIds.ToArray();
                result = new MatchSessionSnapshot(
                    Guid.NewGuid(),
                    request.LobbyInstanceId,
                    request.LobbyCode,
                    request.HostPlayerId,
                    Array.AsReadOnly(players),
                    _timeProvider.GetUtcNow().UtcDateTime);
                _matches.Add(request.LobbyInstanceId, result);
            }
        }

        return result;
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
