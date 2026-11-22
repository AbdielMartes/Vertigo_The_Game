using System.Runtime.Serialization;
using System.Xml;
using CoreWCF;
using CoreWCF.Channels;
using Vertigo.Contracts.Endpoints;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

internal static class LobbySessionHeader
{
    public static (string? Token, string? Rejection) Read(MessageHeaders? headers, string? bodyToken)
    {
        string? token = bodyToken;
        string? rejection = null;
        if (headers is not null)
        {
            try
            {
                int index = headers.FindHeader(SessionHeader.Name, SessionHeader.Namespace);
                if (index >= 0)
                {
                    if (bodyToken is not null)
                    {
                        rejection = "MultipleCredentialSources";
                    }
                    else
                    {
                        token = headers.GetHeader<string>(index);
                    }
                }
            }
            catch (MessageHeaderException)
            {
                rejection = "DuplicateSessionHeader";
            }
            catch (Exception ex) when (ex is SerializationException or XmlException)
            {
                rejection = "InvalidSessionHeader";
            }
        }

        return (token, rejection);
    }
}
