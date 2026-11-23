using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Host.Transport;

internal sealed class ServiceDispatchErrors : IServiceBehavior, IErrorHandler, IDispatchMessageInspector
{
    internal const string FailureOwnerScope = "ServiceFailureOwner";
    private static readonly EventId InvalidMessageEvent = new(2101, "InvalidServiceMessage");
    private static readonly EventId UnexpectedFailureEvent = new(2102, "UnexpectedServiceFailure");
    private static readonly EventId ForbiddenLobbyAccessEvent = new(2103, "ForbiddenLobbyAccess");

    private readonly ILogger<ServiceDispatchErrors> _logger;
    private readonly Dictionary<string, (string Operation, string FaultAction)> _operations = new();

    public ServiceDispatchErrors(ILogger<ServiceDispatchErrors> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase host)
    {
        foreach (ServiceEndpoint endpoint in description.Endpoints)
        {
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                FaultDescription? fault = operation.Faults.FirstOrDefault(item => item.DetailType == typeof(ServiceFault));
                if (fault is not null && !operation.IsOneWay)
                {
                    _operations.TryAdd(operation.Messages[0].Action, (operation.Name, fault.Action));
                    if (!operation.OperationBehaviors.Contains(typeof(FailureScopeBehavior)))
                    {
                        operation.OperationBehaviors.Add(new FailureScopeBehavior(_logger));
                    }
                }
            }
        }

        ServiceDebugBehavior? debug = description.Behaviors.Find<ServiceDebugBehavior>();
        if (debug is not null)
        {
            debug.IncludeExceptionDetailInFaults = false;
        }

        foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers.OfType<ChannelDispatcher>())
        {
            dispatcher.IncludeExceptionDetailInFaults = false;
            dispatcher.ErrorHandlers.Add(this);
            foreach (EndpointDispatcher endpoint in dispatcher.Endpoints)
            {
                endpoint.DispatchRuntime.MessageInspectors.Add(this);
            }
        }
    }

    public void AddBindingParameters(ServiceDescription description, ServiceHostBase host,
        Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
    {
    }

    public void Validate(ServiceDescription description, ServiceHostBase host)
    {
    }

    public bool HandleError(Exception error)
    {
        return false;
    }

    public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
    {
        string? action = OperationContext.Current?.IncomingMessageHeaders?.Action;
        if (action is null || !_operations.TryGetValue(action, out (string Operation, string FaultAction) contract))
        {
            return;
        }

        if (error is FaultException<ServiceFault> { Detail: { } detail }
            && !string.IsNullOrWhiteSpace(detail.ErrorCode) && !string.IsNullOrWhiteSpace(detail.MessageKey))
        {
            LogSecurityRejection(contract.Operation, detail.ErrorCode);
        }
        else
        {
            fault = CreatePublicFault(error, version, contract);
        }
    }

    private Message CreatePublicFault(Exception error, MessageVersion version,
        (string Operation, string FaultAction) contract)
    {
        ServiceFault detail = new();
        if (error is EmptyRequestBodyException || error is NetDispatcherFaultException { InnerException: XmlException or FormatException
            or SerializationException } || error is CommunicationException { InnerException: SerializationException })
        {
            _logger.LogWarning(InvalidMessageEvent, "Could not deserialize request for {Operation}.",
                contract.Operation);
            detail.ErrorCode = ServiceErrorCodes.InvalidData;
            detail.MessageKey = ServiceMessageKeys.InvalidData;
        }
        else
        {
            Exception cause = error.GetBaseException();
            _logger.LogError(UnexpectedFailureEvent,
                "Operation {Operation} failed with {ErrorType} ({HResult}). Stack trace: {StackTrace}",
                contract.Operation, cause.GetType().FullName, cause.HResult, cause.StackTrace);
            detail.ErrorCode = ServiceErrorCodes.Unexpected;
            detail.MessageKey = ServiceMessageKeys.Unexpected;
        }

        FaultException<ServiceFault> response = new(detail, new FaultReason(detail.ErrorCode));
        return Message.CreateMessage(version, response.CreateMessageFault(), contract.FaultAction);
    }

    public object? AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
    {
        if (request.IsEmpty && request.Headers.Action is string action && _operations.ContainsKey(action))
        {
            throw new EmptyRequestBodyException();
        }

        return null;
    }

    public void BeforeSendReply(ref Message reply, object correlationState)
    {
    }

    private sealed class EmptyRequestBodyException() : Exception("The operation requires a SOAP body.");

    private sealed class FailureScopeBehavior(ILogger logger) : IOperationBehavior
    {
        public void ApplyDispatchBehavior(OperationDescription operation, DispatchOperation dispatch)
        {
            dispatch.Invoker = new ScopedInvoker(dispatch.Invoker, logger);
        }

        public void AddBindingParameters(OperationDescription operation, BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operation, ClientOperation client)
        {
        }

        public void Validate(OperationDescription operation)
        {
        }
    }

    private sealed class ScopedInvoker(IOperationInvoker inner, ILogger logger) : IOperationInvoker
    {
        public object[] AllocateInputs()
        {
            return inner.AllocateInputs();
        }

        public async ValueTask<(object returnValue, object[] outputs)> InvokeAsync(object instance, object[] inputs)
        {
            using IDisposable? scope = logger.BeginScope(new Dictionary<string, object> { [FailureOwnerScope] = true });
            return await inner.InvokeAsync(instance, inputs);
        }
    }

    private void LogSecurityRejection(string operation, string code)
    {
        if (code is ServiceErrorCodes.LobbyNotMember or ServiceErrorCodes.LobbyNotHost
            or ServiceErrorCodes.LobbyInviteNotAllowed)
        {
            Dictionary<string, object> properties = new() { ["EventCategory"] = "Security" };
            using IDisposable? scope = _logger.BeginScope(properties);
            _logger.LogWarning(ForbiddenLobbyAccessEvent, "Operation {Operation} was denied. Reason: {Reason}.",
                operation, code);
        }
    }
}
