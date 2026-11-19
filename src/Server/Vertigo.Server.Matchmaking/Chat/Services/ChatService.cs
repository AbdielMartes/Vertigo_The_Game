using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Chat.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
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
