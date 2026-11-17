using System;
using System.ServiceModel;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Contracts.Models;
using Xunit;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchBoardErrorHandlingTests
{
    private readonly ClientErrorText _errors = new(new KeyLocalizer());

    [Fact]
    public void FromException_Timeout_ReturnsTimeoutMessage()
    {
        string actual = _errors.FromException(new TimeoutException(), "Error.Unknown");

        Assert.Equal("Error.Timeout", actual);
    }

    [Fact]
    public void FromException_NetworkFailure_ReturnsConnectionMessage()
    {
        string actual = _errors.FromException(new EndpointNotFoundException("Test endpoint unavailable."), "Error.Unknown");

        Assert.Equal("Error.Connection", actual);
    }

    [Fact]
    public void FromException_BusinessFault_ReturnsServerMessageKey()
    {
        ServiceFault detail = new()
        {
            ErrorCode = "LOBBY_NOT_FOUND",
            MessageKey = "Lobby.NotFound"
        };
        FaultException<ServiceFault> fault = new(detail, new FaultReason("Lobby not found"));

        string actual = _errors.FromException(fault, "Error.Unknown");

        Assert.Equal("Lobby.NotFound", actual);
    }

    [Fact]
    public void FromException_UnexpectedFailure_ReturnsGenericError()
    {
        string actual = _errors.FromException(new InvalidOperationException(), "Error.Unknown");

        Assert.Equal("Error.Unknown", actual);
    }

    private sealed class KeyLocalizer : ITextLocalizer
    {
        public string Get(string key)
        {
            return key;
        }
    }
}
