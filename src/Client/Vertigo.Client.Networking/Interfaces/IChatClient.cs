using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IChatClient
{
    ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken);

    ClientResult<bool> SendMatchMessage(MatchChatRequest request);
}
