using System.ServiceModel;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class ServiceCaller : IServiceCaller
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    public ClientResult<TResult> Call<TChannel, TArgument, TResult>(
        string address,
        Func<TChannel, TArgument, TResult> operation,
        TArgument argument)
        where TChannel : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentNullException.ThrowIfNull(operation);
        ChannelFactory<TChannel>? channelFactory = null;
        TChannel? channel = null;
        ClientResult<TResult> result;

        try
        {
            channelFactory = new ChannelFactory<TChannel>(CreateBinding(), new EndpointAddress(address));
            channel = channelFactory.CreateChannel();
            TResult value = operation(channel, argument);
            result = ClientResult<TResult>.Success(value);
        }
        catch (FaultException<ServiceFault> ex) when (ex.Detail is { } detail
            && !string.IsNullOrWhiteSpace(detail.ErrorCode) && !string.IsNullOrWhiteSpace(detail.MessageKey))
        {
            result = ClientResult<TResult>.BusinessFailure(detail.ErrorCode, detail.MessageKey);
        }
        catch (FaultException)
        {
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Unexpected);
        }
        catch (TimeoutException)
        {
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Timeout);
        }
        catch (CommunicationException)
        {
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.ServerUnreachable);
        }
        catch (Exception)
        {
            result = ClientResult<TResult>.TechnicalFailure(FailureKind.Unexpected);
        }
        finally
        {
            CloseCommunicationObject(channel as ICommunicationObject);
            CloseCommunicationObject(channelFactory);
        }

        return result;
    }

    private static NetTcpBinding CreateBinding()
    {
        return new NetTcpBinding(SecurityMode.None)
        {
            OpenTimeout = OperationTimeout,
            SendTimeout = OperationTimeout
        };
    }

    private static void CloseCommunicationObject(ICommunicationObject? communicationObject)
    {
        if (communicationObject is not null)
        {
            try
            {
                if (communicationObject.State == CommunicationState.Faulted)
                {
                    communicationObject.Abort();
                }
                else
                {
                    communicationObject.Close();
                }
            }
            catch (CommunicationException)
            {
                communicationObject.Abort();
            }
            catch (TimeoutException)
            {
                communicationObject.Abort();
            }
        }
    }
}
