using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class ChatClient : IChatClient
{
    private readonly IServiceCaller _serviceCaller;

    public ChatClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<bool> SendMessage(string lobbyCode, string text, string sessionToken)
    {
        return _serviceCaller.Call<IChatService, (string, string, string), bool>(
            ServiceEndpoints.ChatServiceAddress,
            (channel, args) => channel.SendMessage(args.Item1, args.Item2, args.Item3),
            (lobbyCode, text, sessionToken));
    }
}
