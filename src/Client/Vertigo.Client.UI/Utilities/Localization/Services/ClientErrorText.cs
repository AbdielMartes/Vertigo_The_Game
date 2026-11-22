using System;
using System.ServiceModel;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.Utilities.Localization.Services;

public sealed class ClientErrorText : IClientErrorText
{
    private readonly ITextLocalizer _localizer;

    public ClientErrorText(ITextLocalizer localizer)
    {
        _localizer = localizer;
    }

    public string FromResult<T>(ClientResult<T> result, string fallbackKey)
    {
        string key = result.Failure switch
        {
            FailureKind.Business when !string.IsNullOrWhiteSpace(result.MessageKey) => result.MessageKey,
            FailureKind.Timeout => "Error.Timeout",
            FailureKind.ServerUnreachable => "Error.Connection",
            _ => fallbackKey
        };
        return _localizer.Get(key);
    }

    public string FromException(Exception exception, string fallbackKey)
    {
        string key = exception switch
        {
            FaultException<ServiceFault> fault when !string.IsNullOrWhiteSpace(fault.Detail?.MessageKey)
                => fault.Detail.MessageKey,
            FaultException => fallbackKey,
            TimeoutException => "Error.Timeout",
            CommunicationException => "Error.Connection",
            _ => fallbackKey
        };
        return _localizer.Get(key);
    }

    public bool IsUnauthorized<T>(ClientResult<T> result)
    {
        return result.Failure == FailureKind.Business
            && result.ErrorCode == ServiceErrorCodes.Unauthorized;
    }

    public bool IsUnauthorized(Exception exception)
    {
        return exception is FaultException<ServiceFault> fault
            && fault.Detail?.ErrorCode == ServiceErrorCodes.Unauthorized;
    }
}
