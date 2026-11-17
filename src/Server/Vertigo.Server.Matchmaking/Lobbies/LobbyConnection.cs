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
