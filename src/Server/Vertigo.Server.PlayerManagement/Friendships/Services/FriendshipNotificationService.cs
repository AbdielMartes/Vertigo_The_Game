using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class FriendshipNotificationService : IFriendshipNotificationService
{
    private readonly IFriendshipNotifier _notifier;
    private readonly IServiceScopeFactory _scopeFactory;

    public FriendshipNotificationService(IFriendshipNotifier notifier, IServiceScopeFactory scopeFactory)
    {
        _notifier = notifier;
        _scopeFactory = scopeFactory;
    }

    public bool Subscribe(string sessionToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISessionIdentityResolver resolver = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        SessionIdentity identity = resolver.RequireRegistered(sessionToken);

        IFriendshipCallback callback = OperationContext.Current.GetCallbackChannel<IFriendshipCallback>();
        IContextChannel channel = (IContextChannel)callback;
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        _notifier.Subscribe(identity.PlayerId, callback, sessionId);

        channel.Closed -= OnChannelClosed;
        channel.Faulted -= OnChannelClosed;
        channel.Closed += OnChannelClosed;
        channel.Faulted += OnChannelClosed;

        return true;
    }

    public void Unsubscribe()
    {
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        _notifier.UnsubscribeSession(sessionId);
    }

    private void OnChannelClosed(object? sender, EventArgs eventArgs)
    {
        string sessionId = (sender as IContextChannel)?.SessionId ?? string.Empty;

        _notifier.UnsubscribeSession(sessionId);
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
