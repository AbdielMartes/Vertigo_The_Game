using System.ServiceModel;
using Vertigo.Client.Networking.Proxies;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Errors;

public sealed class ServiceCallerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Call_IncompleteServiceFault_UsesGenericFailure(bool emptyDetail)
    {
        ServiceCaller caller = new();
        ServiceFault? detail = emptyDetail ? new ServiceFault() : null;

        ClientResult<string> result = caller.Call<IStatisticsService, string, string>("net.tcp://127.0.0.1:1/Statistics",
            (_, _) => throw new FaultException<ServiceFault>(detail!), string.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(FailureKind.Unexpected, result.Failure);
        Assert.Empty(result.MessageKey);
    }

    [Fact]
    public void Call_DeclaredFault_PreservesItsPublicCode()
    {
        ServiceCaller caller = new();
        ServiceFault detail = new() { ErrorCode = ServiceErrorCodes.InvalidData, MessageKey = ServiceMessageKeys.InvalidData };

        ClientResult<string> result = caller.Call<IStatisticsService, string, string>("net.tcp://127.0.0.1:1/Statistics",
            (_, _) => throw new FaultException<ServiceFault>(detail), string.Empty);

        Assert.Equal(FailureKind.Business, result.Failure);
        Assert.Equal(ServiceErrorCodes.InvalidData, result.ErrorCode);
        Assert.Equal(ServiceMessageKeys.InvalidData, result.MessageKey);
    }
}
