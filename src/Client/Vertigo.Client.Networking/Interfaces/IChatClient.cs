using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.Networking.Interfaces;

public interface IChatClient
{
    ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken);
}
