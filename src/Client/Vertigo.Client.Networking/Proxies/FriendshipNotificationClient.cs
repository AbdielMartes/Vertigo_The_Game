using System.ServiceModel;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class FriendshipNotificationClient : IFriendshipNotificationClient
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    private FriendshipCallbackHandler? _callbackHandler;
    private DuplexChannelFactory<IFriendshipNotificationService>? _channelFactory;
    private readonly object _syncRoot = new();
    private IFriendshipNotificationService? _channel;
    private bool _isDisposed;

    public FriendshipNotificationClient()
    {
    }

    private event EventHandler<FriendshipInvitationEntry>? _friendRequestReceived;
    private event EventHandler<FriendEntry>? _friendshipEstablished;
    private event EventHandler<LobbyInvitationEntry>? _lobbyInvitationReceived;
    private event EventHandler<string>? _lobbyInvitationWithdrawn;


    public event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived
    {
        add => _friendRequestReceived += value;
        remove => _friendRequestReceived -= value;
    }

    public event EventHandler<FriendEntry>? FriendshipEstablished
    {
        add => _friendshipEstablished += value;
        remove => _friendshipEstablished -= value;
    }

    public event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived
    {
        add => _lobbyInvitationReceived += value;
        remove => _lobbyInvitationReceived -= value;
    }

    public event EventHandler<string>? LobbyInvitationWithdrawn
    {
        add => _lobbyInvitationWithdrawn += value;
        remove => _lobbyInvitationWithdrawn -= value;
    }

    public bool Subscribe(string sessionToken)
    {
        IFriendshipNotificationService channel = GetOrCreateChannel();

        try
        {
            return channel.Subscribe(sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public void Unsubscribe()
    {
        IFriendshipNotificationService channel = GetOrCreateChannel();

        try
        {
            channel.Unsubscribe();
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
        }
    }

    public void Disconnect()
    {
        IFriendshipNotificationService? oldChannel;
        DuplexChannelFactory<IFriendshipNotificationService>? oldFactory;
        lock (_syncRoot)
        {
            oldChannel = _channel;
            oldFactory = _channelFactory;
            _channel = null;
            _channelFactory = null;
            _callbackHandler = null;
        }
        (oldChannel as ICommunicationObject)?.Abort();
        if (oldFactory is not null)
        {
            oldFactory.Abort();
        }
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
        }
        Disconnect();
    }

    private static bool IsTransportFailure(Exception exception)
    {
        bool isCommunicationFailure = exception is CommunicationException and not FaultException;
        bool isTimeout = exception is TimeoutException;

        return isCommunicationFailure || isTimeout;
    }

    private static bool IsUnusable(ICommunicationObject communicationObject)
    {
        bool isUnusable = communicationObject.State is CommunicationState.Faulted
            or CommunicationState.Closing
            or CommunicationState.Closed;

        return isUnusable;
    }

    private static void CloseSafely(ICommunicationObject communicationObject)
    {
        try
        {
            communicationObject.Close();
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

    private IFriendshipNotificationService GetOrCreateChannel()
    {
        lock (_syncRoot)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_channel is not null && !IsUnusable((ICommunicationObject)_channel))
            {
                return _channel;
            }
            _channelFactory?.Abort();
            _callbackHandler = new FriendshipCallbackHandler();
            FriendshipCallbackHandler handler = _callbackHandler;
            handler.FriendRequestReceived += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _friendRequestReceived?.Invoke(this, value);
                }
            };
            handler.FriendshipEstablished += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _friendshipEstablished?.Invoke(this, value);
                }
            };
            handler.LobbyInvitationReceived += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _lobbyInvitationReceived?.Invoke(this, value);
                }
            };
            handler.LobbyInvitationWithdrawn += (sender, value) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    _lobbyInvitationWithdrawn?.Invoke(this, value);
                }
            };
            NetTcpBinding binding = new(SecurityMode.None)
            {
                OpenTimeout = OperationTimeout,
                SendTimeout = OperationTimeout
            };
            _channelFactory = new DuplexChannelFactory<IFriendshipNotificationService>(
                new InstanceContext(handler), binding,
                new EndpointAddress(ServiceEndpoints.FriendshipNotificationServiceAddress));
            _channel = _channelFactory.CreateChannel();
            return _channel;
        }
    }

    private void AbortChannel(IFriendshipNotificationService channel)
    {
        ((ICommunicationObject)channel).Abort();

        lock (_syncRoot)
        {
            if (ReferenceEquals(_channel, channel))
            {
                _channel = null;
            }
        }
    }
}
