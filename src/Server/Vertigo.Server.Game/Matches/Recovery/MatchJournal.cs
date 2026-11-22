using System.Globalization;
using System.Text.Json;
using Vertigo.Contracts.Internal;
using Vertigo.Server.CrossCutting.Recovery;
using Vertigo.Server.Game.Matches.Models;

namespace Vertigo.Server.Game.Matches.Recovery;

internal sealed class MatchJournal
{
    internal sealed record Header(MatchSessionSnapshot Session, RecoverableLobby? Lobby, Guid Seed);
    internal sealed record Action(string Kind, Guid PlayerId, JsonElement? Payload = null);
    internal sealed record Entry(DateTimeOffset Time, Action Action);
    internal sealed record Commit(Entry Entry, long BoardRevision, long LifecycleRevision);

    private const string StartFile = "start.json";
    private const string FormatDirectory = "matches-v1";
    private readonly string _root;

    public MatchJournal(RecoveryDirectory directory)
    {
        _root = Path.Combine(directory.Root, FormatDirectory);
        Directory.CreateDirectory(_root);
    }

    public IEnumerable<Header> ReadStartedMatches()
    {
        return Directory.EnumerateDirectories(_root)
            .Select(path => Path.Combine(path, StartFile))
            .Where(File.Exists)
            .Select(RecoveryDirectory.Read<Header>);
    }

    public void Start(Header header)
    {
        RecoveryDirectory.Write(Path.Combine(MatchPath(header.Session.MatchId), StartFile), header);
    }

    public void Append(Guid matchId, int sequence, Commit commit)
    {
        RecoveryDirectory.Write(Path.Combine(MatchPath(matchId), EventName(sequence)), commit);
    }

    public IEnumerable<Commit> ReadEvents(Guid matchId)
    {
        int sequence = 0;
        foreach (string path in Directory.EnumerateFiles(MatchPath(matchId), "*.event.json")
            .OrderBy(path => path, StringComparer.Ordinal))
        {
            if (Path.GetFileName(path) != EventName(++sequence))
            {
                throw new InvalidDataException("A match recovery event is missing.");
            }

            yield return RecoveryDirectory.Read<Commit>(path);
        }
    }

    private static string EventName(int sequence)
    {
        return sequence.ToString("D10", CultureInfo.InvariantCulture) + ".event.json";
    }

    public static MatchRecord CreateRecord(Header header)
    {
        MatchRandom random = new(header.Seed);
        return new MatchRecord(header.Session, header.Session.CreatedAtUtc,
            (random.CreateDeck, random.Next));
    }

    private string MatchPath(Guid matchId)
    {
        return Path.Combine(_root, matchId.ToString("N"));
    }

    public static void Apply(MatchRecord record, Entry entry)
    {
        Action action = entry.Action;
        DateTimeOffset now = entry.Time;
        Guid player = action.PlayerId;
        switch (action.Kind)
        {
            case nameof(record.TryPlace): record.TryPlace(player, Read<MatchPlacement>(action), now); break;
            case nameof(record.TryUseMasterPlan): record.TryUseMasterPlan(player, Read<MatchMasterPlan>(action), now); break;
            case nameof(record.TryDismantle): record.TryDismantle(player, Read<MatchDismantle>(action), now); break;
            case nameof(record.TryVoteToExpel): record.TryVoteToExpel(player, Read<MatchExpulsionVote>(action), now); break;
            case nameof(record.TryLeaveMatch): record.TryLeaveMatch(player, now); break;
            case nameof(record.TryExpireTurn): record.TryExpireTurn(now); break;
            case nameof(record.TryConfirmPresence): record.TryConfirmPresence(player, now); break;
            case nameof(record.TrySetConnected): record.TrySetConnected(player, false, now); break;
            case nameof(record.TryReconnect): record.TryReconnect(player, now); break;
            case nameof(record.TryTransition): record.TryTransition(Read<MatchLifecycleStatus>(action), now); break;
            case nameof(record.PauseForRecovery): record.PauseForRecovery(now); break;
            default: throw new InvalidDataException("Unknown match recovery event.");
        }
    }

    private static T Read<T>(Action action)
    {
        return action.Payload is JsonElement data
            ? data.Deserialize<T>() ?? throw new InvalidDataException("Invalid match event payload.")
            : throw new InvalidDataException("Missing match event payload.");
    }
}
