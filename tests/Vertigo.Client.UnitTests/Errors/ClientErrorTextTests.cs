using System.Globalization;
using System.ServiceModel;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Errors;

public sealed class ClientErrorTextTests
{
    [Theory]
    [InlineData("es-MX", "El mensaje contiene lenguaje no permitido.")]
    [InlineData("en-US", "The message contains language that is not allowed.")]
    public void FromResult_BlockedChatLanguage_ReturnsLocalizedExplanation(string cultureName, string expected)
    {
        CultureInfo previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            ClientErrorText errors = new(new TextLocalizer());
            ClientResult<bool> failed = ClientResult<bool>.BusinessFailure(
                ServiceErrorCodes.ChatLanguageBlocked, ServiceMessageKeys.ChatLanguageBlocked);

            Assert.StartsWith(expected, errors.FromResult(failed, "Error.Unknown"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Fact]
    public void FromResult_Timeout_ReturnsLocalizedMessage()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ClientResult<bool> failed = ClientResult<bool>.TechnicalFailure(FailureKind.Timeout);

        string message = errors.FromResult(failed, "Error.Unknown");

        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.NotEqual("Error.Timeout", message);
    }

    [Fact]
    public void IsUnauthorized_BusinessFailure_ReturnsTrue()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ClientResult<bool> failed = ClientResult<bool>.BusinessFailure(
            ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);

        Assert.True(errors.IsUnauthorized(failed));
    }

    [Fact]
    public void IsUnauthorized_OtherFailure_ReturnsFalse()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ClientResult<bool> failed = ClientResult<bool>.BusinessFailure(
            ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);

        Assert.False(errors.IsUnauthorized(failed));
    }

    [Fact]
    public void FromException_ServerFault_ReturnsLocalizedMessage()
    {
        ClientErrorText errors = new(new TextLocalizer());
        ServiceFault fault = new()
        {
            ErrorCode = ServiceErrorCodes.LobbyNotFound,
            MessageKey = ServiceMessageKeys.LobbyNotFound
        };
        FaultException<ServiceFault> exception = new(fault);

        string message = errors.FromException(exception, "Error.Unknown");

        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.NotEqual(fault.MessageKey, message);
    }
}
