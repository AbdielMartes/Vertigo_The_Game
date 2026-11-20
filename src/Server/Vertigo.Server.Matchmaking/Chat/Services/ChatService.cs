using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Chat.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public sealed class ChatService : IChatService
{
    private const int MaximumMessageLength = 500;

    private readonly ILobbyDirectory _lobbies;
    private readonly ChatServiceDependencies _dependencies;

    public ChatService(ILobbyDirectory lobbies, ChatServiceDependencies dependencies)
    {
        _lobbies = lobbies;
        _dependencies = dependencies;
    }

    public bool SendMessage(string lobbyCode, string text, string sessionToken)
    {
        SessionIdentity sender = _dependencies.Sessions.Require(sessionToken);
        if (string.IsNullOrWhiteSpace(lobbyCode) || string.IsNullOrWhiteSpace(text)
            || text.Length > MaximumMessageLength)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return Publish(sender, (lobbyCode, text, null));
    }

    public bool SendMatchMessage(MatchChatRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SessionIdentity sender = _dependencies.Sessions.Require(request.SessionToken);
        if (request.MatchId == Guid.Empty || string.IsNullOrWhiteSpace(request.LobbyCode)
            || !MatchChatRules.CanSend(request.Text))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return Publish(sender, (request.LobbyCode, request.Text, request.MatchId));
    }

    private bool Publish(SessionIdentity sender, (string LobbyCode, string Text, Guid? MatchId) content)
    {
        if (_dependencies.LanguageFilter.ContainsBlockedLanguage(content.Text))
        {
            throw CreateFault(ServiceErrorCodes.ChatLanguageBlocked, ServiceMessageKeys.ChatLanguageBlocked);
        }

        LobbyChatMessage message = new()
        {
            LobbyCode = content.LobbyCode.Trim(),
            MatchId = content.MatchId,
            SenderId = sender.PlayerId,
            SenderUsername = sender.Username,
            Text = content.Text.Trim(),
            SentAt = _dependencies.TimeProvider.GetUtcNow().UtcDateTime
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
