using System;
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.UI.Utilities.Localization.Interfaces;

public interface IClientErrorText
{
    string FromResult<T>(ClientResult<T> result, string fallbackKey);
    string FromException(Exception exception, string fallbackKey);
    bool IsUnauthorized<T>(ClientResult<T> result);
    bool IsUnauthorized(Exception exception);
}
